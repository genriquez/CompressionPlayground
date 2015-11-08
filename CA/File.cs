using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA
{
    class File
    {
        public static IEnumerable<byte> OpenAsByteCollection(string filename)
        {
            using (var file = System.IO.File.Open(filename, FileMode.Open))
            using (var reader = new BinaryReader(file))
            {
                while (true)
                {
                    byte b;

                    try { b = reader.ReadByte(); }
                    catch (EndOfStreamException) { break; }

                    yield return b;
                }
            }
        }

        public static IEnumerable<bool> OpenAsBooleanCollection(string filename)
        {
            foreach (var b in OpenAsByteCollection(filename))
            {
                var i = 128;
                while (i > 0)
                {
                    i = i >> 1;

                    yield return (b & i) > 0;
                }
            }
        }

        public static void Write(string filename, IEnumerable<bool> binary)
        {
            using (var file = System.IO.File.Open(filename, FileMode.Truncate))
            using (var writer = new BinaryWriter(file))
            {
                var buffer = new List<byte>();

                foreach (var bit in binary)
                {
                    buffer.Add((byte)(bit ? 1 : 0));

                    if (buffer.Count == 8)
                    {
                        byte b = buffer.Aggregate((byte)0, (total, current) => (byte)((total << 1) + current));

                        writer.Write(b);
                        buffer.Clear();
                    }
                }
            }
        }

        public static void Write(string filename, IEnumerable<byte> binary)
        {
            using (var file = System.IO.File.Open(filename, FileMode.Truncate))
            using (var writer = new BinaryWriter(file))
            {
                foreach (var b in binary)
                {
                    writer.Write(b);
                }
            }
        }
    }
}
