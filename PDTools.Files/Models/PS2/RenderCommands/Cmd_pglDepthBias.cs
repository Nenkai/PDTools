using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

public class Cmd_pglDepthBias : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglDepthBias;

    public float Value { get; set; }

    public Cmd_pglDepthBias()
    {

    }

    public Cmd_pglDepthBias(float value)
    {
        Value = value;
    }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        Value = bs.ReadSingle();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteSingle(Value);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pglDepthBias)}";
    }
}
