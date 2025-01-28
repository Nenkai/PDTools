using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

/// <summary>
/// Same as command 54 but sets all 4 values to specified.
/// </summary>
public class Cmd_GT3_3_4f : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglGT3_3_4f;

    // Color. RGB only, A is ignored

    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
    public float A { get; set; }

    public Cmd_GT3_3_4f()
    {
    
    }

    public Cmd_GT3_3_4f(float r, float g, float b, float a)
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
        return $"{nameof(Cmd_GT3_3_4f)}";
    }
}
