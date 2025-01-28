using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;

using PDTools.Utils;
using PDTools.Files.Textures.PS2.GSPixelFormats;

using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.HighPerformance;

namespace PDTools.Files.Textures.PS2;

public class TextureSetBuilder
{
    private readonly ILogger _logger;

    // Underlaying texture set
    private TextureSet1 _texSet = new();

    // List of all textures in the builder
    private List<TextureTask> _textures = [];


    /* Used to keep track of GS blocks without texture data allocated
     * So that we can put other textures's data in them */
    private Dictionary<int, GSBlock> _unusedGsBlocksIndices = [];

    /* Used to keep track of all GS blocks we've used up */
    private List<ushort> _usedGsBlocksIndices = [];

    private int _lastFreeVerticalBlock = -1;

    private ushort _tbp_Textures = 0;
    private GSMemory _gsMemory = new();

    private List<List<ClutPatchTask>> _clutPatchSets = [];

    public int TextureCount => _textures.Count;
    public IReadOnlyList<TextureTask> Textures => _textures;
    public TextureSetBuilder(ILogger log = null)
    {
        _logger = log;
    }

    /// <summary>
    /// Adds an image to the texture set.
    /// </summary>
    /// <param name="imagePath">Path to the image.</param>
    /// <param name="config">Texture config</param>
    public void AddImage(string imagePath, TextureConfig config)
    {
        Image<Rgba32> img = Image.Load<Rgba32>(imagePath);

        AddImage(img, config);
    }

