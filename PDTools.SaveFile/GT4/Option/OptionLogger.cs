using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

using PDTools.Utils;
using System.IO;

namespace PDTools.SaveFile.GT4.Option;

public class OptionLogger : IGameSerializeBase<OptionLogger>
{
    public byte[] Loggers { get; set; } = new byte[5];

    public void CopyTo(OptionLogger dest)
    {
        if (Loggers.Length != 5 || dest.Loggers.Length != 5)
            throw new InvalidDataException("Source and Destination Logger length must be 5.");

        dest.Loggers = new byte[Loggers.Length];
        Array.Copy(Loggers, dest.Loggers, Loggers.Length);
    }

    public void Pack(GT4Save save, ref SpanWriter sw)
    { 
        sw.WriteBytes(Loggers.AsSpan(0, 5));

        sw.Align(GT4Save.ALIGNMENT);
    }

    public void Unpack(GT4Save save, ref SpanReader sr)
    {
        Loggers = sr.ReadBytes(5);

        sr.Align(GT4Save.ALIGNMENT);
    }
}
