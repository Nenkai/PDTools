using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using PDTools.Files.Textures.PS2.GSPixelFormats;

namespace PDTools.Files.Textures.PS2;

/// <summary>
/// Used for (de)swizzling
/// </summary>
public class GSMemory
{
    /// <summary>
    /// Number of blocks in a single gs page.
    /// </summary>
    public const int BLOCKS_PER_PAGE = 32;

    /// <summary>
    /// Size of one gs block in bytes.
    /// </summary>
    public const int BLOCK_SIZE_BYTES = 256;

    /// <summary>
    /// Size of one gs page in bytes.
    /// </summary>
    public const int PAGE_SIZE_BYTES = BLOCK_SIZE_BYTES * BLOCKS_PER_PAGE; // 8 Kbytes

    public const int GS_BLOCK_COLS = 1;
    public const int GS_BLOCK_ROWS = 4;

    public const int MAX_PAGES = 512;
    public const int MAX_BLOCKS = 16384;

    private readonly uint[] _gsMemory = new uint[1024 * 1024];

    /// <summary>
    /// Writes PSMCT32 Texture data to GS Memory.
    /// </summary>
    /// <param name="dbp">Block Position</param>
    /// <param name="dbw">Buffer Width</param>
    /// <param name="dsax">X Start</param>
    /// <param name="dsay">Y Start</param>
    /// <param name="rrw"></param>
    /// <param name="rrh"></param>
    /// <param name="data">Input data as PSMCT32</param>
    public void WriteTexPSMCT32(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, Span<uint> data)
    {
        Span<uint> src = data;
        int startBlockPos = dbp * 64;

        for (int y = dsay; y < dsay + rrh; y++)
        {
            for (int x = dsax; x < dsax + rrw; x++)
            {
                int pageX = x / 64;
                int pageY = y / 32;
                int page = pageX + pageY * dbw;

                int px = x - (pageX * 64);
                int py = y - (pageY * 32);

                int blockX = px / 8;
                int blockY = py / 8;
                int block = GSPixelFormat.PSM_CT32.BlockLayout[blockX + blockY * 8];

                int bx = px - blockX * 8;
                int by = py - blockY * 8;

                int column = by / 2;

                int cx = bx;
                int cy = by - column * 2;
                int cw = GSPixelFormat.PSM_CT32.columnWord32[cx + cy * 8];

                _gsMemory[startBlockPos + page * 2048 + block * 64 + column * 16 + cw] = src[0];
                src = src[1..];
            }
        }
    }

    /// <summary>
    /// Reads PSMZ32 Texture data from GS Memory.
    /// </summary>
    /// <param name="dbp">Block Position</param>
    /// <param name="dbw">Buffer Width</param>
    /// <param name="dsax">X Start</param>
    /// <param name="dsay">Y Start</param>
    /// <param name="rrw"></param>
    /// <param name="rrh"></param>
    /// <param name="data">Where the data will be stored</param>
    public void ReadTexPSMCT32(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, Span<uint> data)
    {
        Span<uint> src = data;
        int startBlockPos = dbp * 64;

        for (int y = dsay; y < dsay + rrh; y++)
        {
            for (int x = dsax; x < dsax + rrw; x++)
            {
                int pageX = x / 64;
                int pageY = y / 32;
                int page = pageX + pageY * dbw;

                int px = x - (pageX * 64);
                int py = y - (pageY * 32);

                int blockX = px / 8;
                int blockY = py / 8;
                int block = GSPixelFormat.PSM_CT32.BlockLayout[blockX + blockY * 8];

                int bx = px - blockX * 8;
                int by = py - blockY * 8;

                int column = by / 2;

                int cx = bx;
                int cy = by - column * 2;
                int cw = GSPixelFormat.PSM_CT32.columnWord32[cx + cy * 8];

                src[0] = _gsMemory[startBlockPos + page * 2048 + block * 64 + column * 16 + cw];
                src = src[1..];
            }
        }
    }

    static readonly int[] blockZ32 =
    [
        24, 25, 28, 29,  8,  9, 12, 13,
        26, 27, 30, 31, 10, 11, 14, 15,
        16, 17, 20, 21,  0,  1,  4,  5,
        18, 19, 22, 23,  2,  3,  6,  7
    ];

    static readonly int[] columnWordZ32 =
    [
        0,  1,  4,  5,  8,  9, 12, 13,
        2,  3,  6,  7, 10, 11, 14, 15
    ];

