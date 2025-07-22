using PDTools.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;

public class SmoothMeshTessellationCommand14 : SmoothMeshTessellationCommandBase
{
    public ushort VertexIndexStart { get; set; }
    public ushort VertexIndexCount { get; set; }

    public override void Read(SmoothMeshTessellationCommandContext ctx, ref BitStream bs)
    {
        VertexIndexStart = (ushort)bs.ReadBits(12);
        VertexIndexCount = (ushort)bs.ReadBits(12);
    }
}