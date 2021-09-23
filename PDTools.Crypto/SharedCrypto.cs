using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace PDTools.Crypto
{
    public class SharedCrypto
    {
        /// <summary>
        /// Original implementation of EncryptUnit. Decrypts a buffer.
        /// </summary>
        /// <param name="buffer">Input buffer.</param>
        /// <param name="length">Length of the buffer to work with.</param>
        /// <param name="crcSeed">Value for seeding the crc in the buffer to then verify the buffer's crc.</param>
        /// <param name="mult">Swap bytes multiplier 1, between 0 and 1.</param>
        /// <param name="mult">Swap bytes multiplier 2, between 0 and 1.</param>
        /// <param name="useMt">Whether to have an extra (undocumented) step.</param>
        /// <param name="bigEndian">Non original, but to specify for the type of endianess to work with.</param>
        /// <returns>Size of the decrypted data. -1 if incorrect.</returns>
        public static int EncryptUnit_Decrypt(Span<byte> buffer, int length, uint crcSeed, double mult, double mult2, bool useMt, bool bigEndian = true)
        {
            // First 8 reserved for encryption
            int actualDataSize = buffer.Length - 8;

            GT4MC_swapPlace(buffer, length, buffer, 4, mult2);

            if (useMt)
            {
                
            }

            GT4MC_swapPlace(buffer.Slice(4), length - 4, buffer.Slice(4), 4, mult);

            // PDISTD::MTRandom::MTRandom(rand, (uint*)buffer[1] + *(uint*)buffer);
            int firstVal = bigEndian ? BinaryPrimitives.ReadInt32BigEndian(buffer) : BinaryPrimitives.ReadInt32LittleEndian(buffer);
            int secondVal = bigEndian ? BinaryPrimitives.ReadInt32BigEndian(buffer.Slice(4)) : BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(4));
            uint seed = (uint)(secondVal + firstVal);
            var rand = new MTRandom(seed);

            // GT4MC::easyDecrypt((uint*)buffer + 2, dataSize, rand, startCipher);
            Span<uint> dataInts = MemoryMarshal.Cast<byte, uint>(buffer);
            if (bigEndian) // Non original, just adapted to work with both endianess
            {
                for (int i = 0; i < dataInts.Length; i++)
                    dataInts[i] = BinaryPrimitives.ReverseEndianness(dataInts[i]);
            }

            // Create the cipher based on the two key ints, decrypt the actual data 
            uint startCipher = (uint)(secondVal ^ firstVal);
            GT4MC_easyDecrypt(buffer.Slice(8), actualDataSize, rand, ref startCipher, bigEndian);

            // Seed potential CRC based on key
            Span<uint> uintBuf = MemoryMarshal.Cast<byte, uint>(buffer);
            uintBuf[1] ^= crcSeed;

            // Verify checksum of the actual encrypted data.
            if (uintBuf[1] == CRC32.crc32_0x77073096(buffer.Slice(8), actualDataSize))
                return actualDataSize;

            return -1;
        }

        public static int RandomUpdateOld1(ref uint value)
        {
            uint v1 = 17 * value + 17;
            value = v1;

            // Old LE method
            // uint low = (v1 << 16);
            // uint high = (v1 & 0xFFFF0000) >> 16;

            //uint swapped = low + high;

            var bitReverse = BitReverse(value);

            var or = bitReverse << 0x18 | (bitReverse & 0xFF00) << 0x8 | bitReverse >> 0x18 | (bitReverse >> 0x8) & 0xFF00;
            var shifted = or << 0x8;
            shifted += or;
            shifted += 0x101;

            return (int)(or ^ BitOperations.RotateRight(shifted, 0x10));
        }

    private static uint BitReverse(uint value)
    {
        var left = (uint)1 << 31;
        uint right = 1;
        uint result = 0;

        for (var i = 31; i >= 1; i -= 2)
        {
            result |= (value & left) >> i;
            result |= (value & right) << i;
            left >>= 1;
            right <<= 1;
        }
        return result;
    }

    public static void r_shufflebit(Memory<byte> buffer, int size, MTRandom randomizer)
            => Shuffle(buffer, 8 * size, randomizer, swapbit);

        public static void Shuffle(Memory<byte> buffer, int size, MTRandom randomizer,
            Action<Memory<byte>, int, int> shuffler)
        {
            int max = size - 1;

            short[] temp;
            if (size == 1)
                temp = new short[0];
            else
                temp = new short[size];

            if (size != 1)
            {
                for (int i = max; i > 0; i--)
                {
                    float randVal = randomizer.getFloat();

                    int h = i + 1;
                    float index;
                    if (h < 0)
                        index = randVal * ((h & 1 | (h >> 1)) + (h & 1 | (h >> 1)));
                    else
                        index = randVal * h;

                    temp[i - 1] = (short)index;
                }
            }

            if (size != 1)
            {
                int cPos = 0;
                for (int i = 1; i < size; i++)
                {
                    int pos = temp[cPos++];
                    shuffler(buffer, i, pos);
                }
            }


        }

        public static void swapbit(Memory<byte> data, int oldIndex, int newIndex)
        {
            int indexA = oldIndex >> 3;
            int posA = newIndex >> 3;
            int indexB = oldIndex & 7;
            int posB = newIndex & 7;

            if (oldIndex != newIndex)
            {
                byte old = data.Span[posA];

                uint v9 = 1u << posB;
                uint v10 = 1u << indexB;

                int v11 = ~(1 << posB);
                int v12 = ~(1 << indexB);

                bool unkBool = (old & v9) != 0;

                byte unk;
                if ((data.Span[indexA] & v10) != 0)
                    unk = (byte)(data.Span[posA] | v9);
                else
                    unk = (byte)(data.Span[posA] & v11);

                data.Span[posA] = unk;

                if (unkBool)
                    data.Span[indexA] |= (byte)v10;
                else
                    data.Span[indexA] &= (byte)v12;
            }
        }

        public static void GT4MC_swapPlace(Span<byte> data, int size, Span<byte> data2, int count, double mult)
        {
            int index = (size - count);
            double offsetToSwapAt = mult * index;

            GT4MC_swapPlace2(data, size, data2, count, (uint)offsetToSwapAt);
        }

        public static void GT4MC_swapPlace2(Span<byte> data, int size, Span<byte> data2, int count, uint offsetToSwapAt)
        {
            if (count == 0)
                return;

            for (int i = 0; i < count; i++)
            {
                byte swapA = data[(int)(offsetToSwapAt + i)];
                byte swapB = data2[i];

                data[(int)(offsetToSwapAt + i)] = swapB;
                data2[i] = swapA;
            }
        }

        private static void GT4MC_easyDecrypt(Span<byte> data, int len, MTRandom rand, ref uint seed, bool bigEndian)
        {
            int pos = 0;
            while (pos != len && pos + 4 <= len)
            {
                int pseudoRandVal = rand.getInt32();
                int updated = RandomUpdateOld1(ref seed);

                Span<uint> current = MemoryMarshal.Cast<byte, uint>(data);

                uint result = (uint)((current[0] + updated) ^ pseudoRandVal);
                current[0] = bigEndian ? BinaryPrimitives.ReverseEndianness(result) : result;

                pos += 4;
                data = data.Slice(4);
            }

            while (pos != len)
            {
                int pseudoRandVal = rand.getInt32();
                int updated = RandomUpdateOld1(ref seed);

                uint result = (uint)((data[0] + updated) ^ pseudoRandVal);
                data[0] = (byte)result;

                pos++;
                data = data[1..];
            }
        }
    }
}
