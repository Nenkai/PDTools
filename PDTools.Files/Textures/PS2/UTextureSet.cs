using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Syroot.BinaryData;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace PDTools.Files.Textures.PS2;

/// <summary>
/// UTexture Set - Texture holder for GT2K.
/// </summary>
public class UTextureSet : TextureSetPS2Base
{
    /// <summary>
    /// Magic - "UTex".
    /// </summary>
    public const uint MAGIC = 0x78655455;
    public const uint HeaderSize = 0x14;

    public void FromStream(Stream stream)
    {
        long basePos = stream.Position;

        if (stream.Length - stream.Position < HeaderSize)
            throw new InvalidDataException("UTextureSet header size too small");

        var bs = stream is BinaryStream ? (BinaryStream)stream : new BinaryStream(stream);
        uint magic = bs.ReadUInt32();
        if (magic != MAGIC)
            throw new InvalidDataException("Expected UTex magic");

        int relocPtr = bs.ReadInt32();
        bs.ReadUInt16(); // base tbp
        TotalBlockSize = bs.ReadUInt16();
        ushort pgluTextureCount = bs.ReadUInt16();
        bs.ReadUInt16();
        bs.ReadUInt32();
        uint textureInfosOffset = bs.ReadUInt32();

        for (var i = 0; i < pgluTextureCount; i++)
        {
            PGLUtexture tex = new PGLUtexture();
            tex.Read(bs);
            pgluTextures.Add(tex);
        }

        bs.Position = basePos + textureInfosOffset;
        while (true)
        {
            long baseThingPos = bs.Position;
            uint nextOffset = bs.ReadUInt32();
            if (nextOffset == 0)
                break;

            // Read old format
            GSTransfer transfer = new GSTransfer();
            transfer.ReadOld(bs);

            GSTransfers.Add(transfer);
            bs.Position = (baseThingPos + nextOffset);
        }

        InitializeGSMemory();
    }

    /// <summary>
    /// Gets a texture by index in this texture set.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public override Image<Rgba32> GetTextureImage(int index, int varIndex = 0)
    {
        if (index > pgluTextures.Count)
            throw new IndexOutOfRangeException("Texture index is out of range.");

        PGLUtexture texture = pgluTextures[index];
        return GetImageData(texture);
    }

    // Credits tiledggd
    private static Rgba32[] MakeTiledPalette(Span<Rgba32> pal)
    {
        const int tileSizeW = 8;
        const int tileSizeH = 2;

        Rgba32[] outpal = new Rgba32[256];
        int ntx = 16 / tileSizeW,
            nty = 16 / tileSizeH;
        int i = 0;

        for (int ty = 0; ty < nty; ty++)
            for (int tx = 0; tx < ntx; tx++)
                for (int y = 0; y < tileSizeH; y++)
                    for (int x = 0; x < tileSizeW; x++)
                        outpal[(ty * tileSizeH + y) * 16 + (tx * tileSizeW + x)] = pal[i++];

        return outpal;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;
            foreach (var texture in pgluTextures)
                hash += texture.GetHashCode();

            foreach (var transfer in GSTransfers)
                hash += transfer.GetHashCode();

            return hash;
        }
    }
}
