using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh;

public class PackedMeshKey
{
    public uint PackedMeshID;
    public string Name;

    public static PackedMeshKey FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        PackedMeshKey meshInfo = new();
        int strOffset = bs.ReadInt32();
        meshInfo.PackedMeshID = bs.ReadUInt32();

        bs.Position = mdlBasePos + strOffset;
        meshInfo.Name = bs.ReadString(StringCoding.ZeroTerminated);

        return meshInfo;
    }

    public static int GetSize()
    {
        return 0x08;
    }

    public override string ToString()
    {
        return $"{Name} (PackedMeshID: {PackedMeshID})";
    }
}
