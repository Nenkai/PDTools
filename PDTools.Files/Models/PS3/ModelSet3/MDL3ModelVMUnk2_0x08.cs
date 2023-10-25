using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3
{
    public class MDL3ModelVMUnk_0x08
    {
        public short Unk { get; set; }

        public float[] Data { get; set; }

        public static MDL3ModelVMUnk_0x08 FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3ModelVMUnk_0x08 unk = new();

            unk.Unk = bs.ReadInt16(); // Unused maybe
            short vec2Count = bs.ReadInt16();

            int dataOffset = bs.ReadInt32();
            bs.Position = mdlBasePos + dataOffset;

            unk.Data = bs.ReadSingles(vec2Count * 2);

            return unk;
        }

        public static int GetSize()
        {
            return 0x08;
        }
    }
}
