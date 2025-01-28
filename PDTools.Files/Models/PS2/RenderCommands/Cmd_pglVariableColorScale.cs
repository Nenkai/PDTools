using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands;

/// <summary>
/// GT4 and above. Calls pglVariableColorScale
/// </summary>
public class Cmd_pglVariableColorScale : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.VM_pglVariableColorScale;

    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
    public float A { get; set; }

    public Cmd_pglVariableColorScale()
    {

    }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        R = bs.ReadSingle();
        G = bs.ReadSingle();
        B = bs.ReadSingle();
        A = bs.ReadSingle();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteSingle(R);
        bs.WriteSingle(G);
        bs.WriteSingle(B);
        bs.WriteSingle(A);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pglVariableColorScale)}";
    }
}
