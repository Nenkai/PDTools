using PDTools.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;

public class SmoothMeshTessellationCommand7 : SmoothMeshTessellationCommandBase
{
    /// <summary>
    /// Index into PackedMesh header -> 0x1C
    /// </summary>
    public ushort UnkDataID { get; set; }

    public override void Read(SmoothMeshTessellationCommandContext ctx, ref BitStream bs)
    {
        UnkDataID = (ushort)bs.ReadBits(12);

        if (ctx.LastLayoutID != -1)
        {
            ctx.Unk0x1CIDs[ctx.Opcode7LayoutIndex ^ 1] = (ushort)ctx.LastLayoutID;
        }

        ctx.LastLayoutID = (short)UnkDataID;
        ctx.Opcode7LayoutIndex ^= 1; // Toggle
    }
}
