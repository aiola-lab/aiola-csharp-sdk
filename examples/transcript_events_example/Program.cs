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
        const string bearer_token = "< your-bearer-token >";

        var sdk = new AiolaStreamingClient(new StreamingConfig
        {
            Endpoint = "< your-api-base-url >",
            Namespace = "/events",
            FlowId = "<your-flow-id>",
            ExecutionId = "123321123321",
            LangCode = "en_US",
            TimeZone = "UTC",
            AuthType = "Bearer",
            Transports = "websocket",
            AuthCredentials = new Dictionary<string, string> { { "token", bearer_token } },

            // 🔹 Register Callbacks Here
            Callbacks = new StreamingCallbacks
            {
                OnConnect = async () => Console.WriteLine("✅ Connected to Aiola Streaming API."),
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
        });

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