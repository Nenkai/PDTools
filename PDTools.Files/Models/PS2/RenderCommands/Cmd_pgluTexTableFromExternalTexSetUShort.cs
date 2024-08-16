using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands;

public class Cmd_pgluTexTableFromExternalTexSetUShort : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pgluTexTableFromExternalTexSetUShort;

    public ushort ExternalTexSetIndex { get; set; }

    public Cmd_pgluTexTableFromExternalTexSetUShort(ushort value)
    {
        ExternalTexSetIndex = value;
    }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        bs.WriteUInt16(ExternalTexSetIndex);
    }

    public override void Write(BinaryStream bs)
    {
        ExternalTexSetIndex = bs.ReadUInt16();
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pgluTexTableFromExternalTexSetUShort)}";
    }
}