    /// <summary>
    /// Adds an image to the texture set.
    /// </summary>
    /// <param name="img">Image.</param>
    /// <param name="config">Texture config</param>
    private void AddImage(Image<Rgba32> img, TextureConfig config)
    {
        _logger?.LogInformation("Adding image {x}x{y}, format={format}", img.Width, img.Height, config.Format);

        if (config.IsTextureMap)
            img.Mutate(e => e.Resize((int)BitOperations.RoundUpToPowerOf2((uint)img.Width), (int)BitOperations.RoundUpToPowerOf2((uint)img.Height)));

        var pgluTexture = new PGLUtexture();
        pgluTexture.tex0.PSM = config.Format;

        // Calculate bounds of textures, which may be beyond actual render dimensions
        uint widthPow2 = BitOperations.RoundUpToPowerOf2((uint)img.Width);
        uint heightPow2 = BitOperations.RoundUpToPowerOf2((uint)img.Height);

        if (config.WrapModeS == SCE_GS_CLAMP_PARAMS.SCE_GS_REGION_CLAMP)
            pgluTexture.tex0.TW_TextureWidth = (byte)Math.Log(widthPow2, 2);
        else
        {
            if (!BitOperations.IsPow2(config.RepeatWidth))
                throw new ArgumentException("Repeat width must be a power of 2.");

            pgluTexture.tex0.TW_TextureWidth = (byte)Math.Log(config.RepeatWidth, 2);
        }

        if (config.WrapModeS == SCE_GS_CLAMP_PARAMS.SCE_GS_REGION_CLAMP)
            pgluTexture.tex0.TH_TextureHeight = (byte)Math.Log(heightPow2, 2);
        else
        {
            if (!BitOperations.IsPow2(config.RepeatHeight))
                throw new ArgumentException("Repeat height must be a power of 2.");

            pgluTexture.tex0.TH_TextureHeight = (byte)Math.Log(config.RepeatHeight, 2);
        }

        // Calculate TBW
        pgluTexture.tex0.TBW_TextureBufferWidth = (byte)((img.Width + 63) / 64);
        if ((pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT4 || pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8) && (pgluTexture.tex0.TBW_TextureBufferWidth & 1) != 0)
            pgluTexture.tex0.TBW_TextureBufferWidth++;

        // Set actual render dimensions
        pgluTexture.ClampSettings.WMS = config.WrapModeS;
        pgluTexture.ClampSettings.WMT = config.WrapModeT;
        pgluTexture.ClampSettings.MAXU = (ulong)img.Width - 1;
        pgluTexture.ClampSettings.MAXV = (ulong)img.Height - 1;

        // PSMT8 and PSMT4 are indexed formats, flag as clut use 
        if (pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8 || pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT4)
            pgluTexture.tex0.CLD_ClutBufferLoadControl = 1;

        // Color enabled
        pgluTexture.tex0.TCC_ColorComponent = 1;

        var textureTask = new TextureTask();
        textureTask.Image = img;
        textureTask.TexturePixelFormat = GSPixelFormat.GetFormatFromPSMFormat(pgluTexture.tex0.PSM);
        textureTask.SizeInGSBlocks = (ushort)(textureTask.TexturePixelFormat.GetLastBlockIndexForImageDimensions(img.Width, img.Height) + 1);
        textureTask.UnusedGSBlocks = textureTask.TexturePixelFormat.GetUnusedBlocks(textureTask.Image.Width, textureTask.Image.Height, out int firstFreeVerticalBlock);
        textureTask.FirstFreeVerticalBlock = firstFreeVerticalBlock;
        textureTask.PGLUTexture = pgluTexture;

        // Quantize image and create palette for formats that requires it
        if (pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8 || pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT4)
        {
            int paletteSize = pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8 ? 256 : 16;
            Rgba32[] fullPalette = new Rgba32[paletteSize];
            textureTask.IndexedImage = new byte[img.Height, img.Width];

            bool fits = ImageFitsColorPalette(img, paletteSize, out List<Rgba32> colorPalette);
            if (!fits)
            {
                _logger?.LogInformation("Quantizing texture {x}x{y} for format {format} as color palette is larger than can fit..", img.Width, img.Height, pgluTexture.tex0.PSM);

                // Quantize image
                var quantizer = new WuQuantizer(new QuantizerOptions { Dither = null, MaxColors = paletteSize });
                ImageFrame<Rgba32> frame = img.Frames.RootFrame;

                using IQuantizer<Rgba32> frameQuantizer = quantizer.CreatePixelSpecificQuantizer<Rgba32>(Configuration.Default);
                using IndexedImageFrame<Rgba32> result = frameQuantizer.BuildPaletteAndQuantizeFrame(frame, frame.Bounds());

                // Copy the result (access to imagesharp's indexedimage buffer is not possible)
                Memory2D<byte> mem2d = new(textureTask.IndexedImage);
                for (int y = 0; y < result.Height; y++)
                {
                    ReadOnlySpan<byte> row = result.DangerousGetRowSpan(y);
                    mem2d[y.., 0..].TryGetMemory(out Memory<byte> targetRow);
                    row.CopyTo(targetRow.Span);
                }
                
                ReadOnlyMemory<Rgba32> palette = result.Palette;
                palette.CopyTo(fullPalette);
            }
            else
            {
                _logger?.LogInformation("Adding texture {x}x{y} for format {format}..", img.Width, img.Height, pgluTexture.tex0.PSM);

                // Avoid quantization, build directly if it fits
                for (int y = 0; y < img.Height; y++)
                {
                    for (int x = 0; x < img.Width; x++)
                    {
                        int col = colorPalette.IndexOf(img[x, y]);
                        textureTask.IndexedImage[y, x] = (byte)col;
                    }
                }

                colorPalette.CopyTo(fullPalette);
            }

            // Scale down alpha for PS2 (0-128)
            for (int i = 0; i < fullPalette.Length; i++)
                fullPalette[i].A = (byte)Tex1Utils.Normalize(fullPalette[i].A, 0x00, 0xFF, 0x00, 0x80);

            if (pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8)
            {
                var (TiledPalette, LinearToTiledPaletteIndices) = MakeTiledPaletteFromLinearPalette(fullPalette);
                textureTask.Palette = TiledPalette;
            }
            else
            {
                textureTask.Palette = fullPalette;
            }
        }

        _texSet.pgluTextures.Add(pgluTexture);
        CreateImageData(textureTask, pgluTexture.tex0.PSM);
        _textures.Add(textureTask);
    }


