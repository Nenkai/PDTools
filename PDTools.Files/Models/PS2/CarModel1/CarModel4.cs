using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using PDTools.Files.Models.PS2.ModelSet;

namespace PDTools.Files.Models.PS2.CarModel1;

/// <summary>
/// GT4 Car Model container (CAR4 / "CAR4" magic).
///
/// The file is a 0x40-byte header followed by a variable number of sub-sections.
/// Each field in the header is an absolute file offset; zero means the section is absent.
///
/// Sub-sections are stored here as raw byte arrays so the container can be round-tripped
/// losslessly even when a particular sub-format is not yet fully understood.
/// Helper Get*() methods parse the byte arrays on demand for callers that need the
/// object-model view (e.g. the model dumper).
/// </summary>
public class CarModel4
{
    /// <summary>"CAR4" little-endian magic.</summary>
    public const uint MAGIC      = 0x34524143u;

    /// <summary>Size of the CAR4 file header.</summary>
    public const uint HeaderSize = 0x40u;

    // ── Sub-section raw bytes (null = section absent) ───────────────────────

    /// <summary>0x10 — GT4 car-info block (contains lights, cameras, exhaust positions, etc.).</summary>
    public byte[] CarInfoBytes { get; set; }

    /// <summary>0x14 — Collision boundary data.</summary>
    public byte[] CollisionModelBytes { get; set; }

    /// <summary>0x18 — Main body ModelSet2 (MDLS).</summary>
    public byte[] MainModelSetBytes { get; set; }

    /// <summary>0x1C — Color-patch table for the main model set (internal format unknown).</summary>
    public byte[] MainModelColorPatchBytes { get; set; }

    /// <summary>0x20 — Wheel ModelSet2 (MDLS).</summary>
    public byte[] WheelModelSetBytes { get; set; }

    /// <summary>0x24 — Color-patch table for the wheel model set.</summary>
    public byte[] WheelColorPatchBytes { get; set; }

    /// <summary>0x28 — Wing / spoiler ModelSet2 (MDLS, optional).</summary>
    public byte[] WingModelSetBytes { get; set; }

    /// <summary>0x2C — Tire ModelSet2 slot 0 (optional).</summary>
    public byte[] TireModelSet0Bytes { get; set; }

    /// <summary>0x30 — Tire ModelSet2 slot 1 (optional).</summary>
    public byte[] TireModelSet1Bytes { get; set; }

    /// <summary>0x34 — Built-in driver model (optional).</summary>
    public byte[] BuiltinDriverModelBytes { get; set; }

    /// <summary>0x38 — Extra texture set (optional).</summary>
    public byte[] TexSetBytes { get; set; }

