using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh;

public class PMSHElementBitLayout
{
    public int TotalBitCount { get; set; }
    public byte XBitCount { get; set; }
    public byte YBitCount { get; set; }
    public byte ZBitCount { get; set; }
    public byte WBitCount { get; set; }
    public byte XBitEnd { get; set; }
    public byte YBitEnd { get; set; }
    public byte ZBitEnd { get; set; }
    public byte WBitEnd { get; set; }
    public short Unk { get; set; }
    public short Unk2 { get; set; }
    public float ScaleX { get; set; }
    public float ScaleY { get; set; }
    public float ScaleZ { get; set; }
    public float ScaleW { get; set; }
    public float OffsetX { get; set; }
    public float OffsetY { get; set; }
    public float OffsetZ { get; set; }
    public float OffsetW { get; set; }


    public static PMSHElementBitLayout FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        PMSHElementBitLayout def = new();

        def.TotalBitCount = bs.ReadInt32();
        def.XBitCount = bs.Read1Byte();
        def.YBitCount = bs.Read1Byte();
        def.ZBitCount = bs.Read1Byte();
        def.WBitCount = bs.Read1Byte();
        def.XBitEnd = bs.Read1Byte();
        def.YBitEnd = bs.Read1Byte();
        def.ZBitEnd = bs.Read1Byte();
        def.WBitEnd = bs.Read1Byte();
        def.Unk = bs.ReadInt16();
        def.Unk2 = bs.ReadInt16();
        def.ScaleX = bs.ReadSingle();
        def.ScaleY = bs.ReadSingle();
        def.ScaleZ = bs.ReadSingle();
        def.ScaleW = bs.ReadSingle();
        def.OffsetX = bs.ReadSingle();
        def.OffsetY = bs.ReadSingle();
        def.OffsetZ = bs.ReadSingle();
        def.OffsetW = bs.ReadSingle();
        return def;
    }

    public static int GetSize()
    {
        return 0x30;
    }
}
