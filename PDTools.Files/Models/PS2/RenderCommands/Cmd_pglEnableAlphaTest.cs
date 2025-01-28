using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

public class Cmd_pglEnableAlphaTest : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglEnableAlphaTest;

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        
    }

    public override void Write(BinaryStream bs)
    {
        
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pglEnableAlphaTest)}";
    }
}
