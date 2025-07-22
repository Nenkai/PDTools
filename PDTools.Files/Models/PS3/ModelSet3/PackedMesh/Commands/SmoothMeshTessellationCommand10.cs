using PDTools.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;

public class SmoothMeshTessellationCommand10 : SmoothMeshTessellationCommandBase
{
    public ushort Unk { get; set; }
    public List<SmoothMeshTessellationCommand10Entry> Entries { get; set; } = [];
    public override void Read(SmoothMeshTessellationCommandContext ctx, ref BitStream bs)
    {
        byte numEntries = (byte)bs.ReadBits(8);
        Unk = (ushort)bs.ReadBits(12);

        for (int i = 0; i < numEntries + 1; i++)
        {
            Entries.Add(new SmoothMeshTessellationCommand10Entry(
                Unk: (ushort)bs.ReadBits(5),
                Unk2: (byte)bs.ReadBits(10)
            ));
        }
    }
}

public record SmoothMeshTessellationCommand10Entry(ushort Unk, ushort Unk2);