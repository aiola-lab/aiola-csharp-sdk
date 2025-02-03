using System.Text.Json;
using SocketIOClient;


public class AiolaStreamingClient
{
    private readonly StreamingConfig _config;
    private readonly SocketIOClient.SocketIO _sio;
    private readonly StreamingStats _stats;

    public AiolaStreamingClient(StreamingConfig config)
    {
        _config = config;

        // Build URL with query parameters
        var queryParams = new Dictionary<string, string>
        {
            { "flow_id", _config.FlowId },
            { "execution_id", _config.ExecutionId },
            { "lang_code", _config.LangCode },
            { "time_zone", _config.TimeZone }
        };

        // Get Authentication Headers
        var authHeaders = AuthService.GetAuthHeaders(_config.AuthType, _config.AuthCredentials);
        foreach (var kvp in authHeaders)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }


        if (_config.Transports == "polling")
        {
            // Initialize SocketIO client Polling
            _sio = new SocketIOClient.SocketIO(_config.Endpoint + _config.Namespace + "?", new SocketIOOptions
            {
                Transport = SocketIOClient.Transport.TransportProtocol.Polling,
                Path = "/api/voice-streaming/socket.io/",
                AutoUpgrade = false,
                Query = queryParams,
                ExtraHeaders = authHeaders
            });

        }
        else
        {
            // Initialize SocketIO client Polling
            _sio = new SocketIOClient.SocketIO(_config.Endpoint + _config.Namespace + "?", new SocketIOOptions
            {
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
                Path = "/api/voice-streaming/socket.io/",
                AutoUpgrade = true,
                Query = queryParams,

                ExtraHeaders = authHeaders
            });
        }


        var bearerHeader = AuthUtils.GetBearerAuthHeader(authHeaders);
        Console.WriteLine("socket.Namespace: " + _sio.Namespace);
        Console.WriteLine(bearerHeader);


        _sio.HttpClient.AddHeader(bearerHeader.Key, bearerHeader.Value);


        _stats = new StreamingStats
        {
            TotalAudioSentDuration = 0,
            ConnectionStartTime = null
        };

        SetupEventHandlers();
    }

    private void SetupEventHandlers()
    {
        _sio.OnConnected += (sender, e) =>
        {
            _stats.ConnectionStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _config.Callbacks?.OnConnect?.Invoke();
        };

        _sio.OnDisconnected += (sender, reason) =>
        {
            Console.WriteLine($"Disconnected: {reason}");
            if (_config.Callbacks?.OnDisconnect != null)
            {
                long connectionDuration = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - (_stats.ConnectionStartTime ?? 0);
                _config.Callbacks.OnDisconnect(connectionDuration, _stats.TotalAudioSentDuration);
            }
        };

        _sio.OnError += (sender, e) =>
        {
            Console.WriteLine($"Socket error: {e}");
            _config.Callbacks?.OnError?.Invoke(new Dictionary<string, string> { { "socket_error", e } });
        };


        _sio.On("transcript", response =>
        {
            try
            {
                var jsonArray = JsonSerializer.Deserialize<JsonElement>(response.ToString());
                _config.Callbacks?.OnTranscript?.Invoke(jsonArray);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing transcript: {ex.Message}");
            }
        });

        _sio.On("events", response =>
        {

            try
            {
                var jsonArray = JsonSerializer.Deserialize<JsonElement>(response.ToString());
                _config.Callbacks?.OnEvents?.Invoke(jsonArray);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing events: {ex.Message}");
            }
        });

        _sio.On("error", response =>
       {
           Console.WriteLine("error");
           string data = response.GetValue<string>();
           _config.Callbacks?.OnError?.Invoke(new Dictionary<string, string> { { "socket_error", data } });
       });
    }

    public async Task SetKeywordsAsync(string[] keywords)
    {
        if (!_sio.Connected)
        {
            Console.WriteLine("Socket is not connected. Unable to send keywords.");
            _config.Callbacks?.OnError?.Invoke(new Dictionary<string, string> { { "message", "Socket not connected." } });
            return;
        }

        try
        {
            string binaryData = JsonSerializer.Serialize(keywords);
            Console.WriteLine($"set_keywords: {binaryData}");

            await _sio.EmitAsync("set_keywords", binaryData, _config.Namespace);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error emitting keywords: {ex.Message}");
            _config.Callbacks?.OnError?.Invoke(new Dictionary<string, string> { { "message", ex.Message } });
        }
    }

    public async Task WriteAudioChunkAsync(byte[] chunk)
    {
        if (_sio.Connected)
        {
            await _sio.EmitAsync("binary_data", chunk);
        }
    }

    public async Task ConnectAsync()
    {
        if (!_sio.Connected)
        {
            await _sio.ConnectAsync();
        }
    }

    public async Task DisconnectAsync()
    {
        if (_sio.Connected)
        {
            await _sio.DisconnectAsync();
        }
    }
}