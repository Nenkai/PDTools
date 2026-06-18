using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.PS2.Runway;

/// <summary>
/// GT3 (PS2) runway file — magic "GTRW", little-endian.
///
/// Binary layout (all offsets relative to file start):
///   0x00        uint32  Magic = 0x57525447 ("GTRW")
///   0x04        uint32  Runtime ptr (always 0 on disk)
///   0x08        uint32  Runtime ptr (always 0 on disk)
///   0x0C        uint32  RelocSize  (file length − 16)
///   0x10        uint32  Version/flags field A
///   0x14        uint32  Version/flags field B
///   0x18        float   TrackV   – total track length in metres
///   0x1C        float   A1VCoord – V of the start/finish gate record (mirrors GateRecords a=1 entry)
///   0x20        uint16  HeaderField20  (observed = 1, meaning unclear)
///   0x22        uint16  GateRecordCount
///   0x24        uint32  GateSectionOffset (always 0x60)
///   0x28        uint32  SpawnCount
///   0x2C        uint32  SpawnSectionOffset
///   0x30–0x5F   48 raw bytes (section counts, offsets, and flags whose meaning is partially known)
///   0x60+       Gate records  (GateRecordCount × 8 bytes)
///   SpawnOff+   Spawn positions (SpawnCount × 16 bytes)
///   …           All remaining sections (checkpoint pairs, clusters, road mesh, BSP…) — opaque blob
/// </summary>
public class GT3RunwayData
{
    // ── Magic ────────────────────────────────────────────────────────────────
    public const uint MAGIC = 0x57525447u; // "GTRW"

    // ── Parsed header fields ─────────────────────────────────────────────────
    public uint Field04         { get; set; }
    public uint Field08         { get; set; }

    /// <summary>File length − 16.  Patched to the actual output length on Write().</summary>
    public uint RelocSize       { get; set; }

    public uint Field10         { get; set; }
    public uint Field14         { get; set; }

    /// <summary>Total track length in metres.</summary>
    public float TrackV         { get; set; }

    /// <summary>
    /// V-coordinate of the start/finish gate (the SectorId=1 sentinel record).
    /// Near 0 for a forward file; near TrackV for a reversed file.
    /// Updated automatically by ReverseDirection().
    /// </summary>
    public float A1VCoord       { get; set; }

    public ushort HeaderField20 { get; set; }

    /// <summary>Raw 48-byte blob covering header offsets 0x30–0x5F.</summary>
    public byte[] OpaqueHeaderTail { get; set; }

    // ── Data lists ───────────────────────────────────────────────────────────

    /// <summary>
    /// Sector/timing-gate table.  Each record carries a V-coordinate, a sector ID,
    /// and a gate flag (−1 = sentinel, 0 = timing gate, 1 = normal checkpoint).
    /// In the forward file the records are in ascending V order:
    ///   [0] SectorId=0, GateFlag=−1, V=0      — global origin sentinel
    ///   [1] SectorId=1, GateFlag=−1, V≈0      — start/finish gate sentinel
    ///   [2…N-1] SectorId=2, GateFlag=0 or 1   — main track (gates and normals)
    /// </summary>
    public List<GT3GateRecord> GateRecords { get; set; } = [];

    /// <summary>
    /// Starting-grid positions.  Same 16-byte {X,Y,Z,Rotation} layout as RNW4.
    /// GTRW does not use packed config slots — gate V-coords live in GateRecords.
    /// </summary>
    public List<RunwayStartingPosition> SpawnPositions { get; set; } = [];

    /// <summary>
    /// Raw bytes covering everything from the end of the spawn block to EOF.
    /// Includes checkpoint pairs, cluster table, road mesh, BSP tree, etc.
    /// Preserved verbatim on Write().
    /// </summary>
    public byte[] TrailingData { get; set; }

    // ── Reading ──────────────────────────────────────────────────────────────

