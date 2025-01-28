using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Files.Models.Shaders;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.Materials;

public class MDL3MaterialShaderReferences
{
    public int Unk { get; set; }
    public ShadersProgram_0x20 ShaderProgram { get; set; }
    public ShadersProgram_0x2C ShaderProgram2 { get; set; }
    public int Unk2 { get; set; }
    public short Unk3 { get; set; }

    public short[] UnkData { get; set; }
    public static MDL3MaterialShaderReferences FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        MDL3MaterialShaderReferences entry = new MDL3MaterialShaderReferences();

        int unkCount_0x08 = bs.ReadInt32();
        entry.Unk = bs.ReadInt32();
        int offset_0x08 = bs.ReadInt32();
        int shaderProgramOffset = bs.ReadInt32();
        int shader0x2COffset = bs.ReadInt32();
        entry.Unk2 = bs.ReadInt32();
        entry.Unk3 = bs.ReadInt16();
        bs.ReadInt16();
        bs.ReadInt32();

        if (shaderProgramOffset != 0)
        {
            bs.Position = mdlBasePos + shaderProgramOffset;
            entry.ShaderProgram = ShadersProgram_0x20.FromStream(bs, mdlBasePos);
        }

        if (shader0x2COffset != 0)
        {
            bs.Position = mdlBasePos + shader0x2COffset;
            entry.ShaderProgram2 = ShadersProgram_0x2C.FromStream(bs, mdlBasePos);
        }

        if (unkCount_0x08 > 0 && unkCount_0x08 != 65536)
        {
            bs.Position = mdlBasePos + offset_0x08;
            int count = bs.ReadInt32();
            entry.UnkData = bs.ReadInt16s(count);
        }

        return entry;
    }
}
