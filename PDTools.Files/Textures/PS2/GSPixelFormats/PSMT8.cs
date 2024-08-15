using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDTools.Files.Textures.PS2.GSPixelFormats;

namespace PDTools.Files.Textures.PS2.GSFormats;

// PSMT8
// - Page: 128x64 pixels
// - Block: 16x16 pixels
// - Columns: 16x4 pixels
/*
public const int GS_PSMT8_PAGE_COLS = 8;
public const int GS_PSMT8_PAGE_ROWS = 4;
public const int GS_PSMT8_COLUMN_WIDTH = 16;
public const int GS_PSMT8_COLUMN_HEIGHT = 4;

public const int GS_PSMT8_BLOCK_WIDTH = (GS_PSMT8_COLUMN_WIDTH * GS_BLOCK_COLS);    // 16
public const int GS_PSMT8_BLOCK_HEIGHT = GS_PSMT8_COLUMN_HEIGHT * GS_BLOCK_ROWS;    // 16
public const int GS_PSMT8_PAGE_WIDTH = GS_PSMT8_BLOCK_WIDTH * GS_PSMT8_PAGE_COLS;   // 128
public const int GS_PSMT8_PAGE_HEIGHT = GS_PSMT8_BLOCK_HEIGHT * GS_PSMT8_PAGE_ROWS; // 64
*/

public class PSMT8 : GSPixelFormat
{
    public override SCE_GS_PSM PSM => SCE_GS_PSM.SCE_GS_PSMT8;
    public override int PageColumns { get; } = 8;
    public override int PageRows { get; } = 4;
    public override int ColumnWidth { get; } = 16;
    public override int ColumnHeight { get; } = 4;

    public override int BlockWidth => ColumnWidth * GSMemory.GS_BLOCK_COLS;
    public override int BlockHeight => ColumnHeight * GSMemory.GS_BLOCK_ROWS;

    public override int PageWidth => BlockWidth * PageColumns;
    public override int PageHeight => BlockHeight * PageRows;

    /// <summary>
    /// GS Block layout in a page for <see cref="SCE_GS_PSM.SCE_GS_PSMT8"/>
    /// </summary>
    public override int[] BlockLayout => new int[32]
    {
         0,  1,  4,  5, 16, 17, 20, 21,
         2,  3,  6,  7, 18, 19, 22, 23,
         8,  9, 12, 13, 24, 25, 28, 29,
        10, 11, 14, 15, 26, 27, 30, 31
    };

    public int[][] columnWord8 = new int[2][/*64*/]
    {
            new int[64]
            {
                 0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
                 2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,

                 8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,
                10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7

            },
            new int[64]
            {
                  8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,
                 10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,

                  0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
                  2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15
            }
    };

    public int[] columnByte8 = new int[64]
    {
            0, 0, 0, 0, 0, 0, 0, 0,  2, 2, 2, 2, 2, 2, 2, 2,
            0, 0, 0, 0, 0, 0, 0, 0,  2, 2, 2, 2, 2, 2, 2, 2,

            1, 1, 1, 1, 1, 1, 1, 1,  3, 3, 3, 3, 3, 3, 3, 3,
            1, 1, 1, 1, 1, 1, 1, 1,  3, 3, 3, 3, 3, 3, 3, 3
    };

    public override (int Width, int Height) GetPaletteDimensions()
    {
        return (16, 16);
    }
}
