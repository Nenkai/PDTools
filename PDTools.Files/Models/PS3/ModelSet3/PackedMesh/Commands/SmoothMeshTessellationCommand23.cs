using PDTools.Utils;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;

public class SmoothMeshTessellationCommand23 : SmoothMeshTessellationCommandBase
{
    public byte EntryNum { get; set; }
    public ushort[] Unks { get; set; } = [];

    public override void Read(SmoothMeshTessellationCommandContext ctx, ref BitStream bs)
    {
        EntryNum = (byte)bs.ReadBits(6);
        Unks = new ushort[EntryNum + 1];

        for (int i = 0; i < EntryNum + 1; i++)
        {
            Unks[i] = (ushort)bs.ReadBits(ctx.Opcode21_Bit1 ? 16 : 12);
        }
    }
}