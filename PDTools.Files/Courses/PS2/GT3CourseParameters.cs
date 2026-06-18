using System;
using System.Collections.Generic;
using System.IO;

namespace PDTools.Files.Courses.PS2;

/// <summary>
/// GT3 "22_CourseParameters" — AI driving-line data (racing/AI lines, the pit-lane
/// line, pit boxes). It is a sibling course file, NOT part of the runway file.
///
/// Layout (little-endian):
///   0x00  u32  totalSize          (offset of the end of the last section)
///   0x04  u32  count              (section-table entry count; observed 8 for 7 sections)
///   0x08  u32  0
///   0x0C  u32[] section offsets    (until the build string)
///   0x28  ASCII "build MM/DD/YY HH:MM:SS\n" + padding up to the first section
///   &lt;section&gt;*  each: u32 nodeCount, then nodeCount × 40-byte node
///   node = 10 × int32: [0]flag [1]X [2]Z [3]Y [4..9] partly decoded
///
/// The pit-lane line is the section whose first node lies on the runway's
/// pitlane-flagged surface (tri+12 bit0). Coordinates are integers ≈ ×4000 the
/// runway's float metres (transform not yet calibrated).
///
/// Read()/Write() are lossless: the header (TOC + build string + padding) and the
/// trailing alignment bytes are preserved verbatim, and the section offsets +
/// totalSize are recomputed on write so edited line sizes stay consistent.
/// </summary>
public class GT3CourseParameters
{
    public const int NodeSize   = 40;   // bytes per node (10 × int32)
    public const int NodeInts   = 10;

    /// <summary>Header field at 0x04 — a constant 8 on every observed file (NOT the
    /// section count, which is always 7). Written verbatim into synthetic headers.</summary>
    public const uint HeaderCountConst = 8;

    /// <summary>Offset where the first section begins (0x48 on every file): the TOC
    /// (0x00..0x27) + the ASCII build string (0x28) + zero-pad up to here.</summary>
    public const int FirstSectionOffset = 0x48;

    /// <summary>Every official file's total length is a multiple of 0x40 (the data ends
    /// at totalSize, then zero-pad up to the next 0x40 boundary).</summary>
    public const int FileAlignment = 0x40;

    /// <summary>
    /// If &gt; 1, <see cref="Write"/> zero-pads the output up to this byte alignment
    /// AFTER <see cref="TailBlob"/> (totalSize still marks the data end). 0 = write
    /// exactly (preserves byte-identical round-trips of files read from disk, whose
    /// <see cref="TailBlob"/> already carries the original padding). Synthetic files
    /// (<see cref="CreateForSynthesis"/>) set this to <see cref="FileAlignment"/> so the
    /// engine/volume sees a correctly-aligned file — without it the file is short and
    /// downstream data shifts (crash).
    /// </summary>
    public int PadToAlignment { get; set; } = 0;

    /// <summary>
    /// Fixed-point scale between integer course coords and runway float metres
    /// (12 fractional bits). The X axis is flipped vs the runway mesh.
    /// Calibrated against apricot: racing line lands within ~6 m of the centerline.
    /// </summary>
    public const float WorldScale = 4096f;

    /// <summary>Convert a node's integer (X,Z) to runway-mesh world metres (X flipped).</summary>
    public static (float x, float z) ToWorld(int courseX, int courseZ)
        => (-courseX / WorldScale, courseZ / WorldScale);

    /// <summary>Raw header bytes [0, FirstSectionOffset): TOC + build string + padding.</summary>
    public byte[] HeaderBlob { get; set; } = [];

    /// <summary>Raw bytes after the last section (alignment padding), preserved verbatim.</summary>
    public byte[] TailBlob { get; set; } = [];

    public List<Line> Lines { get; set; } = [];

    /// <summary>One AI driving line / pit line / pit-box set: a list of 10-int32 nodes.</summary>
    public class Line
    {
        public List<int[]> Nodes { get; set; } = [];

        public int Count => Nodes.Count;
        public int X(int i) => Nodes[i][1];
        public int Z(int i) => Nodes[i][2];
        public int Y(int i) => Nodes[i][3];
    }

    private static uint U32(byte[] d, int o) => BitConverter.ToUInt32(d, o);

    /// <summary>
    /// Builds a from-scratch instance with a synthetic header (no donor file). The
    /// caller fills <see cref="Lines"/> (always 7 sections; empties are allowed) and
    /// calls <see cref="Write"/>, which patches the section offsets + totalSize. The
    /// header is the verified-regular layout: count=8 at 0x04, 7 offset slots
    /// (0x0C..0x27, patched on write), an ASCII <c>build MM/DD/YY HH:MM:SS\n</c> string
    /// at 0x28, zero-padded to the first section at 0x48.
    /// </summary>
    public static GT3CourseParameters CreateForSynthesis(DateTime buildTime)
    {
        var h = new byte[FirstSectionOffset];
        BitConverter.GetBytes(HeaderCountConst).CopyTo(h, 0x04);

        string build = "build " + buildTime.ToString("MM/dd/yy HH:mm:ss",
                           System.Globalization.CultureInfo.InvariantCulture) + "\n";
        byte[] bs = System.Text.Encoding.ASCII.GetBytes(build);
        Array.Copy(bs, 0, h, 0x28, Math.Min(bs.Length, FirstSectionOffset - 0x28));

        return new GT3CourseParameters { HeaderBlob = h, PadToAlignment = FileAlignment };
    }

