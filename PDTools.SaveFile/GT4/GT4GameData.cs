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
        public void LoadFile(string gameDataFilePath)
        {
#if DEBUG
            if (File.Exists("file.bin"))
                File.Delete("file.bin");

            File.Copy(gameDataFilePath, "file.bin");

            byte[] rawSaveFile = File.ReadAllBytes("file.bin");
#endif

            // 240c8e8d
            // Decrypt whole file
            int decryptedLen = SharedCrypto.EncryptUnit_Decrypt(rawSaveFile, rawSaveFile.Length, 0, Mult, Mult2, useMt: false, bigEndian: false);
            if (decryptedLen == -1)
                throw new Exception("Failed to decrypt the GT4 save.");

            Span<byte> packBuffer = rawSaveFile.AsSpan(SharedCrypto.EncryptUnit_HdrLen, rawSaveFile.Length - SharedCrypto.EncryptUnit_HdrLen);
            if (!VerifyPackHeader(packBuffer, out Span<byte> saveBuffer))
                throw new Exception("Failed to decrypt the GT4 Pack Header save.");

            Buffer = saveBuffer.ToArray();

#if DEBUG
            File.WriteAllBytes("save.out", Buffer);
#endif
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
