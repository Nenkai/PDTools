using PDTools.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;

public class SmoothMeshTessellationCommand5 : SmoothMeshTessellationCommandBase
{
    public ushort Unk { get; set; }
    public List<SmoothMeshTessellationCommand5GroupData> VertexGroups { get; set; } = [];
    public List<SmoothMeshTessellationCommand5GroupData> IndexGroups { get; set; } = [];

    public override void Read(SmoothMeshTessellationCommandContext ctx, ref BitStream bs)
    {
        Unk = (ushort)bs.ReadBits(12);
        byte vertexGroupCount = (byte)bs.ReadBits(4);
        byte indexGroupCount = (byte)bs.ReadBits(4);

        for (int i = 0; i < vertexGroupCount; i++)
        {
            VertexGroups.Add(
                new SmoothMeshTessellationCommand5GroupData(Start: (ushort)bs.ReadBits(16), Num: (ushort)bs.ReadBits(10)
            ));
        }

        for (int i = 0; i < indexGroupCount; i++)
        {
            IndexGroups.Add(
                new SmoothMeshTessellationCommand5GroupData(Start: (ushort)bs.ReadBits(16), Num: (ushort)bs.ReadBits(10)
            ));
        }
    }
}

public record SmoothMeshTessellationCommand5GroupData(ushort Start, ushort Num);
