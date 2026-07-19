using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

/// <summary>
/// GT4 and above. Unknown. Skips 3 bytes.
/// </summary>
public class Cmd_Unk65 : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.Unk_65;

    public byte Arg0 { get; set; }
    public byte Arg1 { get; set; }
    public byte Arg2 { get; set; }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        Arg0 = bs.Read1Byte();
        Arg1 = bs.Read1Byte();
        Arg2 = bs.Read1Byte();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteByte(Arg0);
        bs.WriteByte(Arg1);
        bs.WriteByte(Arg2);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_Unk65)}";
    }
}
