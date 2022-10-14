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

        public bool UseOldRandomUpdateCrypto { get; set; } = true;

        public UserProfileGT4 Profile { get; set; } = new UserProfileGT4();
        public OptionGT4 Option { get; set; } = new OptionGT4();
        public ContextGT4 Context { get; set; } = new ContextGT4();

        public int SaveLength { get; set; }

        public void LoadFile(GT4Save gt4Save, string gameDataFilePath)
        {
            byte[] rawSaveFile = File.ReadAllBytes(gameDataFilePath);

            // Decrypt whole file
            if (!DecryptSave(rawSaveFile, out Memory<byte> decrypted))
            {
                throw new Exception("Failed to decrypt the GT4 save.");
            }

#if DEBUG
            // For testing whether saves are decrypted -> read -> written -> encrypted fine
            File.WriteAllBytes("save.out", decrypted.ToArray());
#endif

            if (decrypted.Length == 0x3A070)
            {
                if (gt4Save.IsGT4Retail())
                {
                    // Warn
                }
            }
            else if (decrypted.Length == 0x3A160)
            {
                // GT4O (US, JP)
                if (!gt4Save.IsGT4Online())
                {
                    // Warn
                }

            }

            SaveLength = decrypted.Length;
            ReadSave(gt4Save, decrypted);


#if DEBUG
            byte[] resaved = WriteSave(gt4Save);
            File.WriteAllBytes("save.out_repacked", resaved);

            byte[] encrypted = EncryptSave(resaved);
            DecryptSave(encrypted, out Memory<byte> redecrypted);

            File.WriteAllBytes("save.out_repak_encr_decrypt", redecrypted.ToArray());
#endif
        }

        private void ReadSave(GT4Save gt4Save, Memory<byte> buffer)
        {
            SpanReader sr = new SpanReader(buffer.Span);
            Profile.Unpack(gt4Save, ref sr);
            Option.Unpack(gt4Save, ref sr);
            Context.Unpack(gt4Save, ref sr);
        }

        public void SaveTo(GT4Save save, string fileName)
        {
            byte[] output = WriteSave(save);
            byte[] encrypted = EncryptSave(output);
            File.WriteAllBytes(fileName, encrypted);
        }

        private byte[] WriteSave(GT4Save gt4Save)
        {
            byte[] output = new byte[SaveLength];
            SpanWriter sw = new SpanWriter(output);
            Profile.Pack(gt4Save, ref sw);
            Option.Pack(gt4Save, ref sw);
            Context.Pack(gt4Save, ref sw);

            return output;
        }

        private bool DecryptSave(byte[] rawSaveFile, out Memory<byte> saveBuffer)
        {
            saveBuffer = default;

            byte[] workBuffer = new byte[rawSaveFile.Length];
            rawSaveFile.AsSpan().CopyTo(workBuffer);

            int decryptedLen = SharedCrypto.EncryptUnit_Decrypt(workBuffer, workBuffer.Length, 0, Mult, Mult2, useMt: false, bigEndian: false, randomUpdateOld1_OldVersion: true);
            if (decryptedLen == -1)
            {
                // GT4O and above uses a tweaked version of RandomUpdateOld1, try it
                rawSaveFile.AsSpan().CopyTo(workBuffer);
                decryptedLen = SharedCrypto.EncryptUnit_Decrypt(workBuffer, workBuffer.Length, 0, Mult, Mult2, useMt: false, bigEndian: false, randomUpdateOld1_OldVersion: false);
                if (decryptedLen == -1)
                    return false;

                UseOldRandomUpdateCrypto = false;
            }

            Memory<byte> packBuffer = workBuffer.AsMemory(SharedCrypto.EncryptUnit_HdrLen, workBuffer.Length - SharedCrypto.EncryptUnit_HdrLen);
            if (!VerifyPackHeader(packBuffer, out saveBuffer))
                return false;

            return true;
        }

        private byte[] EncryptSave(Span<byte> saveBuffer)
        {
            byte[] packBuffer = SerializePack(saveBuffer);

            uint fileLength = MiscUtils.AlignValue((uint)(SharedCrypto.EncryptUnit_HdrLen + packBuffer.Length), 0x40);
            byte[] fileBuffer = new byte[fileLength];
            packBuffer.CopyTo(fileBuffer.AsSpan(SharedCrypto.EncryptUnit_HdrLen));

            SharedCrypto.EncryptUnit_Encrypt(fileBuffer, fileBuffer.Length, 0, Mult, Mult2, useMt: false, bigEndian: false, UseOldRandomUpdateCrypto);
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

        private static bool VerifyPackHeader(Memory<byte> packBuffer, out Memory<byte> saveBuffer)
        {
            uint magic = BinaryPrimitives.ReadUInt32LittleEndian(packBuffer.Span);
            if (magic != 0xFF60BAD3)
                throw new InvalidDataException("Save magic did not match.");

            int inputLength = -BinaryPrimitives.ReadInt32LittleEndian(packBuffer.Span.Slice(0x04));
            uint crc = BinaryPrimitives.ReadUInt32LittleEndian(packBuffer.Span.Slice(0x08));

            if (CRC32.crc32_0x77073096(packBuffer.Span.Slice(0x10), inputLength) == crc)
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
    }
}
