using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Utils;

namespace PDTools.Files.Textures.PS2
{
    public static class Tex1Utils
    {
        const int GS_BLOCK_COLS = 1;
        const int GS_BLOCK_ROWS = 4;

        // PSMT8
        const int GS_PSMT8_PAGE_COLS = 8;
        const int GS_PSMT8_PAGE_ROWS = 4;
        const int GS_PSMT8_COLUMN_WIDTH = 16;
        const int GS_PSMT8_COLUMN_HEIGHT = 4;

        const int GS_PSMT8_BLOCK_WIDTH = (GS_PSMT8_COLUMN_WIDTH * GS_BLOCK_COLS);
        const int GS_PSMT8_BLOCK_HEIGHT = GS_PSMT8_COLUMN_HEIGHT * GS_BLOCK_ROWS;
        const int GS_PSMT8_PAGE_WIDTH = GS_PSMT8_BLOCK_WIDTH * GS_PSMT8_PAGE_COLS;
        const int GS_PSMT8_PAGE_HEIGHT = GS_PSMT8_BLOCK_HEIGHT * GS_PSMT8_PAGE_ROWS;

        // PSMT4
        const int GS_PSMT4_PAGE_COLS = 4;
        const int GS_PSMT4_PAGE_ROWS = 8;
        const int GS_PSMT4_COLUMN_WIDTH = 32;
        const int GS_PSMT4_COLUMN_HEIGHT = 4;

        const int GS_PSMT4_BLOCK_WIDTH = (GS_PSMT4_COLUMN_WIDTH * GS_BLOCK_COLS);
        const int GS_PSMT4_BLOCK_HEIGHT = GS_PSMT4_COLUMN_HEIGHT * GS_BLOCK_ROWS;
        const int GS_PSMT4_PAGE_WIDTH = GS_PSMT4_BLOCK_WIDTH * GS_PSMT4_PAGE_COLS;
        const int GS_PSMT4_PAGE_HEIGHT = GS_PSMT4_BLOCK_HEIGHT * GS_PSMT4_PAGE_ROWS;

        // PSMCT32
        const int GS_PSM_CT32_PAGE_COLS = 8;
        const int GS_PSM_CT32_PAGE_ROWS = 4;
        const int GS_PSM_CT32_COLUMN_WIDTH = 8;
        const int GS_PSM_CT32_COLUMN_HEIGHT = 2;

        const int GS_PSM_CT32_BLOCK_WIDTH = (GS_PSM_CT32_COLUMN_WIDTH * GS_BLOCK_COLS);
        const int GS_PSM_CT32_BLOCK_HEIGHT = GS_PSM_CT32_COLUMN_HEIGHT * GS_BLOCK_ROWS;
        const int GS_PSM_CT32_PAGE_WIDTH = GS_PSM_CT32_BLOCK_WIDTH * GS_PSM_CT32_PAGE_COLS;
        const int GS_PSM_CT32_PAGE_HEIGHT = GS_PSM_CT32_BLOCK_HEIGHT * GS_PSM_CT32_PAGE_ROWS;

