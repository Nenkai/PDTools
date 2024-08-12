
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Files.Textures.PS2.GSFormats;
using PDTools.Utils;

namespace PDTools.Files.Textures.PS2.GSPixelFormats;

public abstract class GSPixelFormat
{
    /// <summary>
    /// Number of blocks in a single gs page.
    /// </summary>
    public const int BLOCKS_PER_PAGE = 32;

    /// <summary>
    /// Size of one gs block in bytes.
    /// </summary>
    public const int BLOCK_SIZE_BYTES = 256;

    public abstract SCE_GS_PSM PSM { get; }
    public abstract int PageColumns { get; }
    public abstract int PageRows { get; }
    public abstract int ColumnWidth { get; }
    public abstract int ColumnHeight { get; }
    public abstract int BlockWidth { get; }
    public abstract int BlockHeight { get; }
    public abstract int PageWidth { get; }
    public abstract int PageHeight { get; }

    public abstract int[] BlockLayout { get; }

    private static PSM_CT32 _psmct32 = new PSM_CT32();
    public static readonly PSM_CT32 PSM_CT32 = _psmct32;

    private static PSMT4 _psmct4 = new PSMT4();
    public static readonly PSMT4 PSMT4 = _psmct4;

    private static PSMT8 _psmct8 = new PSMT8();
    public static readonly PSMT8 PSMT8 = _psmct8;

    public static GSPixelFormat GetFormatFromPSMFormat(SCE_GS_PSM psm)
    {
        return psm switch
        {
            SCE_GS_PSM.SCE_GS_PSMT4 => _psmct4,
            SCE_GS_PSM.SCE_GS_PSMT8 => _psmct8,
            SCE_GS_PSM.SCE_GS_PSMCT32 => _psmct32,
            _ => throw new NotSupportedException($"Format {psm} not yet supported"),
        };
    }

    /// <summary>
    /// GS's Users Manual - Page 161 to 170 are useful
    /// </summary>
    /// <param name="pixelFormat"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ushort GetLastBlockIndexForImageDimensions(int x, int y)
    {
        // https://patchwork.kernel.org/project/linux-mips/patch/25b6c975d334c0678ab3963d6c76584ed9471c35.1567326213.git.noring@nocrew.org/
        // https://patchwork.kernel.org/project/linux-mips/patch/afe499daf7605ced1373efafbc9c28a035d646df.1567326213.git.noring@nocrew.org/
        // arch/mips/include/asm/mach-ps2/gs.h
        // arch/mips/include/uapi/asm/gs.h

        if (x == 0 && y == 0)
            return 0;

        // Get last pixels
        x = Math.Max(0, x - 1);
        y = Math.Max(0, y - 1);

        // tbw is calculated on the fly, at least it seems, based on x
        int pagesPerRow = GetNumPagesPerRow(x, y);

        int pageX = x / PageWidth;
        int pageY = y / PageHeight;
        int page = pageX + pageY * (int)pagesPerRow;

        int px = x - (pageX * PageWidth);
        int py = y - (pageY * PageHeight);

        int blockX = px / BlockWidth;
        int blockY = py / BlockHeight;
        int pageBlockIndex = BlockLayout[blockX + blockY * PageColumns];

        int block = (page * GSMemory.BLOCKS_PER_PAGE) + pageBlockIndex;
        Debug.Assert(pageBlockIndex < GSMemory.MAX_BLOCKS, "Block Index superior than max blocks in GS memory");

        return (ushort)block;
    }

    public ushort GetBlockIndex(int x, int y, int pagesPerRow)
    {
        int pageX = x / PageWidth;
        int pageY = y / PageHeight;
        int page = pageX + pageY * pagesPerRow;

        int px = x - (pageX * PageWidth);
        int py = y - (pageY * PageHeight);

        int blockX = px / BlockWidth;
        int blockY = py / BlockHeight;
        int pageBlockIndex = BlockLayout[blockX + blockY * PageColumns];

        int block = (page * GSMemory.BLOCKS_PER_PAGE) + pageBlockIndex;
        Debug.Assert(pageBlockIndex < GSMemory.MAX_BLOCKS, "Block Index superior than max blocks in GS memory");

        return (ushort)block;
    }

    public int GetNumPagesPerRow(int width, int height)
    {
        return (int)Math.Max(1, MiscUtils.AlignValue((uint)width, (uint)PageWidth) / PageWidth);
    }

    public List<ushort> GetUnusedBlocks(int width, int height, out int firstFreeVerticalBlock)
    {
        firstFreeVerticalBlock = -1;

        var blockIndices = new List<ushort>();

        int lastBlockIndex = GetLastBlockIndexForImageDimensions(width, height);
        int pagesPerRow = GetNumPagesPerRow(width, height);

        for (ushort blockIndex = 0; blockIndex < lastBlockIndex + 1; blockIndex++)
        {
            var (blockX, blockY) = GetPositionOfBlock(blockIndex, pagesPerRow);
            if (blockX >= width || blockY >= height)
            {
                blockIndices.Add(blockIndex);

                if (firstFreeVerticalBlock == -1 && blockY >= height)
                    firstFreeVerticalBlock = blockIndex;
            }
        }

        return blockIndices;
    }

    public List<ushort> GetUsedBlocks(int width, int height)
    {
        var blockIndices = new List<ushort>();

        int lastBlockIndex = GetLastBlockIndexForImageDimensions(width, height);
        int pagesPerRow = GetNumPagesPerRow(width, height);

        for (ushort i = 0; i < lastBlockIndex + 1; i++)
        {
            var (blockX, blockY) = GetPositionOfBlock(i, pagesPerRow);
            if (blockX < width && blockY < height)
                blockIndices.Add(i);
        }

        return blockIndices;
    }

    public (int BlockX, int BlockY) GetPositionOfBlock(int blockIndex, int pagesPerRow)
    {
        if (blockIndex >= GSMemory.MAX_BLOCKS)
            throw new ArgumentOutOfRangeException(nameof(blockIndex), $"Block index is above GS memory capacity ({blockIndex} >= {GSMemory.MAX_BLOCKS}).");

        int pageIndex = blockIndex / GSMemory.BLOCKS_PER_PAGE;

        int pageX = (pageIndex % pagesPerRow) * PageWidth;
        int pageY = (pageIndex / pagesPerRow) * PageHeight;

        int pageBlockIndex = blockIndex % GSMemory.BLOCKS_PER_PAGE;

        int idx = BlockLayout.AsSpan().IndexOf(pageBlockIndex);
        int pageRow = idx / PageColumns;
        int pageCol = idx % PageColumns;

        int blockX = pageCol * BlockWidth;
        int blockY = pageRow * BlockHeight;

        return (pageX + blockX, pageY + blockY);
    }

    public int CalcImageTbwWidth(int x, int y)
    {
        return (int)MiscUtils.AlignValue((uint)x, (uint)PageWidth);
    }
}
