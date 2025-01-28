using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

/// <summary>
/// Sets the depth func to use. Sets GS TEST's ZTST value
/// </summary>
public class Cmd_pglDepthFunc : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglDepthFunc;

    /// <summary>
    /// 0 = NEVER, 1 = ALWAYS, 2 = GEQUAL, 3 = GREATER
    /// </summary>
    public byte Func { get; set; }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        Func = bs.Read1Byte();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteByte(Func);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pglDepthFunc)}";
    }
}
