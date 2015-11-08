using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Helpers
{
    public static class BinaryConversionExtensions
    {
        public static byte ToByte(this IEnumerable<bool> binary)
        {
            return binary.Aggregate((byte)0, (b, m) => (byte)((b << 1) + (m ? 1 : 0)));
        }

        public static IEnumerable<bool> ToBits(this byte b, byte bitWidth = 8)
        {
            return Enumerable.Range(0, bitWidth).Select(i => (b & (byte)Math.Pow(2, i)) > 0).Reverse();
        }
    }
}
