using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Materials
{
    public class MDL3MaterialData_0x14
    {
        public string Name { get; set; }
        public int UnkIndex { get; set; }
        public byte Unk0x01 { get; set; }
        public byte Version { get; set; }
        public MDL3TextureKey TextureKey { get; set; }
        public static MDL3MaterialData_0x14 FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3MaterialData_0x14 entry = new();
            int nameOffset = bs.ReadInt32();
            entry.UnkIndex = bs.ReadInt32();
            bs.ReadInt32(); // Empty
            int unkOffset_0x0C = bs.ReadInt32();
            short unkCount = bs.ReadInt16();
            short unkCount2 = bs.ReadInt16();
            int unkOffset_0x10 = bs.ReadInt32();
            int typeOrVersion = bs.ReadInt32();
            int keyOffset = bs.ReadInt32();

            bs.Position = mdlBasePos + nameOffset;
            entry.Name = bs.ReadString(StringCoding.ZeroTerminated);

            bs.Position = mdlBasePos + unkOffset_0x0C;
            // TODO - this is something we go through in another master model structure

            bs.Position = mdlBasePos + keyOffset;

            return entry;
        }

        public static int GetSize()
        {
            return 0x28;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
