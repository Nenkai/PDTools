using PDTools.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;

public class SmoothMeshTessellationCommand11 : SmoothMeshTessellationCommandBase
{
    public ushort Unk { get; set; }
    public ushort Unk2 { get; set; }
    public override void Read(SmoothMeshTessellationCommandContext ctx, ref BitStream bs)
    {
        Unk = (ushort)bs.ReadBits(10);
        Unk2 = (ushort)bs.ReadBits(12);
    }
}