    /// <summary>
    /// Writes PSMZ32 Texture data to GS Memory.
    /// </summary>
    /// <param name="dbp">Block Position</param>
    /// <param name="dbw">Buffer Width</param>
    /// <param name="dsax">X Start</param>
    /// <param name="dsay">Y Start</param>
    /// <param name="rrw"></param>
    /// <param name="rrh"></param>
    /// <param name="data">Input data as PSMZ32</param>
    public void WriteTexPSMZ32(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, Span<uint> data)
    {
        Span<uint> src = data;
        int startBlockPos = dbp * 64;

        for (int y = dsay; y < dsay + rrh; y++)
        {
            for (int x = dsax; x < dsax + rrw; x++)
            {
                int pageX = x / 64;
                int pageY = y / 32;
                int page = pageX + pageY * dbw;

                int px = x - (pageX * 64);
                int py = y - (pageY * 32);

                int blockX = px / 8;
                int blockY = py / 8;
                int block = blockZ32[blockX + blockY * 8];

                int bx = px - blockX * 8;
                int by = py - blockY * 8;

                int column = by / 2;

                int cx = bx;
                int cy = by - column * 2;
                int cw = columnWordZ32[cx + cy * 8];

                _gsMemory[startBlockPos + page * 2048 + block * 64 + column * 16 + cw] = src[0];
                src = src[1..];
            }
        }
    }

    /// <summary>
    /// Reads PSMZ32 Texture data from GS Memory.
    /// </summary>
    /// <param name="dbp">Block Position</param>
    /// <param name="dbw">Buffer Width</param>
    /// <param name="dsax">X Start</param>
    /// <param name="dsay">Y Start</param>
    /// <param name="rrw"></param>
    /// <param name="rrh"></param>
    /// <param name="data">Where the data will be stored</param>
    public void ReadTexPSMZ32(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, Span<uint> data)
    {
        Span<uint> src = data;
        int startBlockPos = dbp * 64;

        for (int y = dsay; y < dsay + rrh; y++)
        {
            for (int x = dsax; x < dsax + rrw; x++)
            {
                int pageX = x / 64;
                int pageY = y / 32;
                int page = pageX + pageY * dbw;

                int px = x - (pageX * 64);
                int py = y - (pageY * 32);

                int blockX = px / 8;
                int blockY = py / 8;
                int block = blockZ32[blockX + blockY * 8];

                int bx = px - blockX * 8;
                int by = py - blockY * 8;

                int column = by / 2;

                int cx = bx;
                int cy = by - column * 2;
                int cw = columnWordZ32[cx + cy * 8];

                src[0] = _gsMemory[startBlockPos + page * 2048 + block * 64 + column * 16 + cw];
                src = src[1..];
            }
        }
    }

    static readonly int[] block16 =
    [
         0,  2,  8, 10,
         1,  3,  9, 11,
         4,  6, 12, 14,
         5,  7, 13, 15,
        16, 18, 24, 26,
        17, 19, 25, 27,
        20, 22, 28, 30,
        21, 23, 29, 31
    ];

    static readonly int[] columnWord16 =
    [
        0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
        2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15
    ];

    static readonly int[] columnHalf16 =
    [
        0, 0, 0, 0, 0, 0, 0, 0,  1, 1, 1, 1, 1, 1, 1, 1,
        0, 0, 0, 0, 0, 0, 0, 0,  1, 1, 1, 1, 1, 1, 1, 1
    ];


    /// <summary>
    /// Writes PSMCT16 Texture data to GS Memory.
    /// </summary>
    /// <param name="dbp">Block Position</param>
    /// <param name="dbw">Buffer Width</param>
    /// <param name="dsax">X Start</param>
    /// <param name="dsay">Y Start</param>
    /// <param name="rrw"></param>
    /// <param name="rrh"></param>
    /// <param name="data">Input data as PSMCT16</param>
    public void WriteTexPSMCT16(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, Span<ushort> data)
    {
        //dbw >>= 1;
        Span<ushort> src = data;
        int startBlockPos = dbp * 64;

        for (int y = dsay; y < dsay + rrh; y++)
        {
            for (int x = dsax; x < dsax + rrw; x++)
            {
                int pageX = x / 64;
                int pageY = y / 64;
                int page = pageX + pageY * dbw;

                int px = x - (pageX * 64);
                int py = y - (pageY * 64);

                int blockX = px / 16;
                int blockY = py / 8;
                int block = block16[blockX + blockY * 4];

                int bx = px - blockX * 16;
                int by = py - blockY * 8;

                int column = by / 2;

                int cx = bx;
                int cy = by - column * 2;
                int cw = columnWord16[cx + cy * 16];
                int ch = columnHalf16[cx + cy * 16];

                Span<ushort> dst = MemoryMarshal.Cast<uint, ushort>(_gsMemory.AsSpan(startBlockPos + page * 2048 + block * 64 + column * 16 + cw));
                dst[ch] = src[0];
                src = src[1..];
            }
        }
    }

