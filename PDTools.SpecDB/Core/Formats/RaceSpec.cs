using System;
using System.Collections.Generic;
using System.Text;

using System.Buffers.Binary;
using Syroot.BinaryData;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Formats;

public class RaceSpec
{
    public const int HeaderSize = 0x08;
    public const int EntrySize = 0x10;

    public byte[] Buffer { get; set; }

    public int EntryCount
    {
        get => BinaryPrimitives.ReadInt32LittleEndian(Buffer);
    }

    public RaceSpec(byte[] buffer)
    {
        Buffer = buffer;
    }

    public int GetCarIdByIndex(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, EntryCount, nameof(index));

        SpanReader sr = new SpanReader(Buffer);
        sr.Position = HeaderSize + index * 0x10;

        int id = sr.ReadInt32();
        return id;
    }

    public int GetCarColorByIndex(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, EntryCount, nameof(index));

        SpanReader sr = new SpanReader(Buffer);
        sr.Position = HeaderSize + index * 0x10;
        sr.Position += 8;

        int color = sr.ReadInt32();
        return color;
    }
}
