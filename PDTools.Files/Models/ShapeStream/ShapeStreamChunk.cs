using System;
using System.IO;
using System.Collections.Generic;
using System.Buffers;
using System.Buffers.Binary;

using Syroot.BinaryData;
using Syroot.BinaryData.Memory;
using Syroot.BinaryData.Core;

using ICSharpCode.SharpZipLib.Zip.Compression;

using PDTools.Files.Models.PS3.ModelSet3.ShapeStream;

namespace PDTools.Files.Models.ShapeStream;

public class ShapeStreamChunk
{
    public BinaryStream Stream;

    /// <summary>
    /// A chunk is never more than 0x10000 compressed (64kb)
    /// </summary>
    public const int MaxCompressedChunkSize = 0x10000;

    public Dictionary<ushort, ShapeStreamShape> Meshes { get; set; } = [];

    public static ShapeStreamChunk FromStream(Stream stream, MDL3ShapeStreamingChunkInfo shapeStreamInfo)
    {
        ShapeStreamChunk chunk = new();

        BinaryStream bs = new BinaryStream(stream);
        bs.Seek(shapeStreamInfo.DeflatedChunkOffset, SeekOrigin.Begin);

        byte[] deflatedChunk = ArrayPool<byte>.Shared.Rent((int)shapeStreamInfo.DeflatedChunkSize);
        bs.ReadExactly(deflatedChunk);

        var inflater = new Inflater(noHeader: true);
        inflater.SetInput(deflatedChunk, 0, deflatedChunk.Length);

        // Double the size should be safe to allocate
        byte[] inflatedBuffer = ArrayPool<byte>.Shared.Rent(0x20000);
        inflater.Inflate(inflatedBuffer);

        var meshReader = new SpanReader(inflatedBuffer, Endian.Big);
        foreach (MDL3ShapeStreamingInfoMeshEntry entry in shapeStreamInfo.Entries.Values)
        {
            meshReader.Position = (int)entry.OffsetInChunk;

            ShapeStreamShape mesh = new();
            mesh.ShapeStreamChunk = chunk;
            mesh.InfoMeshEntry = entry;

            uint meshSize = meshReader.ReadUInt32();
            mesh.MeshData = inflatedBuffer.AsMemory((int)entry.OffsetInChunk, (int)meshSize); // Skip chunk size
            meshReader.ReadInt32(); // Reloc Ptr
            meshReader.Position += 0x4;
            mesh.VerticesOffset = meshReader.ReadUInt32();
            mesh.TriOffset = meshReader.ReadUInt32();
            meshReader.Position += 0x4;
            mesh.BBoxOffset = meshReader.ReadUInt32();
            mesh.Unk0x1COffset = meshReader.ReadUInt32();

            chunk.Meshes.Add(entry.MeshIndex, mesh);
        }

        ArrayPool<byte>.Shared.Return(deflatedChunk);

        return chunk;
    }
}