    public void AddClutPatch(int clutPatchSetIndex, int pgluTextureIndex, string path)
    {
        if (clutPatchSetIndex == 0)
            throw new ArgumentException("Clut patch set 0 is reserved for the base texture.", nameof(clutPatchSetIndex));

        PGLUtexture pgluTexture = _texSet.pgluTextures[pgluTextureIndex];
        using var img = Image.Load<Rgba32>(path);
        int paletteSize = pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8 ? 256 : 16;
        Rgba32[] fullPalette = new Rgba32[paletteSize];

        bool fits = ImageFitsColorPalette(img, paletteSize, out List<Rgba32> colorPalette);
        if (fits)
            colorPalette.CopyTo(fullPalette);
        else
            throw new Exception($"Texture file '{path}' must use less than {paletteSize} colors ahead of time.");

        ClutPatchTask clutPatch = new ClutPatchTask(fullPalette);

        while (_clutPatchSets.Count <= clutPatchSetIndex)
            _clutPatchSets.Add([]);

        _clutPatchSets[clutPatchSetIndex].Add(clutPatch);
    }

    /// <summary>
    /// Builds the texture set.
    /// </summary>
    /// <returns></returns>
    public TextureSet1 Build()
    {
        WriteTexturesOptimized();

        WritePalettes();

        WriteClutPatches();

        bool swizzle = _textures.Count >= 2;
        if (swizzle)
            BuildSwizzledTransfers();
        else
            BuildTransfers();

        _texSet.TotalBlockSize = _tbp_Textures;
        return _texSet;
    }

    private void WritePalettes()
    {
        if (_clutPatchSets.Count > 1)
            _texSet.ClutPatchSet.Add(new ClutPatchSet());

        for (ushort textureIndex = 0; textureIndex < _textures.Count; textureIndex++)
        {
            TextureTask texture = _textures[textureIndex];
            if (texture.Palette is not null)
            {
                int width = texture.PGLUTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8 ? 16 : 8;
                int height = texture.PGLUTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8 ? 16 : 2;

                (ushort CBP, byte CSA) = FitPaletteToGSMemory(texture.PGLUTexture.tex0.PSM, width, height, texture.Palette, reuseOldPaletteLocations: false);
                texture.PGLUTexture.tex0.CBP_ClutBlockPointer = CBP;
                texture.PGLUTexture.tex0.CSA_ClutEntryOffset = CSA;
            }

            if (_clutPatchSets.Count > 1)
            {
                _texSet.ClutPatchSet[0].TexturesToPatch.Add(new TextureClutPatch()
                {
                    CBP_ClutBufferBasePointer = texture.PGLUTexture.tex0.CBP_ClutBlockPointer,
                    Format = texture.PGLUTexture.tex0.PSM,
                    PGLUTextureIndex = textureIndex,
                    CSA_ClutEntryOffset = texture.PGLUTexture.tex0.CSA_ClutEntryOffset,
                });
            }
        }
    }

