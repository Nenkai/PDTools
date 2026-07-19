using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

public class Cmd_pgluTexTableFromExternalTexSetByte : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pgluTexTableFromExternalTexSetByte;

    public byte ExternalTexSetIndex { get; set; }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        ExternalTexSetIndex = bs.Read1Byte();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteByte(ExternalTexSetIndex);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pgluTexTableFromExternalTexSetByte)}";
    }
}
