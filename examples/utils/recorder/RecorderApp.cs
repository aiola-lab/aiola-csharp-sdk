using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;


#if WINDOWS
using NAudio.Wave; // Windows: Use NAudio
#else
using SharperPortAudio.Base; // macOS/Linux: Use SharperPortAudio
#endif

public class RecorderApp
{
    private readonly AiolaStreamingClient _sdk;
    private readonly Action<Dictionary<string, string>> _onStreamError;
    private readonly AudioConfig _audioConfig; // 🔹 Use AudioConfig for settings
    private readonly List<byte> collectByteBuffer; // Move initialization to the constructor

#if WINDOWS
    private WaveInEvent? _waveIn; // NAudio for Windows
#else
    private SharperPortAudio.Base.Stream? _audioStream; // PortAudio Stream for macOS/Linux
#endif

    public RecorderApp(AiolaStreamingClient sdk, Action<Dictionary<string, string>> onStreamError, AudioConfig audioConfig)
    {
        _sdk = sdk;
        _onStreamError = onStreamError;
        _audioConfig = audioConfig;
        collectByteBuffer = new List<byte>();
    }

    public async Task StartStreamingAsync()
    {
        try
        {
            await Task.Run(() => StartAudioStreaming());
        }
        catch (Exception ex)
        {
            throw new AiolaStreamingError($"Audio streaming error: {ex.Message}");
        }
    }

    private void StartAudioStreaming()
    {
#if WINDOWS
        Console.WriteLine("Using NAudio for Windows");

        _waveIn = new WaveInEvent
        {
            WaveFormat = new WaveFormat(_audioConfig.SampleRate, 16, _audioConfig.Channels), // 🔹 Use AudioConfig
            BufferMilliseconds = 100
        };

        _waveIn.DataAvailable += async (sender, e) =>
        {
            try
            {
                byte[] buffer = new byte[e.BytesRecorded];
                Array.Copy(e.Buffer, buffer, e.BytesRecorded);
                await _sdk.WriteAudioChunkAsync(buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pipe error: {ex.Message}");
                _onStreamError?.Invoke(new Dictionary<string, string> { { "audio_status", ex.Message } });
            }
        };

        _waveIn.StartRecording();

#else
        Console.WriteLine("Using SharperPortAudio for macOS/Linux");

        PortAudio.Initialize(); // Ensure PortAudio is initialized

        int deviceCount = PortAudio.DeviceCount;
        
        if (deviceCount < 0)
        {
            throw new Exception("ERROR: No audio devices found!");
        }

        int inputDevice = _audioConfig.DeviceNumber; // 🔹 Select device from config

        var deviceInfo = PortAudio.GetDeviceInfo(inputDevice);
        Console.WriteLine($"Device {inputDevice}: {deviceInfo.name}");
        Console.WriteLine($"  Max input channels: {deviceInfo.maxInputChannels}");
        Console.WriteLine($"  Max output channels: {deviceInfo.maxOutputChannels}");
        Console.WriteLine($"  Default sample rate: {deviceInfo.defaultSampleRate}");
        Console.WriteLine($"Default Sample Rate: {deviceInfo.defaultSampleRate}");
        Console.WriteLine($"Max Input Channels: {deviceInfo.maxInputChannels}");
        Console.WriteLine($"Max Output Channels: {deviceInfo.maxOutputChannels}");
        Console.WriteLine();
        


        StreamParameters inputParams = new StreamParameters()
        {
            device = inputDevice,
            channelCount = _audioConfig.Channels, // 🔹 Use AudioConfig
            sampleFormat = _audioConfig.Dtype == "int16" ? SampleFormat.Int16 : SampleFormat.Float32, // 🔹 Dynamically choose dtype
            suggestedLatency = PortAudio.GetDeviceInfo(inputDevice).defaultLowInputLatency
        };


        int sampleRate = _audioConfig.SampleRate; // 🔹 Use AudioConfig
        Console.WriteLine($"  sampleRate: {sampleRate}");
        uint framesPerBuffer =0;// (uint)_audioConfig.ChunkSize; // 🔹 Use AudioConfig
        Console.WriteLine($"  framesPerBuffer: {framesPerBuffer}");

        SharperPortAudio.Base.Stream.Callback callback = (IntPtr input, IntPtr output,
            uint frameCount, ref StreamCallbackTimeInfo timeInfo,
            StreamCallbackFlags statusFlags, IntPtr userData) =>
        {
            // Allocate buffer for recorded samples
            short[] buffer = new short[frameCount]; 
            
            // Copy PCM Int16 audio from unmanaged memory to managed array
            Marshal.Copy(input, buffer, 0, (int)frameCount);

            byte[] byteBuffer = new byte[buffer.Length * sizeof(short)];
            Buffer.BlockCopy(buffer, 0, byteBuffer, 0, byteBuffer.Length);
            

            lock (collectByteBuffer) // Ensure thread safety
            {
                collectByteBuffer.AddRange(byteBuffer);

                while (collectByteBuffer.Count >= _audioConfig.ChunkSize)
                {
                    byte[] chunkToSend = collectByteBuffer.Take(_audioConfig.ChunkSize).ToArray();
                    collectByteBuffer.RemoveRange(0, _audioConfig.ChunkSize); // Remove sent data

                    _sdk.WriteAudioChunkAsync(chunkToSend).Wait();
                }
            }

            return StreamCallbackResult.Continue;
        };

        _audioStream = new SharperPortAudio.Base.Stream(inputParams, null, sampleRate, 256, StreamFlags.ClipOff, callback, IntPtr.Zero);

        Console.WriteLine("Recording");
        _audioStream.Start();
#endif
    }

    public static void WriteWavFile(string filePath, float[] audioData, int sampleRate, int channels)
    {
        WaveFormat waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);

        using (WaveFileWriter waveFileWriter = new WaveFileWriter(filePath, waveFormat))
        {
            Console.WriteLine("filePath: "+filePath);
            waveFileWriter.WriteSamples(audioData, 0, audioData.Length);
        }
    }

    public void CloseAudioStreaming()
    {
        Console.WriteLine("Closing Audio Streaming...");

#if WINDOWS
        if (_waveIn != null)
        {
            try
            {
                _waveIn.StopRecording();
                _waveIn.Dispose();
                Console.WriteLine("Recording stopped.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping recording: {ex.Message}");
            }
            finally
            {
                _waveIn = null;
            }
        }
#else
        if (_audioStream != null)
        {
            try
            {
                _audioStream.Stop();
                _audioStream.Close();
                Console.WriteLine("Recording stopped.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping recording: {ex.Message}");
            }
            finally
            {
                _audioStream = null;
                PortAudio.Terminate();
            }
        }
#endif
    }
}