using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Materials
{
    public class MDL3MaterialDataKey
    {
        public string Name { get; set; }
        public int UnkIndex { get; set; }

        public static MDL3MaterialDataKey FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3MaterialDataKey entry = new();

            int elemNameOffset = bs.ReadInt32();
            entry.UnkIndex = bs.ReadInt32();
            bs.Position = mdlBasePos + elemNameOffset;
            entry.Name = bs.ReadString(StringCoding.ZeroTerminated);

            return entry;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