    public GT3RunwayData FromStream(Stream stream)
    {
        BinaryStream bs = new BinaryStream(stream);
        long basePos = bs.Position;

        // ── Header ──────────────────────────────────────────────────────────
        uint magic = bs.ReadUInt32();                    // 0x00
        if (magic != MAGIC)
            throw new InvalidDataException(
                $"Not a GTRW file (magic 0x{magic:X8}; expected 0x{MAGIC:X8}).");

        Field04         = bs.ReadUInt32();               // 0x04
        Field08         = bs.ReadUInt32();               // 0x08
        RelocSize       = bs.ReadUInt32();               // 0x0C
        Field10         = bs.ReadUInt32();               // 0x10
        Field14         = bs.ReadUInt32();               // 0x14
        TrackV          = bs.ReadSingle();               // 0x18
        A1VCoord        = bs.ReadSingle();               // 0x1C
        HeaderField20   = bs.ReadUInt16();               // 0x20
        int gateCount   = bs.ReadUInt16();               // 0x22
        uint gateOff    = bs.ReadUInt32();               // 0x24 (= 0x60)
        uint spawnCount = bs.ReadUInt32();               // 0x28
        uint spawnOff   = bs.ReadUInt32();               // 0x2C

        OpaqueHeaderTail = bs.ReadBytes(0x30);           // 0x30–0x5F (48 bytes)

        // ── Gate records ─────────────────────────────────────────────────────
        bs.Position = basePos + gateOff;
        for (int i = 0; i < gateCount; i++)
            GateRecords.Add(GT3GateRecord.FromStream(bs));

        // ── Spawn positions ──────────────────────────────────────────────────
        bs.Position = basePos + spawnOff;
        for (int i = 0; i < (int)spawnCount; i++)
            SpawnPositions.Add(RunwayStartingPosition.FromStream(bs));

        // ── Trailing opaque data ─────────────────────────────────────────────
        long trailingStart = basePos + spawnOff + spawnCount * RunwayStartingPosition.GetSize();
        bs.Position = trailingStart;
        TrailingData = bs.ReadBytes((int)(stream.Length - bs.Position));

        return this;
    }

    // ── Gadget transplant ────────────────────────────────────────────────────

    /// <summary>
    /// Replaces this file's gadget section with the gadget section from a
    /// reference reversed GTRW file.  Gadgets define the physical checkpoint
    /// trigger volumes (T1/T2/T3 gate beams and start-line stanchions) that
    /// the game uses for timing detection.  The reference file must be a
    /// reversed variant of the same course so that world coordinates are valid.
    ///
    /// OpaqueHeaderTail byte indices (relative to file header offset 0x30):
    ///   [0x20] = header 0x50 = gadget_count  (updated to reference count)
    ///   [0x24] = header 0x54 = gadget_offset (unchanged — same absolute start)
    ///   [0x2C] = header 0x5C = light_vfx_ptr (shifted by gadget size delta)
    /// </summary>
    /// <summary>
    /// Replaces the gate/sector records from a reference reversed GTRW file.
    /// The reference sectordata encodes the correct reversed-track V positions for the
    /// a=1 finish-line sentinel (in the footer, not the header) and the T1/T2/T3
    /// checkpoint thresholds that match the reversed pitbox V-coordinate system.
    /// </summary>
    public void ApplyReferenceSectorData(Stream referenceStream)
    {
        referenceStream.Position = 0;
        var refBytes = new byte[referenceStream.Length];
        referenceStream.ReadExactly(refBytes, 0, refBytes.Length);

        // Gate records start at 0x60; count is at header 0x22; each record is 8 bytes.
        int refGateCount  = BitConverter.ToUInt16(refBytes, 0x22);
        float refTrackV   = BitConverter.ToSingle(refBytes, 0x18);
        const int kGateOff    = 0x60;
        const int kGateStride = 8;

        if (refGateCount <= 0 || kGateOff + refGateCount * kGateStride > refBytes.Length) return;

        GateRecords.Clear();
        for (int i = 0; i < refGateCount; i++)
        {
            int off = kGateOff + i * kGateStride;
            var rec = new GT3GateRecord
            {
                SectorId = BitConverter.ToInt16(refBytes, off),
                GateFlag = BitConverter.ToInt16(refBytes, off + 2),
                TrackV   = BitConverter.ToSingle(refBytes, off + 4),
            };
            GateRecords.Add(rec);
        }

        // Sync A1VCoord header field to the reference file's a=1 sentinel value.
        foreach (var rec in GateRecords)
            if (rec.IsSentinel && rec.SectorId == 1) { A1VCoord = rec.TrackV; break; }
    }

