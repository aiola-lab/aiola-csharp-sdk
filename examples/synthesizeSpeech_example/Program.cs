class Program
{
    static async Task Main(string[] args)
    {
        /*
            Voice Options for Aiola TTS:

            ───────────────────────────────
            African Female Voices:
            ───────────────────────────────
            af_bella   → Bella
            af_nicole  → Nicole
            af_sarah   → Sarah
            af_sky     → Sky

            ───────────────────────────────
            American Male Voices:
            ───────────────────────────────
            am_adam    → Adam
            am_michael → Michael

            ───────────────────────────────
            British Female Voices:
            ───────────────────────────────
            bf_emma    → Emma
            bf_isabella→ Isabella

            ───────────────────────────────
            British Male Voices:
            ───────────────────────────────
            bm_george  → George
            bm_lewis   → Lewis
        */
        string ttsUrl = "<your-api-base-url>/api/tts";  // Replace with your API base URL
        string bearerToken = "<your-bearer-token>";  // Replace with your Bearer token
        string voice = "af_bella";


        var ttsClient = new AiolaTTSClient(ttsUrl, bearerToken);

        try
        {
            // Synthesize Speech Example
            Console.WriteLine("Synthesizing speech...");

            string text = "Hello, this is a test of the aiOla TTS synthesis feature. You can download the audio after processing.";
            byte[] audioData = await ttsClient.SynthesizeAsync(text, voice);

            SaveAudioFile(audioData);
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }

    public static void SaveAudioFile(byte[] audioData)
    {
        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        string filename = $"synthesized_{timestamp}.wav";

        File.WriteAllBytes(filename, audioData);
        Console.WriteLine($"✅ Audio saved as {filename}");
    }
}