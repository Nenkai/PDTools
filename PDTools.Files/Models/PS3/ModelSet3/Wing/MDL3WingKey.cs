using System;
using System.Collections.Generic;
using System.Linq;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.Wing;

public class MDL3WingKey
{
    public uint WingDataID { get; set; }
    public string Name { get; set; }

    public static MDL3WingKey FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3Version)
    {
        MDL3WingKey modelKey = new();
        int strOffset = bs.ReadInt32();
        modelKey.WingDataID = bs.ReadUInt32();

        bs.Position = mdlBasePos + strOffset;
        modelKey.Name = bs.ReadString(StringCoding.ZeroTerminated);

        return modelKey;
    }

    public static int GetSize()
    {
        return 0x08;
    }

    public override string ToString()
    {
        return $"{Name} (Wing Data ID: {WingDataID})";
    }
}
