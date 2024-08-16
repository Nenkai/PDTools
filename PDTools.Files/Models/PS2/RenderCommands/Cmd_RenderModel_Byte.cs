using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands;

public class Cmd_RenderModel_Byte : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.RenderModel_Byte;

    public byte ModelIndex { get; set; }

    public Cmd_RenderModel_Byte()
    {

    }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        ModelIndex = bs.Read1Byte();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteByte(ModelIndex);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_RenderModel_Byte)}";
    }
}
