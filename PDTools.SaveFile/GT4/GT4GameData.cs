using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;
using System.IO;
using Syroot.BinaryData.Memory;

using PDTools.Crypto;
using PDTools.Utils;

using PDTools.SaveFile.GT4.UserProfile;
using PDTools.SaveFile.GT4.Option;

namespace PDTools.SaveFile.GT4
{
    public class GT4GameData
    {
        public const float Mult = 0.13579f;
        public const float Mult2 = 0.65486f;

        public const int SerializePackHeaderLength = 0x10;

        public byte[] Buffer { get; set; }

        public bool UseOldRandomUpdateCrypto { get; set; } = true;

        public UserProfileGT4 Profile { get; set; } = new UserProfileGT4();
        public OptionGT4 Option { get; set; } = new OptionGT4();
        public ContextGT4 Context { get; set; } = new ContextGT4();

        public void LoadFile(GT4Save gt4Save, string gameDataFilePath)
        {
            byte[] rawSaveFile = File.ReadAllBytes(gameDataFilePath);

            // Decrypt whole file
            DecryptSave(rawSaveFile);

            if (Buffer.Length == 0x3A070)
            {
                gt4Save.GameType = GT4GameType.GT4_EU;
            }
            else if (Buffer.Length == 0x3A160)
            {
                // GT4O (US)
                if (gt4Save.GameType == GT4GameType.GT4_US)
                    gt4Save.GameType = GT4GameType.GT4O_US;
            }

            ReadSave(gt4Save);

#if DEBUG
            // For testing whether saves are decrypted -> read -> written -> encrypted fine
            File.WriteAllBytes("save.out", Buffer);

            byte[] resaved = WriteSave(gt4Save);
            File.WriteAllBytes("save.out_repacked", resaved);

            byte[] encrypted = EncryptSave(resaved);
            DecryptSave(encrypted);

            File.WriteAllBytes("save.out_repak_encr_decrypt", resaved);
#endif
        }

        private void ReadSave(GT4Save gt4Save)
        {
            SpanReader sr = new SpanReader(Buffer);
            Profile.Unpack(gt4Save, ref sr);
            Option.Unpack(gt4Save, ref sr);
            Context.Unpack(gt4Save, ref sr);
        }

        private byte[] WriteSave(GT4Save gt4Save)
        {
            byte[] output = new byte[Buffer.Length];
            SpanWriter sw = new SpanWriter(output);
            Profile.Pack(gt4Save, ref sw);
            Option.Pack(gt4Save, ref sw);
            Context.Pack(gt4Save, ref sw);

            return output;
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

        private static byte[] EncryptSave(Span<byte> saveBuffer)
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
            byte[] encryptedSaveFile = EncryptSave(saveBuffer);
            File.WriteAllBytes(inputFile + ".enc", encryptedSaveFile);
        }
    }
}
