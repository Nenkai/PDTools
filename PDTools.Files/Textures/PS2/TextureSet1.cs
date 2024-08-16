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

using PDTools.Utils;

namespace PDTools.Files.Textures.PS2;

/* So. Tex1 might seem like a simple format, but it can get complicated really quick.
 * If you wanna follow along, grab 010 Editor and this template
 * https://github.com/Nenkai/GT-File-Specifications-Documentation/blob/master/Formats/GT4/GT4_Tex1_TexSet.bt
 * 
 * PGLUTextures defines the textures in the set, and passes GS registers for each one. 
 * Any tbp field (including mipmap) is remapped at runtime. 
 * 
 * The GS Transfers are the hard part. 
 * 
 * But before explaining the transfers, it's important to be familiar with the GS's block/page system,
 * so refer to Page 161<->175 of the GS's Users Manual (Docs&Training\HardwareManuals in PS2 SDK).
 * 
 * The important registers to keep in mind are TBP and CBP (in tex0). These are block pointers/offsets.
 * Blocks are in essence just 256 bytes aka "64 words". They kinda work as a separate coordinate system and denotes where pixels go in GS (per pixel format).
 *
 * When you have a texture that's for instance 350x350, the height and width are raised to the next power of 2, so 512x512.
 * That leaves a space with what's rendered and what isn't (350<->512), so extra data can be put there, it can be the image's palette, or another texture
 * So don't be surprised if you see the CBP register of a texture in the middle of what would appear to be the main texture's.
 * 
 * Now, for GS transfers.
 * 
 * Suppose you have one basic texture with a palette, PDI's builder simply builds two transfers - one with the image data, the other with the palette.
 * Simple enough, right?
 * Texture sets with more than one texture are "swizzled" into buffers converted from i.e 4bit/8bit to PSMCT32 (32 bit) so the GS can load them faster.
 * To read them (and convert to png), I used GSTextureConvert.
 * https://ps2linux.no-ip.info/playstation2-linux.com/projects/ezswizzle/
 * 
 * You can also use TextureSwizzling.pdf for some notes, along with:
 * - Docs&Training\Starting Guides\Graphics Synthesizer Starting Guide.pdf (PS2 SDK)
 * - Source in Shell\Tools\shellTexture\
 * - ee\sample\graphics\textrans\bitconv
 * 
 * For an example, look at advertise/us/premium.img (GT4 Online).
 * There's 3 transfers, 64x1216, 32x16 and 8x8.
 * 
 * TextureSet1 makes uses of 4 rather complex optimizations (in order of most important):
 * 1. [PARTIALLY DONE] Textures, or palettes, can be inside the non-rendered area of other textures, to save on GS blocks
 *    ^ could use some more optis or algorithms to reorder the textures in a way that maximizes usage of unused blocks
 *    
 * 2. Texture data is sometimes reused with the same TBP when a different palette is used, to save on GS blocks
 *    ^ This one is super weird. See: GT3's vw0020 (day) - Texture Set 0, Texture Index 30 and 31
 *      - TBP E0 & CSA 4: 128x128 - SCE_GS_PSMT4 (SCE_GS_REGION_CLAMP, SCE_GS_REGION_CLAMP)
 *      - TBP E0 & CSA 2: 256x128 - SCE_GS_PSMT4 (SCE_GS_REGION_CLAMP, SCE_GS_REGION_CLAMP)
 *      
 * 3. [DONE] When a different palette is used for certain textures, the CSA register can be set, which presumably avoids using an extra block for a palette.
 *    Example: A free block (256 bytes) left by a texture that uses PSMT4 (8x2 palette, 64 bytes) essentially means that 4 palettes that can be stored there (csa 0, 2, 4, 6).
 * 
 * 4. [DONE] Swizzling - Multiple texture buffers of different formats swizzled into PSMCT32 for faster upload to GS
 *   -> Done in TextureSetBuilder in Build(), read the note though
 */

/// <summary>
/// Texture Set 1 - Texture holder for GT3/4/TT.
/// </summary>
public class TextureSet1 : TextureSetPS2Base
{
    /// <summary>
    /// Magic - "Tex1".
    /// </summary>
    public const uint MAGIC = 0x31786554;
    public const uint HeaderSize = 0x30;

    public List<ClutPatchSet> ClutPatchSet { get; set; } = [];

