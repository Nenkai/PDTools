using PDTools.Files.Models.ModelSet3;
using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.Shaders
{
    public class ShadersProgram_0x40
    {
        public byte[] Program { get; set; }
        public static ShadersProgram_0x40 FromStream(BinaryStream bs, long basePos)
        {
            var prog = new ShadersProgram_0x40();
            int unkOffset = bs.ReadInt32();
            int programOffset = bs.ReadInt32();
            int programSize = bs.ReadInt32();

            bs.Position = basePos + programOffset;
            prog.Program = bs.ReadBytes(programSize);

            return prog;
        }
    }
}
