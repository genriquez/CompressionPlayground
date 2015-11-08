using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CA.Helpers;

namespace CA
{
    class BlockSubset
    {
        private const int MaxBlockSize = 256 * 256;

        public static void Calculate(IEnumerable<byte> bytes, byte bitWidth)
        {
            var maxValues = Math.Pow(2, bitWidth);
            var blockValuesBuffer = new List<byte>();
            var blockSizes = new List<int>();

            var currentBlockSize = 0;
            foreach (var b in bytes)
            {
                var isRepeatedByte = blockValuesBuffer.Contains(b);
                if (!isRepeatedByte && blockValuesBuffer.Count == maxValues || currentBlockSize == MaxBlockSize)
                {
                    blockSizes.Add(currentBlockSize);
                    blockValuesBuffer.Clear();
                    currentBlockSize = 0;
                }

                if (!isRepeatedByte)
                {
                    blockValuesBuffer.Add(b);
                }

                currentBlockSize++;
            }

            float totalBits = blockSizes.Sum() * 8;
            float compressedBits = blockSizes.Sum() * bitWidth + blockSizes.Count * (256 /* bitmask */ + 16 /* block size */);

            Console.WriteLine("Total blocks: {0:N0}", blockSizes.Count);
            Console.WriteLine("Average block size: {0:N0}", blockSizes.Average());
            Console.WriteLine("Max block size: {0}", blockSizes.Max());
            Console.WriteLine("Estimated file size: {0:N2}%", 100 * compressedBits / totalBits);
        }

        public static IEnumerable<byte> Compress(IEnumerable<byte> bytes, byte bitWidth)
        {
            foreach (var block in GetBlocks(bytes, bitWidth))
            {
                var blockSize = block.Contents.Count;

                var byteMapIndex = 0;
                var byteMap = block.Values.OrderBy(b => b).ToDictionary(b => b, b => (byte)byteMapIndex++);

                // Block size as uint16
                yield return (byte)(blockSize >> 8);
                yield return (byte)(blockSize & 255);

                // Byte value bitmask
                var byteMask = Enumerable.Range(0, 256).Select(i => block.Values.Contains((byte)i)).ToList().AsEnumerable();
                for (var i = 0; i < 32; i++)
                {
                    var maskPart = byteMask.Take(8);
                    byteMask = byteMask.Skip(8);

                    yield return maskPart.ToByte();
                }

                // Compressed contents
                foreach(var cb in GetCompressedBytes(block.Contents, byteMap, bitWidth))
                {
                    yield return cb;
                }
            }
        }

        public static IEnumerable<byte> Uncompress(IEnumerable<byte> bytes, byte bitWidth)
        {
            while (bytes.Any())
            {
                var blockSize = bytes.Take(2).Aggregate(0, (t, b) => (UInt16)((t << 8) + b));
                bytes = bytes.Skip(2);

                var rawByteMap = bytes.Take(32).SelectMany(b => b.ToBits()).ToList();
                bytes = bytes.Skip(32);

                var byteMap = Enumerable.Range(0, 256).Select(i => rawByteMap[i] ? i : -1).Where(i => i != -1).ToList();

                var compressedByteCount = (int)Math.Ceiling(blockSize * bitWidth / 8f);
                var compressedContents = bytes.Take(compressedByteCount);
                bytes = bytes.Skip(compressedByteCount);

                var bitBuffer = new List<bool>();
                foreach (var cb in compressedContents)
                {
                    bitBuffer.AddRange(cb.ToBits());

                    while (bitBuffer.Count >= bitWidth)
                    {
                        var part = bitBuffer.Take(bitWidth);
                        bitBuffer = bitBuffer.Skip(bitWidth).ToList();

                        yield return (byte)byteMap[part.ToByte()];
                    }
                }
            }
        }

        private static IEnumerable<Block> GetBlocks(IEnumerable<byte> bytes, byte bitWidth)
        {
            var maxValues = Math.Pow(2, bitWidth);

            var currentBlock = new Block();
            foreach (var b in bytes)
            {
                var isRepeatedByte = currentBlock.Values.Contains(b);
                if (!isRepeatedByte && currentBlock.Values.Count == maxValues || currentBlock.Contents.Count == MaxBlockSize)
                {
                    yield return currentBlock;
                    currentBlock = new Block();
                }

                if (!isRepeatedByte)
                {
                    currentBlock.Values.Add(b);
                }

                currentBlock.Contents.Add(b);
            }

            if (currentBlock.Contents.Any())
            {
                yield return currentBlock;
            }
        }

        private static IEnumerable<byte> GetCompressedBytes(IEnumerable<byte> contents, Dictionary<byte, byte> byteMap, byte bitWidth)
        {
            var bitBuffer = new List<bool>();

            foreach (var b in contents)
            {
                var mappedValue = byteMap[b];

                // Output value in bitWidth format
                bitBuffer.AddRange(mappedValue.ToBits(bitWidth));

                if (bitBuffer.Count >= 8)
                {   // If 8 bits were accumulated, output a single byte
                    var part = bitBuffer.Take(8);
                    bitBuffer = bitBuffer.Skip(8).ToList();

                    yield return part.ToByte();
                }
            }

            if (bitBuffer.Count > 0)
            {   // Output remaining contents
                var value = bitBuffer.ToByte();
                value = (byte)(value << (8 - bitBuffer.Count)); //Pad missing bits with 0

                yield return value;
            }
        }

        class Block
        {
            public readonly List<byte> Contents = new List<byte>();

            public readonly List<byte> Values = new List<byte>();
        }
    }
}
