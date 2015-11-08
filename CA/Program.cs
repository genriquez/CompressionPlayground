using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA
{
    class Program
    {
        static void Main(string[] args)
        {
            //var binary = File.OpenAsBooleanCollection("Content\\file.bin");
            var bytes = File.OpenAsByteCollection("Content\\file5.bin");

            //CodeDistance.Calculate(binary);
            //SegmentDictionary.Calculate(binary, 8);
            //BlockSubset.Calculate(bytes, 6);

            var compressed = BlockSubset.Compress(bytes, 6).ToList();
            var uncompressed = BlockSubset.Uncompress(compressed, 6).ToList();

            Console.WriteLine("Original file size: {0} bytes", bytes.Count());
            Console.WriteLine("Compressed file size: {0} bytes", compressed.Count);
            Console.WriteLine("Uncompressed file size: {0} bytes", uncompressed.Count);

            File.Write("Content\\target.bin", uncompressed);

            Console.WriteLine("\n-- END --");
            Console.ReadLine();
        }
    }
}
