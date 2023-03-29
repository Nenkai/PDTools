using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Materials
{
    public class MDL3MaterialData_0x1C
    {
        public int Unk { get; set; }
        public int Unk2 { get; set; }

        public static MDL3MaterialData_0x1C FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3MaterialData_0x1C entry = new MDL3MaterialData_0x1C();

            entry.Unk = bs.ReadInt32();
            entry.Unk2 = bs.ReadInt32();

            return entry;
        }
    }
}
