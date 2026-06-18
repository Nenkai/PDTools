using System;
using System.Collections.Generic;
using System.IO;

namespace PDTools.Files.Courses.PS2.Runway;

/// <summary>
/// Reverses the driving direction of a GTRW "CourseRunwayData" runway file
/// (full single-file, little-endian format). Validated byte-for-byte against
/// known-good forward/reverse pairs (smtsouth: 0 bytes off).
///
/// Four transforms flip the lap — all other sections are left unchanged:
///   1. Path spline  — new[k] = T(fwd[(PC-2-k)%PC]); swap L/R, mirror dist,
///                     negate tangent and aux.
///   2. Start grid   — rotated 180° about the start/finish point.
///   3. Sectors      — dist mirrored (L-d), re-sorted ascending; start/finish
///                     markers (type≠2) stay at distance 0.
///   4. Mesh         — each node's g path-segment refs reflected by S=(PC-3)%PC
///                     and re-sorted ascending. Geometry and adjacency unchanged.
///
/// File size is preserved — header offsets and EOF need no changes.
/// </summary>
public static class GT3RunwayReverser
{
    // ── Helpers ──────────────────────────────────────────────────────────────

    private static ushort RU16(byte[] b, int o) => BitConverter.ToUInt16(b, o);
    private static uint   RU32(byte[] b, int o) => BitConverter.ToUInt32(b, o);
    private static float  RF32(byte[] b, int o) => BitConverter.ToSingle(b, o);

    private static void WU16(byte[] b, int o, int v)
    {
        b[o]     = (byte)(v        & 0xFF);
        b[o + 1] = (byte)((v >> 8) & 0xFF);
    }
    private static void WF32(byte[] b, int o, float v)
        => Array.Copy(BitConverter.GetBytes(v), 0, b, o, 4);

    // Floating-point modulo, always non-negative.
    private static float FMod(float x, float m)
    {
        float r = x % m;
        return r < 0f ? r + m : r;
    }

    // ── Section descriptor ───────────────────────────────────────────────────

    private readonly struct Info
    {
        public readonly int   Size;
        public readonly float L;            // lap length
        public readonly int   SectorCount;
        public readonly int   GridCount,  GridOff;   // start grid (spawn slots)
        public readonly int   MeshCount,  MeshOff;   // collision mesh nodes
        public readonly int   PoolOff;               // surface vertex pool
        public readonly int   PathCount,  PathOff;   // centreline path spline
        public readonly int   Grid1Off,   Grid2Off;  // broadphase grids
        public readonly int   TailOff;               // staging / tail block

        public Info(byte[] d)
        {
            Size        = d.Length;
            L           = RF32(d, 0x18);
            SectorCount = RU16(d, 0x22);
            GridCount   = (int)RU32(d, 0x28);
            GridOff     = (int)RU32(d, 0x2C);
            MeshCount   = (int)RU32(d, 0x30);
            MeshOff     = (int)RU32(d, 0x34);
            PoolOff     = (int)RU32(d, 0x3C);
            PathCount   = (int)RU32(d, 0x40);
            PathOff     = (int)RU32(d, 0x44);
            Grid1Off    = (int)RU32(d, 0x48);
            Grid2Off    = (int)RU32(d, 0x4C);
            TailOff     = (int)RU32(d, 0x54);
        }
    }

    // ── Validation ───────────────────────────────────────────────────────────

    private static Info Validate(byte[] d)
    {
        if (d.Length < 0x60
            || d[0] != (byte)'G' || d[1] != (byte)'T'
            || d[2] != (byte)'R' || d[3] != (byte)'W')
            throw new InvalidDataException("Not a GTRW runway file (missing GTRW magic).");

        var info = new Info(d);

        if (RU32(d, 0x24) != 0x60)
            throw new InvalidDataException("Unexpected sectors offset — layout not supported.");
        if (info.L <= 10f || info.L >= 1e6f)
            throw new InvalidDataException("Lap length out of range — aborting to avoid corruption.");

        int[] order = { info.MeshOff, info.PoolOff, info.PathOff,
                        info.Grid1Off, info.Grid2Off, info.TailOff };
        for (int i = 0; i < order.Length; i++)
            if (order[i] < 0x60 || order[i] > info.Size)
                throw new InvalidDataException("Section offsets inconsistent — file not supported.");
        for (int i = 1; i < order.Length; i++)
            if (order[i] < order[i - 1])
                throw new InvalidDataException("Section offsets not monotonically increasing — not supported.");

        if (info.MeshCount < 2 || info.MeshCount > 100_000)
            throw new InvalidDataException("Mesh node count out of range — aborting.");

        return info;
    }

