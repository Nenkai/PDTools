using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

using Syroot.BinaryData;

namespace PDTools.RText;

/// <summary>
/// Gran Turismo 4/Tourist Trophy/PSP/GT5P Localization Strings
/// </summary>
public class RT03 : RTextBase
{
    public static readonly string Magic = "RT03";
    public const int HeaderSize = 0x10;

    public RT03(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException();

    }

    public RT03()
    {
    }

    public void GetPageByIndex()
    {
        throw new NotImplementedException();
    }

    public void GetStringByPage(int pageIndex)
    {
        throw new NotImplementedException();
    }


    public override void Read(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var reader = new BinaryStream(ms, ByteConverter.Little);
        reader.BaseStream.Position = 0;
        if (reader.ReadString(4) != Magic)
            throw new Exception("Invalid magic, doesn't match RT03.");

        reader.ReadInt32(); // Relocation Ptr
        reader.ReadUInt32(); // Empty - skipped by GT4
        int entryCount = reader.ReadInt32();

        for (int i = 0; i < entryCount; i++)
        {
            ms.Position = HeaderSize + (i * 0x10);
            var page = new RT03Page();
            page.Read(reader);
            _pages.Add(page.Name, page);
        }
    }

    #region Saving
    public override void Save(string fileName)
    {
        using var ms = new FileStream(fileName, FileMode.Create);
        using var bs = new BinaryStream(ms, ByteConverter.Little);
        bs.Write(Encoding.ASCII.GetBytes(Magic));
        bs.Write(0);
        bs.Write(0);
        bs.Write(_pages.Count);

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
    #endregion
}