    public static GT3CourseParameters Read(byte[] d)
    {
        var cp = new GT3CourseParameters();
        int totalSize = (int)U32(d, 0x00);

        // Section offsets run from 0x0C until a u32 stops looking like an ascending,
        // in-range offset (the next field is the ASCII build string).
        var offs = new List<int>();
        for (int p = 0x0C; p + 4 <= d.Length; p += 4)
        {
            uint v = U32(d, p);
            if (v < 0x0C || v > (uint)d.Length) break;
            if (offs.Count > 0 && v <= (uint)offs[^1]) break;
            offs.Add((int)v);
        }
        if (offs.Count == 0)
            throw new InvalidDataException("CourseParameters: no section offsets found.");

        cp.HeaderBlob = d[..offs[0]];

        foreach (int o in offs)
        {
            int nc = (int)U32(d, o);
            var line = new Line();
            for (int k = 0; k < nc; k++)
            {
                int b = o + 4 + k * NodeSize;
                var node = new int[NodeInts];
                for (int j = 0; j < NodeInts; j++)
                    node[j] = BitConverter.ToInt32(d, b + j * 4);
                line.Nodes.Add(node);
            }
            cp.Lines.Add(line);
        }

        cp.TailBlob = totalSize <= d.Length ? d[totalSize..] : [];
        return cp;
    }

    public byte[] Write()
    {
        var outp = new List<byte>(HeaderBlob);   // offsets/totalSize patched below
        var sectionOffsets = new List<int>(Lines.Count);

        foreach (var line in Lines)
        {
            sectionOffsets.Add(outp.Count);
            Append32(outp, (uint)line.Nodes.Count);
            foreach (var node in line.Nodes)
                for (int j = 0; j < NodeInts; j++)
                    Append32(outp, unchecked((uint)node[j]));
        }
        int totalSize = outp.Count;   // data end (excludes tail/alignment padding)
        outp.AddRange(TailBlob);

        // Zero-pad the file up to the alignment boundary (synthetic files only; read
        // files keep PadToAlignment = 0 so their original TailBlob padding is exact).
        if (PadToAlignment > 1)
        {
            int aligned = (outp.Count + PadToAlignment - 1) / PadToAlignment * PadToAlignment;
            while (outp.Count < aligned) outp.Add(0);
        }

        var arr = outp.ToArray();
        Patch32(arr, 0x00, (uint)totalSize);                       // totalSize (data end)
        for (int i = 0; i < sectionOffsets.Count; i++)
            Patch32(arr, 0x0C + i * 4, (uint)sectionOffsets[i]);   // section offset table
        return arr;
    }

    /// <summary>Section index of the direction-independent reference line (S4).</summary>
    public const int ReferenceSectionIndex = 4;

    /// <summary>
    /// Reverse the driving direction of the AI lines, to match a reversed runway.
    /// reverse-gtrw keeps the collision mesh byte-identical, so node positions (f1/f2)
    /// and the f4 collision-node link stay valid — they simply travel with the reversed
    /// nodes (f4 then runs the opposite way, e.g. 11..93 → 93..11).
    ///
    /// Per line the node ORDER is reversed and each node's f6 turn-direction term has its
    /// SIGN flipped (a left bend becomes a right bend when you drive the other way; f6=0
    /// on synthesized/plain lines, so this is a no-op there). The reference line
    /// (<see cref="ReferenceSectionIndex"/>) is left untouched — in PD's own forward/
    /// reverse pairs it is direction-independent (≈byte-identical fwd vs rev).
    ///
    /// Validated against apricot_f vs apricot_r: reversing the forward racing line lands
    /// its new first node where the reverse file's first node is. The transform is its
    /// own inverse (reversing twice restores the original).
    /// </summary>
    public void ReverseDirection()
    {
        for (int s = 0; s < Lines.Count; s++)
        {
            if (s == ReferenceSectionIndex) continue;   // direction-independent reference

            var nodes = Lines[s].Nodes;
            nodes.Reverse();
            foreach (var node in nodes)
                node[6] = unchecked(-node[6]);          // turn-direction sign flips
        }
    }

    private static void Append32(List<byte> l, uint v)
    {
        l.Add((byte)v); l.Add((byte)(v >> 8)); l.Add((byte)(v >> 16)); l.Add((byte)(v >> 24));
    }

    private static void Patch32(byte[] b, int o, uint v)
    {
        b[o] = (byte)v; b[o + 1] = (byte)(v >> 8); b[o + 2] = (byte)(v >> 16); b[o + 3] = (byte)(v >> 24);
    }
}