    /// <summary>
    /// Copies spawn grid positions (X/Z/Rotation) from a reference reversed GTRW file.
    /// The spawn grid must be on the correct side of the finish line gate; using forward
    /// spawn positions places cars on the wrong side, causing wrong-way detection and a
    /// misplaced start-line trigger.
    /// </summary>
    public void ApplyReferenceSpawns(Stream referenceStream)
    {
        referenceStream.Position = 0;
        var refBytes = new byte[referenceStream.Length];
        referenceStream.ReadExactly(refBytes, 0, refBytes.Length);

        uint refSpawnCount  = BitConverter.ToUInt32(refBytes, 0x28);
        uint refSpawnOffset = BitConverter.ToUInt32(refBytes, 0x2C);
        int  stride = RunwayStartingPosition.GetSize(); // 16

        if (refSpawnOffset == 0 || refSpawnCount == 0) return;
        if ((int)(refSpawnOffset + refSpawnCount * stride) > refBytes.Length) return;

        // Replace our SpawnPositions with those from the reference file.
        SpawnPositions.Clear();
        for (int i = 0; i < (int)refSpawnCount; i++)
        {
            int off = (int)(refSpawnOffset + i * stride);
            var sp = new RunwayStartingPosition
            {
                X        = BitConverter.ToSingle(refBytes, off),
                Y        = BitConverter.ToSingle(refBytes, off + 4),
                Z        = BitConverter.ToSingle(refBytes, off + 8),
                Rotation = BitConverter.ToSingle(refBytes, off + 12),
            };
            SpawnPositions.Add(sp);
        }
    }

    public void ApplyReferenceGadgets(Stream referenceStream)
    {
        var refBytes = new byte[referenceStream.Length];
        referenceStream.ReadExactly(refBytes, 0, refBytes.Length);

        // ── Read gadget bounds from the reference file ────────────────────
        uint refGadCount  = BitConverter.ToUInt32(refBytes, 0x50);
        uint refGadOffset = BitConverter.ToUInt32(refBytes, 0x54);
        uint refLvxOffset = BitConverter.ToUInt32(refBytes, 0x5C);
        if (refGadOffset == 0 || refLvxOffset <= refGadOffset) return;   // nothing to copy
        int refGadSize = (int)(refLvxOffset - refGadOffset);

        // ── Our current gadget bounds (from OpaqueHeaderTail) ─────────────
        uint ourGadOffset = BitConverter.ToUInt32(OpaqueHeaderTail, 0x24); // header 0x54
        uint ourLvxOffset = BitConverter.ToUInt32(OpaqueHeaderTail, 0x2C); // header 0x5C
        if (ourGadOffset == 0 || ourLvxOffset <= ourGadOffset) return;
        int ourGadSize = (int)(ourLvxOffset - ourGadOffset);

        // ── Locate sections within TrailingData ───────────────────────────
        uint spawnSectOff = (uint)(0x60u + GateRecords.Count * GT3GateRecord.GetSize());
        uint spawnEnd     = (uint)(spawnSectOff + SpawnPositions.Count * RunwayStartingPosition.GetSize());

        int tdGadStart  = (int)(ourGadOffset - spawnEnd);  // gadget start in TrailingData
        int tdAfterGads = (int)(ourLvxOffset  - spawnEnd);  // light_vfx start in TrailingData
        if (tdGadStart < 0 || tdAfterGads > TrailingData.Length) return;

        // ── Extract new gadget bytes from reference file ───────────────────
        var newGadBytes = new byte[refGadSize];
        Array.Copy(refBytes, (int)refGadOffset, newGadBytes, 0, refGadSize);

        // ── Rebuild TrailingData with new gadgets ─────────────────────────
        int sizeDelta    = refGadSize - ourGadSize;
        var newTrailing  = new byte[TrailingData.Length + sizeDelta];

        Array.Copy(TrailingData, 0, newTrailing, 0, tdGadStart);                            // before gadgets
        Array.Copy(newGadBytes, 0, newTrailing, tdGadStart, refGadSize);                    // new gadgets
        Array.Copy(TrailingData, tdAfterGads, newTrailing,                                   // light_vfx+
                   tdGadStart + refGadSize, TrailingData.Length - tdAfterGads);

        TrailingData = newTrailing;

        // ── Patch OpaqueHeaderTail fields ─────────────────────────────────
        // gadget_count  (OpaqueHeaderTail[0x20] = header 0x50)
        PatchU32(OpaqueHeaderTail, 0x20, refGadCount);

        // light_vfx_ptr (OpaqueHeaderTail[0x2C] = header 0x5C)
        PatchU32(OpaqueHeaderTail, 0x2C, (uint)(ourGadOffset + refGadSize));

        // RelocSize (header 0x0C) is patched by Write() from actual output length.
    }

