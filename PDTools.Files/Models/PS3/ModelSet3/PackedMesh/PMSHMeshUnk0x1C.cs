using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh;

public class PMSHMeshUnk0x1C
{
    public byte Field_0x00 { get; set; }
    public byte Count_0x01 { get; set; }
    public byte[] Data { get; set; }

    public static PMSHMeshUnk0x1C FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        PMSHMeshUnk0x1C entry = new();

        entry.Field_0x00 = bs.Read1Byte();
        entry.Count_0x01 = bs.Read1Byte();
        ushort dataSize = bs.ReadUInt16();
        uint dataOffset = bs.ReadUInt32();

        bs.Position = mdlBasePos + dataOffset;
        entry.Data = bs.ReadBytes(dataSize);
        
        return entry;
    }

    public static int GetSize()
    {
        return 0x08;
    }
}