    /// <summary>
    /// Attempts to fit the specified palette into unused blocks. Otherwise allocates new blocks.
    /// </summary>
    /// <param name="textureFormat"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="palette"></param>
    /// <param name="reuseOldPaletteLocations"></param>
    /// <returns></returns>
    private (ushort CBP, byte CSA) FitPaletteToGSMemory(SCE_GS_PSM textureFormat, int width, int height, Rgba32[] palette, bool reuseOldPaletteLocations = false)
    {
        if (reuseOldPaletteLocations)
        {
            for (int i = 0; i < _texSet.pgluTextures.Count; i++)
            {
                // Is there an identical palette somewhere already?
                PGLUtexture pgluTexture = _texSet.pgluTextures[i];
                if (pgluTexture.tex0.PSM == textureFormat && _textures[i].Palette.AsSpan().SequenceEqual(palette))
                {
                    // return its cbp - Save on size
                    return (pgluTexture.tex0.CBP_ClutBlockPointer, pgluTexture.tex0.CSA_ClutEntryOffset);
                }
            }
        }

        /* The game cheats a bit with "CSA" - is it even used for its original purpose?
         * "CSA" here is used as an offset WITHIN the block itself
         * So a block (256 bytes) can store 4 PSMT4 palettes (64 * 4)
         * CSA goes every 32 */

        List<ushort> usedBlocksOfTexture = GSPixelFormat.PSM_CT32.GetUsedBlocks(width, height);
        int size = Tex1Utils.GetDataSize(width, height, SCE_GS_PSM.SCE_GS_PSMCT32);
        int csa = 0;
        byte csaTakenSpace = (byte)Math.Min(size / 32, 8);
        int idx = CanFitBlocksInUnusedBlocks(usedBlocksOfTexture, csaTakenSpace);

        ushort cbp;
        if (idx != -1)
        {
            cbp = (ushort)idx;

            // Is this a palette that fits in one singular block?
            if (usedBlocksOfTexture.Count == 1)
            {
                GSBlock block = _unusedGsBlocksIndices[idx + usedBlocksOfTexture[0]];
                csa = block.CurrentCSA;

                block.CurrentCSA += csaTakenSpace;
                if (block.CurrentCSA >= 8)
                {
                    // Block CSA is 8 (32 * 8 = 256 bytes). This block is filled, move on
                    _unusedGsBlocksIndices.Remove(block.Index);
                    _usedGsBlocksIndices.Add((ushort)block.Index);
                    _tbp_Textures++;
                }
            }
            else
            {
                for (int i = 0; i < usedBlocksOfTexture.Count; i++)
                {
                    _unusedGsBlocksIndices.Remove((ushort)(idx + usedBlocksOfTexture[i]));
                    _usedGsBlocksIndices.Add((ushort)(idx + usedBlocksOfTexture[i]));
                }
            }
        }
        else
        {
            cbp = _tbp_Textures;
            _tbp_Textures += (ushort)usedBlocksOfTexture.Count;

            if (usedBlocksOfTexture.Count == 1)
            {
                csa = csaTakenSpace;

                GSBlock partialFilledBlock = new GSBlock(_tbp_Textures + usedBlocksOfTexture[0], csaTakenSpace);
                _unusedGsBlocksIndices.Add(partialFilledBlock.Index, partialFilledBlock);
            }
            else
            {
                for (int i = 0; i < usedBlocksOfTexture.Count; i++)
                    _usedGsBlocksIndices.Add((ushort)(_tbp_Textures + usedBlocksOfTexture[i]));
            }
        }

        switch (textureFormat)
        {
            case SCE_GS_PSM.SCE_GS_PSMT8:
                _gsMemory.WriteTexPSMCT32(cbp, 1,
                    0, 0,
                    16, 16,
                    MemoryMarshal.Cast<Rgba32, uint>(palette),
                    csa * 32);
                break;

            case SCE_GS_PSM.SCE_GS_PSMT4:
                _gsMemory.WriteTexPSMCT32(cbp, 1,
                    0, 0,
                    8, 2,
                    MemoryMarshal.Cast<Rgba32, uint>(palette),
                    csa * 32);
                break;
        }

        return (cbp, (byte)csa);
    }

    private void WriteClutPatches()
    {
        if (_texSet.ClutPatchSet.Count < 1)
            return;

        for (ushort i = 0; i < _texSet.pgluTextures.Count; i++)
        {
            var clutPatch = _texSet.ClutPatchSet[0].TexturesToPatch[i];
            var pgluTexture= _texSet.pgluTextures[i];

            clutPatch.CBP_ClutBufferBasePointer = pgluTexture.tex0.CBP_ClutBlockPointer;
            clutPatch.CSA_ClutEntryOffset = pgluTexture.tex0.CSA_ClutEntryOffset;
            clutPatch.PGLUTextureIndex = i;
            clutPatch.Format = SCE_GS_PSM.SCE_GS_PSMCT32;
        }

        for (int varIndex = 1; varIndex < _clutPatchSets.Count; varIndex++)
        {
            var clutPatchSet = new ClutPatchSet();
            _texSet.ClutPatchSet.Add(clutPatchSet);

            for (ushort textureIndex = 0; textureIndex < _clutPatchSets[varIndex].Count; textureIndex++)
            {
                ClutPatchTask clutPatchTask = _clutPatchSets[varIndex][textureIndex];

                SCE_GS_PSM textureFormat = _textures[textureIndex].PGLUTexture.tex0.PSM;
                int width = textureFormat == SCE_GS_PSM.SCE_GS_PSMT8 ? 16 : 8;
                int height = textureFormat == SCE_GS_PSM.SCE_GS_PSMT8 ? 16 : 2;

                var clutPatch = new TextureClutPatch();
                (ushort CBP, byte CSA) = FitPaletteToGSMemory(textureFormat, width, height, clutPatchTask.Palette, reuseOldPaletteLocations: true);
                clutPatch.CBP_ClutBufferBasePointer = CBP;
                clutPatch.CSA_ClutEntryOffset = CSA;
                clutPatch.PGLUTextureIndex = textureIndex;
                clutPatch.Format = SCE_GS_PSM.SCE_GS_PSMCT32;

                clutPatchSet.TexturesToPatch.Add(clutPatch);
            }
        }
    }

