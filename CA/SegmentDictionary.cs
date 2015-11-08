using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CA.Helpers;

namespace CA
{
    class SegmentDictionary
    {
        public static void Calculate(IEnumerable<bool> binary, byte segmentSize)
        {
            var segmentOcurrence = Enumerable.Range(0, (int)Math.Pow(2, segmentSize)).ToDictionary(i => (byte)i, i => 0);

            var tracker = new SegmentTracker(segmentSize);
            tracker.OnNewSegment += segment => segmentOcurrence[segment] = segmentOcurrence[segment] + 1;

            foreach(var bit in binary)
            {
                tracker.Push(bit);
            }

            foreach(var kvp in segmentOcurrence)
            {
                Console.WriteLine("Segment {0} ocurred {1} times", Convert.ToString(kvp.Key, 2).PadLeft(segmentSize, '0'), kvp.Value);
            }
        }
    }
}