    /// <summary>
    /// Reads PSMCT16 Texture data from GS Memory.
    /// </summary>
    /// <param name="dbp">Block Position</param>
    /// <param name="dbw">Buffer Width</param>
    /// <param name="dsax">X Start</param>
    /// <param name="dsay">Y Start</param>
    /// <param name="rrw"></param>
    /// <param name="rrh"></param>
    /// <param name="data">Where the data will be stored</param>
    public void ReadTexPSMCT16(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, Span<ushort> data)
    {
        //dbw >>= 1;
        Span<ushort> src = data;
        int startBlockPos = dbp * 64;

        for (int y = dsay; y < dsay + rrh; y++)
        {
            for (int x = dsax; x < dsax + rrw; x++)
            {
                int pageX = x / 64;
                int pageY = y / 64;
                int page = pageX + pageY * dbw;

                int px = x - (pageX * 64);
                int py = y - (pageY * 64);

                int blockX = px / 16;
                int blockY = py / 8;
                int block = block16[blockX + blockY * 4];

                int bx = px - blockX * 16;
                int by = py - blockY * 8;

                int column = by / 2;

                int cx = bx;
                int cy = by - column * 2;
                int cw = columnWord16[cx + cy * 16];
                int ch = columnHalf16[cx + cy * 16];

                Span<ushort> dst = MemoryMarshal.Cast<uint, ushort>(_gsMemory.AsSpan(startBlockPos + page * 2048 + block * 64 + column * 16 + cw));
                src[0] = dst[ch];
                src = src[1..];
            }
        }
    }

    static readonly int[] blockZ16 =
    [
         24, 26, 16, 18,
         25, 27, 17, 19,
         28, 30, 20, 22,
         29, 31, 21, 23,
          8, 10,  0,  2,
          9, 11,  1,  3,
         12, 14,  4,  6,
         13, 15,  5,  7
    ];

    static readonly int[] columnWordZ16 =
    [
        0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
        2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15
    ];

    static readonly int[] columnHalfZ16 =
    [
        0, 0, 0, 0, 0, 0, 0, 0,  1, 1, 1, 1, 1, 1, 1, 1,
        0, 0, 0, 0, 0, 0, 0, 0,  1, 1, 1, 1, 1, 1, 1, 1
    ];

    /// <summary>
    /// Writes PSMZ16 Texture data to GS Memory.
    /// </summary>
    /// <param name="dbp">Block Position</param>
    /// <param name="dbw">Buffer Width</param>
    /// <param name="dsax">X Start</param>
    /// <param name="dsay">Y Start</param>
    /// <param name="rrw"></param>
    /// <param name="rrh"></param>
    /// <param name="data">Input data as PSMZ16S</param>
    public void WriteTexPSMZ16(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, Span<ushort> data)
    {
        //dbw >>= 1;
        Span<ushort> src = data;
        int startBlockPos = dbp * 64;

        for (int y = dsay; y < dsay + rrh; y++)
        {
            for (int x = dsax; x < dsax + rrw; x++)
            {
                int pageX = x / 64;
                int pageY = y / 64;
                int page = pageX + pageY * dbw;

                int px = x - (pageX * 64);
                int py = y - (pageY * 64);

                int blockX = px / 16;
                int blockY = py / 8;
                int block = blockZ16[blockX + blockY * 4];

                int bx = px - blockX * 16;
                int by = py - blockY * 8;

                int column = by / 2;

                int cx = bx;
                int cy = by - column * 2;
                int cw = columnWordZ16[cx + cy * 16];
                int ch = columnHalfZ16[cx + cy * 16];

                Span<ushort> dst = MemoryMarshal.Cast<uint, ushort>(_gsMemory.AsSpan(startBlockPos + page * 2048 + block * 64 + column * 16 + cw));
                dst[ch] = src[0];
                src = src[1..];
            }
        }
    }

