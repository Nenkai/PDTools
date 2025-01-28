using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

/// <summary>
/// Sets FOGCOL register
/// </summary>
public class Cmd_pglSetFogColor : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglSetFogColor;

    /// <summary>
    /// GS FOGCOL value. RGB only, A is ignored
    /// </summary>
    public uint Color { get; set; }

    public Cmd_pglSetFogColor()
    {

    }

    public Cmd_pglSetFogColor(uint color)
    {
        Color = color;
    }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        Color = bs.ReadUInt32();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteUInt32(Color);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pglSetFogColor)}";
    }
}
