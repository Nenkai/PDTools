using PDTools.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;

public class SmoothMeshTessellationCommand9 : SmoothMeshTessellationCommandBase
{
    public List<SmoothMeshTessellationCommand9Entry> Entries { get; set; } = [];
    public override void Read(SmoothMeshTessellationCommandContext ctx, ref BitStream bs)
    {
        byte numEntries = (byte)bs.ReadBits(6);
        for (int i = 0; i < numEntries; i++)
        {
            Entries.Add(new SmoothMeshTessellationCommand9Entry(
                Unk: (ushort)bs.ReadBits(12),
                Unk2: (byte)bs.ReadBits(5)
            ));
        }
    }
}

public record SmoothMeshTessellationCommand9Entry(ushort Unk, ushort Unk2);