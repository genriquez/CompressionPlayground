using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA
{
    class CodeDistance
    {
        private static IEnumerable<bool[]> EnumerateNibbles(IEnumerable<bool> binary)
        {
            var nibble = new List<bool>();

            foreach (var b in binary)
            {
                nibble.Add(b);

                if (nibble.Count == 4)
                {
                    yield return nibble.ToArray();
                    nibble.Clear();
                }
            }
        }

        public static void Calculate(IEnumerable<bool> binary)
        {
            bool[] last = null;
            var codeDistance = Enumerable.Range(0, 5).ToDictionary(i => i, i => 0);

            foreach (var nibble in EnumerateNibbles(binary))
            {
                if (last == null)
                {
                    last = nibble;
                }
                else
                {
                    var distance = Enumerable.Range(0, 4).Sum(i => last[i] != nibble[i] ? 1 : 0);
                    codeDistance[distance] = codeDistance[distance] + 1;
                }
            }

            foreach (var kvp in codeDistance)
            {
                Console.WriteLine("Distance {0}: {1} entries", kvp.Key, kvp.Value);
            }

            var totalBits = codeDistance.Sum(kvp => kvp.Value) * 4d;
            var compressedPart = codeDistance[0];
            var expandedPart = codeDistance.Where(kvp => kvp.Key > 0).Sum(kvp => kvp.Value) * 5d;

            Console.WriteLine("\nM1 - Estimated file size: {0:N2}%", 100 * (compressedPart + expandedPart) / totalBits);

            compressedPart = (codeDistance[0] + codeDistance[1]) * 3;
            expandedPart = codeDistance.Where(kvp => kvp.Key > 1).Sum(kvp => kvp.Value) * 5d;
            Console.WriteLine("\nM2 - Estimated file size: {0:N2}%", 100 * (compressedPart + expandedPart) / totalBits);
        }
    }
}
