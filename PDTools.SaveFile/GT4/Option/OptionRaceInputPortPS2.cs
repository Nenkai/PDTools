using Syroot.BinaryData.Memory;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDTools.SaveFile.GT4.Option;

public class OptionRaceInputPortPS2 : IGameSerializeBase<OptionRaceInputPortPS2>
{
    public byte[] Data { get; set; } = new byte[0xB8];

    public void CopyTo(OptionRaceInputPortPS2 dest)
    {
        if (Data.Length != 0xB8 || dest.Data.Length != 0xB8)
            throw new InvalidDataException("Source and Destination OptionRaceInputPortPS2 length must be 0xB8.");

        dest.Data = new byte[Data.Length];
        Array.Copy(Data, dest.Data, Data.Length);
    }

    public void Pack(GT4Save save, ref SpanWriter sw)
    {
        sw.WriteBytes(Data.AsSpan(0, 0xB8));
    }

    public void Unpack(GT4Save save, ref SpanReader sr)
    {
        Data = sr.ReadBytes(0xB8);
    }
}
