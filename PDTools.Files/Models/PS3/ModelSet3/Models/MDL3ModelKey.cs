using System;
using System.Collections.Generic;
using System.Linq;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.Models;

public class MDL3ModelKey
{
    public uint ModelID { get; set; }
    public string Name { get; set; }

    public static MDL3ModelKey FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        MDL3ModelKey modelKey = new();
        int strOffset = bs.ReadInt32();
        modelKey.ModelID = bs.ReadUInt32();

        bs.Position = mdlBasePos + strOffset;

        // first will be empty so skip it
        modelKey.Name = bs.ReadString(StringCoding.ZeroTerminated);

        return modelKey;
    }

    public static int GetSize()
    {
        return 0x08;
    }

    public override string ToString()
    {
        return $"{Name} (ModelID: {ModelID})";
    }
}
