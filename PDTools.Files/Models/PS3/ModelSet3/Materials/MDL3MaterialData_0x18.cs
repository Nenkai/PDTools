using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.Materials;

public class MDL3MaterialData_0x18
{
    public short[] Data { get; set; }

    public static MDL3MaterialData_0x18 FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        MDL3MaterialData_0x18 entry = new MDL3MaterialData_0x18();

        int shortCount = bs.ReadInt32();
        int offset = bs.ReadInt32();

        bs.Position = mdlBasePos + offset;
        entry.Data = bs.ReadInt16s(shortCount);

        return entry;
    }

    public static int GetSize()
    {
        return 0x08;
    }
}
