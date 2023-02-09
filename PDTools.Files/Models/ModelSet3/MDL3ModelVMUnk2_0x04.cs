using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

using PDTools.Files.Models.ModelSet3.Commands;
using System.IO;

namespace PDTools.Files.Models.ModelSet3
{
    public class MDL3ModelVMUnk_0x04
    {
        public string Name { get; set; }
        public short StackStorage2Index { get; set; }
        public short MaterialIndex { get; set; }

        public static MDL3ModelVMUnk_0x04 FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3ModelVMUnk_0x04 unk = new();

            int nameOffset = bs.ReadInt32();
            unk.StackStorage2Index = bs.ReadInt16();
            unk.MaterialIndex = bs.ReadInt16();

            int dataOffset = bs.ReadInt32();
            bs.ReadInt32();

            bs.Position = mdlBasePos + nameOffset;
            unk.Name = bs.ReadString(StringCoding.ZeroTerminated);
            return unk;
        }

        public static int GetSize()
        {
            return 0x10;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
