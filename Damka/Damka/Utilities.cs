using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damka
{
    class Utilities
    {
        public static byte SetByteValue(byte b, int value, bool firstHalf)
        {
            if (firstHalf)
            {
                b = (byte)((value << 4) | (b & 0xf));
            }
            else
            {
                b = (byte)((b & 0xf0) | value);
            }
            return b;
        }

        public static int GetByteValue(byte b, bool firstHalf)
        {
            if (firstHalf)
            {
                return (b & 0xF0) >> 4;
            }
            else
            {
                return b & 0x0F;
            }
        }
    }
}