        /// <summary>
        /// GS's Users Manual - Page 161 to 170 are useful
        /// </summary>
        /// <param name="pixelFormat"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static int FindBlockIndexAtPosition(SCE_GS_PSM pixelFormat, int w, int h)
        {
            // https://patchwork.kernel.org/project/linux-mips/patch/25b6c975d334c0678ab3963d6c76584ed9471c35.1567326213.git.noring@nocrew.org/
            // https://patchwork.kernel.org/project/linux-mips/patch/afe499daf7605ced1373efafbc9c28a035d646df.1567326213.git.noring@nocrew.org/
            // arch/mips/include/asm/mach-ps2/gs.h
            // arch/mips/include/uapi/asm/gs.h

            int blockCount = 0;

            if (pixelFormat == SCE_GS_PSM.SCE_GS_PSMT8) // Page 166
            {
                int blocksPerPage = GS_PSMT8_PAGE_COLS * GS_PSMT8_PAGE_ROWS;
                var pagesPerRow = MiscUtils.AlignValue((uint)w, GS_PSMT8_PAGE_WIDTH) / GS_PSMT8_PAGE_WIDTH;

                if (h > GS_PSMT8_PAGE_HEIGHT)
                {
                    if ((h % GS_PSMT8_PAGE_HEIGHT) == 0)
                        h--;

                    int blocksPerPageRow = (int)(pagesPerRow * blocksPerPage);
                    int pageRowsToSkip = (h / GS_PSMT8_PAGE_HEIGHT);
                    blockCount += pageRowsToSkip * blocksPerPageRow;
                    h %= GS_PSMT8_PAGE_HEIGHT;
                }

                if (w >= GS_PSMT8_PAGE_WIDTH)
                {
                    if (w == (pagesPerRow * GS_PSMT8_PAGE_WIDTH))
                        w--;

                    int pagesToSkip = (w / GS_PSMT8_PAGE_WIDTH);
                    blockCount += (pagesToSkip * blocksPerPage);
                    w %= GS_PSMT8_PAGE_WIDTH;
                }

                if (h > 64)
                {
                    blockCount += blocksPerPage;
                    h -= 64;
                }

                if (w > 64)
                {
                    blockCount += (4 * 4);
                    w -= 64;
                }

                if (h > 32)
                {
                    blockCount += (2 * 4);
                    h -= 32;
                }

                if (w > 32)
                {
                    blockCount += (2 * 2);
                    w -= 32;
                }

                if (h > 16)
                {
                    blockCount += 2;
                    h -= 16;
                }

                if (w > 16)
                {
                    blockCount++;
                    w -= 16;
                }

                if (h > 0 || w > 0)
                {
                    blockCount++;
                    h = 0;
                    w = 0;
                }
            }
            else if (pixelFormat == SCE_GS_PSM.SCE_GS_PSMT4) // Page 168
            {
                int blocksPerPage = GS_PSMT4_PAGE_COLS * GS_PSMT4_PAGE_ROWS;
                var pagesPerRow = MiscUtils.AlignValue((uint)w, GS_PSMT4_PAGE_WIDTH) / GS_PSMT4_PAGE_WIDTH;

                if (h > GS_PSMT4_PAGE_HEIGHT)
                {
                    if ((h % GS_PSMT4_PAGE_HEIGHT) == 0)
                        h--;

                    int totalBlocksPerPageRow = (int)(blocksPerPage * pagesPerRow);
                    int pageRowsToSkip = (h / GS_PSMT4_PAGE_HEIGHT);
                    blockCount += pageRowsToSkip * totalBlocksPerPageRow;
                    h %= GS_PSMT4_PAGE_HEIGHT;
                }

                if (w >= GS_PSMT4_PAGE_WIDTH)
                {
                    if (w == (pagesPerRow * GS_PSMT4_PAGE_WIDTH))
                        w--;

                    int pagesToSkip = (w / GS_PSMT4_PAGE_WIDTH);
                    blockCount += (pagesToSkip * blocksPerPage);
                    w %= GS_PSMT4_PAGE_WIDTH;
                }

                if (h > 64)
                {
                    blockCount += (4 * 4);
                    h %= 64;
                }

                if (w > 64)
                {
                    blockCount += (2 * 4);
                    w %= 64;
                }

                if (h > 32)
                {
                    blockCount += (2 * 2);
                    h %= 32;
                }

                if (w > 32)
                {
                    blockCount += 2;
                    w -= 32;
                }

                if (h > 16)
                {
                    blockCount++;
                    h -= 16;
                }

                if (h > 0 || w > 0)
                {
                    blockCount++;
                    h = 0;
                    w = 0;
                }
            }
            else if (pixelFormat == SCE_GS_PSM.SCE_GS_PSMCT32 || pixelFormat == SCE_GS_PSM.SCE_GS_PSMCT24 ||
                pixelFormat == SCE_GS_PSM.SCE_GS_PSMZ32 || pixelFormat == SCE_GS_PSM.SCE_GS_PSMZ24) // Page 162 & 168
            {
                int blocksPerPage = GS_PSM_CT32_PAGE_COLS * GS_PSM_CT32_PAGE_ROWS;
                var pagesPerRow = MiscUtils.AlignValue((uint)w, GS_PSM_CT32_PAGE_WIDTH) / GS_PSM_CT32_PAGE_WIDTH;

                if (h > GS_PSM_CT32_PAGE_HEIGHT)
                {
                    if ((h % GS_PSM_CT32_PAGE_HEIGHT) == 0)
                        h--;

                    int blocksPerPageRow = (int)(pagesPerRow * blocksPerPage);
                    int pageRowsToSkip = (h / GS_PSM_CT32_PAGE_HEIGHT);
                    blockCount += pageRowsToSkip * blocksPerPageRow;
                    h %= GS_PSM_CT32_PAGE_HEIGHT;
                }

                if (w >= GS_PSM_CT32_PAGE_WIDTH)
                {
                    if (w == (pagesPerRow * GS_PSM_CT32_PAGE_WIDTH))
                        w--;

                    int pagesToSkip = (w / GS_PSM_CT32_PAGE_WIDTH);
                    blockCount += (pagesToSkip * blocksPerPage);
                    w %= GS_PSM_CT32_PAGE_WIDTH;
                }

                if (w > 32)
                {
                    blockCount += (4 * 4);
                    w -= 32;
                }

                if (h > 16)
                {
                    blockCount += (2 * 4);
                    h -= 16;
                }

                if (w > 16)
                {
                    blockCount += (2 * 2);
                    w -= 16;
                }

                if (h > 8)
                {
                    blockCount += 2;
                    h -= 8;
                }

                if (w > 8)
                {
                    blockCount++;
                    w -= 8;
                }

                if (h > 0 || w > 0)
                {
                    blockCount++;
                    h = 0;
                    w = 0;
                }
            }
            else
            {
                throw new NotImplementedException($"FindBlockIndex unimplemented format: {pixelFormat}");
            }

            return blockCount;
        }

