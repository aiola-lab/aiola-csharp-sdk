using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Example;

class Program
{
    static async Task Main(string[] args)
    {
        const string bearer_token = "77b572fee80111e99f1028bd6035e8e9";

        AiolaStreamingClient? sdk = null; // Declare first to use in Callbacks

        var config = new StreamingConfig
        {
            Endpoint = "https://tesla.internal.aiola.ai",
            Namespace = "/events",
            FlowId = "09235016-0971-41e2-a346-afd122585b41",
            ExecutionId = "123321123321",
            LangCode = "en_US",
            TimeZone = "UTC",
            AuthType = "Bearer",
            Transports = "websocket",
            AuthCredentials = new Dictionary<string, string> { { "token", bearer_token } },

            Callbacks = new StreamingCallbacks
            {
                OnConnect = async () =>
                {
                    Console.WriteLine("✅ Connected to Aiola Streaming API.");

                    if (sdk != null)
                    {
                        try
                        {
                            Console.WriteLine("⏳ Waiting 30 seconds before setting keywords...");
                            await Task.Delay(30000);  // ✅ Works correctly!
                            await sdk.SetKeywordsAsync(new string[] { "aiola", "ngnix" });
                            Console.WriteLine("🔹 Keywords sent successfully: [aiola, ngnix]");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Error setting keywords: {ex.Message}");
                        }
                    }
                },

                OnDisconnect = async (duration, totalAudioSent) =>
                {
                    Console.WriteLine($"❌ Disconnected after {duration} ms. Total Audio Sent: {totalAudioSent} bytes.");
                    await Task.CompletedTask;
                },

                OnTranscript = async transcript =>
                {
                    Console.WriteLine($"📝 Transcript Received: {transcript}");
                    await Task.CompletedTask;
                },

                OnEvents = async events =>
                {
                    Console.WriteLine($"📢 Event Received: {events}");
                    await Task.CompletedTask;
                },

                OnError = async error =>
                {
                    Console.WriteLine($"❌ Error: {string.Join(", ", error.Select(kv => $"{kv.Key}: {kv.Value}"))}");
                    await Task.CompletedTask;
                }
            }
        };

        // Now initialize the SDK AFTER config is created
        sdk = new AiolaStreamingClient(config);

        var recorder = new RecorderApp(sdk,
            error => Console.WriteLine($"Streaming error: {error["audio_status"]}"),
            AudioConfig.Default()
        );

        Console.WriteLine("Starting audio streaming...");

        // ✅ Run the SDK & Recorder as Background Tasks
        var sdkTask = Task.Run(async () => await sdk.ConnectAsync());
        var recorderTask = Task.Run(async () => await recorder.StartStreamingAsync());

        Console.WriteLine("Press any key to stop...");
        Console.ReadKey();

        Console.WriteLine("Stopping audio streaming...");
        recorder.CloseAudioStreaming();


        // ❗ Optionally, wait for tasks to complete before exiting
        await Task.WhenAll(sdkTask, recorderTask);
    }
}