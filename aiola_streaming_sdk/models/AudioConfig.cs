public class AudioConfig
{
    public int DeviceNumber { get; set; } = 0; 
    public int SampleRate { get; set; } = 16000; // Default 16kHz
    public int Channels { get; set; } = 1; // Default Mono
    public int ChunkSize { get; set; } = 4096; // Default buffer size
    public string AudioType { get; set; } = "wav"; // Default audio format
    public string Dtype { get; set; } = "int16"; // Default data type (16-bit PCM)

    // ✅ Static method to create default config
    public static AudioConfig Default()
    {
        return new AudioConfig();
    }

    // ✅ Optional: Constructor for customization
    public AudioConfig(int deviceNumber = 0, int sampleRate = 16000, int channels = 1, int chunkSize = 4096, string audioType = "wav", string dtype = "int16")
    {
        DeviceNumber = deviceNumber;
        SampleRate = sampleRate;
        Channels = channels;
        ChunkSize = chunkSize;
        AudioType = audioType;
        Dtype = dtype;
    }
}