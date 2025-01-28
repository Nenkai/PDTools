using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands;

/// <summary>
/// GT4 and above. Calls pglTexGenf(3, 0.0) and pglTexGenf(2, 1.0)
/// </summary>
public class Cmd_pglTexGenf_Default : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglTexGenf_Default;

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        
    }

    public override void Write(BinaryStream bs)
    {
        
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pglTexGenf_Default)}";
    }
}
