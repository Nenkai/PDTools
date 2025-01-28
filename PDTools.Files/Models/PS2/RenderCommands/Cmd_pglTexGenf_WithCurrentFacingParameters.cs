using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands;

/// <summary>
/// GT4 and above. Calls pglTexGenf(3, facing_attenuation_) and pglTexGenf(2, facing_bias_)</br>
/// Both of these globals are set through ModelSet2::setFacingAttenuation and ModelSet2::setFacingBias</br>
/// Which are called by course env ptr code, it seems</br>
/// </summary>
public class Cmd_pglTexGenf_WithCurrentFacingParameters : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglTexGenf_WithCurrentFacingParameters;

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        
    }

    public override void Write(BinaryStream bs)
    {
        
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pglTexGenf_WithCurrentFacingParameters)}";
    }
}