    /// <summary>
    /// Fits all the textures to the emulated GS memory in a block-optimized way and updates their block pointers.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="OutOfMemoryException"></exception>
    private void WriteTexturesOptimized()
    {
        /* Order by textures that allocates the most GS blocks without actually using them
         * That way we can put texture data in there

         * There's still some good improvements that can be made here for certain
           as i am only trying to fit the rest of the textures where they can fit
           instead of arranging them to begin with
        */

        var texturesByUnusedGsBlocks = _textures.OrderByDescending(e => e.SizeInGSBlocks);
        foreach (TextureTask texture in texturesByUnusedGsBlocks)
        {
            List<ushort> usedBlocksOfTexture = texture.TexturePixelFormat.GetUsedBlocks(texture.Image.Width, texture.Image.Height);

            // #1: Start searching if we can fit the texture in all unused GS blocks.
            int unusedBlockFitIndex = CanFitBlocksInUnusedBlocks(usedBlocksOfTexture);
            if (unusedBlockFitIndex != -1)
            {
                // We were able to fit the texture in unused blocks
                for (int i = 0; i < usedBlocksOfTexture.Count; i++)
                {
                    if (_unusedGsBlocksIndices.ContainsKey((ushort)(unusedBlockFitIndex + usedBlocksOfTexture[i])))
                        _unusedGsBlocksIndices.Remove((ushort)(unusedBlockFitIndex + usedBlocksOfTexture[i]));

                    _usedGsBlocksIndices.Add((ushort)(unusedBlockFitIndex + usedBlocksOfTexture[i]));
                }

                texture.PGLUTexture.tex0.TBP0_TextureBaseAddress = (ushort)unusedBlockFitIndex;
                WriteTextureToGSMemory(texture.PGLUTexture.tex0.TBP0_TextureBaseAddress, texture.PGLUTexture.tex0.TBW_TextureBufferWidth,
                    texture.Image.Width, texture.Image.Height,
                    texture.PGLUTexture.tex0.PSM,
                    texture.PackedImageData);
                continue;
            }

            // #2: Check if we can fit the texture after the last vertical row (provided the page layout is ok?)
            int afterLastRowFitBlockIdx = -1;
            if (_tbp_Textures != 0 && _lastFreeVerticalBlock != -1)
            {
                for (ushort blockIdx = (ushort)_lastFreeVerticalBlock; blockIdx < _tbp_Textures; blockIdx++)
                {
                    int j = 0;
                    for (j = 0; j < usedBlocksOfTexture.Count; j++)
                    {
                        if (_usedGsBlocksIndices.Contains((ushort)(blockIdx + usedBlocksOfTexture[j])))
                        {
                            // Starting block index is not suitable to fit the texture, move to next one
                            break;
                        }
                    }

                    if (j == usedBlocksOfTexture.Count)
                    {
                        afterLastRowFitBlockIdx = blockIdx;
                        break;
                    }
                }

                if (afterLastRowFitBlockIdx != -1)
                {
                    for (int i = 0; i < usedBlocksOfTexture.Count; i++)
                    {
                        if (_unusedGsBlocksIndices.ContainsKey((ushort)(afterLastRowFitBlockIdx + usedBlocksOfTexture[i])))
                            _unusedGsBlocksIndices.Remove((ushort)(afterLastRowFitBlockIdx + usedBlocksOfTexture[i]));

                        _usedGsBlocksIndices.Add((ushort)(afterLastRowFitBlockIdx + usedBlocksOfTexture[i]));
                    }

                    texture.PGLUTexture.tex0.TBP0_TextureBaseAddress = (ushort)afterLastRowFitBlockIdx;
                    _lastFreeVerticalBlock = (ushort)(afterLastRowFitBlockIdx + texture.FirstFreeVerticalBlock);

                    _tbp_Textures = _usedGsBlocksIndices.Max(e => e);
                    WriteTextureToGSMemory(texture.PGLUTexture.tex0.TBP0_TextureBaseAddress, texture.PGLUTexture.tex0.TBW_TextureBufferWidth,
                        texture.Image.Width, texture.Image.Height,
                        texture.PGLUTexture.tex0.PSM,
                        texture.PackedImageData);
                    continue;
                }
            }


            // Unable to fit anywhere (it seems). We are allocating new blocks starting from tbp
            texture.PGLUTexture.tex0.TBP0_TextureBaseAddress = _tbp_Textures;
            _lastFreeVerticalBlock = (ushort)(_tbp_Textures + texture.FirstFreeVerticalBlock);

            for (int i = 0; i < texture.UnusedGSBlocks.Count; i++)
            {
                ushort idx = (ushort)(_tbp_Textures + texture.UnusedGSBlocks[i]);
                _unusedGsBlocksIndices.Add(idx, new GSBlock(idx, 0));
            }

            for (int j = 0; j < usedBlocksOfTexture.Count; j++)
                _usedGsBlocksIndices.Add((ushort)(_tbp_Textures + usedBlocksOfTexture[j]));

            if (_tbp_Textures + texture.SizeInGSBlocks >= GSMemory.MAX_BLOCKS)
                throw new OutOfMemoryException($"Textures take more space than the maximum GS memory capacity ({_tbp_Textures + texture.SizeInGSBlocks} >= {GSMemory.MAX_BLOCKS}).");

            WriteTextureToGSMemory(texture.PGLUTexture.tex0.TBP0_TextureBaseAddress, texture.PGLUTexture.tex0.TBW_TextureBufferWidth,
                    texture.Image.Width, texture.Image.Height,
                    texture.PGLUTexture.tex0.PSM,
                    texture.PackedImageData);

            uint textureTbp = texture.SizeInGSBlocks;
            if (textureTbp == 1)
                textureTbp = 4;
            _tbp_Textures += (ushort)textureTbp;
        }
    }

