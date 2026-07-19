using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

/// <summary>
/// GT4 and above. Calls pglGetCullFace and pglCullFace(mode). Operand: 3 bytes (mode + 2). arg0==0 → disable.
/// </summary>
public class Cmd_pglCullFace : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglCullFace;

    public byte Mode { get; set; }
    public byte Arg1 { get; set; }
    public byte Arg2 { get; set; }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        Mode = bs.Read1Byte();
        Arg1 = bs.Read1Byte();
        Arg2 = bs.Read1Byte();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteByte(Mode);
        bs.WriteByte(Arg1);
        bs.WriteByte(Arg2);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pglCullFace)}";
    }
}
