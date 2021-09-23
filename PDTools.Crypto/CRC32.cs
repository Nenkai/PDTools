using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Crypto
{
    public static class CRC32
    {
		private const uint _poly1 = 0xedb88320;
		public static uint[] checksum_0x77073096 = new uint[256];
		public static uint[] checksum_0x77073096_rev = new uint[256];
		private const int _poly2 = 0x04c11db7;
		public static uint[] checksum_0x04C11DB7 = new uint[256];
		public static uint[] checksum_0x04C11DB7_rev = new uint[256];

		static CRC32()
		{
			uint fwd, rev;
			for (uint i = 0; i < checksum_0x77073096.Length; i++)
			{
				fwd = i;
				rev = i << 24;
				for (int j = 8; j > 0; j--)
				{
					if ((fwd & 1) == 1)
						fwd = ((fwd >> 1) ^ _poly1);
					else
						fwd >>= 1;

					if ((rev & 0x80000000) != 0)
						rev = ((rev ^ _poly1) << 1) | 1;
					else
						rev <<= 1;
				}
				checksum_0x77073096[i] = fwd;
				checksum_0x77073096_rev[i] = rev;
			}

			for (uint i = 0; i < checksum_0x04C11DB7.Length; i++)
			{
				fwd = i << 24;
				rev = i;

				for (int j = 8; j > 0; j--)
				{
					if ((fwd & 0x80000000) != 0)
						fwd = (fwd << 1) ^ _poly2;
					else
						fwd <<= 1;

					
					if ((rev & 1) == 1)
						rev = ((rev >> 1) ^ _poly2);
					else
						rev >>= 1;
				}
				checksum_0x04C11DB7[i] = fwd;
				checksum_0x04C11DB7_rev[i] = rev;
			}
		}

		public static uint CRC32_0x04C11DB7(string keyMagic, uint initialValue = 0)
		{
			uint crc = initialValue;
			for (byte i = 0; i < keyMagic.Length; i++)
			{
				int b = keyMagic[i] & 0xFF;
				crc = (crc << 8) ^ checksum_0x04C11DB7[(crc >> 24) ^ b];
			}

			return crc;
		}

		public static uint CRC32_0x04C11DB7(Span<byte> bytes, uint initialValue = 0)
		{
			uint crc = initialValue;
			for (byte i = 0; i < bytes.Length; i++)
			{
				int b = bytes[i] & 0xFF;
				crc = (crc << 8) ^ checksum_0x04C11DB7[(crc >> 24) ^ b];
			}

			return crc;
		}

		/// <summary>
		/// For GT5P JP Demo and others
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static uint CRC32_0x04C11DB7_UIntInverted(uint input)
		{
			uint result = 0x0;
			for (int i = 0; i < 4; i++)
			{
				result = (result << 0x8) ^ CRC32.checksum_0x04C11DB7[(result ^ input) >> 24];
				input <<= 0x8;
			}
			return ~result;
		}

		/*
		public static uint CRC32_0x04C11DB7_UIntInvertedReverse(uint input)
		{
			uint inputNormal = ~input;
			uint result = 0;

			byte[] bytes = BitConverter.GetBytes(inputNormal);

			for (int i = 0; i < 4; i++)
			{
				result = (result << 0x8) ^ CRC32.checksum_0x04C11DB7_rev[(result >> 0x24) ^ bytes[i]];
				
				
			}

			return result;

		}*/

		public static uint crc32_0x77073096(Span<byte> data, int length)
        {
            uint checksum = 0xFFFFFFFF;
            if (length > 0)
            {
                for (int i = 0; i < data.Length; i++)
                    checksum = checksum_0x77073096[(byte)(checksum ^ data[i])] ^ (checksum >> 8);
            }

            return ~checksum;
        }
	}
}