    private void BuildTransfers()
    {
        foreach (var texture in _textures)
        {
            _logger?.LogDebug("Adding transfer {x}x{y}, tbp={tbp}", texture.Image.Width, texture.Image.Height, texture.PGLUTexture.tex0.TBP0_TextureBaseAddress);

            AddTransfer(texture.TexturePixelFormat,
                texture.PGLUTexture.tex0.TBP0_TextureBaseAddress, texture.PGLUTexture.tex0.TBW_TextureBufferWidth,
                (ushort)texture.Image.Width, (ushort)texture.Image.Height, texture.PackedImageData);

            if (texture.Palette != null)
            {
                ushort cbp = texture.PGLUTexture.tex0.CBP_ClutBlockPointer;
                if (texture.PGLUTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8)
                {
                    AddTransfer(GSPixelFormat.PSM_CT32, cbp, 4, // bw = 4, important
                        16, 16, MemoryMarshal.Cast<Rgba32, byte>(texture.Palette).ToArray());
                }
                else
                {
                    AddTransfer(GSPixelFormat.PSM_CT32, cbp, 1,
                        8, 2, MemoryMarshal.Cast<Rgba32, byte>(texture.Palette).ToArray());
                }
            }
        }
    }

    private void BuildSwizzledTransfers()
    {
        int lastUsedBlock = _usedGsBlocksIndices.Max(e => e);

        // Make sure we calculate (and align) the size from the blocks instead since we're swizzling
        var transferSizes = Tex1Utils.CalculateSwizzledTransferSizes(lastUsedBlock * GSMemory.BLOCK_SIZE_BYTES);

        int tbp = 0;
        foreach (var (Width, Height) in transferSizes)
        {
            _logger?.LogDebug("Adding swizzled transfer {x}x{y}, tbp={tbp}", Width, Height, tbp);

            byte[] transferData = new byte[Width * Height * 4];
            _gsMemory.ReadTexPSMCT32(tbp, 1,
                0, 0,
                Width, Height,
                MemoryMarshal.Cast<byte, uint>(transferData));
            AddTransfer(GSPixelFormat.PSM_CT32, (ushort)tbp, 1, (ushort)Width, (ushort)Height, transferData);

            tbp += transferData.Length / GSMemory.BLOCK_SIZE_BYTES;
        }
    }

