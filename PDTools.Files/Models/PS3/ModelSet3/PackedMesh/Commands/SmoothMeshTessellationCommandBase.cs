using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Utils;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;

public abstract class SmoothMeshTessellationCommandBase
{
    public virtual void Read(SmoothMeshTessellationCommandContext ctx, ref BitStream bs)
    {

    }

    public static SmoothMeshTessellationCommandBase Create(byte opcode)
    {
        return opcode switch
        {
            0 => new SmoothMeshTessellationCommand0(),
            1 => new SmoothMeshTessellationCommand1(),
            2 => new SmoothMeshTessellationCommand2(),
            5 => new SmoothMeshTessellationCommand5(),
            6 => new SmoothMeshTessellationCommand6(),
            7 => new SmoothMeshTessellationCommand7(),
            8 => new SmoothMeshTessellationCommand8(),
            9 => new SmoothMeshTessellationCommand9(),
            10 => new SmoothMeshTessellationCommand10(),
            11 => new SmoothMeshTessellationCommand11(),
            12 => new SmoothMeshTessellationCommand12(),
            13 => new SmoothMeshTessellationCommand13(),
            14 => new SmoothMeshTessellationCommand14(),
            15 => new SmoothMeshTessellationCommand15(),
            16 => new SmoothMeshTessellationCommand16(),
            17 => new SmoothMeshTessellationCommand17(),
            21 => new SmoothMeshTessellationCommand21(),
            22 => new SmoothMeshTessellationCommand22(),
            23 => new SmoothMeshTessellationCommand23(),
            _ => throw new NotImplementedException($"Smooth mesh command {opcode} not implemented"),
        };
    }
}
