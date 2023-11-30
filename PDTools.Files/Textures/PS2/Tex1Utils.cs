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
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static int FindBlockIndexAtPosition(SCE_GS_PSM pixelFormat, int x, int y)
        {
            // https://patchwork.kernel.org/project/linux-mips/patch/25b6c975d334c0678ab3963d6c76584ed9471c35.1567326213.git.noring@nocrew.org/
            // https://patchwork.kernel.org/project/linux-mips/patch/afe499daf7605ced1373efafbc9c28a035d646df.1567326213.git.noring@nocrew.org/
            // arch/mips/include/asm/mach-ps2/gs.h
            // arch/mips/include/uapi/asm/gs.h

            if (x == 0 && y == 0)
                return 0;

            if (pixelFormat == SCE_GS_PSM.SCE_GS_PSMT8) // Page 166
            {
                int blocksPerPage = GS_PSMT8_PAGE_COLS * GS_PSMT8_PAGE_ROWS;
                var pagesPerRow = MiscUtils.AlignValue((uint)x, GS_PSMT8_PAGE_WIDTH) / GS_PSMT8_PAGE_WIDTH;

                int pageX = x / 128;
                int pageY = y / 64;
                int page = pageX + pageY * (int)pagesPerRow;

                int px = x - (pageX * 128);
                int py = y - (pageY * 64);

                int blockX = px / 16;
                int blockY = py / 16;
                int block = GSMemory.block8[blockX + blockY * 8];

                return (page * blocksPerPage) + block;
            }
            else if (pixelFormat == SCE_GS_PSM.SCE_GS_PSMT4) // Page 168
            {
                int blocksPerPage = GS_PSMT4_PAGE_COLS * GS_PSMT4_PAGE_ROWS;
                var pagesPerRow = MiscUtils.AlignValue((uint)x, GS_PSMT4_PAGE_WIDTH) / GS_PSMT4_PAGE_WIDTH;

                int pageX = x / 128;
                int pageY = y / 128;
                int page = pageX + (pageY * (int)pagesPerRow);

                int px = x - (pageX * 128);
                int py = y - (pageY * 128);

                int blockX = px / 32;
                int blockY = py / 16;
                int block = GSMemory.block4[blockX + blockY * 4];

                return (page * blocksPerPage) + block;
            }
            else if (pixelFormat == SCE_GS_PSM.SCE_GS_PSMCT32 || pixelFormat == SCE_GS_PSM.SCE_GS_PSMCT24) // Page 162 & 168
            {
                var pagesPerRow = MiscUtils.AlignValue((uint)x, GS_PSM_CT32_PAGE_WIDTH) / GS_PSM_CT32_PAGE_WIDTH;

                int pageX = x / 64;
                int pageY = y / 32;
                int page = pageX + (pageY * (int)pagesPerRow);

                int px = x - (pageX * 64);
                int py = y - (pageY * 32);

                int blockX = px / 8;
                int blockY = py / 8;
                int block = GSMemory.block32[blockX + blockY * 8];

                return page * 32 + block;
            }
            else
            {
                throw new NotImplementedException($"FindBlockIndex unimplemented format: {pixelFormat}");
            }
        }

        public static int GetDataSize(int width, int height, SCE_GS_PSM format)
        {
            int bpp = Tex1Utils.GetBitsPerPixel(format);
            int bytes = (int)Math.Round((double)(width * height * ((double)bpp / 8)), MidpointRounding.AwayFromZero);
            return bytes;
        }

        public static int GetBitsPerPixel(SCE_GS_PSM psm)
        {
            return psm switch
            {
                SCE_GS_PSM.SCE_GS_PSMCT32 => 32,
                SCE_GS_PSM.SCE_GS_PSMCT24 => 32, // RGB24, uses 24-bit per pixel with the upper 8 bit unused.
                SCE_GS_PSM.SCE_GS_PSMCT16 => 16, // RGBA16 unsigned, pack two pixels in 32-bit in little endian order.
                SCE_GS_PSM.SCE_GS_PSMCT16S => 16, // RGBA16 signed, pack two pixels in 32-bit in little endian order.
                SCE_GS_PSM.SCE_GS_PSMT8 => 8, // 8-bit indexed, packing 4 pixels per 32-bit.
                SCE_GS_PSM.SCE_GS_PSMT4 => 4, // 4-bit indexed, packing 8 pixels per 32-bit.
                SCE_GS_PSM.SCE_GS_PSMT8H => 4, // 8-bit indexed, but the upper 24-bit are unused.
                SCE_GS_PSM.SCE_GS_PSMT4HL => 4, // 4-bit indexed, but the upper 24-bit are unused.
                SCE_GS_PSM.SCE_GS_PSMT4HH => 4,
                SCE_GS_PSM.SCE_GS_PSMZ32 => 32,  // 32-bit Z buffer
                SCE_GS_PSM.SCE_GS_PSMZ24 => 32, // 24-bit Z buffer with the upper 8-bit unused
                SCE_GS_PSM.SCE_GS_PSMZ16 => 16, // 16-bit unsigned Z buffer, pack two pixels in 32-bit in little endian order.
                SCE_GS_PSM.SCE_GS_PSMZ16S => 16, // 16-bit signed Z buffer, pack two pixels in 32-bit in little endian order.
                _ => throw new InvalidOperationException($"Invalid pixel surface type '{psm}'")
            };
        }

        public static double Normalize(double val, double valmin, double valmax, double min, double max)
        {
            return (((val - valmin) / (valmax - valmin)) * (max - min)) + min;
        }
    }
}
