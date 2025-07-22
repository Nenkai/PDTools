using PDTools.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;

public class SmoothMeshTessellationCommand13 : SmoothMeshTessellationCommandBase
{
    // 256 bits
    public float[] UnkBits { get; set; } = new float[8];

    public override void Read(SmoothMeshTessellationCommandContext ctx, ref BitStream bs)
    {
        // Accurate
        long newOffset = ((bs.GetBitPosition() - 5) + 13) & 0xfffffff8;
        bs.SeekToBit(newOffset);

        for (int i = 0; i < UnkBits.Length; i++)
        {
            UnkBits[i] = bs.ReadSingle();
        }
    }
}