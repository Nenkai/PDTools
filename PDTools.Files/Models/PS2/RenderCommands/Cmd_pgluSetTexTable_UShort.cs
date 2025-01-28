using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

public class Cmd_pgluSetTexTable_UShort : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pgluSetTexTable_UShort;

    /// <summary>
    /// Texture set table index to use. Mostly for LODs
    /// </summary>
    public ushort TexSetTableIndex { get; set; }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        TexSetTableIndex = bs.ReadUInt16();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteUInt16(TexSetTableIndex);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pgluSetTexTable_UShort)} - TexSetTableIndex: {TexSetTableIndex}";
    }
}
