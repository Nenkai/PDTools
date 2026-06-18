using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.IO;

using PDTools.Utils;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.PS2.Runway;

public class RunwayData
{
    // ── File magic ───────────────────────────────────────────────────────────

    /// <summary>'4WNR' big-endian magic.</summary>
    public const uint MAGIC_BE = 0x524E5734u;

    /// <summary>'RNW4' little-endian magic.</summary>
    public const uint MAGIC_LE = 0x34574E52u;

    // ── Header fields ────────────────────────────────────────────────────────

    public uint Magic { get; set; }

    /// <summary>
    /// Value stored at header offset 0x08 (RelocSize / relocation data size).
    /// Preserved here so the builder can write back the original value rather than
    /// the actual computed file length.  0 = use computed size (safe default).
    /// </summary>
    public uint OriginalRelocSize { get; set; }

    /// <summary>Version word from offset 0x10 (high 16 = major, low 16 = minor).</summary>
    public uint Version { get; set; }
    public ushort VersionMajor => (ushort)(Version >> 16);
    public ushort VersionMinor => (ushort)(Version & 0xFFFF);

    public uint Flags { get; set; }

    /// <summary>Total track length in metres (the VCoord at wrap).</summary>
    public float TrackV { get; set; }

    public float StartVCoord { get; set; }
    public float GoalVCoord  { get; set; }

    /// <summary>World-space AABB: [0] = min, [1] = max.</summary>
    public Vector3[] Bounds { get; set; } = new Vector3[2];

    // ── Counts stored verbatim from the header ───────────────────────────────
    public short CheckpointListCount { get; set; }   // 0x38
    public short UnkCount            { get; set; }   // 0x3A
    public int   Unk0x40             { get; set; }   // 0x40 (int32 after the first four counts)
    public short GadgetsCount        { get; set; }   // 0x44

    /// <summary>BSP tree depth (number of levels including root).</summary>
    public byte TreeMaxDepth { get; set; }

    // ── Opaque header blobs ──────────────────────────────────────────────────

    /// <summary>19 raw bytes from header offset 0x4D to 0x5F (unknown count / flag fields).</summary>
    public byte[] UnknownHeader0x4D { get; set; }

    /// <summary>
    /// 60 raw bytes from header offset 0x84 to 0xBF.
    /// The first four int32 words are absolute file offsets into the trailing-data region.
    /// These are patched on write so they reflect the new layout; the stored copy always
    /// contains the values as read from the source file.
    /// </summary>
    public byte[] ExtraHeaderBytes { get; set; }

    // ── Spawn positions ──────────────────────────────────────────────────────

    /// <summary>
    /// Six starting-grid positions at file offset 0xC0 (6 × 0x10 bytes).
    /// Rotation is in radians; to reverse the course, add π (≈ 3.14159) to each angle.
    /// </summary>
    public List<RunwayStartingPosition> SpawnPositions { get; set; } = [];

    // ── Main data lists ──────────────────────────────────────────────────────

    public List<RunwayCheckpoint> Checkpoints            { get; set; } = [];
    public List<short>            CheckpointLookupIndices { get; set; } = [];
    public List<RunwayRoadVert>   Vertices               { get; set; } = [];
    public List<RunwayRoadTri>    Tris                   { get; set; } = [];
    public List<RunwayCluster>    Clusters               { get; set; } = [];

    // ── Opaque section blobs (raw bytes including any trailing padding) ───────

    public byte[] LightSetsRawBytes  { get; set; }
    public byte[] UnkOffset3RawBytes { get; set; }
    public byte[] GadgetsRawBytes    { get; set; }

    /// <summary>
    /// Raw bytes that follow the cluster tri-index arrays, referenced by the offset
    /// words in <see cref="ExtraHeaderBytes"/>.  Contains physics/gadget/lighting data
    /// whose internal structure is unknown; it is preserved verbatim on write.
    /// This section is the cause of wall-collision failure when absent.
    /// </summary>
    public byte[] TrailingRawBytes { get; set; }

    // ── BSP tree ─────────────────────────────────────────────────────────────

    public Node Root { get; set; }

    // ── Original-offset tracking (set by FromStream, used by Write) ──────────
    // These let Write() detect which blob sections shared the same file position
    // in the source file (so they're written only once) and correctly relocate the
    // trailing data referenced by ExtraHeaderBytes.

    private int _originalLightSetsOffset;
    private int _originalUnkOffset3;
    private int _originalGadgetsOffset;
    private int _originalTrailingDataOffset;  // absolute from file base; 0 = absent

    // When a blob section's offset equals roadVertsOffset in the source file the
    // blob is absent (the game just finds road verts there).  Track these flags so
    // Write() can reproduce the same header layout without mis-reading road-vert
    // bytes as blob data.
    private bool _lightSetsSharesWithRoadVerts;
    private bool _unk3SharesWithRoadVerts;
    private bool _gadgetsSharesWithRoadVerts;

    // ── Reading ──────────────────────────────────────────────────────────────

