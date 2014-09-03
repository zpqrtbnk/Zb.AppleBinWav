using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZbAppleBinWav
{
    // implementing Intel-HEX format
    // see http://en.wikipedia.org/wiki/Intel_HEX

    class IntelHex
    {
        public class Block
        {
            public byte[] Data { get; set; }
            public int Address { get; set; }
        }

        private byte Checksum(string line)
        {
            byte checksum = 0;
            for (var i = 1; i < line.Length - 2; i += 2)
            {
                var value = byte.Parse(line.Substring(i, 2), NumberStyles.HexNumber);
                checksum += value;
            }
            return (byte)(1 + (checksum ^ 0xFF)) ;
        }

        public IEnumerable<Block> ReadAllBlocks(string filename)
        {
            var text = File.ReadAllLines(filename);
            return ReadAllBlocks(text);
        }

        public IEnumerable<Block> ReadAllBlocks(string[] text)
        {
            var lineCount = 0;
            var blocks = new List<IntelHex.Block>();

            var blockStream = new MemoryStream();
            var blockWriter = new BinaryWriter(blockStream);
            var blockAddress = -1;
            var runningAddress = -1;

            foreach (var line in text)
            {
                if (!line.StartsWith(":"))
                    throw new Exception(string.Format("Line {0} does not start with ':'.", lineCount));

                var byteCount = int.Parse(line.Substring(1, 2), NumberStyles.HexNumber);
                var address = int.Parse(line.Substring(3, 4), NumberStyles.HexNumber);
                var recordType = int.Parse(line.Substring(7, 2), NumberStyles.HexNumber);

                var bytes = new byte[byteCount];
                var i = 0;
                while (i < byteCount)
                {
                    bytes[i] = byte.Parse(line.Substring(9 + 2 * i, 2), NumberStyles.HexNumber);
                    i++;
                }

                var calcChecksum = Checksum(line);
                var checksum = int.Parse(line.Substring(9 + 2 * i, 2), NumberStyles.HexNumber);
                if (checksum != calcChecksum)
                    throw new Exception(string.Format("Line {0} has invalid checksum 0x{1:X2}, expecting 0x{2:X2}.", lineCount, checksum, calcChecksum));

                if (recordType == 00)
                {
                    // data

                    if (runningAddress < 0)
                    {
                        blockAddress = address;
                        runningAddress = address + byteCount;
                    }
                    else
                    {
                        // this actually might be a block change
                        // but we don't support it at the moment
                        // would need to return multiple blocks...
                        if (address != runningAddress)
                            throw new Exception(string.Format("Line {0} has unexpected address 0x{1:X4}, expecting 0x{2:X4}.", lineCount, address, runningAddress));
                        runningAddress += byteCount;
                    }
                    
                    blockWriter.Write(bytes);
                }
                else if (recordType == 01)
                {
                    // eof
                    if (blockAddress > 0)
                        blocks.Add(new Block
                        {
                            Address = blockAddress,
                            Data = blockStream.ToArray()
                        });
                }
                else
                {
                    throw new Exception(string.Format("Line {0} has unsupported record type 0x{1:X2}.", lineCount, recordType));
                }

                lineCount++;
            }

            return blocks;
        }
    }
}
