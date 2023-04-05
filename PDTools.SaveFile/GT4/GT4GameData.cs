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
using System.Buffers;
using System.Runtime.InteropServices;
using Syroot.BinaryData;

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
            byte[] rawSaveFile;
            try
            {
                rawSaveFile = File.ReadAllBytes(gameDataFilePath);
            }
            catch (Exception e)
            {
                throw new IOException("Save folder access was denied, make sure that another program such as PCSX2 is not running.");
            }

            // Decrypt whole file
            if (!DecryptGameDataBuffer(rawSaveFile, out Memory<byte> decrypted, out bool usingOldRandomUpdateCrypto))
                throw new Exception("Failed to decrypt the GT4 save.");

            UseOldRandomUpdateCrypto = usingOldRandomUpdateCrypto;

#if DEBUG
            // For testing whether saves are decrypted -> read -> written -> encrypted fine
            File.WriteAllBytes("save.out", decrypted.ToArray());
#endif

            if (decrypted.Length == 0x3A070)
            {
                if (GT4Save.IsGT4Retail(gt4Save.Type))
                {
                    // Warn
                }
            }
            else if (decrypted.Length == 0x3A160)
            {
                // GT4O (US, JP)
                if (!GT4Save.IsGT4Online(gt4Save.Type))
                {
                    // Warn
                }

            }

            SaveLength = decrypted.Length;
            ReadSave(gt4Save, decrypted);


#if DEBUG
            /*
            byte[] resaved = WriteSave(gt4Save);
            File.WriteAllBytes("save.out_repacked", resaved);

            byte[] encrypted = EncryptGameDataBuffer(resaved, UseOldRandomUpdateCrypto);
            DecryptGameDataBuffer(encrypted, out Memory<byte> redecrypted, out _);

            File.WriteAllBytes("save.out_repak_encr_decrypt", redecrypted.ToArray());
            */
#endif
        }

        public void CopyTo(GT4GameData dest)
        {
            Profile.CopyTo(dest.Profile);
            Option.CopyTo(dest.Option);
            Context.CopyTo(dest.Context);
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
            byte[] buffer = new byte[0x50000];
            int length = WriteSave(save, buffer);
            byte[] encrypted = EncryptGameDataBuffer(buffer.AsSpan(0, length), UseOldRandomUpdateCrypto);

            using (var fs = new FileStream(fileName, FileMode.Create))
                fs.Write(encrypted, 0, encrypted.Length);
        }

        private int WriteSave(GT4Save gt4Save, byte[] buffer)
        {
            SpanWriter sw = new SpanWriter(buffer);
            Profile.Pack(gt4Save, ref sw);
            Option.Pack(gt4Save, ref sw);
            Context.Pack(gt4Save, ref sw);

            return sw.Position;
        }

        public static bool DecryptGameDataBuffer(byte[] rawSaveFile, out Memory<byte> saveBuffer, out bool usingOldRandomUpdateCrypto)
        {
            usingOldRandomUpdateCrypto = true;

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

                usingOldRandomUpdateCrypto = false;
            }

            Memory<byte> packBuffer = workBuffer.AsMemory(SharedCrypto.EncryptUnit_HdrLen, workBuffer.Length - SharedCrypto.EncryptUnit_HdrLen);
            if (!VerifyPackHeader(packBuffer, out saveBuffer))
                return false;

            return true;
        }

        public static byte[] EncryptGameDataBuffer(Span<byte> saveBuffer, bool useOldRandomUpdateCrypto)
        {
            byte[] packBuffer = SerializePack(saveBuffer);

            uint fileLength = MiscUtils.AlignValue((uint)(SharedCrypto.EncryptUnit_HdrLen + packBuffer.Length), 0x40);
            byte[] fileBuffer = new byte[fileLength];
            packBuffer.CopyTo(fileBuffer.AsSpan(SharedCrypto.EncryptUnit_HdrLen));

            SharedCrypto.EncryptUnit_Encrypt(fileBuffer, fileBuffer.Length, 0, Mult, Mult2, useMt: false, bigEndian: false, useOldRandomUpdateCrypto);
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

        // Returns expected file size
        public static int GetExpectedGameDataSize(GT4SaveType gameType)
        {
            switch (gameType)
            {
                case GT4SaveType.GT4_EU:
                case GT4SaveType.GT4_US:
                case GT4SaveType.GT4_JP:
                case GT4SaveType.GT4_KR:
                    return 0x3A070;
                case GT4SaveType.GT4O_US:
                case GT4SaveType.GT4O_JP:
                    return 0x3A160;

                default:
                    return -1;
            }
        }
    }
}
