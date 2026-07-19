using System;
using System.Collections.Generic;
using System.IO;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.CarModel1;

/// <summary>
/// GT4 "Pat0" colour patch — the byte-diff colour-variation table applied over an MDLS (ModelSet2).
/// Menu cars embed it in the CAR4 (header @0x1C main model, @0x24 wheel); race cars ship the main one
/// as an external ".pat" file. Each paint colour is a set of patches; every patch overwrites a run of
/// bytes at an MDLS-relative offset (palette/CLUT data in the texset region + material colour in the
/// materials region). Every paint patches the SAME set of offsets, differing only in the data written.
/// </summary>
public class Pat0ColorPatch
{
    public const uint MAGIC = 0x30746150; // "Pat0"

    /// <summary>One entry per paint colour; each is that colour's list of byte-patches.</summary>
    public List<List<ModelPatch>> Paints { get; set; } = [];

    public int PaintCount => Paints.Count;
    public int PatchesPerPaint => Paints.Count > 0 ? Paints[0].Count : 0;

    public static Pat0ColorPatch Read(byte[] blob)
    {
        var p = new Pat0ColorPatch();
        p.FromStream(new MemoryStream(blob, writable: false));
        return p;
    }

    public void FromStream(Stream stream)
    {
        long basePos = stream.Position;
        var bs = new BinaryStream(stream, ByteConverter.Little);

        uint magic = bs.ReadUInt32();
        if (magic != MAGIC)
            throw new InvalidDataException($"Expected Pat0 magic, got 0x{magic:X8}.");

        // Header 0x20: magic + relocPtr + empty[2] + PaintCount(short @0x10) + PatchCount(short @0x12) + empty2[3].
        bs.Position = basePos + 0x10;
        ushort paintCount = bs.ReadUInt16();
        ushort patchCount = bs.ReadUInt16();

        // int PatchOffsets[PaintCount * PatchCount] @ 0x20 — each a blob-relative offset to a patch header.
        bs.Position = basePos + 0x20;
        int total = paintCount * patchCount;
        var offsets = new uint[total];
        for (int i = 0; i < total; i++)
            offsets[i] = bs.ReadUInt32();

        for (int i = 0; i < paintCount; i++)
        {
            var patches = new List<ModelPatch>(patchCount);
            for (int j = 0; j < patchCount; j++)
            {
                bs.Position = basePos + offsets[(i * patchCount) + j];
                var mp = new ModelPatch
                {
                    TargetOffset = bs.ReadInt32(),
                    Size = bs.ReadInt32(),
                };
                int padded = (mp.Size + 3) & ~3; // stored padded to a multiple of 4; Size is the strict length
                mp.Data = bs.ReadBytes(padded);
                patches.Add(mp);
            }
            Paints.Add(patches);
        }
    }

    /// <summary>
    /// Returns a copy of <paramref name="mdls"/> with paint <paramref name="paintIndex"/> applied (each
    /// patch's strict-size bytes written at its MDLS target offset). Mirrors the reference tool's applicator;
    /// used to reconstruct a colour variation for validation.
    /// </summary>
    public byte[] ApplyPaint(byte[] mdls, int paintIndex)
    {
        var outp = (byte[])mdls.Clone();
        foreach (ModelPatch mp in Paints[paintIndex])
        {
            if (mp.TargetOffset < 0 || mp.TargetOffset + mp.Size > outp.Length)
                throw new InvalidDataException($"Patch target 0x{mp.TargetOffset:X}+{mp.Size} exceeds MDLS size 0x{outp.Length:X}.");
            Array.Copy(mp.Data, 0, outp, mp.TargetOffset, mp.Size);
        }
        return outp;
    }
}

/// <summary>A single byte-patch within a Pat0 paint colour.</summary>
public class ModelPatch
{
    /// <summary>MDLS-relative byte offset this patch overwrites.</summary>
    public int TargetOffset { get; set; }

    /// <summary>Strict patch length (<see cref="Data"/> may be padded up to a multiple of 4).</summary>
    public int Size { get; set; }

    public byte[] Data { get; set; }
}
