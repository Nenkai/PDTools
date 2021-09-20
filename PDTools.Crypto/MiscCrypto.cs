using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            uint v1Crc = CRC32.CRC32_0x04C11DB7_UInt(v1);
            uint v2Crc = CRC32.CRC32_0x04C11DB7_UInt(v2);
            
            v1 = 0x1;
            v2 = 0x0;

            int bitCount = 0x20;
            while (bitCount != 0x0)
            {
                uint b1 = v1 & (v1Crc << 0x19 | v1Crc >> 0x7);
                uint b2 = v2 & (v2Crc << 0x11 | v2Crc >> 0xf);

                v1 <<= 0x2;
                v2 <<= 0x1;

                if (b1 != 0x0)
                    v1 |= 0x1;

                if (b2 != 0x0)
                    v2 |= 0x2;

                bitCount--;
            } 

            return v1;
        }
    }
}
