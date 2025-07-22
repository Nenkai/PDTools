using PDTools.Utils;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;

public class SmoothMeshTessellationCommand21 : SmoothMeshTessellationCommandBase
{
    public bool Bit1 { get; set; }
    public bool HasUnkExtraData { get; set; }
    public byte[][] UnkExtraData { get; set; } = [];

    public override void Read(SmoothMeshTessellationCommandContext ctx, ref BitStream bs)
    {
        byte numEntries = (byte)bs.ReadBits(6);
        Bit1 = bs.ReadBoolBit();
        HasUnkExtraData = bs.ReadBoolBit();

        ctx.Opcode21_Bit1 = Bit1;

        if (HasUnkExtraData)
        {
            bs.AlignToNextByte();

            UnkExtraData = new byte[numEntries + 1][];
            for (int i = 0; i < numEntries + 1; i++)
            {
                UnkExtraData[i] = new byte[320 / 8];
                bs.ReadIntoByteArray(320 / 8, UnkExtraData[i], BitStream.Byte_Bits);
            }
        }
    }
}