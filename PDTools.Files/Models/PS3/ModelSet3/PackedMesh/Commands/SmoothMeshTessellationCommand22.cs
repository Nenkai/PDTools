using PDTools.Utils;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;

public class SmoothMeshTessellationCommand22 : SmoothMeshTessellationCommandBase
{
    public bool IsSwitchLayout { get; set; }
    public byte EntryNum { get; set; }
    public ushort[][] UnkGroups { get; set; } = [];

    public override void Read(SmoothMeshTessellationCommandContext ctx, ref BitStream bs)
    {
        IsSwitchLayout = bs.ReadBoolBit();
        EntryNum = (byte)bs.ReadBits(8);

        if (IsSwitchLayout)
            ctx.Opcode22LayoutIndex ^= 1; // Toggle

        PMSHMeshUnk0x1C unk0X1C = ctx.Unk0X1Cs[ctx.Unk0x1CIDs[ctx.Opcode22LayoutIndex]];
        uint numEntries = MiscUtils.AlignValue((uint)EntryNum + 1, 4) / 4;

        UnkGroups = new ushort[numEntries][];
        for (int i = 0; i < numEntries; i++)
        {
            UnkGroups[i] = new ushort[unk0X1C.Count_0x01 * 4];
            for (int j = 0; j < unk0X1C.Count_0x01 * 4; j++)
            {
                UnkGroups[i][j] = (ushort)bs.ReadBits(ctx.BitsPerIndex);
            }
        }
    }
}