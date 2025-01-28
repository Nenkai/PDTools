using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands;

/// <summary>
/// GT4 and above. Calls pglCullFace(1) 
/// </summary>
public class Cmd_pglCullFace_1 : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglCullFace_1;

    public float Color { get; set; }

    public Cmd_pglCullFace_1()
    {

    }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        
    }

    public override void Write(BinaryStream bs)
    {
        
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pglCullFace_1)}";
    }
}
