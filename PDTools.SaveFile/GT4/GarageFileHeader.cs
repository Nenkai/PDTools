using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.IO;

using PDTools.Crypto;

namespace PDTools.SaveFile.GT4
{
    public class GarageFile
    {
        public const int GarageFileHeaderSize = 0x40;
        public const int GarageCarSize = 0x540;

        public void Load(string garageFilePath, uint key)
        {
            byte[] garageFile = File.ReadAllBytes(garageFilePath);

            decryptHeader(garageFile, key);

            for (uint i = 0; i < 1000; i++)
            {
                Memory<byte> carBuffer = garageFile.AsMemory(GarageFileHeaderSize + (GarageCarSize * (int)i));
                DecryptCar(carBuffer, key, i);
            }
        }

        public static void decryptHeader(Memory<byte> buffer, uint baseKey)
        {
            uint ogKey = baseKey;

            int seed1 = SharedCrypto.RandomUpdateOld1(ref baseKey);
            int seed2 = SharedCrypto.RandomUpdateOld1(ref baseKey);

            var rand = new MTRandom((uint)seed2);
            SharedCrypto.r_shufflebit(buffer, 0x40, rand);

            uint ciph = (uint)(BinaryPrimitives.ReadUInt32LittleEndian(buffer.Span.Slice(0x3C)) ^ seed1);
            rand = new MTRandom(ogKey + ciph);

            for (int i = 0; i < 0x3C; i++)
                buffer.Span[i] ^= (byte)rand.getInt32();
        }

        private static void DecryptCar(Memory<byte> carBuffer, uint uniqueIdKey, uint carIndex)
        {
            var rand = new MTRandom(uniqueIdKey + carIndex);
            SharedCrypto.r_shufflebit(carBuffer, GarageCarSize, rand);

            uint seed = BinaryPrimitives.ReadUInt32LittleEndian(carBuffer.Span);
            uint crc = BinaryPrimitives.ReadUInt32LittleEndian(carBuffer.Span.Slice(4));

            var rand2 = new MTRandom(uniqueIdKey ^ seed);

            Span<uint> bufInts = MemoryMarshal.Cast<byte, uint>(carBuffer.Span.Slice(8));
            for (var i = 0; i < (GarageCarSize - 8) / 4; i++)
            {
                bufInts[0] = (bufInts[0] + (uint)SharedCrypto.RandomUpdateOld1(ref crc)) ^ (uint)rand2.getInt32();
                bufInts = bufInts.Slice(1);
            }
        }
    }
}
