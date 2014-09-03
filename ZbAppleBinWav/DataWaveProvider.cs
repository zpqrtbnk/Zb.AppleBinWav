using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace ZbAppleBinWav
{
    public class DataWaveProvider : IWaveProvider
    {
        private readonly WaveFormat _waveFormat = new WaveFormat(44100, 8, 1);
        private readonly byte[] _bytes;
        private int _pos;

        public DataWaveProvider(byte[] bytes)
        {
            // bytes are DATA bytes not WAV bytes
            // first convert to WAV data
            var wavData = BytesToWav.Encode(bytes, bytes.Length);
            // then convert WAV data to WAV bytes
            var wavBytes = GetWav(wavData);
            _bytes = wavBytes;
        }

        // returning 0 will stop, anything else will ask for more...
        // offset never increases... because offset is offset within *buffer*
        public int Read(byte[] buffer, int offset, int count)
        {
            var avail = _bytes.Length - _pos;
            if (avail <= 0)
                return 0;
            var read = 0;
            if (avail > count)
            {
                Buffer.BlockCopy(_bytes, _pos, buffer, offset, count);
                read = count;
            }
            else
            {
                Buffer.BlockCopy(_bytes, _pos, buffer, offset, avail);
                read = avail;
            }
            Debug.WriteLine("read {0} (length={1}, offset={2}, count={3})", read, _bytes.Length, offset, count);
            _pos += read;
            return read;
        }

        public WaveFormat WaveFormat
        {
            get { return _waveFormat; }
        }

        #region Generate WAV bytes

        private static byte[] GetWav(byte[] samples)
        {
            const short numberOfChannels = 1; // mono audio
            const short bytesPerSample = 1; // 8bit samples
            const int samplingRate = 44100; // 44.1 kHz
            var totalBytes = checked(44 + (samples.Length * bytesPerSample * numberOfChannels)); // size of headers + data
            var output = new byte[totalBytes];
            Buffer.BlockCopy(GetLEBytes(0x46464952), 0, output, 0, 4); // "RIFF"
            Buffer.BlockCopy(GetLEBytes(totalBytes - 8), 0, output, 4, 4); // RIFF chunk size
            Buffer.BlockCopy(GetLEBytes(0x45564157), 0, output, 8, 4); // "WAVE"
            Buffer.BlockCopy(GetLEBytes(0x20746D66), 0, output, 12, 4); // "fmt "
            Buffer.BlockCopy(GetLEBytes(16), 0, output, 16, 4); // fmt chunk size
            Buffer.BlockCopy(GetLEBytes((short)1), 0, output, 20, 2); // compression code (1 - PCM/Uncompressed)
            Buffer.BlockCopy(GetLEBytes((short)numberOfChannels), 0, output, 22, 2); // number of channels
            Buffer.BlockCopy(GetLEBytes(samplingRate), 0, output, 24, 4); // sampling rate
            Buffer.BlockCopy(GetLEBytes(samplingRate * bytesPerSample * numberOfChannels), 0, output, 28, 4); // bytes/second
            Buffer.BlockCopy(GetLEBytes((short)(bytesPerSample * numberOfChannels)), 0, output, 32, 2); // block size
            Buffer.BlockCopy(GetLEBytes((short)(bytesPerSample * 8)), 0, output, 34, 2); // bits per sample
            Buffer.BlockCopy(GetLEBytes(0x61746164), 0, output, 36, 4); // "data"
            Buffer.BlockCopy(GetLEBytes(totalBytes - 44), 0, output, 40, 4); // data chunk size
            for (var i = 0; i < samples.Length; i++)
            {
                //Buffer.BlockCopy(GetLEBytes(samples[i]), 0, output, (bytesPerSample * i * numberOfChannels) + 44, bytesPerSample);
                Buffer.BlockCopy(BitConverter.GetBytes(samples[i]), 0, output, (bytesPerSample * i * numberOfChannels) + 44, bytesPerSample);
            }

            //File.WriteAllBytes(filename, output);
            return output;
        }

        private static byte[] GetLEBytes(short value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.GetBytes(value);
            }
            else
            {
                return BitConverter.GetBytes((short)((value & 0xFF) << 8 | (value & 0xFF00) >> 8));
            }
        }

        private static byte[] GetLEBytes(int value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.GetBytes(value);
            }
            else
            {
                return BitConverter.GetBytes((int)((value & 0xFF) << 24) | (int)((value & 0xFF00) << 8)
                | (int)((value & 0xFF0000) >> 8) | (int)((value & 0xFF000000) >> 24));
            }
        }

        #endregion
    }
}
