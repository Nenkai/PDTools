using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;
using PDTools.Files.Models.PS2.RenderCommands;

namespace PDTools.Files.Models.PS2.Commands;

/// <summary>
/// Calls glDisable(15). Disables face culling
/// </summary>
public class Cmd_pglDisableCullFace : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglDisableCullFace;

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        
    }

    public override void Write(BinaryStream bs)
    {
        
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pglDisableCullFace)}";
    }
}