    public void FromStream(Stream stream)
    {
        long basePos = stream.Position;

        if (stream.Length - stream.Position < HeaderSize)
            throw new InvalidDataException("TextureSet1 header size too small");

        var bs = new BinaryStream(stream);
        uint magic = bs.ReadUInt32();
        if (magic != MAGIC)
            throw new InvalidDataException("Expected Tex1 magic");

        int relocPtr = bs.ReadInt32();
        int empty = bs.ReadInt32();
        int textureSetSize = bs.ReadInt32();

        if (stream.Length - basePos < textureSetSize)
            throw new InvalidDataException("Texture data provided is smaller than texture set specified size.");

        bs.Position = basePos;
        _inputData = bs.ReadBytes(textureSetSize);
        bs.Position = basePos + 0x10;

        short baseTbp = bs.ReadInt16(); // Realistically always 0, only remapped at runtime
        TotalBlockSize = bs.ReadUInt16();
        ushort pgluTextureCount = bs.ReadUInt16();
        ushort textureInfoCount = bs.ReadUInt16();
        uint pgluTextureMapOffset = bs.ReadUInt32();
        uint textureInfosOffset = bs.ReadUInt32();
        uint clutPatchesOffset = bs.ReadUInt32();

        bs.Position = basePos + (int)pgluTextureMapOffset;
        for (var i = 0; i < pgluTextureCount; i++)
        {
            PGLUtexture tex = new PGLUtexture();
            tex.Read(bs);
            pgluTextures.Add(tex);
        }

        for (var i = 0; i < textureInfoCount; i++)
        {
            GSTransfer transfer = new GSTransfer();
            bs.Position = basePos + (int)textureInfosOffset + (i * GSTransfer.GetSize());
            transfer.Read(bs, basePos);
            GSTransfers.Add(transfer);
        }

        if (clutPatchesOffset != 0)
        {
            bs.Position = basePos + clutPatchesOffset;
            uint clutPatchCount = bs.ReadUInt32();
            uint[] clutPatchesOffsets = bs.ReadUInt32s((int)clutPatchCount);

            for (int i = 0; i < clutPatchCount; i++)
            {
                bs.Position = basePos + clutPatchesOffsets[i];
                ClutPatchSet patch = new ClutPatchSet();
                patch.Read(bs);
                ClutPatchSet.Add(patch);
            }
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

        TextureClutPatch textureClutPatch = null;
        if (varIndex < ClutPatchSet.Count)
            textureClutPatch = ClutPatchSet[varIndex].TexturesToPatch.FirstOrDefault(e => e.PGLUTextureIndex == index);

        return GetImageData(texture, textureClutPatch);
    }

    public void Serialize(Stream stream)
    {
        var bs = new BinaryStream(stream, ByteConverter.Little);

        long basePos = bs.Position;

        // Workaround to doing:
        // - bs.Position += HeaderSize
        // Would not write if contents of the tex set were empty
        bs.WriteBytes(new byte[HeaderSize]);
        
        uint pgluTextureOffset = (uint)(bs.Position - basePos);

        for (var i = 0; i < pgluTextures.Count; i++)
            pgluTextures[i].Write(bs);

        uint transferInfoOffset = (uint)(bs.Position - basePos);
        uint dataOffset = (uint)(transferInfoOffset + (GSTransfers.Count * GSTransfer.GetSize()));
        dataOffset = MiscUtils.AlignValue(dataOffset, 0x10);

        long lastOffset = bs.Position;
        long lastOffsetWithPadding = bs.Position;

        for (var i = 0; i < GSTransfers.Count; i++)
        {
            GSTransfer transfer = GSTransfers[i];

            bs.Position = basePos + transferInfoOffset + (i * GSTransfer.GetSize());
            transfer.DataOffset = dataOffset;
            transfer.Write(bs);

            bs.Position = basePos + (int)dataOffset;
            bs.Write(transfer.Data);

            lastOffset = bs.Position;

            bs.Align(0x10, grow: true); // Align. Appears to be the required minimum of padding required, otherwise gs memory gets jumbled for small textures

            lastOffsetWithPadding = lastOffset;
            dataOffset = (uint)(bs.Position - basePos);
        }

        uint clutPatchesOffset = 0;
        if (ClutPatchSet.Count > 0)
        {
            clutPatchesOffset = (uint)(bs.Position - basePos);
            bs.WriteUInt32((uint)ClutPatchSet.Count);

            long tableOffset = bs.Position;
            lastOffset = (uint)(bs.Position + (ClutPatchSet.Count * sizeof(uint)));

            for (int i = 0; i < ClutPatchSet.Count; i++)
            {
                bs.Position = tableOffset + (i * sizeof(uint));
                bs.WriteUInt32((uint)(lastOffset - basePos));

                bs.Position = lastOffset;

                ClutPatchSet patch = ClutPatchSet[i];
                patch.Write(bs);

                lastOffset = (uint)bs.Position;
            }

            bs.Align(0x10, grow: true);
            lastOffset = bs.Position;
            lastOffsetWithPadding = bs.Position;
           
        }

        uint fileSize = (uint)(lastOffset - basePos);

        // Write header
        bs.Position = basePos;
        bs.WriteUInt32(MAGIC); // Tex1
        bs.WriteUInt32(0); // Reloc ptr
        bs.WriteUInt32(0); // Unk ptr
        bs.WriteUInt32(fileSize); // File size
        bs.WriteUInt16(0); // Base TBP - should be zero
        bs.WriteUInt16(TotalBlockSize); // Total size in blocks
        bs.WriteUInt16((ushort)pgluTextures.Count);
        bs.WriteUInt16((ushort)GSTransfers.Count);
        bs.WriteUInt32(pgluTextureOffset);
        bs.WriteUInt32(transferInfoOffset);
        bs.WriteUInt32(clutPatchesOffset);

        bs.Position = lastOffsetWithPadding;
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

            foreach (var patch in ClutPatchSet)
                hash += patch.GetHashCode();

            return hash;
        }
    }
}