    // ── Reading ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Parses a CAR4 from <paramref name="stream"/> starting at its current position.
    /// Each sub-section is stored as a raw byte array (offset → next_offset).
    /// </summary>
    public void FromStream(Stream stream)
    {
        long basePos = stream.Position;

        using var br = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        uint magic = br.ReadUInt32();
        if (magic != MAGIC)
            throw new InvalidDataException($"Not a CAR4 file (magic 0x{magic:X8}, expected 0x{MAGIC:X8}).");

        br.ReadUInt32(); // 0x04  RelocatePtr       (runtime)
        br.ReadUInt32(); // 0x08  FileSizeForReloc  (runtime)
        br.ReadUInt32(); // 0x0C  Empty

        uint carInfoOff         = br.ReadUInt32(); // 0x10
        uint collisionModelOff  = br.ReadUInt32(); // 0x14
        uint mainModelSetOff    = br.ReadUInt32(); // 0x18
        uint mainColorPatchOff  = br.ReadUInt32(); // 0x1C
        uint wheelModelSetOff   = br.ReadUInt32(); // 0x20
        uint wheelColorPatchOff = br.ReadUInt32(); // 0x24
        uint wingModelSetOff    = br.ReadUInt32(); // 0x28
        uint tire0Off           = br.ReadUInt32(); // 0x2C
        uint tire1Off           = br.ReadUInt32(); // 0x30
        uint builtinDriverOff   = br.ReadUInt32(); // 0x34
        uint texSetOff          = br.ReadUInt32(); // 0x38
        // 0x3C: padding — header ends at 0x40

        uint fileSize = (uint)(stream.Length - basePos);

        // Build the sorted set of all present section start offsets so we can
        // derive each section's byte range as [start, nextStart).
        uint[] allOffsets =
        [
            carInfoOff, collisionModelOff, mainModelSetOff, mainColorPatchOff,
            wheelModelSetOff, wheelColorPatchOff, wingModelSetOff,
            tire0Off, tire1Off, builtinDriverOff, texSetOff
        ];

        CarInfoBytes             = ReadSection(stream, basePos, carInfoOff,         allOffsets, fileSize);
        CollisionModelBytes      = ReadSection(stream, basePos, collisionModelOff,  allOffsets, fileSize);
        MainModelSetBytes        = ReadSection(stream, basePos, mainModelSetOff,    allOffsets, fileSize);
        MainModelColorPatchBytes = ReadSection(stream, basePos, mainColorPatchOff,  allOffsets, fileSize);
        WheelModelSetBytes       = ReadSection(stream, basePos, wheelModelSetOff,   allOffsets, fileSize);
        WheelColorPatchBytes     = ReadSection(stream, basePos, wheelColorPatchOff, allOffsets, fileSize);
        WingModelSetBytes        = ReadSection(stream, basePos, wingModelSetOff,    allOffsets, fileSize);
        TireModelSet0Bytes       = ReadSection(stream, basePos, tire0Off,           allOffsets, fileSize);
        TireModelSet1Bytes       = ReadSection(stream, basePos, tire1Off,           allOffsets, fileSize);
        BuiltinDriverModelBytes  = ReadSection(stream, basePos, builtinDriverOff,   allOffsets, fileSize);
        TexSetBytes              = ReadSection(stream, basePos, texSetOff,          allOffsets, fileSize);
    }

    /// <summary>
    /// Reads the bytes for one section.  The range is [sectionOffset, nextLargerOffset).
    /// Trailing alignment padding between sections is included in the extracted bytes,
    /// ensuring that writing the bytes back produces an identical file layout.
    /// </summary>
    private static byte[] ReadSection(Stream stream, long basePos,
                                       uint sectionOffset, uint[] allOffsets, uint fileSize)
    {
        if (sectionOffset == 0)
            return null;

        // Next section start = smallest offset that is strictly greater than this one
        uint nextOffset = allOffsets
            .Where(o => o > sectionOffset)
            .DefaultIfEmpty(fileSize)   // fall back to end-of-file
            .Min();

        int size = (int)(nextOffset - sectionOffset);
        if (size <= 0)
            return null;

        stream.Position = basePos + sectionOffset;
        byte[] data = new byte[size];
        int read = 0;
        while (read < size)
        {
            int n = stream.Read(data, read, size - read);
            if (n == 0)
                break;
            read += n;
        }
        return data;
    }