    /// <summary>
    /// Reads PSMZ16 Texture data from GS Memory.
    /// </summary>
    /// <param name="dbp">Block Position</param>
    /// <param name="dbw">Buffer Width</param>
    /// <param name="dsax">X Start</param>
    /// <param name="dsay">Y Start</param>
    /// <param name="rrw"></param>
    /// <param name="rrh"></param>
    /// <param name="data">Where the data will be stored</param>
    public void ReadTexPSMZ16(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, Span<ushort> data)
    {
        //dbw >>= 1;
        Span<ushort> src = data;
        int startBlockPos = dbp * 64;

        for (int y = dsay; y < dsay + rrh; y++)
        {
            for (int x = dsax; x < dsax + rrw; x++)
            {
                int pageX = x / 64;
                int pageY = y / 64;
                int page = pageX + pageY * dbw;

                int px = x - (pageX * 64);
                int py = y - (pageY * 64);

                int blockX = px / 16;
                int blockY = py / 8;
                int block = blockZ16[blockX + blockY * 4];

                int bx = px - blockX * 16;
                int by = py - blockY * 8;

                int column = by / 2;

                int cx = bx;
                int cy = by - column * 2;
                int cw = columnWordZ16[cx + cy * 16];
                int ch = columnHalfZ16[cx + cy * 16];

                Span<ushort> dst = MemoryMarshal.Cast<uint, ushort>(_gsMemory.AsSpan(startBlockPos + page * 2048 + block * 64 + column * 16 + cw));
                src[0] = dst[ch];
                src = src[1..];
            }
        }
    }

    static readonly int[] blockZ16S =
    [
        24,  26,  8, 10,
        25,  27,  9, 11,
        16,  18,  0,  2,
        17,  19,  1,  3,
        28,  30, 12, 14,
        29,  31, 13, 15,
        20,  22,  4,  6,
        21,  23,  5,  7
    ];

    static readonly int[] columnWordZ16S =
    [
         0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
         2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15
    ];

    static readonly int[] columnHalfZ16S =
    [
        0, 0, 0, 0, 0, 0, 0, 0,  1, 1, 1, 1, 1, 1, 1, 1,
        0, 0, 0, 0, 0, 0, 0, 0,  1, 1, 1, 1, 1, 1, 1, 1
    ];

    /// <summary>
    /// Writes PSMZ16S Texture data to GS Memory.
    /// </summary>
    /// <param name="dbp">Block Position</param>
    /// <param name="dbw">Buffer Width</param>
    /// <param name="dsax">X Start</param>
    /// <param name="dsay">Y Start</param>
    /// <param name="rrw"></param>
    /// <param name="rrh"></param>
    /// <param name="data">Input data as PSMZ16S</param>
    public void WriteTexPSMZ16S(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, Span<ushort> data)
    {
        //dbw >>= 1;
        Span<ushort> src = data;
        int startBlockPos = dbp * 64;

        for (int y = dsay; y < dsay + rrh; y++)
        {
            for (int x = dsax; x < dsax + rrw; x++)
            {
                int pageX = x / 64;
                int pageY = y / 64;
                int page = pageX + pageY * dbw;

                int px = x - (pageX * 64);
                int py = y - (pageY * 64);

                int blockX = px / 16;
                int blockY = py / 8;
                int block = blockZ16S[blockX + blockY * 4];

                int bx = px - blockX * 16;
                int by = py - blockY * 8;

                int column = by / 2;

                int cx = bx;
                int cy = by - column * 2;
                int cw = columnWordZ16S[cx + cy * 16];
                int ch = columnHalfZ16S[cx + cy * 16];

                Span<ushort> dst = MemoryMarshal.Cast<uint, ushort>(_gsMemory.AsSpan(startBlockPos + page * 2048 + block * 64 + column * 16 + cw));
                dst[ch] = src[0];
                src = src[1..];
            }
        }

    }

