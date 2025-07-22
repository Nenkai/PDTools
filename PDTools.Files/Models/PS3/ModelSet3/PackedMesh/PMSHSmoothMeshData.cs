using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;
using PDTools.Utils;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh;

/// <summary>
/// Smooth mesh/tessellation data, to be processed by the 'SmoothMesh' WWS SPU Job.
/// </summary>
public class PMSHSmoothMeshData
{
    public byte[] PackedVertexData { get; set; }
    public byte[] PackedIndexBufferData { get; set; }
    public byte[] PackedCommandsData { get; set; }
    public ushort PackedFlexVertCount { get; set; }
    public ushort NonPackedFlexVertCount { get; set; }
    public byte PackedFaceIndexBitSize { get; set; }

    public static PMSHSmoothMeshData FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        PMSHSmoothMeshData data = new();

        int packedVertexDataOffset = bs.ReadInt32();
        int unkOffset_0x04 = bs.ReadInt32();
        int unkOffset_0x08 = bs.ReadInt32();
        int nonPackedVertexDataOffset = bs.ReadInt32();
        int unkOffset_0x10 = bs.ReadInt32();
        int packedIndexBufferOffset = bs.ReadInt32();
        int packedCommandsOffset = bs.ReadInt32();
        bs.ReadInt32(); // Unk 0
        short field_0x20 = bs.ReadInt16(); // Unk
        data.PackedFlexVertCount = bs.ReadUInt16();
        bs.ReadInt16();
        data.NonPackedFlexVertCount = bs.ReadUInt16();
        ushort unkQuadwords = bs.ReadUInt16();
        ushort packedIndexBufferQuadwordSize = bs.ReadUInt16();
        ushort packedCommandsQuadwordSize = bs.ReadUInt16();
        data.PackedFaceIndexBitSize = bs.Read1Byte();
        bs.Read1Byte();

        /* Depends on flex vert definition
        if (packedVertexDataOffset != 0)
        {
            bs.Position = mdlBasePos + packedVertexDataOffset;
        }
        */

        if (packedIndexBufferOffset != 0)
        {
            bs.Position = mdlBasePos + packedIndexBufferOffset;
            data.PackedIndexBufferData = bs.ReadBytes(packedIndexBufferQuadwordSize * 0x10);
        }

        if (packedCommandsOffset != 0)
        {
            bs.Position = mdlBasePos + packedCommandsOffset;
            data.PackedCommandsData = bs.ReadBytes(packedCommandsQuadwordSize * 0x10);
        }

        return data;
    }

    public SmoothMeshTessellationCommandList GetCommands(PackedMeshHeader header)
    {
        BitStream bs = new BitStream(BitStreamMode.Read, PackedCommandsData, BitStreamSignificantBitOrder.LSB);

        var ctx = new SmoothMeshTessellationCommandContext(header, this);
        var list = SmoothMeshTessellationCommandList.Read(ctx, ref bs);

        return list;
    }

    public int GetOffsetOfPackedElement(PMSHElementBitLayoutArray bitLayouts, PMSHFlexVertexDefinition vertDefinition, string type)
    {
        int byteOffset = 0;

        int currentLayoutIndex = 0;
        foreach (var elem in vertDefinition.PackedElements)
        {
            if (elem.Key == "colorSet1")
                continue;

            if (elem.Key == type)
                break;

            byteOffset += (PackedFlexVertCount * bitLayouts.Layouts[currentLayoutIndex].TotalBitCount + 7) / 8;
            currentLayoutIndex++;
        }

        return byteOffset;
    }

    public int GetTotalByteSizeOfPackedElement(PMSHElementBitLayoutArray bitLayouts, PMSHFlexVertexDefinition vertDefinition, string type)
    {
        int currentLayoutIndex = 0;
        foreach (var elem in vertDefinition.Elements)
        {
            if (elem.Name == "colorSet1")
                continue;

            if (elem.Name == type)
                return 4 * PackedFlexVertCount;

            currentLayoutIndex++;
        }

        currentLayoutIndex = 0;
        foreach (var elem in vertDefinition.PackedElements)
        {
            if (elem.Key == "colorSet1")
                continue;

            if (elem.Key == type)
                return (bitLayouts.Layouts[currentLayoutIndex].TotalBitCount * PackedFlexVertCount + 7) / 8;

            currentLayoutIndex++;
        }

        return -1;
    }

    public static int GetSize()
    {
        return 0x30;
    }
}
