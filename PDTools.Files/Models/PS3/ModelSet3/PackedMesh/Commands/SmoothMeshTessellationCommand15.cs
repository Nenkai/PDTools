using PDTools.Utils;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;

public class SmoothMeshTessellationCommand15 : SmoothMeshTessellationCommandBase
{
    public float VertexIndexStart { get; set; }
    public SmoothMeshTessellationCommandList UnkCommandList { get; set; }

    public override void Read(SmoothMeshTessellationCommandContext ctx, ref BitStream bs)
    {
        VertexIndexStart = bs.ReadSingle();
        bs.ReadBits(13); // Unknown
        int numCmdBitsToSkip = (int)bs.ReadBits(12);

        long oldPos = bs.GetBitPosition();
        UnkCommandList = SmoothMeshTessellationCommandList.Read(ctx, ref bs);
        long newPos = bs.GetBitPosition();

        Debug.Assert(oldPos + numCmdBitsToSkip == newPos);
    }
}