    private static void PatchU32(byte[] buf, int off, uint val)
    {
        buf[off]   = (byte)(val & 0xFF);
        buf[off+1] = (byte)((val >> 8)  & 0xFF);
        buf[off+2] = (byte)((val >> 16) & 0xFF);
        buf[off+3] = (byte)((val >> 24) & 0xFF);
    }

    // ── Writing ──────────────────────────────────────────────────────────────

    public void Write(Stream stream)
    {
        BinaryStream bs = new BinaryStream(stream);
        long basePos = bs.Position;

        // Gate and spawn section offsets are determined by layout:
        //   Gate section always starts at 0x60 (immediately after the 96-byte header).
        //   Spawn section follows immediately after the gate records.
        const uint gateOff = 0x60u;
        uint spawnOff = (uint)(gateOff + GateRecords.Count * GT3GateRecord.GetSize());

        // ── Header ──────────────────────────────────────────────────────────
        bs.WriteUInt32(MAGIC);                           // 0x00
        bs.WriteUInt32(Field04);                         // 0x04
        bs.WriteUInt32(Field08);                         // 0x08
        long relocPos = bs.Position;
        bs.WriteUInt32(0);                               // 0x0C  placeholder — patched below
        bs.WriteUInt32(Field10);                         // 0x10
        bs.WriteUInt32(Field14);                         // 0x14
        bs.WriteSingle(TrackV);                          // 0x18
        bs.WriteSingle(A1VCoord);                        // 0x1C
        bs.WriteUInt16(HeaderField20);                   // 0x20
        bs.WriteUInt16((ushort)GateRecords.Count);       // 0x22
        bs.WriteUInt32(gateOff);                         // 0x24
        bs.WriteUInt32((uint)SpawnPositions.Count);      // 0x28
        bs.WriteUInt32(spawnOff);                        // 0x2C
        bs.WriteBytes(OpaqueHeaderTail);                 // 0x30–0x5F

        // ── Gate records ─────────────────────────────────────────────────────
        // Position should already be at basePos + gateOff = basePos + 0x60.
        foreach (var rec in GateRecords)
            rec.ToStream(bs);

        // ── Spawn positions ──────────────────────────────────────────────────
        foreach (var sp in SpawnPositions)
            sp.ToStream(bs);

        // ── Trailing opaque data ─────────────────────────────────────────────
        bs.WriteBytes(TrailingData);

        // ── Patch RelocSize ──────────────────────────────────────────────────
        long endPos = bs.Position;
        bs.Position = relocPos;
        bs.WriteUInt32((uint)(endPos - basePos - 16));
        bs.Position = endPos;
    }

    // ── Reversal ─────────────────────────────────────────────────────────────