    public RunwayData FromStream(Stream stream)
    {
        BinaryStream bs = new BinaryStream(stream);
        long basePos = bs.Position;

        Magic = bs.ReadUInt32(); // 0x00

        if (Magic == MAGIC_BE)
            bs.ByteConverter = ByteConverter.Big;
        else if (Magic == MAGIC_LE)
            bs.ByteConverter = ByteConverter.Little;
        else
            throw new InvalidDataException($"Not a RNW4 file (magic 0x{Magic:X8}).");

        bs.ReadInt32();                                      // 0x04  RelocPtr (runtime)
        OriginalRelocSize = bs.ReadUInt32();                 // 0x08  RelocSize (preserve for exact round-trip)
        Flags   = bs.ReadUInt32();                           // 0x0C
        Version = bs.ReadUInt32();                           // 0x10
        TrackV       = bs.ReadSingle();                      // 0x14
        StartVCoord  = bs.ReadSingle();                      // 0x18
        GoalVCoord   = bs.ReadSingle();                      // 0x1C

        Bounds = new Vector3[]
        {
            new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle()), // 0x20
            new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle()), // 0x2C
        };

        // ── Counts @ 0x38 ─────────────────────────────────────────────────
        CheckpointListCount                 = bs.ReadInt16(); // 0x38
        UnkCount                            = bs.ReadInt16(); // 0x3A
        short checkpointCount               = bs.ReadInt16(); // 0x3C
        short checkpointLookupIndicesCount  = bs.ReadInt16(); // 0x3E
        Unk0x40                             = bs.ReadInt32(); // 0x40
        GadgetsCount                        = bs.ReadInt16();  // 0x44
        ushort roadVerticesCount            = bs.ReadUInt16(); // 0x46 — ushort: Suzuka has >32767 verts
        ushort roadTriCount                 = bs.ReadUInt16(); // 0x48 — ushort: Suzuka has >32767 tris
        ushort clusterCount                 = bs.ReadUInt16(); // 0x4A
        TreeMaxDepth                        = bs.Read1Byte(); // 0x4C

        // 0x4D – 0x5F: 19 unknown bytes
        UnknownHeader0x4D = bs.ReadBytes(0x13);

        // ── Offset table @ 0x60 ───────────────────────────────────────────
        bs.Position = basePos + 0x60;
        int checkpointsOffset            = bs.ReadInt32(); // 0x60
        int checkpointLookupOffset       = bs.ReadInt32(); // 0x64
        int lightSetsOffset              = bs.ReadInt32(); // 0x68
        int unkOffset3                   = bs.ReadInt32(); // 0x6C
        int gadgetsOffset                = bs.ReadInt32(); // 0x70
        int roadVertsOffset              = bs.ReadInt32(); // 0x74
        int roadTrisOffset               = bs.ReadInt32(); // 0x78
        int clustersOffset               = bs.ReadInt32(); // 0x7C
        int traversalDataOffset          = bs.ReadInt32(); // 0x80

        // Store for use in Write() (duplicate-section detection)
        _originalLightSetsOffset = lightSetsOffset;
        _originalUnkOffset3      = unkOffset3;
        _originalGadgetsOffset   = gadgetsOffset;

        // 0x84 – 0xBF: 60 bytes of extra header (4 offset words + constant words)
        ExtraHeaderBytes = bs.ReadBytes(0x3C);

        // Extract the first non-zero trailing-data pointer from ExtraHeaderBytes
        // (bytes 0–3 of the array = file offset 0x84).
        _originalTrailingDataOffset = 0;
        if (ExtraHeaderBytes.Length >= 4)
        {
            _originalTrailingDataOffset = Magic == MAGIC_BE
                ? (ExtraHeaderBytes[0] << 24) | (ExtraHeaderBytes[1] << 16) |
                  (ExtraHeaderBytes[2] <<  8) |  ExtraHeaderBytes[3]
                :  ExtraHeaderBytes[0]         | (ExtraHeaderBytes[1] <<  8) |
                  (ExtraHeaderBytes[2] << 16)  | (ExtraHeaderBytes[3] << 24);
        }

        // ── Spawn positions @ 0xC0 (count derived from distance to checkpoints) ──
        // The spawn block always starts at 0xC0 and ends where checkpoints begin.
        // Most tracks have 6 entries; some have more (e.g. 10).
        bs.Position = basePos + 0xC0;
        int spawnCount = (checkpointsOffset - 0xC0) / RunwayStartingPosition.GetSize();
        for (int i = 0; i < spawnCount; i++)
            SpawnPositions.Add(RunwayStartingPosition.FromStream(bs));

        // ── Section helpers ────────────────────────────────────────────────
        int fileSize = (int)(stream.Length - basePos);

        // Use Distinct so that sections sharing the same offset only appear once;
        // this ensures SectionEnd() returns the correct next boundary.
        int[] allOffsets = new[]
        {
            checkpointsOffset, checkpointLookupOffset,
            lightSetsOffset, unkOffset3, gadgetsOffset,
            roadVertsOffset, roadTrisOffset,
            clustersOffset, traversalDataOffset
        }.Where(o => o > 0).Distinct().OrderBy(o => o).ToArray();

        int SectionEnd(int off)
        {
            foreach (int o in allOffsets)
                if (o > off) return o;
            return fileSize;
        }

        // ── Checkpoints ───────────────────────────────────────────────────
        for (int i = 0; i < checkpointCount; i++)
        {
            bs.Position = basePos + checkpointsOffset + i * RunwayCheckpoint.GetSize();
            Checkpoints.Add(RunwayCheckpoint.FromStream(bs));
        }

        // ── Checkpoint lookup indices ─────────────────────────────────────
        for (int i = 0; i < checkpointLookupIndicesCount; i++)
        {
            bs.Position = basePos + checkpointLookupOffset + i * sizeof(short);
            CheckpointLookupIndices.Add(bs.ReadInt16());
        }

        // ── Opaque blobs (light sets, unk3, gadgets) ──────────────────────
        // Several of these pointers may share the same file offset (common in GT4
        // tracks that have no light-set or gadget data at that position).
        // Each unique offset is read exactly once to avoid duplicating data on write.
        //
        // When a blob offset equals roadVertsOffset the blob is absent: the header
        // simply points to the start of the road-vert data as a sentinel.  Guard
        // against this to avoid reading road-vert bytes as blob data (which would
        // then be written twice in Write(), doubling the file size).

        _lightSetsSharesWithRoadVerts = lightSetsOffset > 0 && lightSetsOffset == roadVertsOffset;
        _unk3SharesWithRoadVerts      = unkOffset3     > 0 && unkOffset3     == roadVertsOffset;
        _gadgetsSharesWithRoadVerts   = gadgetsOffset  > 0 && gadgetsOffset  == roadVertsOffset;

        if (lightSetsOffset > 0 && !_lightSetsSharesWithRoadVerts)
        {
            bs.Position = basePos + lightSetsOffset;
            LightSetsRawBytes = bs.ReadBytes(SectionEnd(lightSetsOffset) - lightSetsOffset);
        }

        if (unkOffset3 > 0 && unkOffset3 != lightSetsOffset && !_unk3SharesWithRoadVerts)
        {
            bs.Position = basePos + unkOffset3;
            UnkOffset3RawBytes = bs.ReadBytes(SectionEnd(unkOffset3) - unkOffset3);
        }

        if (gadgetsOffset > 0 && gadgetsOffset != lightSetsOffset && gadgetsOffset != unkOffset3 && !_gadgetsSharesWithRoadVerts)
        {
            bs.Position = basePos + gadgetsOffset;
            GadgetsRawBytes = bs.ReadBytes(SectionEnd(gadgetsOffset) - gadgetsOffset);
        }

        // ── Road vertices ─────────────────────────────────────────────────
        for (int i = 0; i < roadVerticesCount; i++)
        {
            bs.Position = basePos + roadVertsOffset + i * RunwayRoadVert.GetSize();
            Vertices.Add(RunwayRoadVert.FromStream(bs));
        }

        // ── Road tris ─────────────────────────────────────────────────────
        for (int i = 0; i < roadTriCount; i++)
        {
            bs.Position = basePos + roadTrisOffset + i * RunwayRoadTri.GetSize();
            Tris.Add(RunwayRoadTri.FromStream(bs));
        }

        // ── BSP traversal tree ────────────────────────────────────────────
        bs.Position = basePos + traversalDataOffset;
        Root = TraverseRead(bs, TreeMaxDepth - 1);

        // ── Clusters (header + per-cluster tri-index list) ────────────────
        for (int i = 0; i < clusterCount; i++)
        {
            bs.Position = basePos + clustersOffset + i * RunwayCluster.GetSize();
            Clusters.Add(RunwayCluster.FromStream(bs));
        }

        // ── Trailing data (after cluster tri-index arrays) ────────────────
        // The four offset words in ExtraHeaderBytes (0x84–0x93) point into this
        // region, which holds physics/gadget/lighting data used for wall collision.
        // It must be read and preserved verbatim; omitting it breaks wall collision.
        if (_originalTrailingDataOffset > 0 && _originalTrailingDataOffset < fileSize)
        {
            bs.Position = basePos + _originalTrailingDataOffset;
            TrailingRawBytes = bs.ReadBytes(fileSize - _originalTrailingDataOffset);
        }

        return this;
    }

    // ── BSP tree helpers ─────────────────────────────────────────────────────

    private static Node TraverseRead(BinaryStream bs, int depthLeft)
    {
        if (depthLeft < 0)
            return null;

        var node = new Node();
        byte data  = bs.Read1Byte();
        node.Axis  = (byte)(data >> 6);
        node.Value = ((float)(data & 0b111111) + 0.5f) * 0.015625f;

        node.Left  = TraverseRead(bs, depthLeft - 1);
        node.Right = TraverseRead(bs, depthLeft - 1);
        return node;
    }

    private static void TraverseWrite(BinaryStream bs, Node node, int depthLeft)
    {
        if (depthLeft < 0)
            return;

        if (node == null)
        {
            // Write zeroed bytes for entire missing subtree (pre-order: root+left+right)
            int nodeCount = (1 << (depthLeft + 1)) - 1;
            for (int i = 0; i < nodeCount; i++)
                bs.WriteByte(0);
            return;
        }

        // Reverse of read: valueBits = round(value / 0.015625) - 0.5  clamped to [0,63]
        byte valueBits = (byte)Math.Clamp(
            (int)MathF.Round(node.Value / 0.015625f - 0.5f), 0, 63);
        bs.WriteByte((byte)((node.Axis << 6) | valueBits));

        TraverseWrite(bs, node.Left,  depthLeft - 1);
        TraverseWrite(bs, node.Right, depthLeft - 1);
    }

    // ── Writing ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Serialises the runway to <paramref name="stream"/> starting at its current position.
    /// All section offsets are recomputed from scratch so the file is self-consistent
    /// after <see cref="ReverseDirection"/> changes the checkpoint-lookup table size.
    ///
    /// Alignment:
    ///   The PS2 DMA engine requires 16-byte (quadword) alignment for the sections it
    ///   fetches directly.  The writer inserts alignment padding before:
    ///     • the opaque-blob region (light-sets / unk3 / gadgets),
    ///     • road vertices,
    ///     • the BSP traversal tree,
    ///     • the trailing-data region.
    ///   Sections that follow naturally-aligned predecessors (road tris after road verts,
    ///   clusters after BSP) need no explicit padding.
    /// </summary>
    public void Write(Stream stream)
    {
        long basePos = stream.Position;
        var  bs      = new BinaryStream(stream,
            Magic == MAGIC_BE ? ByteConverter.Big : ByteConverter.Little);

        // Local helper: write a raw blob and return its offset from basePos,
        // or 0 if the blob is absent.
        int WriteBlob(byte[] data)
        {
            if (data == null || data.Length == 0) return 0;
            int off = (int)(stream.Position - basePos);
            stream.Write(data);
            return off;
        }

        // ── Spawn positions @ 0xC0 ───────────────────────────────────────
        stream.Position = basePos + 0xC0;
        foreach (var sp in SpawnPositions)
            sp.ToStream(bs);
        // 6 × 0x10 = 0x60 bytes → stream now at basePos + 0x120

        // ── Checkpoints ──────────────────────────────────────────────────
        int checkpointsOffset = (int)(stream.Position - basePos); // = 0x120
        foreach (var cp in Checkpoints)
            cp.ToStream(bs);

        // ── Checkpoint lookup indices ─────────────────────────────────────
        int checkpointLookupOffset = (int)(stream.Position - basePos);
        foreach (short idx in CheckpointLookupIndices)
            bs.WriteInt16(idx);

        // ── 16-byte alignment before opaque blobs ─────────────────────────
        // The checkpoint-lookup table's byte count may not be a multiple of 16
        // (e.g. after ReverseDirection adds entries).  Pad to a quadword boundary
        // so that the blob section and all subsequent sections remain PS2-aligned.
        AlignStream(stream, 16);

        // ── Opaque blobs (light-sets, unk3, gadgets) ─────────────────────
        // If two or three of these pointers shared the same file offset in the
        // source file, write the blob only once and reuse the offset for the
        // others.  Writing duplicates shifts every section that follows.
        //
        // Blobs that originally pointed at roadVertsOffset (absent/sentinel) are
        // not written here; their header offset is patched to roadVertsOffset after
        // the road-vert section is placed.  Use -1 as a sentinel meaning "copy from
        // roadVertsOffset once that is known".

        const int kSharesWithRoadVerts = -1;

        int lightSetsWriteOff = _lightSetsSharesWithRoadVerts ? kSharesWithRoadVerts : WriteBlob(LightSetsRawBytes);

        int unkOffset3WriteOff;
        if (_unk3SharesWithRoadVerts)
            unkOffset3WriteOff = kSharesWithRoadVerts;
        else if (_originalUnkOffset3 > 0 && _originalUnkOffset3 == _originalLightSetsOffset)
            unkOffset3WriteOff = lightSetsWriteOff;   // shared — reuse
        else
            unkOffset3WriteOff = WriteBlob(UnkOffset3RawBytes);

        int gadgetsWriteOff;
        if (_gadgetsSharesWithRoadVerts)
            gadgetsWriteOff = kSharesWithRoadVerts;
        else if (_originalGadgetsOffset > 0 && _originalGadgetsOffset == _originalLightSetsOffset)
            gadgetsWriteOff = lightSetsWriteOff;
        else if (_originalGadgetsOffset > 0 && _originalGadgetsOffset == _originalUnkOffset3)
            gadgetsWriteOff = unkOffset3WriteOff;
        else
            gadgetsWriteOff = WriteBlob(GadgetsRawBytes);

        // ── Road vertices (16-byte aligned) ──────────────────────────────
        AlignStream(stream, 16);
        int roadVertsOffset = (int)(stream.Position - basePos);
        foreach (var v in Vertices)
            v.ToStream(bs);

        // Patch any blob offsets that were deferred because they share roadVertsOffset.
        if (lightSetsWriteOff == kSharesWithRoadVerts) lightSetsWriteOff = roadVertsOffset;
        if (unkOffset3WriteOff == kSharesWithRoadVerts) unkOffset3WriteOff = roadVertsOffset;
        if (gadgetsWriteOff   == kSharesWithRoadVerts) gadgetsWriteOff    = roadVertsOffset;

        // ── Road tris ─────────────────────────────────────────────────────
        // Each RunwayRoadVert is 0x10 bytes, so tris always follow at a
        // 16-byte-aligned address — no explicit padding needed here.
        int roadTrisOffset = (int)(stream.Position - basePos);
        foreach (var t in Tris)
            t.ToStream(bs);

        // ── BSP traversal tree (16-byte aligned, 1<<TreeMaxDepth bytes) ──
        // Node count = 2^TreeMaxDepth − 1 (pre-order traversal).
        // Total block = 1<<TreeMaxDepth bytes (nodes + 1 alignment byte).
        //   depth=10 → 0x0400 (1024)   depth=12 → 0x1000 (4096)
        AlignStream(stream, 16);
        int traversalDataOffset = (int)(stream.Position - basePos);
        TraverseWrite(bs, Root, TreeMaxDepth - 1);

        int bspTotalSize = 1 << TreeMaxDepth; // depth-generic: 0x400 for d=10, 0x1000 for d=12
        long bspWritten = stream.Position - (basePos + traversalDataOffset);
        if (bspWritten < bspTotalSize)
        {
            int pad = bspTotalSize - (int)bspWritten;
            for (int i = 0; i < pad; i++) bs.WriteByte(0);
        }

        // ── Cluster headers + tri-index arrays ───────────────────────────
        // clustersOffset = traversalDataOffset + (1<<TreeMaxDepth), always
        // a multiple of 16 since both the BSP start and its size are aligned.
        int clustersOffset  = (int)(stream.Position - basePos);
        int triDataStart    = clustersOffset + Clusters.Count * RunwayCluster.GetSize();

        // Compute the absolute file position of each cluster's tri-index array
        var triOffsets = new int[Clusters.Count];
        int triCursor  = triDataStart;
        for (int i = 0; i < Clusters.Count; i++)
        {
            triOffsets[i] = (int)(basePos + triCursor);
            triCursor     += Clusters[i].TriIndices.Length * sizeof(short);
        }

        for (int i = 0; i < Clusters.Count; i++)
            Clusters[i].WriteHeader(bs, triOffsets[i]);

        foreach (var cluster in Clusters)
            cluster.WriteTriData(bs);

        // ── Trailing data (16-byte aligned) ──────────────────────────────
        // This region holds the physics/gadget/lighting structures referenced by
        // the four pointer words in ExtraHeaderBytes (0x84–0x93).  The PS2 engine
        // uses these for wall collision, so the region must be present and correctly
        // located.  The offset words are patched below to reflect the new position.
        int trailingDelta = 0;
        if (TrailingRawBytes != null && TrailingRawBytes.Length > 0)
        {
            AlignStream(stream, 16);
            int newTrailingOffset = (int)(stream.Position - basePos);
            trailingDelta = newTrailingOffset - _originalTrailingDataOffset;
            stream.Write(TrailingRawBytes);
        }

        long endPos = stream.Position;

        // ── Header @ basePos ──────────────────────────────────────────────
        stream.Position = basePos;
        bs.WriteUInt32(Magic);                      // 0x00
        bs.WriteInt32(0);                           // 0x04  RelocPtr (runtime; always 0 on disk)
        // Always use the actual file size so the game loads the complete file including
        // trailing physics/collision data.  If the reversed file is larger than the
        // source (due to extra non-contiguous lookup entries), preserving the original
        // RelocSize would cause the game to stop reading before the trailing data.
        bs.WriteUInt32((uint)(endPos - basePos));   // 0x08  RelocSize
        bs.WriteUInt32(Flags);                      // 0x0C
        bs.WriteUInt32(Version);                    // 0x10
        bs.WriteSingle(TrackV);                     // 0x14
        bs.WriteSingle(StartVCoord);                // 0x18
        bs.WriteSingle(GoalVCoord);                 // 0x1C
        bs.WriteSingle(Bounds[0].X);                // 0x20
        bs.WriteSingle(Bounds[0].Y);
        bs.WriteSingle(Bounds[0].Z);
        bs.WriteSingle(Bounds[1].X);                // 0x2C
        bs.WriteSingle(Bounds[1].Y);
        bs.WriteSingle(Bounds[1].Z);

        // Counts @ 0x38
        bs.WriteInt16(CheckpointListCount);                       // 0x38
        bs.WriteInt16(UnkCount);                                  // 0x3A
        bs.WriteInt16((short)Checkpoints.Count);                  // 0x3C
        bs.WriteInt16((short)CheckpointLookupIndices.Count);      // 0x3E
        bs.WriteInt32(Unk0x40);                                   // 0x40
        bs.WriteInt16(GadgetsCount);                              // 0x44
        bs.WriteUInt16((ushort)Vertices.Count);                    // 0x46
        bs.WriteUInt16((ushort)Tris.Count);                       // 0x48
        bs.WriteUInt16((ushort)Clusters.Count);                   // 0x4A
        bs.WriteByte(TreeMaxDepth);                               // 0x4C

        // 0x4D–0x5F unknown
        if (UnknownHeader0x4D != null)
            stream.Write(UnknownHeader0x4D, 0, Math.Min(UnknownHeader0x4D.Length, 0x13));
        else
            stream.Write(new byte[0x13]);

        // Offset table @ 0x60
        stream.Position = basePos + 0x60;
        bs.WriteInt32(checkpointsOffset);    // 0x60
        bs.WriteInt32(checkpointLookupOffset); // 0x64
        bs.WriteInt32(lightSetsWriteOff);    // 0x68
        bs.WriteInt32(unkOffset3WriteOff);   // 0x6C
        bs.WriteInt32(gadgetsWriteOff);      // 0x70
        bs.WriteInt32(roadVertsOffset);      // 0x74
        bs.WriteInt32(roadTrisOffset);       // 0x78
        bs.WriteInt32(clustersOffset);       // 0x7C
        bs.WriteInt32(traversalDataOffset);  // 0x80

        // Extra header @ 0x84–0xBF
        // Clone the stored bytes so we can patch without mutating the source data.
        // The four pointer words (bytes 0–15 of the array = file offsets 0x84–0x93)
        // are adjusted by the net delta between where the trailing data now lands
        // versus where it was in the original file.
        byte[] patchedExtra = ExtraHeaderBytes != null
            ? (byte[])ExtraHeaderBytes.Clone()
            : new byte[0x3C];

        if (trailingDelta != 0)
            PatchExtraHeaderOffsets(patchedExtra, trailingDelta);

        stream.Write(patchedExtra, 0, Math.Min(patchedExtra.Length, 0x3C));

        stream.Position = endPos;
    }

    // ── Direction reversal ───────────────────────────────────────────────────

    /// <summary>
    /// Transforms this runway so that it runs in the opposite direction:
    ///
    ///   • Checkpoint order reversed (last→first); Left↔Right positions swapped on each gate;
    ///     V = (TrackV − oldV) mod TrackV.
    ///   • Checkpoint-pair indices in each cluster's lookup entry are remapped:
    ///     pair j → (N−2−j) for j &lt; N−1; pair (N−1) stays as (N−1) (wrap pair).
    ///   • Non-contiguous transformed pair sets are pushed into new lookup entries.
    ///   • StartVCoord/GoalVCoord are transformed by the same V formula.
    ///   • Spawn-position rotations are flipped by π radians so cars face the new direction.
    ///
    /// The Left↔Right swap is essential: QuadSTCompute performs signed-area (cross-product)
    /// tests that depend on correct winding order.  Reversing traversal direction flips the
    /// winding; swapping Left↔Right restores it, exactly equivalent to rotating each gate
    /// 180° about its Middle point.
    ///
    /// Triangle UnkBits (SectorId / CpSubIndex) are NOT modified.  Comparison against GT4's
    /// own reverse-variant files confirms Polyphony leaves them unchanged: SectorId encodes
    /// the physical mesh region, not the traversal order.
    ///
    /// Geometry (vertices, tris, clusters, BSP tree, bounds) and the collision/physics
    /// trailing data are otherwise unchanged.
    /// The ExtraHeaderBytes offset words are patched by Write() when it knows the
    /// final layout, so no adjustment is needed here.
    /// </summary>
    public void ReverseDirection()
    {
        int   N           = Checkpoints.Count;
        float totalTrackV = TrackV;

        // ── Step 1: Reverse checkpoint order, mirror V, swap Left↔Right ─────
        // Left↔Right swap is required: QuadSTCompute uses signed cross-products
        // whose winding depends on which side is "left" vs "right" relative to the
        // travel direction.  Reversing the traversal order flips the winding;
        // swapping Left↔Right corrects it.  Geometrically this is identical to
        // rotating each gate 180° about its Middle point.
        var reversed = new List<RunwayCheckpoint>(N);
        for (int i = 0; i < N; i++)
        {
            var src = Checkpoints[N - 1 - i];
            reversed.Add(new RunwayCheckpoint
            {
                Left   = src.Right,   // swap: corrects QuadSTCompute winding
                Middle = src.Middle,
                Right  = src.Left,    // swap: corrects QuadSTCompute winding
                TrackV = (totalTrackV - src.TrackV) % totalTrackV,
            });
        }
        Checkpoints = reversed;

        // ── Step 2: Transform StartVCoord / GoalVCoord ───────────────────
        StartVCoord = (totalTrackV - StartVCoord) % totalTrackV;
        GoalVCoord  = (totalTrackV - GoalVCoord)  % totalTrackV;

        // ── Step 3: Flip spawn rotations by π; transform config gate records ─
        // The spawn block begins with zero or more configuration records followed by
        // real starting-grid slots.  A slot is a config record if it appears before
        // the first slot with a genuine heading angle (0 < |Rot| ≤ 2π).
        //
        // Config records pack checkpoint-gate V-coordinates into their struct fields:
        //   |Rot| > 2π : Z and Rotation both hold gate V-coords (two gates per slot).
        //   Rot == 0   : X (and Z if nonzero) hold additional gate V-coords.
        //
        // The forward file stores gate V-coords in ascending order (traversal order)
        // across the config slots, in field sequence: slot0.Z, slot0.Rot, slot1.X, …
        // Reversal must:
        //   1. Collect all gate values from all config slots in field order.
        //   2. V-mirror each:  new_v = TrackV - old_v
        //   3. Reverse the list — so the smallest reversed-V (first gate reached in
        //      the reversed lap) goes into the first field position, preserving the
        //      ascending-V invariant that the game requires.
        //   4. Write the reordered, mirrored values back into the same field slots.
        //
        // Real spawn slots: flip heading by π.
        const float TwoPiF = MathF.PI * 2f;

        // Find the index of the first real spawn slot.
        int firstRealSpawn = SpawnPositions.Count;
        for (int i = 0; i < SpawnPositions.Count; i++)
        {
            float absRot = MathF.Abs(SpawnPositions[i].Rotation);
            if (absRot > 0f && absRot <= TwoPiF)
            {
                firstRealSpawn = i;
                break;
            }
        }

        // ── Collect all gate V-coords from config slots in field order ──────
        // Each entry: (slot index, field name, forward V-value)
        var gateFields = new List<(int slot, string field)>();
        var gateValues = new List<float>();

        for (int i = 0; i < firstRealSpawn; i++)
        {
            var sp = SpawnPositions[i];
            if (MathF.Abs(sp.Rotation) > TwoPiF)
            {
                gateFields.Add((i, "Z"));   gateValues.Add(sp.Z);
                gateFields.Add((i, "Rot")); gateValues.Add(sp.Rotation);
            }
            else // Rot == 0
            {
                if (sp.X != 0f) { gateFields.Add((i, "X")); gateValues.Add(sp.X); }
                if (sp.Z != 0f) { gateFields.Add((i, "Z")); gateValues.Add(sp.Z); }
            }
        }

        // ── V-mirror all values then reverse the list ─────────────────────
        var newGateValues = new float[gateValues.Count];
        for (int g = 0; g < gateValues.Count; g++)
            newGateValues[gateValues.Count - 1 - g] = totalTrackV - gateValues[g];

        // ── Write back in the same field positions ─────────────────────────
        for (int g = 0; g < gateFields.Count; g++)
        {
            var (idx, field) = gateFields[g];
            var sp = SpawnPositions[idx];
            switch (field)
            {
                case "Z":   sp.Z        = newGateValues[g]; break;
                case "Rot": sp.Rotation = newGateValues[g]; break;
                case "X":   sp.X        = newGateValues[g]; break;
            }
        }

        // ── Flip real spawn headings by π ─────────────────────────────────
        for (int i = firstRealSpawn; i < SpawnPositions.Count; i++)
            SpawnPositions[i].Rotation = NormaliseAngle(SpawnPositions[i].Rotation + MathF.PI);

        // ── Step 4: Remap checkpoint-pair indices in-place ───────────────────
        // Transform each lookup entry j: j < N-1 → N-2-j,  j = N-1 → N-1.
        // Cluster CheckpointLookupIndexStart / CheckpointLookupLength are left
        // unchanged — the same windows into the table cover the correct reversed
        // pairs.  No entries are added, so the file size stays identical to the
        // source.  The source file's per-cluster ordering (including N-1 first
        // in custom entries that span the start/finish boundary) is preserved,
        // which is what getVCoord requires for correct V-coordinate tie-breaking.
        for (int i = 0; i < CheckpointLookupIndices.Count; i++)
        {
            short j = CheckpointLookupIndices[i];
            CheckpointLookupIndices[i] = j < N - 1 ? (short)(N - 2 - j) : (short)(N - 1);
        }

        // Triangle UnkBits (SectorId / CpSubIndex) are intentionally NOT modified.
        // Empirical comparison against GT4's own reverse-variant files confirms that
        // Polyphony leaves the triangle sector IDs unchanged when reversing a track:
        // the SectorId encodes the physical timing-sector region on the mesh, not
        // the traversal order.  Inverting them shifts the T1/T2/T3 boundary by one
        // checkpoint in the blueprint sector-map and does not match the originals.
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private static float NormaliseAngle(float rad)
    {
        const float TwoPi = MathF.PI * 2f;
        rad %= TwoPi;
        if (rad >  MathF.PI) rad -= TwoPi;
        if (rad <= -MathF.PI) rad += TwoPi;
        return rad;
    }

    /// <summary>
    /// Adjusts offset words in a clone of <see cref="ExtraHeaderBytes"/> by
    /// <paramref name="delta"/> bytes.  Only words whose stored value is
    /// ≥ <see cref="_originalTrailingDataOffset"/> are touched — those are
    /// confirmed pointers into the trailing-data region.  Words that are zero
    /// (absent) or smaller than that threshold are counts/constants (e.g. the
    /// 3751-entry count at ExtraHdr[1] in 20r60r) and must not be modified.
    /// Works in-place on <paramref name="bytes"/>; call with a clone to avoid
    /// mutating the stored source data.
    /// </summary>
    private void PatchExtraHeaderOffsets(byte[] bytes, int delta)
    {
        bool bigEndian = (Magic == MAGIC_BE);
        for (int i = 0; i < 4; i++)
        {
            int bytePos = i * 4;
            if (bytePos + 4 > bytes.Length) break;

            int val = bigEndian
                ? (bytes[bytePos    ] << 24) | (bytes[bytePos + 1] << 16) |
                  (bytes[bytePos + 2] <<  8) |  bytes[bytePos + 3]
                :  bytes[bytePos    ]         | (bytes[bytePos + 1] <<  8) |
                  (bytes[bytePos + 2] << 16)  | (bytes[bytePos + 3] << 24);

            // Skip zero (absent) and any word smaller than the start of the
            // trailing-data region — those are counts/constants, not pointers.
            if (val == 0 || val < _originalTrailingDataOffset) continue;

            val += delta;

            if (bigEndian)
            {
                bytes[bytePos    ] = (byte)(val >> 24);
                bytes[bytePos + 1] = (byte)(val >> 16);
                bytes[bytePos + 2] = (byte)(val >>  8);
                bytes[bytePos + 3] = (byte) val;
            }
            else
            {
                bytes[bytePos    ] = (byte) val;
                bytes[bytePos + 1] = (byte)(val >>  8);
                bytes[bytePos + 2] = (byte)(val >> 16);
                bytes[bytePos + 3] = (byte)(val >> 24);
            }
        }
    }

    private static void AlignStream(Stream stream, int alignment)
    {
        long pos     = stream.Position;
        long aligned = (pos + alignment - 1) & ~(long)(alignment - 1);
        long pad     = aligned - pos;
        if (pad > 0)
        {
            Span<byte> zeros = stackalloc byte[(int)pad];
            zeros.Clear();
            stream.Write(zeros);
        }
    }

    // ── Blueprint helpers ────────────────────────────────────────────────────

    /// <summary>
    /// Whether <see cref="UnkOffset3RawBytes"/> shares the same file offset as
    /// <see cref="LightSetsRawBytes"/> in the source file.  Used by the blueprint
    /// exporter to generate the correct sharing flags in blueprint.yaml.
    /// </summary>
    public bool Unk3SharesOffsetWithLightsets =>
        _originalUnkOffset3 > 0 && _originalUnkOffset3 == _originalLightSetsOffset;

    /// <summary>
    /// Whether <see cref="GadgetsRawBytes"/> shares the same file offset as
    /// <see cref="LightSetsRawBytes"/> in the source file.
    /// </summary>
    public bool GadgetsSharesOffsetWithLightsets =>
        _originalGadgetsOffset > 0 && _originalGadgetsOffset == _originalLightSetsOffset;

    /// <summary>
    /// Whether <see cref="GadgetsRawBytes"/> shares the same file offset as
    /// <see cref="UnkOffset3RawBytes"/> (and not with lightsets) in the source file.
    /// </summary>
    public bool GadgetsSharesOffsetWithUnk3 =>
        _originalGadgetsOffset > 0 &&
        _originalGadgetsOffset == _originalUnkOffset3 &&
        _originalGadgetsOffset != _originalLightSetsOffset;

    /// <summary>
    /// Configures original-offset tracking for blueprint round-trips.
    /// Must be called after building a <see cref="RunwayData"/> from a blueprint
    /// (instead of <see cref="FromStream"/>) so that <see cref="Write"/> can
    /// correctly deduplicate blob sections and patch ExtraHeaderBytes offset words.
    /// </summary>
    /// <param name="lightSetsOffset">Fake original offset for lightsets (nonzero = present).</param>
    /// <param name="unkOffset3">Same value as lightSetsOffset if shared, otherwise distinct nonzero.</param>
    /// <param name="gadgetsOffset">Same as lightsets/unk3 if shared, otherwise distinct nonzero.</param>
    /// <param name="trailingDataOffset">
    ///   Value of the trailing-data pointer as it appears in ExtraHeaderBytes[0:4].
    ///   Used by <see cref="PatchExtraHeaderOffsets"/> to distinguish pointer words
    ///   from count words (any word &lt; this threshold is treated as a count).
    /// </param>
    public void SetOriginalOffsets(
        int lightSetsOffset, int unkOffset3, int gadgetsOffset, int trailingDataOffset)
    {
        _originalLightSetsOffset    = lightSetsOffset;
        _originalUnkOffset3         = unkOffset3;
        _originalGadgetsOffset      = gadgetsOffset;
        _originalTrailingDataOffset = trailingDataOffset;
    }

    /// <summary>
    /// Returns raw bytes of the BSP traversal tree written in pre-order.
    /// The length equals <c>2^TreeMaxDepth − 1</c> bytes (one byte per node).
    /// Used by the blueprint exporter to produce bsp_tree.bin.
    /// </summary>
    public byte[] SaveBspToBytes()
    {
        using var ms = new MemoryStream();
        var bs = new BinaryStream(ms,
            Magic == MAGIC_BE ? ByteConverter.Big : ByteConverter.Little);
        TraverseWrite(bs, Root, TreeMaxDepth - 1);
        return ms.ToArray();
    }

    /// <summary>
    /// Loads the BSP traversal tree from raw bytes produced by <see cref="SaveBspToBytes"/>.
    /// <see cref="TreeMaxDepth"/> must be set before calling.
    /// Used by the blueprint builder to restore bsp_tree.bin.
    /// </summary>
    public void LoadBspFromBytes(byte[] bspBytes)
    {
        using var ms = new MemoryStream(bspBytes);
        var bs = new BinaryStream(ms,
            Magic == MAGIC_BE ? ByteConverter.Big : ByteConverter.Little);
        Root = TraverseRead(bs, TreeMaxDepth - 1);
    }

    // ── Collision / track-position query methods ─────────────────────────────

    /// <summary>
    /// Searches the runway for a possible collision at the provided coordinates (Y will be within bounds).
    /// </summary>
    // GT4O US 0x293F90
    public bool search(out RunwayResult result, Vector3 pos)
    {
        if (pos.Y >= Bounds[0].Y)
        {
            Vector3 startPoint = new Vector3(
                pos.X,
                Bounds[1].Y >= pos.Y ? pos.Y : Bounds[1].Y,
                pos.Z);

            Vector3 endPoint = new Vector3(pos.X, Bounds[0].Y, pos.Z);

            int depth = TreeMaxDepth - 1;
            return traverse(out result, Bounds, startPoint, endPoint, Root, depth, 0);
        }

        result = null;
        return false;
    }

    // GT4O US 0x294128
    public bool traverse(out RunwayResult result, Span<Vector3> bounds,
                         Vector3 startPoint, Vector3 endPoint,
                         Node node, int depth, short clusterIndex)
    {
        if (depth < 0)
            return checkHit(out result, startPoint, endPoint, clusterIndex);

        int   axis          = node.Axis;
        float axisBoundsMin = bounds[0].GetAxis(axis);
        float axisBoundsMax = bounds[1].GetAxis(axis);

        float axisV1 = startPoint.GetAxis(axis);
        float axisV2 = endPoint.GetAxis(axis);

        float pos  = MathUtils.Lerp(axisBoundsMin, axisBoundsMax, node.Value);
        float v20  = axisV1 - pos;

        Node nextNode;
        clusterIndex *= 2;

        Span<Vector3> nextBounds = stackalloc Vector3[2];
        if ((axisV1 - pos) * (axisV2 - pos) >= 0.0f)
        {
            nextBounds[0] = bounds[0];
            nextBounds[1] = bounds[1];

            if (v20 < 0.0f)
            {
                nextNode = node.Left;
                depth--;
                nextBounds[1].SetAxis(axis, pos);
            }
            else
            {
                nextNode = node.Right;
                depth--;
                nextBounds[0].SetAxis(axis, pos);
                clusterIndex++;
            }
        }
        else
        {
            throw new NotImplementedException("Reverse/Implement this part");
        }

        return traverse(out result, nextBounds, startPoint, endPoint, nextNode, depth, clusterIndex);
    }

    // GT4O US 0x294508
    public bool checkHit(out RunwayResult result, Vector3 startPoint, Vector3 endPoint, short clusterIndex)
    {
        RunwayCluster cluster = Clusters[clusterIndex];
        Vector3 vecDiff = endPoint - startPoint;

        Vector3 closest = Vector3.Zero;
        short   resTri  = -1;
        float   v20     = float.NaN;
        float   val1 = 0, val2 = 0, val3 = 0;

        for (int i = 0; i < cluster.TriIndices.Length; i++)
        {
            short          currentTriIndex = cluster.TriIndices[i];
            RunwayRoadTri  tri             = Tris[currentTriIndex];
            RunwayRoadVert p1 = Vertices[tri.Vert1];
            RunwayRoadVert p2 = Vertices[tri.Vert2];
            RunwayRoadVert p3 = Vertices[tri.Vert3];

            Vector3 a = p1.Vertex - startPoint;
            Vector3 b = p2.Vertex - startPoint;
            Vector3 c = p3.Vertex - startPoint;

            Vector3 crossed = Vector3.Cross(vecDiff, a);
            float   dot1    = Vector3.Dot(b, crossed);

            if (dot1 <= 0.0f)
            {
                float dot2 = Vector3.Dot(c, crossed);
                if (0.0f <= dot2)
                {
                    crossed = Vector3.Cross(b, c);
                    float dot3 = Vector3.Dot(vecDiff, crossed);
                    if (dot3 <= 0.0f)
                    {
                        b = p2.Vertex - p1.Vertex;
                        c = p3.Vertex - p1.Vertex;

                        crossed = Vector3.Cross(b, c);
                        float dot4 = Vector3.Dot(crossed, vecDiff);

                        float val = 0.0f;
                        if (dot4 != 0.0f)
                        {
                            var last = Vector3.Dot(crossed, a);
                            if (0.0f < last || last < dot4) continue;
                            val = last / dot4;
                        }

                        if (float.IsNaN(v20) || val <= v20)
                        {
                            closest = crossed;
                            resTri  = currentTriIndex;
                            v20     = val;
                            val1    = dot1;
                            val2    = -dot2;
                            val3    = dot3;
                        }
                    }
                }
            }
        }

        if (resTri >= 0)
        {
            result = new RunwayResult();
            result.HitPoint = new Vector3(
                (endPoint.X - startPoint.X) * v20 + startPoint.X,
                (endPoint.Y - startPoint.Y) * v20 + startPoint.Y,
                (endPoint.Z - startPoint.Z) * v20 + startPoint.Z);
            result.Cluster  = clusterIndex;
            result.TriIndex = resTri;

            var            tri = Tris[resTri];
            RunwayRoadVert p1  = Vertices[tri.Vert1];
            RunwayRoadVert p2  = Vertices[tri.Vert2];
            RunwayRoadVert p3  = Vertices[tri.Vert3];

            Vector3 adjusted = result.HitPoint;
            for (int axis = 0; axis < 3; axis++)
            {
                float f1 = p1.Vertex.GetAxis(axis);
                float f2 = p2.Vertex.GetAxis(axis);
                float f3 = p3.Vertex.GetAxis(axis);
                float res = result.HitPoint.GetAxis(axis);
                float min, max;
                if (f1 >= f2) { max = MathF.Max(f1, f3); min = MathF.Min(f2, f3); }
                else          { max = MathF.Max(f2, f3); min = MathF.Min(f1, f3); }
                adjusted.SetAxis(axis, MathF.Max(min, Math.Min(max, res)));
            }
            result.HitPoint = adjusted;

            float combined = val1 + val2 + val3;
            float rsqrt    = 1.0f / closest.Length();

            float v = (float)p1.Unk2 / 255.0f;
            v += (((float)p2.Unk2 / 255.0f - v) * val2 + ((float)p3.Unk2 / 255.0f - v) * val1) / combined;
            result.Unk = Math.Clamp(v, 0.0f, 1.0f);

            result.UnkVec = closest * rsqrt;

            v = (float)(p1.Unk3 & 0x7F) / 127.0f;
            v += (((float)(p2.Unk3 & 0x7F) / 127.0f - v) * val2 + ((float)(p3.Unk3 & 0x7F) / 127.0f - v) * val1) / combined;
            result.Unk2 = Math.Clamp(v, 0.0f, 1.0f);
            result.Unk3 = v20;
            result.Unk4 = -Vector3.Dot(result.UnkVec, p1.Vertex);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>Gets the VCoord (metres from start) for a world position.</summary>
    public float getVCoord(Vector3 position, RunwayHint hint)
    {
        if ((hint.Cluster & 0x8000) == -1 || hint.TriIndex == -1)
        {
            search(out RunwayResult result, position);
            if (result.Cluster == -1 || result.TriIndex == -1) return 0.0f;
            hint.TriIndex = result.TriIndex;
            hint.Cluster  = result.Cluster;
        }

        float lowest = float.NaN;
        float vcoord = 0.0f;

        RunwayCluster cluster = Clusters[hint.Cluster];
        for (int i = cluster.CheckpointLookupIndexStart;
                 i < cluster.CheckpointLookupIndexStart + cluster.CheckpointLookupLength; i++)
        {
            var index  = CheckpointLookupIndices[i];
            RunwayCheckpoint cp     = Checkpoints[index];
            RunwayCheckpoint nextcp = index + 1 != Checkpoints.Count
                                     ? Checkpoints[index + 1] : Checkpoints[0];

            if (QuadSTCompute(out Vector3 result, position, cp.Left,   nextcp.Left,   cp.Middle, nextcp.Middle) ||
                QuadSTCompute(out result,          position, cp.Middle, nextcp.Middle, cp.Right,  nextcp.Right))
            {
                float current = MathF.Abs(result.Z);
                if (float.IsNaN(lowest) || current < lowest)
                {
                    vcoord = cp.TrackV;
                    float nextV = nextcp.TrackV;
                    if (nextcp.TrackV < cp.TrackV) nextV = this.TrackV;
                    vcoord += (nextV - vcoord) * result.X;
                    lowest  = current;
                    if (vcoord > this.TrackV) vcoord -= this.TrackV;
                }
            }
        }

        return vcoord;
    }

    public static bool QuadSTCompute(out Vector3 result, Vector3 pos,
                                     Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        float p3p1XDiff = p3.X - p1.X;
        float p3p1ZDiff = p3.Z - p1.Z;
        float p2p4XDiff = p2.X - p4.X;
        float p2p4ZDiff = p2.Z - p4.Z;

        result = default;
        if (p3p1XDiff == 0.0 && p3p1ZDiff == 0.0 && p2p4XDiff == 0.0 && p2p4ZDiff == 0.0f)
            return false;

        float posP1XDiff = pos.X - p1.X;
        float posP1ZDiff = pos.Z - p1.Z;

        float unk = posP1ZDiff * p3p1XDiff - posP1XDiff * p3p1ZDiff;
        if (unk <= 0.0f)
        {
            float p2p1XDiff = p2.X - p1.X;
            float p2p1ZDiff = p2.Z - p1.Z;

            float unk2 = posP1ZDiff * p2p1XDiff - posP1XDiff * p2p1ZDiff;
            if (0.0f <= unk2)
            {
                float posP4XDiff = pos.X - p4.X;
                float posP4ZDiff = pos.Z - p4.Z;

                float unk3 = posP4ZDiff * p2p4XDiff - posP4XDiff * p2p4ZDiff;
                if (unk3 <= 0.0f)
                {
                    float p3p4XDiff = p3.X - p4.X;
                    float p3p4ZDiff = p3.Z - p4.Z;

                    float unk4 = posP4ZDiff * p3p4XDiff - posP4XDiff * p3p4ZDiff;
                    if (0.0f <= unk4)
                    {
                        float v1 = -p3p4XDiff - p2p1XDiff;
                        float v2 = -p3p4ZDiff - p2p1ZDiff;

                        float aa = posP1ZDiff * v1 - posP1XDiff * v2;

                        if (p3p1XDiff == 0.0f && p3p1ZDiff == 0.0f)
                        {
                            result.X = -unk2 / aa;
                            result.Y = aa / (p2p1ZDiff * v1 - p2p1XDiff - v2);
                        }
                        else
                        {
                            float unk5 = p2p1ZDiff * p3p1XDiff - p2p1XDiff * p3p1ZDiff;
                            float unk6 = aa + unk5;
                            float unk7 = MathF.Sqrt(unk6 * unk6 + unk2 * 4.0f * (p3p1ZDiff * v1 - p3p1XDiff - v2));
                            result.X = (unk + unk) / ((unk5 - aa) - unk7);
                            result.Y = (unk2 * -2.0f) / (unk6 - unk7);
                        }

                        result.Z = pos.Y - ((1.0f - result.X) * (1.0f - result.Y) * p1.Y
                                          + result.X * (1.0f - result.Y) * p2.Y
                                          + (1.0f - result.X) * result.Y * p3.Y
                                          + result.X * result.Y * p4.Y);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    // ── Nested types ─────────────────────────────────────────────────────────

    public class Node
    {
        public byte  Axis;
        public float Value;
        public Node  Left;
        public Node  Right;
    }

    public class RunwayResult
    {
        public Vector3 HitPoint { get; set; }   // 0x00
        public byte    TriUnk   { get; set; }   // 0x0C
        public byte    TriUnk2  { get; set; }   // 0x10
        public float   Unk      { get; set; }   // 0x14
        public float   Unk2     { get; set; }   // 0x18
        public Vector3 UnkVec   { get; set; }   // 0x1C
        public float   Unk4     { get; set; }   // 0x28
        public float   Unk3     { get; set; }   // 0x2C
        public short   TriIndex { get; set; } = -1; // 0x30
        public short   Cluster  { get; set; } = -1; // 0x32
    }

    public struct RunwayHint
    {
        public short TriIndex;
        public short Cluster;

        public RunwayHint(short triIndex, short cluster)
        {
            TriIndex = triIndex;
            Cluster  = cluster;
        }

        public RunwayHint()
        {
            TriIndex = -1;
            Cluster  = -1;
        }
    }
}
