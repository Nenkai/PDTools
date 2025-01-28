using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;

using Syroot.BinaryData;
using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Formats;

public class StringDatabase
{
    /// <summary>
    /// 'GTST'
    /// </summary>
    public const uint MAGIC = 0x54535447;
    public const int HeaderSize = 0x10;

    public Endian Endian { get; set; }
    public int coadType { get; set; }
    public ObservableCollection<string> Strings { get; set; } = [];

    private StringDatabase()
    {

    }

    public StringDatabase(Endian endian)
    {
        Endian = endian;
    }

    public static StringDatabase LoadFromFile(string fileName)
    {
        using var file = File.Open(fileName, FileMode.Open);
        using var bs = new BinaryStream(file);
        if (bs.ReadUInt32() != 0x54535447)
            throw new InvalidDataException($"Invalid StringDatabase file '{fileName}'. Magic does not match.");

        var strDb = new StringDatabase();
        bs.Position = 0x08;
        strDb.coadType = bs.ReadInt32(); // 'Coad Type : %0X' - encoding?
        strDb.Endian = strDb.coadType == 1 ? Endian.Little : Endian.Big;
        if (strDb.Endian == Endian.Big)
            bs.ByteConverter = ByteConverter.Big;

        bs.Position = 0x04;
        int stringsCount = bs.ReadInt32();

        bs.Position = 0x10;
        for (int i = 0; i < stringsCount; i++)
        {
            int indexBlockAdr = bs.ReadInt32();
            using (var seek = bs.TemporarySeek(0x10 + (stringsCount * sizeof(uint) + indexBlockAdr), SeekOrigin.Begin))
            {
                seek.Stream.Position += 2;
                strDb.Strings.Add(seek.Stream.ReadString(StringCoding.ZeroTerminated));
            }
        }

        return strDb;
    }

    public string GetStringByID(int id)
    {
        return Strings[id];
    }

    public int GetOrCreate(string str)
    {
        int index = Strings.IndexOf(str);
        if (index == -1)
        {
            Strings.Add(str);
            index = Strings.Count - 1;
        }
        return index;
    }
}
