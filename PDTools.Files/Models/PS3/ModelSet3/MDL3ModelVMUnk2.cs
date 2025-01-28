using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3;

public class MDL3ModelVMUnk2
{
    public int UnkVMInsPtr { get; set; }
    public int UnkVMInsPtr2 { get; set; }

    public short UnkCountUsedForVMStack2 { get; set; }
    public short UnkCountUsedForVMStack3 { get; set; }

    public List<MDL3ModelVMUnk_0x04> _0x04 { get; set; } = [];
    public List<MDL3ModelVMUnk_0x08> _0x08 { get; set; } = [];

    public static MDL3ModelVMUnk2 FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        MDL3ModelVMUnk2 unk = new();

        short count0x04 = bs.ReadInt16();
        short count0x08 = bs.ReadInt16();

        int offset0x04 = bs.ReadInt32();
        int offset0x08 = bs.ReadInt32();

        unk.UnkVMInsPtr = bs.ReadInt32();
        unk.UnkVMInsPtr2 = bs.ReadInt32();

        int offset0x14 = bs.ReadInt16();
        short count0x14 = bs.ReadInt16();

        unk.UnkCountUsedForVMStack2 = bs.ReadInt16();
        unk.UnkCountUsedForVMStack3 = bs.ReadInt16();
        bs.ReadInt16();

        for (var i = 0; i < count0x04; i++)
        {
            bs.Position = mdlBasePos + offset0x04 + i * MDL3ModelVMUnk_0x04.GetSize();
            var entry = MDL3ModelVMUnk_0x04.FromStream(bs, mdlBasePos, mdl3VersionMajor);
            unk._0x04.Add(entry);
        }

        for (var i = 0; i < count0x08; i++)
        {
            bs.Position = mdlBasePos + offset0x08 + i * MDL3ModelVMUnk_0x08.GetSize();
            var entry = MDL3ModelVMUnk_0x08.FromStream(bs, mdlBasePos, mdl3VersionMajor);
            unk._0x08.Add(entry);
        }

        return unk;
    }

    public static int GetSize()
    {
        return 0x20;
    }
}
