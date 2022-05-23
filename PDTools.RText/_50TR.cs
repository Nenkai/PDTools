using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

using Syroot.BinaryData;

namespace PDTools.RText
{
    /// <summary>
    /// Gran Turismo Sport Localization Strings (Encrypted, LE)
    /// </summary>
    // Magic is also present in GT6, but no handling of LE
    public class _50TR : RTextBase
    {
        public static readonly string Magic = "50TR";
        public const int HeaderSize = 0x20;


        // For GT7 RT05, which uses int64 pointers
        public bool _gt7;

        public _50TR(string filePath, bool gt7 = false)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            _gt7 = gt7;
        }

        public _50TR(bool gt7 = false)
        {
            _gt7 = gt7;
        }

        public void GetCategoryByIndex()
        {
            throw new NotImplementedException();
        }

        public void GetStringByCategory(int categoryIndex)
        {
            throw new NotImplementedException();
        }

        public override void Read(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var reader = new BinaryStream(ms, ByteConverter.Little))
            {
                reader.BaseStream.Position = 0;
                if (reader.ReadString(4) != Magic)
                    throw new Exception($"Invalid magic, doesn't match {Magic}.");

                int entryCount = reader.ReadInt32();

                // Relocation ptr is at 0x10

                // Data starts at 0x20

                for (int i = 0; i < entryCount; i++)
                {
                    ms.Position = HeaderSize + (i * (_gt7 ? 0x18 : 0x10));
                    var page = new RT05Page(gt7: _gt7);
                    page.Read(reader);
                    _pages.Add(page.Name, page);
                }


                foreach (var i in _pages)
                {
                    if (string.IsNullOrEmpty(i.Key))
                        throw new Exception("Corrupted file.");
                }
            }
        }

        public override void Save(string fileName)
        {
            using (var ms = new FileStream(fileName, FileMode.Create))
            using (var bs = new BinaryStream(ms, ByteConverter.Little))
            {
                bs.Write(Encoding.ASCII.GetBytes(Magic));
                bs.Write(_pages.Count);
                bs.Write((byte)1); // "Obfuscated", we don't know, eboot doesn't read it
                bs.BaseStream.Position = HeaderSize;

                int i = 0;
                int baseDataPos = HeaderSize + (_pages.Count * 0x10);
                foreach (var pagePair in _pages)
                {
                    int baseEntryOffset = (int)HeaderSize + (i * 0x10);
                    pagePair.Value.Write(bs, baseEntryOffset, baseDataPos);
                    baseDataPos = (int)ms.Position;
                    i++;
                }
            }
        }
    }
}
