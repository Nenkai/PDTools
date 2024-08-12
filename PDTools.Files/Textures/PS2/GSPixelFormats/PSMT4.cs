using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDTools.Files.Textures.PS2.GSPixelFormats;

namespace PDTools.Files.Textures.PS2.GSFormats;

// PSMT4
// - Page: 128x128 pixels
// - Block: 32x16 pixels
// - Columns: 32x4 pixels

/*
public const int GS_PSMT4_PAGE_COLS = 4;
public const int GS_PSMT4_PAGE_ROWS = 8;
public const int GS_PSMT4_COLUMN_WIDTH = 32;
public const int GS_PSMT4_COLUMN_HEIGHT = 4;

public const int GS_PSMT4_BLOCK_WIDTH = (GS_PSMT4_COLUMN_WIDTH * GS_BLOCK_COLS);    // 32
public const int GS_PSMT4_BLOCK_HEIGHT = GS_PSMT4_COLUMN_HEIGHT * GS_BLOCK_ROWS;    // 16
public const int GS_PSMT4_PAGE_WIDTH = GS_PSMT4_BLOCK_WIDTH * GS_PSMT4_PAGE_COLS;   // 128
public const int GS_PSMT4_PAGE_HEIGHT = GS_PSMT4_BLOCK_HEIGHT * GS_PSMT4_PAGE_ROWS; // 128
*/

public class PSMT4 : GSPixelFormat
{
    public override SCE_GS_PSM PSM => SCE_GS_PSM.SCE_GS_PSMT4;
    public override int PageColumns { get; } = 4;
    public override int PageRows { get; } = 8;
    public override int ColumnWidth { get; } = 32;
    public override int ColumnHeight { get; } = 4;

    public override int BlockWidth => ColumnWidth * GSMemory.GS_BLOCK_COLS;
    public override int BlockHeight => ColumnHeight * GSMemory.GS_BLOCK_ROWS;

    public override int PageWidth => BlockWidth * PageColumns;
    public override int PageHeight => BlockHeight * PageRows;

    /// <summary>
    /// GS Block layout in a page for <see cref="SCE_GS_PSM.SCE_GS_PSMT4"/>
    /// </summary>
    public override int[] BlockLayout => new int[32]
    {
         0,  2,  8, 10,
         1,  3,  9, 11,
         4,  6, 12, 14,
         5,  7, 13, 15,
        16, 18, 24, 26,
        17, 19, 25, 27,
        20, 22, 28, 30,
        21, 23, 29, 31
    };

    public int[][] columnWord4 = new int[2][/*128*/]
    {
        new int[128]
        {
             0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
             2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,

             8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,
            10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7

        },
        new int[128]
        {
            8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,
            10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,

             0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
             2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15

        }
    };

    public int[] columnByte4 = new int[128]
    {
         0, 0, 0, 0, 0, 0, 0, 0,  2, 2, 2, 2, 2, 2, 2, 2,  4, 4, 4, 4, 4, 4, 4, 4,  6, 6, 6, 6, 6, 6, 6, 6,
         0, 0, 0, 0, 0, 0, 0, 0,  2, 2, 2, 2, 2, 2, 2, 2,  4, 4, 4, 4, 4, 4, 4, 4,  6, 6, 6, 6, 6, 6, 6, 6,

         1, 1, 1, 1, 1, 1, 1, 1,  3, 3, 3, 3, 3, 3, 3, 3,  5, 5, 5, 5, 5, 5, 5, 5,  7, 7, 7, 7, 7, 7, 7, 7,
         1, 1, 1, 1, 1, 1, 1, 1,  3, 3, 3, 3, 3, 3, 3, 3,  5, 5, 5, 5, 5, 5, 5, 5,  7, 7, 7, 7, 7, 7, 7, 7
    };
}
