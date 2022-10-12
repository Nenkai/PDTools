using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;
using System.IO;

using PDTools.Crypto;
using PDTools.Utils;

namespace PDTools.SaveFile.GT4
{
    public class GT4GameData
    {
        public const float Mult = 0.13579f;
        public const float Mult2 = 0.65486f;

        public const int SerializePackHeaderLength = 0x10;

        public byte[] Buffer { get; set; }

        public bool UseOldRandomUpdateCrypto { get; set; } = true;

        public void LoadFile(string gameDataFilePath)
        {
            byte[] rawSaveFile = File.ReadAllBytes(gameDataFilePath);

            // Decrypt whole file
            DecryptSave(rawSaveFile);

            if (Buffer.Length == 0x3A070)
            {
                // GT4 (EU)
            }
            else if (Buffer.Length == 0x3A160)
            {
                // GT4O (US)
            }
#if DEBUG
            File.WriteAllBytes("gt4_eu_save.out", Buffer);
#endif
        }

        private void DecryptSave(byte[] rawSaveFile)
        {
            byte[] workBuffer = new byte[rawSaveFile.Length];
            rawSaveFile.AsSpan().CopyTo(workBuffer);

            int decryptedLen = SharedCrypto.EncryptUnit_Decrypt(workBuffer, workBuffer.Length, 0, Mult, Mult2, useMt: false, bigEndian: false, randomUpdateOld1_OldVersion: true);
            if (decryptedLen == -1)
            {
                // GT4O and above uses a tweaked version of RandomUpdateOld1, try it
                rawSaveFile.AsSpan().CopyTo(workBuffer);
                decryptedLen = SharedCrypto.EncryptUnit_Decrypt(workBuffer, workBuffer.Length, 0, Mult, Mult2, useMt: false, bigEndian: false, randomUpdateOld1_OldVersion: false);
                if (decryptedLen == -1)
                    throw new Exception("Failed to decrypt the GT4 Save.");

                UseOldRandomUpdateCrypto = false;
            }

            Span<byte> packBuffer = workBuffer.AsSpan(SharedCrypto.EncryptUnit_HdrLen, workBuffer.Length - SharedCrypto.EncryptUnit_HdrLen);
            if (!VerifyPackHeader(packBuffer, out Span<byte> saveBuffer))
                throw new Exception("Failed to decrypt the GT4 Pack Header save.");

            Buffer = saveBuffer.ToArray();
        }

        private static byte[] EncryptGameDataFileBuffer(Span<byte> saveBuffer)
        {
            byte[] packBuffer = SerializePack(saveBuffer);

            uint fileLength = MiscUtils.AlignValue((uint)(SharedCrypto.EncryptUnit_HdrLen + packBuffer.Length), 0x10);
            byte[] fileBuffer = new byte[fileLength];
            packBuffer.CopyTo(fileBuffer.AsSpan(SharedCrypto.EncryptUnit_HdrLen));

            SharedCrypto.EncryptUnit_Encrypt(fileBuffer, fileBuffer.Length, 0, Mult, Mult2, useMt: false, bigEndian: false);
            return fileBuffer;
        }

        private static byte[] SerializePack(Span<byte> saveBuffer)
        {
            // Not the most efficient way but we'll do it like the game does
            byte[] packBuffer = new byte[SerializePackHeaderLength + saveBuffer.Length];
            saveBuffer.CopyTo(packBuffer.AsSpan(SerializePackHeaderLength));

            // Write pack header
            Span<byte> packPtr = packBuffer;
            BinaryPrimitives.WriteUInt32LittleEndian(packPtr, 0xFF60BAD3);
            BinaryPrimitives.WriteInt32LittleEndian(packPtr.Slice(4), -saveBuffer.Length);
            BinaryPrimitives.WriteUInt32LittleEndian(packPtr.Slice(8), CRC32.crc32_0x77073096(saveBuffer, saveBuffer.Length));
            // Padding

            return packBuffer;
        }

        private static bool VerifyPackHeader(Span<byte> packBuffer, out Span<byte> saveBuffer)
        {
            uint magic = BinaryPrimitives.ReadUInt32LittleEndian(packBuffer);
            if (magic != 0xFF60BAD3)
                throw new InvalidDataException("Save magic did not match.");

            int inputLength = -BinaryPrimitives.ReadInt32LittleEndian(packBuffer.Slice(0x04));
            uint crc = BinaryPrimitives.ReadUInt32LittleEndian(packBuffer.Slice(0x08));

            if (CRC32.crc32_0x77073096(packBuffer.Slice(0x10), inputLength) == crc)
            {
                saveBuffer = packBuffer.Slice(0x10, inputLength);
                return true;
            }
            else
            {
                saveBuffer = default;
                return false;
            }
        }

        public static void EncryptGameDataSaveFile(string inputFile)
        {
            byte[] saveBuffer = File.ReadAllBytes(inputFile);
            byte[] encryptedSaveFile = EncryptGameDataFileBuffer(saveBuffer);
            File.WriteAllBytes(inputFile + ".enc", encryptedSaveFile);
        }
    }
}