    /// <summary>
    /// Reads PSMZ16S Texture data from GS Memory.
    /// </summary>
    /// <param name="dbp">Block Position</param>
    /// <param name="dbw">Buffer Width</param>
    /// <param name="dsax">X Start</param>
    /// <param name="dsay">Y Start</param>
    /// <param name="rrw"></param>
    /// <param name="rrh"></param>
    /// <param name="data">Where the data will be stored</param>
    public void ReadTexPSMZ16S(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, Span<ushort> data)
    {
        //dbw >>= 1;
        Span<ushort> src = data;
        int startBlockPos = dbp * 64;

        for (int y = dsay; y < dsay + rrh; y++)
        {
            for (int x = dsax; x < dsax + rrw; x++)
            {
                int pageX = x / 64;
                int pageY = y / 64;
                int page = pageX + pageY * dbw;

                int px = x - (pageX * 64);
                int py = y - (pageY * 64);

                int blockX = px / 16;
                int blockY = py / 8;
                int block = blockZ16S[blockX + blockY * 4];

                int bx = px - blockX * 16;
                int by = py - blockY * 8;

                int column = by / 2;

                int cx = bx;
                int cy = by - column * 2;
                int cw = columnWordZ16S[cx + cy * 16];
                int ch = columnHalfZ16S[cx + cy * 16];

                Span<ushort> dst = MemoryMarshal.Cast<uint, ushort>(_gsMemory.AsSpan(startBlockPos + page * 2048 + block * 64 + column * 16 + cw));
                src[0] = dst[ch];
                src = src[1..];
            }
        }
    }

    static readonly int[] block16S =
    [
         0,  2, 16, 18,
         1,  3, 17, 19,
         8, 10, 24, 26,
         9, 11, 25, 27,
         4,  6, 20, 22,
         5,  7, 21, 23,
        12, 14, 28, 30,
        13, 15, 29, 31
    ];

    static readonly int[] columnWord16S =
    [
         0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
         2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15
    ];

    static readonly int[] columnHalf16S =
    [
        0, 0, 0, 0, 0, 0, 0, 0,  1, 1, 1, 1, 1, 1, 1, 1,
        0, 0, 0, 0, 0, 0, 0, 0,  1, 1, 1, 1, 1, 1, 1, 1
    ];

    /// <summary>
    /// Writes PSMCT16S Texture data to GS Memory.
    /// </summary>
    /// <param name="dbp">Block Position</param>
    /// <param name="dbw">Buffer Width</param>
    /// <param name="dsax">X Start</param>
    /// <param name="dsay">Y Start</param>
    /// <param name="rrw"></param>
    /// <param name="rrh"></param>
    /// <param name="data">Input data as PSMCT16S</param>
    public void WriteTexPSMCT16S(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, Span<ushort> data)
    {
        //dbw >>= 1;
        Span<ushort> src = data;
        int startBlockPos = dbp * 64;

        for (int y = dsay; y < dsay + rrh; y++)
        {
            for (int x = dsax; x < dsax + rrw; x++)
            {
                int pageX = x / 64;
                int pageY = y / 64;
                int page = pageX + pageY * dbw;

                int px = x - (pageX * 64);
                int py = y - (pageY * 64);

                int blockX = px / 16;
                int blockY = py / 8;
                int block = block16S[blockX + blockY * 4];

                int bx = px - blockX * 16;
                int by = py - blockY * 8;

                int column = by / 2;

                int cx = bx;
                int cy = by - column * 2;
                int cw = columnWord16S[cx + cy * 16];
                int ch = columnHalf16S[cx + cy * 16];

                Span<ushort> dst = MemoryMarshal.Cast<uint, ushort>(_gsMemory.AsSpan(startBlockPos + page * 2048 + block * 64 + column * 16 + cw));
                dst[ch] = src[0];
                src = src[1..];
            }
        }
    }

    /// <summary>
    /// Reads PSMCT16S Texture data from GS Memory.
    /// </summary>
    /// <param name="dbp">Block Position</param>
    /// <param name="dbw">Buffer Width</param>
    /// <param name="dsax">X Start</param>
    /// <param name="dsay">Y Start</param>
    /// <param name="rrw"></param>
    /// <param name="rrh"></param>
    /// <param name="data">Where the data will be stored</param>
    public void ReadTexPSMCT16S(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, Span<ushort> data)
    {
        //dbw >>= 1;
        Span<ushort> src = data;
        int startBlockPos = dbp * 64;

        for (int y = dsay; y < dsay + rrh; y++)
        {
            for (int x = dsax; x < dsax + rrw; x++)
            {
                int pageX = x / 64;
                int pageY = y / 64;
                int page = pageX + pageY * dbw;

                int px = x - (pageX * 64);
                int py = y - (pageY * 64);

                int blockX = px / 16;
                int blockY = py / 8;
                int block = block16S[blockX + blockY * 4];

                int bx = px - blockX * 16;
                int by = py - blockY * 8;

                int column = by / 2;

                int cx = bx;
                int cy = by - column * 2;
                int cw = columnWord16S[cx + cy * 16];
                int ch = columnHalf16S[cx + cy * 16];

                Span<ushort> dst = MemoryMarshal.Cast<uint, ushort>(_gsMemory.AsSpan(startBlockPos + page * 2048 + block * 64 + column * 16 + cw));
                src[0] = dst[ch];
                src = src[1..];
            }
        }
    }



