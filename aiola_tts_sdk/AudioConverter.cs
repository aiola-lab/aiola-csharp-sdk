using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using NAudio.Wave;

public enum AudioFormat
{
    LINEAR16,
    PCM
}

public class AudioConverter
{
    private readonly AudioFormat _targetFormat;

    public AudioConverter(AudioFormat targetFormat)
    {
        _targetFormat = targetFormat;
    }

    public byte[] Convert(byte[] audioBytes)
    {
        using (var stream = new MemoryStream(audioBytes))
        using (var reader = new WaveFileReader(stream))
        {
            switch (_targetFormat)
            {
                case AudioFormat.LINEAR16:
                    return ConvertToLinear16(reader);
                case AudioFormat.PCM:
                    return ConvertToPCM(reader);
                default:
                    throw new NotSupportedException("Unsupported audio format.");
            }
        }
    }

    private byte[] ConvertToLinear16(WaveFileReader reader)
    {
        using (var outStream = new MemoryStream())
        {
            var format = new WaveFormat(16000, 16, 2); // 16kHz, 16-bit, stereo
            using (var conversionStream = new WaveFormatConversionStream(format, reader))
            {
                conversionStream.CopyTo(outStream);
            }
            return outStream.ToArray();
        }
    }

    private byte[] ConvertToPCM(WaveFileReader reader)
    {
        using (var outStream = new MemoryStream())
        {
            var format = new WaveFormat(16000, 16, 1); // 16kHz, 16-bit, mono
            using (var conversionStream = new WaveFormatConversionStream(format, reader))
            {
                conversionStream.CopyTo(outStream);
            }
            return outStream.ToArray();
        }
    }
}