    // ── Transform 1: reverse the centreline path spline ─────────────────────
    // new[k] = T(fwd[(PC-2-k) % PC])  — swap L/R, mirror dist, negate tan+aux.

    private static void ReversePath(byte[] d, in Info i)
    {
        int po = i.PathOff, pc = i.PathCount;
        float L = i.L;

        var snap = new float[pc, 8];
        for (int s = 0; s < pc; s++)
            for (int f = 0; f < 8; f++)
                snap[s, f] = RF32(d, po + s * 32 + f * 4);

        for (int k = 0; k < pc; k++)
        {
            int src = ((pc - 2 - k) % pc + pc) % pc;
            float Lx  = snap[src, 0], Lz  = snap[src, 1];
            float Rx  = snap[src, 2], Rz  = snap[src, 3];
            float dist = snap[src, 4];
            float tx  = snap[src, 5], tz  = snap[src, 6], aux = snap[src, 7];
            int dst = po + k * 32;
            WF32(d, dst +  0, Rx);
            WF32(d, dst +  4, Rz);
            WF32(d, dst +  8, Lx);
            WF32(d, dst + 12, Lz);
            WF32(d, dst + 16, FMod(L - dist, L));
            WF32(d, dst + 20, -tx);
            WF32(d, dst + 24, -tz);
            WF32(d, dst + 28, -aux);
        }
    }

    // ── Transform 2: rotate start grid 180° about the start/finish point ────
    // Must be called BEFORE ReversePath so it reads the forward centreline.

    private static void ReverseStartGrid(byte[] d, in Info i)
    {
        int go = i.GridOff, gc = i.GridCount;
        int po = i.PathOff, pc = i.PathCount;
        float L = i.L;

        // Read forward path stations (8 floats: Lx,Lz,Rx,Rz,dist,tx,tz,aux)
        var midX  = new float[pc]; var midZ  = new float[pc];
        var dist  = new float[pc];
        var tanX  = new float[pc]; var tanZ  = new float[pc];
        for (int s = 0; s < pc; s++)
        {
            int o     = po + s * 32;
            float lx  = RF32(d, o), lz = RF32(d, o + 4), rx = RF32(d, o + 8), rz = RF32(d, o + 12);
            midX[s]   = (lx + rx) * 0.5f;
            midZ[s]   = (lz + rz) * 0.5f;
            dist[s]   = RF32(d, o + 16);
            tanX[s]   = RF32(d, o + 20);
            tanZ[s]   = RF32(d, o + 24);
        }

        // Find the dist→0 wrap (largest drop between consecutive distances)
        int w = 0; float maxGap = float.MinValue;
        for (int s = 0; s < pc; s++)
        {
            float gap = dist[s] - dist[(s + 1) % pc];
            if (gap > maxGap) { maxGap = gap; w = s; }
        }
        int w2 = (w + 1) % pc;
        float den = (L - dist[w]) + dist[w2];
        float f   = den > 1e-6f ? (L - dist[w]) / den : 0f;

        float p0x = midX[w] + f * (midX[w2] - midX[w]);
        float p0z = midZ[w] + f * (midZ[w2] - midZ[w]);
        float tx0 = tanX[w] + f * (tanX[w2] - tanX[w]);
        float tz0 = tanZ[w] + f * (tanZ[w2] - tanZ[w]);
        float tl  = MathF.Sqrt(tx0 * tx0 + tz0 * tz0);
        if (tl < 1e-10f) tl = 1f;
        float px = -tz0 / tl;   // perpendicular to tangent = start-line direction
        float pz =  tx0 / tl;

        // Project grid centroid onto the start line to get the reflection centre
        float cx = 0f, cz = 0f;
        for (int s = 0; s < gc; s++) { cx += RF32(d, go + s * 16); cz += RF32(d, go + s * 16 + 8); }
        cx /= gc; cz /= gc;
        float dot = (cx - p0x) * px + (cz - p0z) * pz;
        float rx2 = p0x + dot * px, rz2 = p0z + dot * pz;

        // Reflect each slot: x → 2rx-x, z → 2rz-z, heading → atan2(-sin,-cos)
        for (int s = 0; s < gc; s++)
        {
            int o = go + s * 16;
            float x = RF32(d, o), y = RF32(d, o + 4), z = RF32(d, o + 8), h = RF32(d, o + 12);
            WF32(d, o +  0, 2f * rx2 - x);
            WF32(d, o +  4, y);
            WF32(d, o +  8, 2f * rz2 - z);
            WF32(d, o + 12, MathF.Atan2(-MathF.Sin(h), -MathF.Cos(h)));
        }
    }

