using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Files.Textures.PS2.GSPixelFormats;

namespace PDTools.Files.Textures.PS2.GSFormats;

// PSMCT32
// - Page: 64x32 pixels
// - Block: 8x8 pixels
// - Columns: 8x2 pixels
/*
public const int GS_PSM_CT32_PAGE_COLS = 8;
public const int GS_PSM_CT32_PAGE_ROWS = 4;
public const int GS_PSM_CT32_COLUMN_WIDTH = 8;
public const int GS_PSM_CT32_COLUMN_HEIGHT = 2;

public const int GS_PSM_CT32_BLOCK_WIDTH = (GS_PSM_CT32_COLUMN_WIDTH * GS_BLOCK_COLS);  // 8
public const int GS_PSM_CT32_BLOCK_HEIGHT = GS_PSM_CT32_COLUMN_HEIGHT * GS_BLOCK_ROWS;  // 8
public const int GS_PSM_CT32_PAGE_WIDTH = GS_PSM_CT32_BLOCK_WIDTH * GS_PSM_CT32_PAGE_COLS;   // 64
public const int GS_PSM_CT32_PAGE_HEIGHT = GS_PSM_CT32_BLOCK_HEIGHT * GS_PSM_CT32_PAGE_ROWS; // 32
*/

public class PSM_CT32 : GSPixelFormat
{
    public override SCE_GS_PSM PSM => SCE_GS_PSM.SCE_GS_PSMCT32;
    public override int PageColumns { get; } = 8;
    public override int PageRows { get; } = 4;
    public override int ColumnWidth { get; } = 8;
    public override int ColumnHeight { get; } = 2;

    public override int BlockWidth => ColumnWidth * GSMemory.GS_BLOCK_COLS;
    public override int BlockHeight => ColumnHeight * GSMemory.GS_BLOCK_ROWS;

    public override int PageWidth => BlockWidth * PageColumns;
    public override int PageHeight => BlockHeight * PageRows;

    public override int[] BlockLayout => new int[32]
    {
          0,  1,  4,  5, 16, 17, 20, 21,
          2,  3,  6,  7, 18, 19, 22, 23,
          8,  9, 12, 13, 24, 25, 28, 29,
         10, 11, 14, 15, 26, 27, 30, 31
    };


    public int[] columnWord32 = new int[16]
    {
        0,  1,  4,  5,  8,  9, 12, 13,
        2,  3,  6,  7, 10, 11, 14, 15
    };
}
