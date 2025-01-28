using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.Shaders;

public class ShadersProgram_0x2C
{
    public byte[] Program { get; set; }
    public List<ShaderProgramEntry_0x2C_0x10> _0x10 { get; set; } = [];
    public List<ShaderProgramEntry_0x2C_0x14> _0x14 { get; set; } = [];
    public List<ShaderProgramEntry_0x2C_0x18> _0x18 { get; set; } = [];
    public List<ShaderProgramEntry_0x2C_0x38> _0x38 { get; set; } = [];
    public short Unk { get; set; }
    public int Unk2 { get; set; }

    public static ShadersProgram_0x2C FromStream(BinaryStream bs, long basePos)
    {
        var prog = new ShadersProgram_0x2C();
        int programOffset = bs.ReadInt32();
        int programSize = bs.ReadInt32();

        short count_0x10 = bs.ReadInt16();
        byte count_0x38 = bs.Read1Byte();
        byte count_0x14 = bs.Read1Byte();
        short count_0x18 = bs.ReadInt16();
        prog.Unk = bs.ReadInt16(); // Empty

        int offset_0x10 = bs.ReadInt32();
        int offset_0x14 = bs.ReadInt32();
        int offset_0x18 = bs.ReadInt32();
        int offset_0x1C = bs.ReadInt32();

        bs.Position += 6 * sizeof(int);

        int offset_0x38 = bs.ReadInt32();
        prog.Unk2 = bs.ReadInt32();
        bs.ReadInt32();
        int offset_0x44 = bs.ReadInt32();

        for (var i = 0; i < count_0x10; i++)
        {
            bs.Position = basePos + offset_0x10 + (i * 0x10);
            var entry = ShaderProgramEntry_0x2C_0x10.FromStream(bs, basePos);
            prog._0x10.Add(entry);
        }

        
        for (var i = 0; i < count_0x14; i++)
        {
            bs.Position = basePos + offset_0x14 + (i * 0x10);
            var entry = ShaderProgramEntry_0x2C_0x14.FromStream(bs, basePos);
            prog._0x14.Add(entry);
        }

        for (var i = 0; i < count_0x18; i++)
        {
            bs.Position = basePos + offset_0x18 + (i * 0x10);
            var entry = ShaderProgramEntry_0x2C_0x18.FromStream(bs, basePos);
            prog._0x18.Add(entry);
        }

        for (var i = 0; i < count_0x38; i++)
        {
            bs.Position = basePos + offset_0x38 + (i * 0x10);
            var entry = ShaderProgramEntry_0x2C_0x38.FromStream(bs, basePos);
            prog._0x38.Add(entry);
        }

        bs.Position = basePos + programOffset;
        prog.Program = bs.ReadBytes(programSize);

        return prog;
    }

    public static int GetSize()
    {
        return 0x48;
    }
}
