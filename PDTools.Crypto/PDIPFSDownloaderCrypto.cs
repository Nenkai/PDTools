using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Buffers.Binary;
namespace PDTools.Crypto
{
    public class PDIPFSDownloaderCrypto
    {
        private static int RoundUp(int toRound, int multiple)
        {
            if (toRound % multiple == 0) return toRound;
            return (multiple - toRound % multiple) + toRound;
        }

        /// <summary>
        /// Must be applied on offsets aligned on 8 (or remainders)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="seed"></param>
        public static void Encrypt(Span<byte> buffer, ref ulong seed)
        {
            int minLongLen = buffer.Length / sizeof(long); // Get taken space in longs
            var bufLongs = MemoryMarshal.Cast<byte, ulong>(buffer);

            // 8 per 8 bytes
            int offset = 0;
            for (int i = 0; i < minLongLen; i++)
            {
                var decValue = BinaryPrimitives.ReverseEndianness(bufLongs[0]);
                bufLongs[0] = BinaryPrimitives.ReverseEndianness(decValue ^ seed);
                seed = PDIPFSDownloaderCrypto.UpdateShiftValue(decValue ^ seed); // seed using encrypted long

                bufLongs = bufLongs.Slice(1);
                offset += sizeof(long);
            }

            // Rem bytes
            int nRemBytes = buffer.Length % sizeof(long);
            if (nRemBytes != 0)
            {
                Span<byte> remBytes = stackalloc byte[8];
                buffer.Slice(offset).CopyTo(remBytes);

                ulong decValue = BinaryPrimitives.ReadUInt64LittleEndian(remBytes);
                decValue = BinaryPrimitives.ReverseEndianness(decValue);
                ulong result = BinaryPrimitives.ReverseEndianness(decValue ^ seed);
                seed = PDIPFSDownloaderCrypto.UpdateShiftValue(decValue ^ seed);

                byte[] res = BitConverter.GetBytes(result);
                res.AsSpan(0, nRemBytes).CopyTo(buffer.Slice(offset));
            }
        }

        /// <summary>
        /// Must be applied on offsets aligned on 8 (or remainders)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="seed"></param>
        public static void Decrypt(Span<byte> buffer, ref ulong seed)
        {
            int minLongLen = buffer.Length / sizeof(long); // Get taken space in longs

            // 8 per 8 bytes
            var bufLongs = MemoryMarshal.Cast<byte, ulong>(buffer);
            int offset = 0;
            for (int i = 0; i < minLongLen; i++)
            {
                var encValue = BinaryPrimitives.ReverseEndianness(bufLongs[0]);
                bufLongs[0] = BinaryPrimitives.ReverseEndianness(encValue ^ seed);
                seed = PDIPFSDownloaderCrypto.UpdateShiftValue(encValue); // seed using encrypted long

                bufLongs = bufLongs.Slice(1);
                offset += sizeof(long);
            }

            // Rem bytes
            int nRemBytes = buffer.Length % sizeof(long);
            if (nRemBytes != 0)
            {
                /*
                Span<byte> remBytes = buffer[offset..];
                for (int i = 0; i < nRemBytes; i++)
                {
                    byte val = remBytes[i];
                    remBytes[i] = (byte)(val ^ (byte)BinaryPrimitives.ReverseEndianness(seed));
                    seed <<= 8;
                }*/

                Span<byte> remBytes = stackalloc byte[8];
                buffer.Slice(offset).CopyTo(remBytes);

                ulong encValue = BinaryPrimitives.ReadUInt64LittleEndian(remBytes);
                encValue = BinaryPrimitives.ReverseEndianness(encValue);
                ulong result = BinaryPrimitives.ReverseEndianness(encValue ^ seed);
                seed = PDIPFSDownloaderCrypto.UpdateShiftValue(encValue);

                byte[] res = BitConverter.GetBytes(result);
                res.AsSpan(0, nRemBytes).CopyTo(buffer.Slice(offset));
            }
        }


        /// <summary>
        /// Updates and seeds a 8 bytes long to be suited for encryption/decryption
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ulong UpdateShiftValue(ulong value)
        {
            uint v1 = 0x0, v2 = 0;

            uint bitMask = 0x1;
            do
            {
                ulong b1 = value & 0x1;
                ulong b2 = value & 0x2;
                value >>= 0x2;
                if (b1 != 0x0)
                    v1 |= bitMask;

                if (b2 != 0x0)
                    v2 |= bitMask;

                bitMask <<= 0x1;
            } while (value != 0);

            uint v1Crc = CRC32.CRC32_0x04C11DB7_UIntInverted(v1);
            uint v2Crc = CRC32.CRC32_0x04C11DB7_UIntInverted(v2);

            uint bMask1 = 0x1;
            ulong bMask2 = 0x0;

            int bitCount = 0x20;
            do
            {
                uint b1 = bMask1 & (v1Crc << 0x19 | v1Crc >> 0x7);
                uint b2 = bMask1 & (v2Crc << 0x11 | v2Crc >> 0xf);

                bMask1 <<= 0x1;
                bMask2 <<= 0x2;

                if (b1 != 0x0)
                    bMask2 |= 0x1;

                if (b2 != 0x0)
                    bMask2 |= 0x2;

                bitCount--;
            } while (bitCount != 0);

            return bMask2;
        }


    }
}