    private static bool ImageFitsColorPalette(Image<Rgba32> img, int paletteSize, out List<Rgba32> colorPalette)
    {
        colorPalette = null;

        HashSet<Rgba32> colors = new HashSet<Rgba32>(paletteSize);
        for (int y = 0; y < img.Height; y++)
        {
            for (int x = 0; x < img.Width; x++)
            {
                if (!colors.Contains(img[x, y]))
                    colors.Add(img[x, y]);

                if (colors.Count > paletteSize)
                    return false;
            }
        }
        colorPalette = colors.ToList();
        return true;
    }

    /// <summary>
    /// Returns whether the specified block list can fit in unused blocks.
    /// </summary>
    /// <param name="usedBlocksOfTexture">Blocks to fit</param>
    /// <param name="csa">CSA to fit in a block</param>
    /// <returns>Block index start. -1 if it could not be fitted.</returns>
    private int CanFitBlocksInUnusedBlocks(List<ushort> usedBlocksOfTexture, int csa = 8)
    {
        int unusedBlockFitIndex = -1;

        foreach (GSBlock block in _unusedGsBlocksIndices.Values)
        {
            // Special case when a texture/palette fits into a single block where we can
            // tweak the csa register to point to it
            if (usedBlocksOfTexture.Count == 1 && block.CurrentCSA + csa <= 8)
            {
                // We can reuse a partially filled block using CSA
                return block.Index;
            }

            int j = 0;
            for (j = 0; j < usedBlocksOfTexture.Count; j++)
            {
                int blockIdx = block.Index + usedBlocksOfTexture[j];

                if (!_unusedGsBlocksIndices.ContainsKey((ushort)blockIdx))
                    break;
            }

            if (j == usedBlocksOfTexture.Count)
            {
                unusedBlockFitIndex = block.Index;
                break;
            }
        }

        return unusedBlockFitIndex;
    }

