using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.Shaders
{
    public class ShadersProgram_0x20
    {
        public byte[] Program { get; set; }
        public List<ShaderProgramEntry_0x20_0x14> _0x14 { get; set; } = new();
        public List<ShaderProgramEntry_0x20_0x18> _0x18 { get; set; } = new();
        public int Unk { get; set; }

        public static ShadersProgram_0x20 FromStream(BinaryStream bs, long basePos)
        {
            var prog = new ShadersProgram_0x20();
            int programOffset = bs.ReadInt32();
            int programSize = bs.ReadInt32();

            short count_0x10 = bs.ReadInt16();
            short count_0x14 = bs.ReadInt16();
            short count_0x18 = bs.ReadInt16();
            bs.ReadInt16(); // Empty

            int offset_0x10 = bs.ReadInt32();
            int offset_0x14 = bs.ReadInt32();
            int offset_0x18 = bs.ReadInt32();
            int offset_0x1C = bs.ReadInt32();
            prog.Unk = bs.ReadInt32();
            int unk2 = bs.ReadInt32();

            for (var i = 0; i < count_0x14; i++)
            {
                bs.Position = basePos + offset_0x14 + (i * 0x10);
                var entry = ShaderProgramEntry_0x20_0x14.FromStream(bs, basePos);
                prog._0x14.Add(entry);
            }

            for (var i = 0; i < count_0x18; i++)
            {
                bs.Position = basePos + offset_0x18 + (i * 0x10);
                var entry = ShaderProgramEntry_0x20_0x18.FromStream(bs, basePos);
                prog._0x18.Add(entry);
            }

            bs.Position = basePos + programOffset;
            prog.Program = bs.ReadBytes(programSize);

            return prog;
        }

        public static int GetSize()
        {
            return 0x28;
        }
    }
}
