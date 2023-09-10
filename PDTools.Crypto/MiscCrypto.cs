using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Hashing;

namespace PDTools.Crypto
{
    public class MiscCrypto
    {
        /// <summary>
        /// Used for some online grim file updating
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ulong UpdateShiftValue(ulong value)
        {
            uint v1 = 0x0, v2 = 0;
            
            uint bitMask = 0x1;
            while (value != 0)
            {
                ulong b1 = value & 0x1;
                ulong b2 = value & 0x2;
                value >>= 0x2;
                if (b1 != 0x0)
                    v1 |= bitMask;

                if (b2 != 0x0)
                    v2 |= bitMask;

                bitMask <<= 0x1;
            }

            uint v1Crc = CRC32.CRC32_0x04C11DB7_UIntInverted(v1);
            uint v2Crc = CRC32.CRC32_0x04C11DB7_UIntInverted(v2);
            
            bitMask = 0x1;
            ulong result = 0x0;

            int bitCount = 0x20;
            while (bitCount != 0x0)
            {
                if (bitCount == 1)
                    ;

                uint b1 = bitMask & (v1Crc << 0x19 | v1Crc >> 0x7);
                uint b2 = bitMask & (v2Crc << 0x11 | v2Crc >> 0xf);

                result <<= 0x2;
                
                if (b1 != 0x0)
                    result |= 0x1;

                if (b2 != 0x0)
                    result |= 0x2;

                bitMask <<= 0x1;
                bitCount--;
            } 

            return result;
        }

        /*
        public static ulong UpdateShiftValueReverse(ulong value)
        {
            ulong result = 0;

            uint v1 = 0, v2 = 0;

            int bitCount = 0x20;
            uint bitMask = 0x80000000;

            while (bitCount != 0x0)
            {
                if ((value & 1) != 0)
                    v1 |= bitMask;

                if ((value & 2) != 0)
                    v2 |= bitMask;

                value >>= 0x2;
                bitMask >>= 0x1;

                bitCount--;
            }

            uint ogCrc1 = (v1 >> 0x19) | (v1 << 0x7);
            uint ogCrc2 = (v2 >> 0x11) | (v2 << 0xf);
            CRC32.CRC32_0x04C11DB7_UIntInvertedReverse(ogCrc2);

            return result;
        }*/
    }
}
