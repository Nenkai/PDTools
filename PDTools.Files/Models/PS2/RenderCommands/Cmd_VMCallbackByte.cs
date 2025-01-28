using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

/// <summary>
/// GT4
/// </summary>
public class Cmd_VMCallbackByte : ModelSetupPS2Command
{
    public byte Param { get; set; }

    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.VMCallback_Byte;

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        Param = bs.Read1Byte();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteByte(Param);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_VMCallbackByte)}";
    }
}
