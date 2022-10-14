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
        public const int GarageCarSize_Retail = 0x500;
        public const int GarageCarSize_New = 0x540;

        public int GarageCarSize { get; set; }
        public bool UseOldRandomUpdateCrypto { get; set; } = true;

        public void Load(GT4Save save, string garageFilePath, uint key, bool useOldRandomUpdateCrypto)
        {
            UseOldRandomUpdateCrypto = useOldRandomUpdateCrypto;

            if (save.IsGT4Online())
                GarageCarSize = GarageCarSize_New;
            else
                GarageCarSize = GarageCarSize_Retail;

            byte[] garageFile = File.ReadAllBytes(garageFilePath);

            decryptHeader(garageFile, key);

            for (uint i = 0; i < 1000; i++)
            {
                Memory<byte> carBuffer = garageFile.AsMemory(GarageFileHeaderSize + (GarageCarSize * (int)i));
                DecryptCar(carBuffer, key, i);
            }
        }

        public void decryptHeader(Memory<byte> buffer, uint key)
        {
            Span<uint> hdr = MemoryMarshal.Cast<byte, uint>(buffer.Span);

            // Keep original key around; this will be changed by RandomUpdateOld1
            uint ogKey = key;

            int seed1 = SharedCrypto.RandomUpdateOld1(ref key, UseOldRandomUpdateCrypto);
            int seed2 = SharedCrypto.RandomUpdateOld1(ref key, UseOldRandomUpdateCrypto);

            var rand = new MTRandom((uint)seed2);
            SharedCrypto.reverse_shufflebit(buffer, 0x40, rand);

            uint ciph = (uint)(hdr[15] ^ seed1);
            hdr[15] = ciph;

            rand = new MTRandom(ogKey + ciph);

            for (int i = 0; i < 0x3C; i++)
                buffer.Span[i] ^= (byte)rand.getInt32();
        }

        public void encryptHeader(Memory<byte> buffer)
        {
            Span<uint> hdr = MemoryMarshal.Cast<byte, uint>(buffer.Span);

            uint key = hdr[14];
            var rand = new MTRandom(key + hdr[15]);
            
            for (int i = 0; i < 0x3C; i++)
                buffer.Span[i] ^= (byte)rand.getInt32();

            hdr[15] = (uint)SharedCrypto.RandomUpdateOld1(ref key, UseOldRandomUpdateCrypto);
            var updated = SharedCrypto.RandomUpdateOld1(ref key, UseOldRandomUpdateCrypto);

            rand = new MTRandom((uint)updated);
            SharedCrypto.shufflebit(buffer, 0x40, rand); // Shuffle back to normal, no reverse
        }

        private void DecryptCar(Memory<byte> carBuffer, uint uniqueIdKey, uint carIndex)
        {
            var rand = new MTRandom(uniqueIdKey + carIndex);
            SharedCrypto.reverse_shufflebit(carBuffer, GarageCarSize, rand);

            uint seed = BinaryPrimitives.ReadUInt32LittleEndian(carBuffer.Span);
            uint crc = BinaryPrimitives.ReadUInt32LittleEndian(carBuffer.Span.Slice(4));

            rand = new MTRandom(uniqueIdKey ^ seed);

            Span<uint> bufInts = MemoryMarshal.Cast<byte, uint>(carBuffer.Span.Slice(8));
            for (var i = 0; i < (GarageCarSize - 8) / 4; i++)
            {
                bufInts[0] = (bufInts[0] + (uint)SharedCrypto.RandomUpdateOld1(ref crc, UseOldRandomUpdateCrypto)) ^ (uint)rand.getInt32();
                bufInts = bufInts.Slice(1);
            }
        }
    }
}
