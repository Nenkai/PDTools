using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;

/// <summary>
/// Used to read the smooth mesh commands
/// </summary>
public class SmoothMeshTessellationCommandContext
{
    // These ones are fields that the SPU job actually stores to keep track of state (for reading & processing)
    public ushort[] Unk0x1CIDs { get; set; } = [0, 0];
    public byte Opcode7LayoutIndex { get; set; } = 0;

    public byte Opcode22LayoutIndex { get; set; } = 1;
    public bool Opcode21_Bit1 { get; set; } = false;

    public int BitsPerIndex { get; set; }
    public List<PMSHMeshUnk0x1C> Unk0X1Cs { get; set; } = []; // This would have been a pointer

    // These fields are unoriginal here, since opcode 7 queues load until next opcode 7, we need to keep track of the last layout ID.
    public short LastLayoutID { get; set; } = -1;

    public SmoothMeshTessellationCommandContext(PackedMeshHeader header, PMSHSmoothMeshData smooth)
    {
        Unk0X1Cs = header.Unk0X1Cs;
        BitsPerIndex = smooth.PackedFaceIndexBitSize;
    }
}
