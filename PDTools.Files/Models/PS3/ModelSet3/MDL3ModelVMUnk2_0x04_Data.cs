using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3;

public class MDL3ModelVMUnk_0x04_Data
{
    public short MaterialDataID { get; set; }
    public short MaterialData0x0CID { get; set; }

    public short[] _0x08Indices { get; set; }
    public static MDL3ModelVMUnk_0x04_Data FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        var data = new MDL3ModelVMUnk_0x04_Data();
        data.MaterialDataID = bs.ReadInt16();
        bs.ReadInt16();
        data.MaterialData0x0CID = bs.ReadInt16();
        bs.ReadInt16();

        data._0x08Indices = bs.ReadInt16s(3);
        // fourth is FF FF

        return data;
    }

    public static int GetSize()
    {
        return 0x10;
    }

}
