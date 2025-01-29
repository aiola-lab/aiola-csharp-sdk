using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

public class AiolaTTSClient
{
    private readonly string _baseUrl;
    private readonly string _bearerToken;
    private readonly AudioConverter _audioConverter;
    private readonly HttpClient _httpClient;

    public AiolaTTSClient(string baseUrl, string bearerToken, AudioFormat audioFormat = AudioFormat.LINEAR16)
    {
        if (string.IsNullOrEmpty(baseUrl))
            throw new ArgumentException("The baseUrl parameter is required.");
        if (string.IsNullOrEmpty(bearerToken))
            throw new ArgumentException("The bearerToken parameter is required.");

        _baseUrl = baseUrl;
        _bearerToken = bearerToken;
        _audioConverter = new AudioConverter(audioFormat);
        _httpClient = new HttpClient();
    }

    private async Task<byte[]> PostRequestAsync(string endpoint, Dictionary<string, string> payload)
    {
        var url = $"{_baseUrl}{endpoint}";
        var content = new FormUrlEncodedContent(payload);

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);

        HttpResponseMessage response = await _httpClient.PostAsync(url, content);
        byte[] responseData = await response.Content.ReadAsByteArrayAsync();

        if (response.IsSuccessStatusCode)
        {
            if (response.Content.Headers.ContentType?.MediaType == "audio/wav")
            {
                return ConvertAudio(responseData);
            }
            return responseData;
        }

        return HandleApiError(response, responseData);
    }

    public async Task<byte[]> SynthesizeAsync(string text, string voice = "af_bella")
    {
        if (string.IsNullOrEmpty(text))
            throw new ArgumentException("The 'text' parameter is required.");

        var payload = new Dictionary<string, string>
        {
            { "text", text },
            { "voice", voice }
        };

        return await PostRequestAsync("/synthesize", payload);
    }

    public async Task<byte[]> SynthesizeStreamAsync(string text, string voice = "af_bella")
    {
        if (string.IsNullOrEmpty(text))
            throw new ArgumentException("The 'text' parameter is required.");

        var payload = new Dictionary<string, string>
        {
            { "text", text },
            { "voice", voice }
        };

        return await PostRequestAsync("/synthesize/stream", payload);
    }

    private byte[] ConvertAudio(byte[] audioData)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return _audioConverter.Convert(audioData);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return ConvertAudioMacLinux(audioData);
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported OS.");
        }
    }

    private byte[] ConvertAudioMacLinux(byte[] audioData)
    {
        string tempInputFile = Path.GetTempFileName() + ".wav";
        string tempOutputFile = Path.GetTempFileName() + ".wav";

        File.WriteAllBytes(tempInputFile, audioData);

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i {tempInputFile} -ac 1 -ar 16000 {tempOutputFile}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(startInfo))
        {
            process.WaitForExit();
        }

        return File.ReadAllBytes(tempOutputFile);
    }

    private byte[] HandleApiError(HttpResponseMessage response, byte[] responseData)
    {
        try
        {
            var errorJson = JsonSerializer.Deserialize<Dictionary<string, string>>(Encoding.UTF8.GetString(responseData));
            throw new Exception($"Error {response.StatusCode}: {errorJson["detail"]}");
        }
        catch
        {
            throw new Exception($"Error {response.StatusCode}: Unknown error occurred.");
        }
    }
}