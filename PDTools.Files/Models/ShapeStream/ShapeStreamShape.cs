using System;
using Syroot.BinaryData.Memory;
using System.IO;
using PDTools.Files.Models.PS3.ModelSet3.ShapeStream;

namespace PDTools.Files.Models.ShapeStream;

public class ShapeStreamShape
{
    public const int HeaderSize = 0x80;

    public ShapeStreamChunk ShapeStreamChunk { get; set; }
    public MDL3ShapeStreamingInfoShapeEntry InfoMeshEntry { get; set; }

    public Memory<byte> MeshData { get; set; }
    public uint ChunkSize { get; set; }
    public uint VerticesOffset { get; set; }
    public uint TriOffset { get; set; }
    public uint BBoxOffset { get; set; }
    public uint Unk0x1COffset { get; set; }
}