    // ── Writing ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Serialises the CAR4 to <paramref name="stream"/>.
    /// Sub-sections are written sequentially starting at offset 0x40; the 0x40-byte
    /// header is written last (so offsets can be determined during the write pass).
    ///
    /// Sections are aligned to 0x40 bytes.  When sub-sections were extracted from an
    /// original file (and therefore already contain trailing alignment padding), the
    /// alignment step is a no-op and the output is bit-identical to the source.
    /// When sections are freshly built (no trailing padding), alignment bytes are
    /// inserted automatically.
    /// </summary>
    public void Write(Stream stream)
    {
        long basePos = stream.Position;
        stream.Position = basePos + HeaderSize; // leave header space

        uint carInfoOff         = WriteSectionAligned(stream, CarInfoBytes,             basePos, 0x80);
        uint collisionModelOff  = WriteSectionAligned(stream, CollisionModelBytes,      basePos, 0x40);
        uint mainModelSetOff    = WriteSectionAligned(stream, MainModelSetBytes,        basePos, 0x40);
        uint mainColorPatchOff  = WriteSectionAligned(stream, MainModelColorPatchBytes, basePos, 0x40);
        uint wheelModelSetOff   = WriteSectionAligned(stream, WheelModelSetBytes,       basePos, 0x40);
        uint wheelColorPatchOff = WriteSectionAligned(stream, WheelColorPatchBytes,     basePos, 0x40);
        uint wingModelSetOff    = WriteSectionAligned(stream, WingModelSetBytes,        basePos, 0x40);
        uint tire0Off           = WriteSectionAligned(stream, TireModelSet0Bytes,       basePos, 0x40);
        uint tire1Off           = WriteSectionAligned(stream, TireModelSet1Bytes,       basePos, 0x40);
        uint builtinDriverOff   = WriteSectionAligned(stream, BuiltinDriverModelBytes,  basePos, 0x40);
        uint texSetOff          = WriteSectionAligned(stream, TexSetBytes,              basePos, 0x40);

        long endPos = stream.Position;

        // Write the 0x40-byte header
        stream.Position = basePos;
        using var bw = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        bw.Write(MAGIC);              // 0x00  "CAR4"
        bw.Write(0u);                 // 0x04  RelocatePtr        (runtime fills)
        bw.Write((uint)endPos);       // 0x08  FileSizeForReloc
        bw.Write(0u);                 // 0x0C  Empty
        bw.Write(carInfoOff);         // 0x10
        bw.Write(collisionModelOff);  // 0x14
        bw.Write(mainModelSetOff);    // 0x18
        bw.Write(mainColorPatchOff);  // 0x1C
        bw.Write(wheelModelSetOff);   // 0x20
        bw.Write(wheelColorPatchOff); // 0x24
        bw.Write(wingModelSetOff);    // 0x28
        bw.Write(tire0Off);           // 0x2C
        bw.Write(tire1Off);           // 0x30
        bw.Write(builtinDriverOff);   // 0x34
        bw.Write(texSetOff);          // 0x38
        bw.Write(0u);                 // 0x3C  padding → header ends at 0x40

        stream.Position = endPos;
    }

    /// <summary>
    /// Aligns the stream position to <paramref name="alignment"/>, then writes
    /// <paramref name="data"/> and returns the section's absolute offset
    /// (relative to <paramref name="basePos"/>), or 0 if data is null/empty.
    /// </summary>
    private static uint WriteSectionAligned(Stream stream, byte[] data, long basePos, int alignment)
    {
        if (data == null || data.Length == 0)
            return 0;

        AlignStream(stream, alignment);
        uint offset = (uint)(stream.Position - basePos);
        stream.Write(data);
        return offset;
    }

    private static void AlignStream(Stream stream, int alignment)
    {
        long pos     = stream.Position;
        long aligned = (pos + alignment - 1) & ~(long)(alignment - 1);
        long padding = aligned - pos;
        if (padding > 0)
            stream.Write(new byte[padding]);
    }

    // ── On-demand parsed helpers ─────────────────────────────────────────────

    /// <summary>Parses and returns the main body ModelSet2, or null if absent.</summary>
    public ModelSet2 GetMainModelSet()  => ParseModelSet2(MainModelSetBytes);

    /// <summary>Parses and returns the wheel ModelSet2, or null if absent.</summary>
    public ModelSet2 GetWheelModelSet() => ParseModelSet2(WheelModelSetBytes);

    /// <summary>Parses and returns the wing ModelSet2, or null if absent.</summary>
    public ModelSet2 GetWingModelSet()  => ParseModelSet2(WingModelSetBytes);

    /// <summary>Parses and returns tire ModelSet2 slot 0, or null if absent.</summary>
    public ModelSet2 GetTireModelSet0() => ParseModelSet2(TireModelSet0Bytes);

    /// <summary>Parses and returns tire ModelSet2 slot 1, or null if absent.</summary>
    public ModelSet2 GetTireModelSet1() => ParseModelSet2(TireModelSet1Bytes);

    private static ModelSet2 ParseModelSet2(byte[] data)
    {
        if (data == null || data.Length < 8)
            return null;

        try
        {
            using var ms = new MemoryStream(data);
            var modelSet = new ModelSet2();
            modelSet.FromStream(ms);
            return modelSet;
        }
        catch
        {
            return null;
        }
    }
}
