using PDTools.Files.Textures.PS2;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Courses.PS2;

public class MiniMapSet
{
    /// <summary>
    /// 'GTCM'
    /// </summary>
    public const uint MAGIC = 0x4D435447;
    public const uint HEADER_SIZE = 0x80;

    public uint Width { get; set; }
    public uint Height { get; set; }
    public float WorldOriginX { get; set; }
    public float WorldOriginY { get; set; }

    /// <summary>
    /// Scale proportion to the actual course.
    /// </summary>
    public float Scale { get; set; }

    /// <summary>
    /// In Degrees (°).
    /// </summary>
    public float Angle { get; set; }

    public TextureSet1 TextureSet { get; set; } = new();

    /// <summary>
    /// Used for logger/data analyzer
    /// </summary>
    public List<MiniMapChunk> Chunks { get; set; } = [];

    /// <summary>
    /// Unfinished, not to be used (doesn't read past header)
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public void FromStream(Stream stream)
    {
        BinaryStream bs = new BinaryStream(stream);
        long basePos = bs.Position;

        MiniMapSet miniMapSet = new MiniMapSet();
        uint magic = bs.ReadUInt32();

        if (magic != MAGIC)
            throw new InvalidDataException("Invalid MiniMapSet magic.");

        bs.Position = basePos + 0x10;
        Width = bs.ReadUInt32();
        Height = bs.ReadUInt32();
        WorldOriginX = bs.ReadSingle();
        WorldOriginY = bs.ReadSingle();
        Scale = bs.ReadSingle();
        Angle = bs.ReadSingle();

        uint texSetOffset = bs.ReadUInt32();
        uint chunkCount = bs.ReadUInt32();
        uint chunksOffset = bs.ReadUInt32();

        ushort unk1 = bs.ReadUInt16(); // Unused
        ushort unk2 = bs.ReadUInt16(); // Unused

        bs.Position = basePos + texSetOffset;
        TextureSet.FromStream(bs);

        bs.Position = chunksOffset;
        for (int i = 0; i < chunkCount; i++)
        {
            var chunk = new MiniMapChunk();
            chunk.Read(bs);
            Chunks.Add(chunk);
        }
    }
}

public class MiniMapChunk
{
    public float A { get; set; }
    public float B { get; set; }
    public float C { get; set; }
    public float D { get; set; }

    public void Read(BinaryStream bs)
    {
        A = bs.ReadSingle();
        B = bs.ReadSingle();
        C = bs.ReadSingle();
        D = bs.ReadSingle();
    }

    public void Write(BinaryStream bs)
    {
        bs.WriteSingle(A);
        bs.WriteSingle(B);
        bs.WriteSingle(C);
        bs.WriteSingle(D);
    }

    public override string ToString()
    {
        return $"<{A},{B},{C},{D}>";
    }
}
