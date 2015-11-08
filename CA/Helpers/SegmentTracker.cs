using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Helpers
{
    class SegmentTracker
    {
        private byte segmentSize;

        private List<bool> buffer = new List<bool>();

        public SegmentTracker(byte segmentSize)
        {
            this.segmentSize = segmentSize;
        }

        public event OnNewSegmentHandler OnNewSegment;

        public delegate void OnNewSegmentHandler(byte segment);

        public void Push(bool bit)
        {
            buffer.Add(bit);

            if (buffer.Count == segmentSize)
            {
                var segment = (byte)buffer.Aggregate(0, (o, b) => (o << 1) + (b ? 1 : 0));
                OnNewSegment(segment);

                buffer.Clear();
            }
        }
    }
}
