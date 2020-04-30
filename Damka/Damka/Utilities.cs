using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damka
{
    class Utilities
    {
        public static byte TwoIntToByte(int first, int second)
        {
            byte firstByte = (byte)(first << 4);
            byte secondByte = ((byte)second);
            byte lastByte = (byte)(firstByte | secondByte);
            return lastByte;
        }

        public static int[] ByteToTwoInts(byte b)
        {
            int first = (b & 0xF0) >> 4;
            int second = b & 0x0F;
            return new int[2] { first, second };
        }

        public static byte SetByteValue(byte b, int value, bool firstHalf)
        {
            if (firstHalf)
            {
                b = (byte)(value << 4);
            }
            else
            {
                b = (byte)(b & 0xF0);
                b = (byte)(b | value);
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