    public void ReverseDirection()
    {
        float totalTrackV = TrackV;
        int   N           = GateRecords.Count;

        uint spawnSectOff = (uint)(0x60u + N * GT3GateRecord.GetSize());
        uint spawnEnd     = (uint)(spawnSectOff + SpawnPositions.Count * RunwayStartingPosition.GetSize());

        uint pfFileOff = BitConverter.ToUInt32(OpaqueHeaderTail, 0x14);
        int  pfCount   = (int)BitConverter.ToUInt32(OpaqueHeaderTail, 0x10);
        int  tdPfStart = (int)(pfFileOff - spawnEnd);
        const int kPfStride = 32;

        // ── Step 1: Read start/finish line geometry from the forward path spline ─
        // The start/finish gate is at the max-V → min-V wrap point in the pitbox.
        // The entry with the MAXIMUM V is physically just before the finish line —
        // verified against a known-good same-size fwd/rev pair: spawn midpoints
        // cluster near max-V.M, not min-V.M (which is ~27 m off on the far side).
        int sfIdx = 0; float sfMaxV = float.MinValue;
        if (tdPfStart >= 0 && tdPfStart + pfCount * kPfStride <= TrailingData.Length)
        {
            for (int k = 0; k < pfCount; k++)
            {
                float v = BitConverter.ToSingle(TrailingData, tdPfStart + k * kPfStride + 16);
                if (v > sfMaxV) { sfMaxV = v; sfIdx = k; }
            }
        }
        int   sfBase = tdPfStart + sfIdx * kPfStride;
        float sfLx   = BitConverter.ToSingle(TrailingData, sfBase +  0);
        float sfLz   = BitConverter.ToSingle(TrailingData, sfBase +  4);
        float sfRx   = BitConverter.ToSingle(TrailingData, sfBase +  8);
        float sfRz   = BitConverter.ToSingle(TrailingData, sfBase + 12);
        float sfNx   = BitConverter.ToSingle(TrailingData, sfBase + 20); // road tangent = line normal
        float sfNz   = BitConverter.ToSingle(TrailingData, sfBase + 24);
        float sfMx   = (sfLx + sfRx) * 0.5f;
        float sfMz   = (sfLz + sfRz) * 0.5f;

        // ── Step 2: V-mirror sector records, then re-sort ascending ──────────
        // Verified against known-good fwd/rev pair: mirror dist (L−d), re-sort
        // ascending, sentinels kept unchanged (gate[0].V=0, gate[1].V=0.025).
        // Re-sorting puts timing gates (gateFlag=0) in the order they are crossed
        // going CCW, so the game's sequential scan correctly assigns T1/T2/T3.
        for (int g = 0; g < GateRecords.Count; g++)
        {
            var rec = GateRecords[g];
            if (rec.SectorId == 0 && rec.GateFlag == -1) continue; // a=0 only: keep V=0
            rec.TrackV = totalTrackV - rec.TrackV; // a=1 mirrors to ≈3921 → sorts to end
        }
        GateRecords.Sort((a, b) => a.TrackV.CompareTo(b.TrackV));
        A1VCoord = GateRecords.FirstOrDefault(r => r.IsSentinel && r.SectorId == 1)?.TrackV ?? A1VCoord;

        // ── Step 3: Reflect start grid across the start/finish line ──────────
        // Verified against known-good pair: spawns are REFLECTED across the gate
        // plane (midpoint sfM, unit normal sfN = road tangent), not just rotated.
        // Reflecting position and heading: heading component along sfN negated.
        // Convention: dir = (sin rot, cos rot) in (X, Z) — verified by reflection
        // formula giving +π/2 → −π/2 for an X-axis-normal start line.
        foreach (var sp in SpawnPositions)
        {
            float d  = (sp.X - sfMx) * sfNx + (sp.Z - sfMz) * sfNz;
            sp.X -= 2f * d * sfNx;
            sp.Z -= 2f * d * sfNz;
            float sinR = MathF.Sin(sp.Rotation), cosR = MathF.Cos(sp.Rotation);
            float proj = sinR * sfNx + cosR * sfNz;
            sp.Rotation = MathF.Atan2(sinR - 2f * proj * sfNx, cosR - 2f * proj * sfNz);
        }

        // ── Step 4: Reverse path spline — 5-part transform ───────────────────
        // Verified 116/116 against known-good fwd/rev pair:
        //   1. Reverse station order:  new[i] ← fwd[(N−2−i) mod N]
        //   2. Swap L/R edge points:   (Lx,Lz) ↔ (Rx,Rz)
        //   3. Mirror lap distance:    dist → TrackV − dist
        //   4. Negate tangent:         (tanX,tanZ) → (−tanX,−tanZ)
        //   5. Negate aux (f7):        aux → −aux   ← the field the old code missed
        // Collision mesh, vertex pool, spatial grids, tail: byte-identical — no transform.
        if (tdPfStart >= 0 && tdPfStart + pfCount * kPfStride <= TrailingData.Length)
        {
            var pfCopy = new byte[pfCount * kPfStride];
            Array.Copy(TrailingData, tdPfStart, pfCopy, 0, pfCopy.Length);

            for (int i = 0; i < pfCount; i++)
            {
                int srcIdx = ((pfCount - 2 - i) % pfCount + pfCount) % pfCount;
                int src    = srcIdx * kPfStride;
                int dst    = tdPfStart + i * kPfStride;

                float f0 = BitConverter.ToSingle(pfCopy, src);
                float f1 = BitConverter.ToSingle(pfCopy, src +  4);
                float f2 = BitConverter.ToSingle(pfCopy, src +  8);
                float f3 = BitConverter.ToSingle(pfCopy, src + 12);
                float f4 = BitConverter.ToSingle(pfCopy, src + 16);
                float f5 = BitConverter.ToSingle(pfCopy, src + 20);
                float f6 = BitConverter.ToSingle(pfCopy, src + 24);
                float f7 = BitConverter.ToSingle(pfCopy, src + 28);

                void WriteF(int off, float val)
                    => Array.Copy(BitConverter.GetBytes(val), 0, TrailingData, dst + off, 4);

                WriteF( 0, f2); WriteF( 4, f3); // Left ← old Right
                WriteF( 8, f0); WriteF(12, f1); // Right ← old Left
                WriteF(16, totalTrackV - f4);   // mirror dist
                WriteF(20, -f5); WriteF(24, -f6); // negate tangent
                WriteF(28, -f7);                  // negate aux
            }

            // Rotate so the entry with minimum V (= physical start/finish) is at index 0.
            // The game renders the start-line marker at entry[0]; without rotation
            // entry[0] sits at V≈1957 (the forward mid-track), placing the visual
            // start line halfway around the circuit.
            int minVIdx2 = 0; float minV2 = float.MaxValue;
            for (int k = 0; k < pfCount; k++)
            {
                float v = BitConverter.ToSingle(TrailingData, tdPfStart + k * kPfStride + 16);
                if (v < minV2) { minV2 = v; minVIdx2 = k; }
            }
            if (minVIdx2 != 0)
            {
                var tmp = new byte[pfCount * kPfStride];
                Array.Copy(TrailingData, tdPfStart, tmp, 0, tmp.Length);
                for (int i = 0; i < pfCount; i++)
                {
                    int srcIdx = (i + minVIdx2) % pfCount;
                    Array.Copy(tmp, srcIdx * kPfStride,
                               TrailingData, tdPfStart + i * kPfStride, kPfStride);
                }
            }
        }
        // ── Step 6: Reverse groundcolldata spatial grid (TODO — structure TBD) ─
        // The spatial grid (cell→node-index tables) encodes traversal direction.
        // Correct transformation requires understanding the exact cell-list format
        // to avoid over-modifying non-node values. Currently disabled.
        if (false) // placeholder — do not execute
        // ── Step 6 body ──────────────────────────────────────────────────────
        // The spatial grid maps world-space cells to mesh node indices.
        // For the reversed track, each node index k must become (S_fwd − k + N) % N,
        // where S_fwd is the forward node closest to the start/finish gate (found
        // via vertex centroid distance) and N is the total node count.
        // This encodes the CCW traversal order without re-tessellating the mesh.
        //
        // Layout within groundcolldata (starting at magic 0x780053):
        //   [0 .. N×64−1]          node descriptors (64 B each)
        //   [N×64 .. minOffA−1]    spatial grid (the section we remap)
        //   [minOffA ..]           vertex / triangle blocks (untouched)
        {
            // Locate groundcolldata: first try the magic 0x780053 (present in some tracks),
            // otherwise fall back to scanning from the start of TrailingData.
            // Some tracks (e.g. smtsouth) have no magic — node descriptors begin immediately.
            const uint kGcMag = 0x00780053u;
            int gcS = -1;
            for (int p = 0; p + 4 <= TrailingData.Length; p++)
                if (BitConverter.ToUInt32(TrailingData, p) == kGcMag) { gcS = p; break; }
            if (gcS < 0) gcS = 0; // no magic — scan from beginning of TrailingData

            {
                // Find first valid 64-byte node descriptor: n1>5, n2>5, deg 1-20, zeros at +28..+63
                int dBase = gcS;
                for (int p = gcS; p + 64 <= TrailingData.Length; p++)
                {
                    int dn1 = BitConverter.ToUInt16(TrailingData, p);
                    int dn2 = BitConverter.ToUInt16(TrailingData, p + 2);
                    int ddeg = BitConverter.ToUInt16(TrailingData, p + 22);
                    if (dn1 > 5 && dn2 > 5 && ddeg >= 1 && ddeg <= 20)
                    { dBase = p; break; }
                }

                // Count consecutive valid descriptors → N
                // Only require n1>0, n2>0, deg in [1,20] — not all-zeros at +28..+63,
                // because detail/junction nodes may have neighbour data there.
                int gcN = 0;
                while (true)
                {
                    int bp = dBase + gcN * 64;
                    if (bp + 64 > TrailingData.Length) break;
                    int dn1 = BitConverter.ToUInt16(TrailingData, bp);
                    int dn2 = BitConverter.ToUInt16(TrailingData, bp + 2);
                    int ddeg = BitConverter.ToUInt16(TrailingData, bp + 22);
                    if (dn1 < 1 || dn2 < 1 || ddeg < 1 || ddeg > 20) break;
                    gcN++;
                }

                if (gcN >= 2)
                {
                    // Full node count for mod-N arithmetic: prefer the 4-byte count stored
                    // just before the descriptor array (or at the start of TrailingData).
                    // This includes detail/junction nodes that may not follow the sequential
                    // 64-byte layout. Fall back to gcN if no valid count is found.
                    int gcNFull = gcN;
                    {
                        // Try: 4 bytes just before dBase
                        if (dBase >= 4)
                        {
                            int tryN = (int)BitConverter.ToUInt32(TrailingData, dBase - 4);
                            if (tryN >= gcN && tryN < 4096) gcNFull = tryN;
                        }
                        // Try: first 4 bytes of TrailingData
                        if (gcNFull == gcN && TrailingData.Length >= 4)
                        {
                            int tryN = (int)BitConverter.ToUInt32(TrailingData, 0);
                            if (tryN >= gcN && tryN < 4096) gcNFull = tryN;
                        }
                    }

                    // offA in the descriptor may be:
                    //   ABSOLUTE (file offset)  — tracks with magic header, offA > spawnEnd + nodeDataBase
                    //   RELATIVE (to node data area = dBase + gcN×64) — tracks without magic
                    // Auto-detect by checking whether offA[0] exceeds the node-data-area file offset.
                    int nodeDataBase = dBase + gcN * 64;
                    uint offA0 = BitConverter.ToUInt32(TrailingData, dBase + 4);
                    bool absoluteOffA = (offA0 > (uint)(spawnEnd + nodeDataBase));

                    int sFwd = 0;
                    float minNodeDist = float.MaxValue;
                    int minVtxTd = TrailingData.Length;

                    for (int n = 0; n < gcN; n++)
                    {
                        int bp = dBase + n * 64;
                        int dn1 = BitConverter.ToUInt16(TrailingData, bp);
                        uint offA = BitConverter.ToUInt32(TrailingData, bp + 4);

                        int vtxTd = absoluteOffA
                            ? (int)(offA - spawnEnd)           // file-absolute → TrailingData offset
                            : nodeDataBase + (int)offA;        // section-relative → TrailingData offset

                        if (vtxTd < minVtxTd) minVtxTd = vtxTd;
                        if (vtxTd < 0 || vtxTd + dn1 * 16 > TrailingData.Length) continue;

                        float cx = 0f, cz = 0f;
                        for (int v = 0; v < dn1; v++)
                        {
                            cx += BitConverter.ToSingle(TrailingData, vtxTd + v * 16 + 0);
                            cz += BitConverter.ToSingle(TrailingData, vtxTd + v * 16 + 8);
                        }
                        cx /= dn1; cz /= dn1;
                        float d = (cx - sfMx) * (cx - sfMx) + (cz - sfMz) * (cz - sfMz);
                        if (d < minNodeDist) { minNodeDist = d; sFwd = n; }
                    }

                    // Spatial grid: from end of descriptor array to start of first vertex block
                    int gridStart2 = nodeDataBase;
                    int gridEnd2   = (minVtxTd < TrailingData.Length) ? minVtxTd : TrailingData.Length;
                    if (gridEnd2 > TrailingData.Length) gridEnd2 = TrailingData.Length;

                    // Remap every uint16 in [0, gcNFull) within the spatial grid.
                    // Use gcNFull (full node count) for the modular formula so that
                    // detail/junction nodes (indices >= gcN) are also correctly remapped.
                    for (int off = gridStart2; off + 2 <= gridEnd2; off += 2)
                    {
                        int k = BitConverter.ToUInt16(TrailingData, off);
                        if (k < gcNFull)
                        {
                            int newK = ((sFwd - k) % gcNFull + gcNFull) % gcNFull;
                            TrailingData[off]     = (byte)(newK & 0xFF);
                            TrailingData[off + 1] = (byte)(newK >> 8);
                        }
                    }
                }
            }
        }
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
}
