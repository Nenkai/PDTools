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
    public class MDL3ModelVMUnk
    {
        public short[] UnkIndices { get; set; }

        public static MDL3ModelVMUnk FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3ModelVMUnk unk = new();

            int indicesOffset = bs.ReadInt32();

            bs.Position += 0x2C;
            short indexCount = bs.ReadInt16();

            bs.Position = indicesOffset;
            unk.UnkIndices = bs.ReadInt16s(indexCount);

            return unk;
        }

        public static int GetSize()
        {
            return 0x40;
        }
    }
}
