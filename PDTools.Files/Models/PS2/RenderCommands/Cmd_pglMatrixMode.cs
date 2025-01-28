using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

public class Cmd_pglMatrixMode : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglMatrixMode;

    /// <summary>
    /// 0 = MODEL_VIEW
    /// 1 = PROJECTION
    /// 2 = TEXTURE
    /// </summary>
    public MatrixMode Mode { get; set; }

    public Cmd_pglMatrixMode()
    {

    }

    public Cmd_pglMatrixMode(MatrixMode mode)
    {
        Mode = mode;
    }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        Mode = (MatrixMode)bs.ReadByte();   
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteByte((byte)Mode);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pglMatrixMode)}";
    }
}

public enum MatrixMode
{
    MODEL_VIEW = 0,
    PROJECTION = 1,
    TEXTURE = 2
}