    /// <summary>
    /// Creates image data for the specified texture.
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="outputFormat"></param>
    private static void CreateImageData(TextureTask texture, SCE_GS_PSM outputFormat)
    {
        int bpp = Tex1Utils.GetBitsPerPixel(outputFormat);

        byte[] imageData = new byte[Tex1Utils.GetDataSize(texture.Image.Width, texture.Image.Height, outputFormat)];
        BitStream bs = new BitStream(BitStreamMode.Write, imageData, BitStreamSignificantBitOrder.MSB);

        if (outputFormat == SCE_GS_PSM.SCE_GS_PSMT8 || outputFormat == SCE_GS_PSM.SCE_GS_PSMT4)
        {
            var img = texture.IndexedImage.AsMemory2D();
            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    ref byte b = ref img.Span[y, x];
                    bs.WriteBits(b, (ulong)bpp);
                }
            }
        }
        else
        {
            Image<Rgba32> image = texture.Image;
            for (var y = 0; y < image.Height; y++)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    bs.WriteByte(image[x, y].R);
                    bs.WriteByte(image[x, y].G);
                    bs.WriteByte(image[x, y].B);
                    bs.WriteByte((byte)Tex1Utils.Normalize(image[x, y].A, 0x00, 0xFF, 0x00, 0x80));
                }
            }
        }

        texture.PackedImageData = imageData;
    }


    /// <summary>
    /// Writes the texture to the gs memory (for retrieval later).
    /// </summary>
    /// <param name="texture"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void WriteTextureToGSMemory(int tbp, int tbw, int width, int height, SCE_GS_PSM psm, byte[] data)
    {
        _logger?.LogInformation("Writing Texture {x}x{y}, psm={psm} tbp={tbp}",
            width, height,
            psm,
            tbp);

        switch (psm)
        {
            case SCE_GS_PSM.SCE_GS_PSMCT32:
                _gsMemory.WriteTexPSMCT32(tbp, tbw,
                    0, 0,
                    width, height,
                    MemoryMarshal.Cast<byte, uint>(data));
                break;

            case SCE_GS_PSM.SCE_GS_PSMT8:
                _gsMemory.WriteTexPSMT8(tbp, tbw,
                    0, 0,
                    width, height,
                    data);
                break;

            case SCE_GS_PSM.SCE_GS_PSMT4:
                _gsMemory.WriteTexPSMT4(tbp, tbw,
                    0, 0,
                    width, height,
                    data);
                break;

            default:
                throw new NotImplementedException("Format not implemented");
        }
    }

    /// <summary>
    /// Registers a new GS transfer to the texture set.
    /// </summary>
    /// <param name="pixelFormat"></param>
    /// <param name="tbp"></param>
    /// <param name="bw"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="data"></param>
    private void AddTransfer(GSPixelFormat pixelFormat, ushort tbp, byte bw, ushort width, ushort height, byte[] data)
    {
        var inf = new GSTransfer()
        {
            BP = tbp,
            BW = bw,
            Format = pixelFormat.PSM,
            Width = width,
            Height = height,
            Data = data
        };

        _texSet.GSTransfers.Add(inf);
    }

    /// <summary>
    /// Makes a palette tiled (used for <see cref="SCE_GS_PSM.SCE_GS_PSMT8"/>.
    /// </summary>
    /// <param name="palette"></param>
    /// <returns></returns>
    public static (Rgba32[] TiledPalette, int[] LinearToTiledPaletteIndices) MakeTiledPaletteFromLinearPalette(ReadOnlyMemory<Rgba32> palette)
    {
        int[] indices = new int[palette.Length];

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
                    {
                        int idx = (ty * tileSizeH + y) * 16 + (tx * tileSizeW + x);
                        outpal[idx] = palette.Span[i];
                        indices[i] = idx;
                        i++;
                    }

        return (outpal, indices);
    }
}

public class TextureTask
{
    /// <summary>
    /// PGLUTexture structure.
    /// </summary>
    public PGLUtexture PGLUTexture { get; set; }

    /// <summary>
    /// Raw Image data.
    /// </summary>
    public Image<Rgba32> Image { get; set; }

    /// <summary>
    /// Indexed image (when the image has a palette). <br/>
    /// <br/>
    /// IndexedImage[Y,X]
    /// </summary>
    public byte[,] IndexedImage { get; set; }

    /// <summary>
    /// GS Format for this texture.
    /// </summary>
    public GSPixelFormat TexturePixelFormat { get; set; }

    public byte[] PackedImageData { get; set; }

    /// <summary>
    /// Palette.
    /// </summary>
    public Rgba32[] Palette { get; set; }

    /// <summary>
    /// Size of this texture in GS Blocks.
    /// </summary>
    public ushort SizeInGSBlocks { get; set; }

    /// <summary>
    /// Unused GS blocks allocated by this texture.
    /// </summary>
    public List<ushort> UnusedGSBlocks { get; set; } = [];

    /// <summary>
    /// Index of the first block below the final row of the image.
    /// </summary>
    public int FirstFreeVerticalBlock { get; set; }
}

public class ClutPatchTask
{
    public Rgba32[] Palette { get; set; }

    public ClutPatchTask(Rgba32[] palette)
    {
        Palette = palette;
    }
}

public class GSBlock
{
    public int Index { get; set; }
    public byte CurrentCSA { get; set; }

    public GSBlock(int index, byte currentCSA)
    {
        Index = index;
        CurrentCSA = currentCSA;
    }
}
