using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.Wing;

public class MDL3WingData
{
    public string Name { get; set; }

    public static MDL3WingData FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        MDL3WingData entry = new();


        return entry;
    }

    public static int GetSize()
    {
        return 0x80;
    }
}
