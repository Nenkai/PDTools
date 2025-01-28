using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

public class Cmd_RenderModel_UShort : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.RenderModel_UShort;

    public ushort ModelIndex { get; set; }

    public Cmd_RenderModel_UShort()
    {

    }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        ModelIndex = bs.ReadUInt16();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteUInt16(ModelIndex);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_RenderModel_UShort)}";
    }
}
