﻿using System;
using System.Diagnostics;
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
            var wavBytes = WavUtility.GetWav(wavData);
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
    }
}