    /// <summary>
    /// Writes PSMT8 Texture data to GS Memory.
    /// </summary>
    /// <param name="dbp">Block Position</param>
    /// <param name="dbw">Buffer Width</param>
    /// <param name="dsax">X Start</param>
    /// <param name="dsay">Y Start</param>
    /// <param name="rrw"></param>
    /// <param name="rrh"></param>
    /// <param name="data">Input data as PSMT8</param>
    public void WriteTexPSMT8(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, Span<byte> data)
    {
        dbw >>= 1;
        Span<byte> src = data;
        int startBlockPos = dbp * 64;

        for (int y = dsay; y < dsay + rrh; y++)
        {
            for (int x = dsax; x < dsax + rrw; x++)
            {
                int pageX = x / 128;
                int pageY = y / 64;
                int page = pageX + pageY * dbw;

                int px = x - (pageX * 128);
                int py = y - (pageY * 64);

                int blockX = px / 16;
                int blockY = py / 16;
                int block = GSPixelFormat.PSMT8.BlockLayout[blockX + blockY * 8];

                int bx = px - (blockX * 16);
                int by = py - (blockY * 16);

                int column = by / 4;

                int cx = bx;
                int cy = by - column * 4;
                int cw = GSPixelFormat.PSMT8.columnWord8[column & 1][cx + cy * 16];
                int cb = GSPixelFormat.PSMT8.columnByte8[cx + cy * 16];

                Span<byte> dst = MemoryMarshal.Cast<uint, byte>(_gsMemory.AsSpan(startBlockPos + page * 2048 + block * 64 + column * 16 + cw));
                dst[cb] = src[0];
                src = src[1..];
            }
        }
    }

    /// <summary>
    /// Reads PSMT8 Texture data from GS Memory.
    /// </summary>
    /// <param name="dbp">Block Position</param>
    /// <param name="dbw">Buffer Width</param>
    /// <param name="dsax">X Start</param>
    /// <param name="dsay">Y Start</param>
    /// <param name="rrw"></param>
    /// <param name="rrh"></param>
    /// <param name="data">Where the data will be stored</param>
    public void ReadTexPSMT8(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, Span<byte> data)
    {
        dbw >>= 1;
        Span<byte> src = data;
        int startBlockPos = dbp * 64;

        for (int y = dsay; y < dsay + rrh; y++)
        {
            for (int x = dsax; x < dsax + rrw; x++)
            {
                int pageX = x / 128;
                int pageY = y / 64;
                int page = pageX + pageY * dbw;

                int px = x - (pageX * 128);
                int py = y - (pageY * 64);

                int blockX = px / 16;
                int blockY = py / 16;
                int block = GSPixelFormat.PSMT8.BlockLayout[blockX + blockY * 8];

                int bx = px - blockX * 16;
                int by = py - blockY * 16;

                int column = by / 4;

                int cx = bx;
                int cy = by - column * 4;
                int cw = GSPixelFormat.PSMT8.columnWord8[column & 1][cx + cy * 16];
                int cb = GSPixelFormat.PSMT8.columnByte8[cx + cy * 16];

                Span<byte> dst = MemoryMarshal.Cast<uint, byte>(_gsMemory.AsSpan(startBlockPos + page * 2048 + block * 64 + column * 16 + cw));
                src[0] = dst[cb];
                src = src[1..];
            }
        }
    }