        public static int DeterminePositionFromBlockIndex(SCE_GS_PSM pixelFormat, int blockIndex, int tw, int th)
        {
            int width = (int)Math.Pow(2, tw);
            int height = (int)Math.Pow(2, th);

            if (pixelFormat == SCE_GS_PSM.SCE_GS_PSMT4)
            {
                int blocksPerPage = GS_PSMT4_PAGE_COLS * GS_PSMT4_PAGE_ROWS;
                var pagesPerRow = MiscUtils.AlignValue((uint)width, GS_PSMT4_PAGE_WIDTH) / GS_PSMT4_PAGE_WIDTH;
                int blocksPerPageRow = (int)(blocksPerPage * pagesPerRow);

                int outWidth = 0, outHeight = 0;

                if (blockIndex > blocksPerPageRow)
                {
                    int count = blockIndex / blocksPerPageRow;

                    outHeight += (count * GS_PSMT4_PAGE_HEIGHT);
                    blockIndex -= (count * blocksPerPageRow);
                }

                if (blockIndex >= 32)
                {
                    int count = blockIndex / 32;

                    outWidth += (count * GS_PSMT4_PAGE_WIDTH);
                    blockIndex -= (count * 32);
                }

                if (blockIndex >= 16)
                {
                    outHeight += (GS_PSMT4_PAGE_HEIGHT / 2);
                    blockIndex -= 16;
                }

                if (blockIndex >= 8)
                {
                    outWidth += (GS_PSMT4_PAGE_WIDTH / 2);
                    blockIndex -= 8;
                }
            }
            else if (pixelFormat == SCE_GS_PSM.SCE_GS_PSMCT32)
            {
                int blocksPerPage = GS_PSM_CT32_PAGE_COLS * GS_PSM_CT32_PAGE_ROWS;
                var pagesPerRow = MiscUtils.AlignValue((uint)width, GS_PSM_CT32_PAGE_WIDTH) / GS_PSM_CT32_PAGE_WIDTH;
                int blocksPerPageRow = (int)(blocksPerPage * pagesPerRow);

                int outWidth = 0, outHeight = 0;

                if (blockIndex > blocksPerPageRow)
                {
                    int count = blockIndex / blocksPerPageRow;

                    outHeight += (count * GS_PSM_CT32_PAGE_HEIGHT);
                    blockIndex -= (count * blocksPerPageRow);
                }

                if (blockIndex >= 32)
                {
                    int count = blockIndex / 32;

                    outWidth += (count * GS_PSM_CT32_PAGE_WIDTH);
                    blockIndex -= (count * 32);
                }

                if (blockIndex >= 16)
                {
                    outWidth += (GS_PSM_CT32_PAGE_WIDTH / 2);
                    blockIndex -= 16;
                }

                if (blockIndex >= 8)
                {
                    outWidth += (GS_PSM_CT32_PAGE_WIDTH / 2);
                    blockIndex -= 8;
                }
            }

            return 0;
        }
    }
}