    // ── Transform 3: mirror sector / checkpoint distances ────────────────────
    // type≠2 markers stay at dist=0; type-2 records mirrored (L-d), re-sorted.

    private static void ReverseSectors(byte[] d, in Info i)
    {
        int sc = i.SectorCount;
        float L = i.L;

        var markers = new List<(int t, int fl, float dist)>();
        var feats   = new List<(int t, int fl, float dist)>();
        for (int s = 0; s < sc; s++)
        {
            int o    = 0x60 + s * 8;
            int type = RU16(d, o), flag = RU16(d, o + 2);
            float dt = RF32(d, o + 4);
            (type != 2 ? markers : feats).Add((type, flag, dt));
        }

        for (int s = 0; s < feats.Count; s++)
        {
            var (t, fl, dt) = feats[s];
            feats[s] = (t, fl, FMod(L - dt, L));
        }
        feats.Sort((a, b) => a.dist.CompareTo(b.dist));

        int idx = 0;
        foreach (var (t, fl, dt) in markers) { int o = 0x60 + idx++ * 8; WU16(d, o, t); WU16(d, o + 2, fl); WF32(d, o + 4, dt); }
        foreach (var (t, fl, dt) in feats)   { int o = 0x60 + idx++ * 8; WU16(d, o, t); WU16(d, o + 2, fl); WF32(d, o + 4, dt); }
    }

    // ── Transform 4: reflect ground-collision path-segment references ─────────
    // Each node carries g segment indices (at its offC record); S = (PC-3)%PC
    // derived from the path reversal formula. Re-sorted ascending per node.
    // Geometry, adjacency and all other node data are left unchanged.

    private static void ReverseMesh(byte[] d, in Info i)
    {
        int mo = i.MeshOff, N = i.MeshCount, PC = i.PathCount;
        int S  = (PC - 3 + PC) % PC;

        for (int n = 0; n < N; n++)
        {
            int o    = mo + n * 64;
            int g    = RU16(d, o + 20);           // geom-count = path-segment ref count
            int offC = (int)RU32(d, o + 16);      // absolute offset of node's 16-byte record

            var refs = new int[g];
            for (int j = 0; j < g; j++)
                refs[j] = ((S - RU16(d, offC + j * 2)) % PC + PC) % PC;
            Array.Sort(refs);

            for (int j = 0; j < g; j++)
                WU16(d, offC + j * 2, refs[j]);
        }
    }

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>
    /// Returns a new byte array containing the reversed GTRW runway.
    /// Throws <see cref="InvalidDataException"/> if the file format is not supported.
    /// </summary>
    public static byte[] Reverse(byte[] data)
    {
        var d    = (byte[])data.Clone();
        var info = Validate(d);

        ReverseStartGrid(d, info);   // reads forward path — must precede ReversePath
        ReversePath(d, info);
        ReverseSectors(d, info);
        ReverseMesh(d, info);

        return d;
    }

    /// <summary>
    /// Read <paramref name="inputPath"/>, reverse it, write to <paramref name="outputPath"/>.
    /// Returns a summary line suitable for logging.
    /// </summary>
    public static string ReverseFile(string inputPath, string outputPath)
    {
        if (Path.GetFullPath(inputPath).Equals(
                Path.GetFullPath(outputPath), StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Output path must differ from the input file.");

        byte[] data   = File.ReadAllBytes(inputPath);
        var    info   = Validate(data);          // validate before cloning
        byte[] result = Reverse(data);

        string dir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        File.WriteAllBytes(outputPath, result);

        return $"{result.Length} bytes | lap {info.L:F2} m | " +
               $"{info.MeshCount} mesh nodes | {info.SectorCount} sectors | " +
               $"{info.PathCount} path stations";
    }
}
