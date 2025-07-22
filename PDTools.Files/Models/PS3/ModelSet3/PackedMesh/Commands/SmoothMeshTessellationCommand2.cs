using PDTools.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;

public class SmoothMeshTessellationCommand2 : SmoothMeshTessellationCommandBase
{
    public ushort Unk { get; set; }

    public override void Read(SmoothMeshTessellationCommandContext ctx, ref BitStream bs)
    {
        Unk = (ushort)bs.ReadBits(12);
    }
}
