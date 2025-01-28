using Syroot.BinaryData.Memory;

using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.SaveFile.GT4.Option;

public class OptionRaceControllerPS2 : IGameSerializeBase<OptionRaceControllerPS2>
{
    public int Unk { get; set; }
    public OptionRaceInputPortPS2[] InputPort { get; set; } = new OptionRaceInputPortPS2[1];

    public void CopyTo(OptionRaceControllerPS2 dest)
    {
        dest.Unk = Unk;
        for (var i = 0; i < 1; i++)
        {
            dest.InputPort[i] = new OptionRaceInputPortPS2();
            InputPort[i].CopyTo(dest.InputPort[i]);
        }
    }

    public void Pack(GT4Save save, ref SpanWriter sw)
    {
        sw.WriteInt32(Unk);
        for (var i = 0; i < 1; i++)
            InputPort[i].Pack(save, ref sw);
    }

    public void Unpack(GT4Save save, ref SpanReader sr)
    {
        Unk = sr.ReadInt32();
        for (var i = 0; i < 1; i++)
        {
            InputPort[i] = new OptionRaceInputPortPS2();
            InputPort[i].Unpack(save, ref sr);
        }
    }
}
