using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

public class Cmd_pglAlphaFail : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglAlphaFail;

    /// <summary>
    /// GS TEST register - AFAIL field
    /// </summary>
    public AlphaFailMethod FailMethod { get; set; }

    public Cmd_pglAlphaFail()
    {

    }

    public Cmd_pglAlphaFail(AlphaFailMethod method)
    {
        FailMethod = method;
    }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        FailMethod = (AlphaFailMethod)bs.Read1Byte();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteByte((byte)FailMethod);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pglAlphaFail)}";
    }
}

public enum AlphaFailMethod
{
    /// <summary>
    /// Neither frame buffer nor Z buffer is updated.
    /// </summary>
    KEEP,

    /// <summary>
    /// Only frame buffer is updated.
    /// </summary>
    FB_ONLY,

    /// <summary>
    /// Only Z buffer is updated.
    /// </summary>
    ZB_ONLY,

    /// <summary>
    /// Only frame-buffer RGB is updated.
    /// </summary>
    RGB_ONLY
}
