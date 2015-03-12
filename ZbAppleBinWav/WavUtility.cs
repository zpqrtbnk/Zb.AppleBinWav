using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZbAppleBinWav
{
    public static class WavUtility
    {
        #region Generate WAV bytes

        public static byte[] GetWav(byte[] samples)
        {
            const short numberOfChannels = 1; // mono audio
            const short bytesPerSample = 1; // 8bit samples
            const int samplingRate = 44100; // 44.1 kHz
            var totalBytes = checked(44 + (samples.Length * bytesPerSample * numberOfChannels)); // size of headers + data
            var output = new byte[totalBytes];
            Buffer.BlockCopy(GetLeBytes(0x46464952), 0, output, 0, 4); // "RIFF"
            Buffer.BlockCopy(GetLeBytes(totalBytes - 8), 0, output, 4, 4); // RIFF chunk size
            Buffer.BlockCopy(GetLeBytes(0x45564157), 0, output, 8, 4); // "WAVE"
            Buffer.BlockCopy(GetLeBytes(0x20746D66), 0, output, 12, 4); // "fmt "
            Buffer.BlockCopy(GetLeBytes(16), 0, output, 16, 4); // fmt chunk size
            Buffer.BlockCopy(GetLeBytes((short)1), 0, output, 20, 2); // compression code (1 - PCM/Uncompressed)
            Buffer.BlockCopy(GetLeBytes((short)numberOfChannels), 0, output, 22, 2); // number of channels
            Buffer.BlockCopy(GetLeBytes(samplingRate), 0, output, 24, 4); // sampling rate
            Buffer.BlockCopy(GetLeBytes(samplingRate * bytesPerSample * numberOfChannels), 0, output, 28, 4); // bytes/second
            Buffer.BlockCopy(GetLeBytes((short)(bytesPerSample * numberOfChannels)), 0, output, 32, 2); // block size
            Buffer.BlockCopy(GetLeBytes((short)(bytesPerSample * 8)), 0, output, 34, 2); // bits per sample
            Buffer.BlockCopy(GetLeBytes(0x61746164), 0, output, 36, 4); // "data"
            Buffer.BlockCopy(GetLeBytes(totalBytes - 44), 0, output, 40, 4); // data chunk size
            for (var i = 0; i < samples.Length; i++)
            {
                //Buffer.BlockCopy(GetLEBytes(samples[i]), 0, output, (bytesPerSample * i * numberOfChannels) + 44, bytesPerSample);
                Buffer.BlockCopy(BitConverter.GetBytes(samples[i]), 0, output, (bytesPerSample * i * numberOfChannels) + 44, bytesPerSample);
            }

            //File.WriteAllBytes(filename, output);
            return output;
        }

        public static byte[] GetLeBytes(short value)
        {
            return BitConverter.IsLittleEndian
                ? BitConverter.GetBytes(value)
                : BitConverter.GetBytes((short)((value & 0xFF) << 8 | (value & 0xFF00) >> 8));
        }

        public static byte[] GetLeBytes(int value)
        {
            return BitConverter.IsLittleEndian
                ? BitConverter.GetBytes(value)
                : BitConverter.GetBytes((int)((value & 0xFF) << 24) | (int)((value & 0xFF00) << 8)
                    | (int)((value & 0xFF0000) >> 8) | (int)((value & 0xFF000000) >> 24));
        }

        #endregion
    }
}
