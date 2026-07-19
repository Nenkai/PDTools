using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

/// <summary>
/// GT3-era. Does something with 4 floats; games above GT3 skip the 4 floats. Operand: 4 floats (16 bytes).
/// </summary>
public class Cmd_GT3_Unk40 : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglGT3_Unk40;

    public float F0 { get; set; }
    public float F1 { get; set; }
    public float F2 { get; set; }
    public float F3 { get; set; }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        F0 = bs.ReadSingle();
        F1 = bs.ReadSingle();
        F2 = bs.ReadSingle();
        F3 = bs.ReadSingle();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteSingle(F0);
        bs.WriteSingle(F1);
        bs.WriteSingle(F2);
        bs.WriteSingle(F3);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_GT3_Unk40)}";
    }
}
