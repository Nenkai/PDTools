using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3;

public class MDL3ModelVMContext
{
    public int UnkVMInsPtr { get; set; }
    public int UnkVMInsPtr2 { get; set; }
    public int Unk { get; set; }
    public static MDL3ModelVMContext FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        MDL3ModelVMContext ctx = new();

        ctx.UnkVMInsPtr = bs.ReadInt32();
        ctx.UnkVMInsPtr2 = bs.ReadInt32();
        bs.ReadInt32();
        ctx.Unk = bs.ReadInt32();

        return ctx;
    }

    public static int GetSize()
    {
        return 0x20;
    }
}