    /// <summary>
    /// Writes PSMT4 Texture data to GS Memory.
    /// </summary>
    /// <param name="dbp">Block Position</param>
    /// <param name="dbw">Buffer Width</param>
    /// <param name="dsax">X Start</param>
    /// <param name="dsay">Y Start</param>
    /// <param name="rrw"></param>
    /// <param name="rrh"></param>
    /// <param name="data">Input data as PSMT4</param>
    public void WriteTexPSMT4(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, Span<byte> data)
    {
        dbw >>= 1;
        Span<byte> src = data;
        int startBlockPos = dbp * 64;

        bool odd = false;

        for (int y = dsay; y < dsay + rrh; y++)
        {
            for (int x = dsax; x < dsax + rrw; x++)
            {
                int pageX = x / 128;
                int pageY = y / 128;
                int page = pageX + pageY * dbw;

                int px = x - (pageX * 128);
                int py = y - (pageY * 128);

                int blockX = px / 32;
                int blockY = py / 16;
                int block = GSPixelFormat.PSMT4.BlockLayout[blockX + blockY * 4];

                int bx = px - blockX * 32;
                int by = py - blockY * 16;

                int column = by / 4;

                int cx = bx;
                int cy = by - column * 4;
                int cw = GSPixelFormat.PSMT4.columnWord4[column & 1][cx + cy * 32];
                int cb = GSPixelFormat.PSMT4.columnByte4[cx + cy * 32];

                Span<byte> dst = MemoryMarshal.Cast<uint, byte>(_gsMemory.AsSpan(startBlockPos + page * 2048 + block * 64 + column * 16 + cw));

                if ((cb & 1) != 0)
                {
                    if (odd)
                        dst[cb >> 1] = (byte)((dst[cb >> 1] & 0x0f) | ((src[0]) & 0xf0));
                    else
                        dst[cb >> 1] = (byte)((dst[cb >> 1] & 0x0f) | (((src[0]) << 4) & 0xf0));
                }
                else
                {
                    if (odd)
                        dst[cb >> 1] = (byte)((dst[cb >> 1] & 0xf0) | (((src[0]) >> 4) & 0x0f));
                    else
                        dst[cb >> 1] = (byte)((dst[cb >> 1] & 0xf0) | ((src[0]) & 0x0f));
                }

                if (odd)
                    src = src[1..];

                odd = !odd;
            }
        }
    }

    /// <summary>
    /// Reads PSMT4 Texture data from GS Memory.
    /// </summary>
    /// <param name="dbp">Block Position</param>
    /// <param name="dbw">Buffer Width</param>
    /// <param name="dsax">X Start</param>
    /// <param name="dsay">Y Start</param>
    /// <param name="rrw"></param>
    /// <param name="rrh"></param>
    /// <param name="data">Where the data will be stored</param>
    public void ReadTexPSMT4(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, Span<byte> data)
    {
        dbw >>= 1;
        Span<byte> src = data;
        int startBlockPos = dbp * 64;

        bool odd = false;

        for (int y = dsay; y < dsay + rrh; y++)
        {
            for (int x = dsax; x < dsax + rrw; x++)
            {
                int pageX = x / 128;
                int pageY = y / 128;
                int page = pageX + pageY * dbw;

                int px = x - (pageX * 128);
                int py = y - (pageY * 128);

                int blockX = px / 32;
                int blockY = py / 16;
                int block = GSPixelFormat.PSMT4.BlockLayout[blockX + blockY * 4];

                int bx = px - blockX * 32;
                int by = py - blockY * 16;

                int column = by / 4;

                int cx = bx;
                int cy = by - column * 4;
                int cw = GSPixelFormat.PSMT4.columnWord4[column & 1][cx + cy * 32];
                int cb = GSPixelFormat.PSMT4.columnByte4[cx + cy * 32];

                Span<byte> dst = MemoryMarshal.Cast<uint, byte>(_gsMemory.AsSpan(startBlockPos + page * 2048 + block * 64 + column * 16 + cw));

                if ((cb & 1) != 0)
                {
                    if (odd)
                        src[0] = (byte)(((src[0]) & 0x0f) | (dst[cb >> 1] & 0xf0));
                    else
                        src[0] = (byte)(((src[0]) & 0xf0) | ((dst[cb >> 1] >> 4) & 0x0f));
                }
                else
                {
                    if (odd)
                        src[0] = (byte)(((src[0]) & 0x0f) | ((dst[cb >> 1] << 4) & 0xf0));
                    else
                        src[0] = (byte)(((src[0]) & 0xf0) | (dst[cb >> 1] & 0x0f));
                }

                if (odd)
                    src = src[1..];

                odd = !odd;
            }
        }
    }
}
