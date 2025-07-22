using PDTools.Utils;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;

public class SmoothMeshTessellationCommand17 : SmoothMeshTessellationCommandBase
{
    public byte Unk { get; set; }
    public override void Read(SmoothMeshTessellationCommandContext ctx, ref BitStream bs)
    {
        Unk = (byte)bs.ReadBits(2);
    }
}