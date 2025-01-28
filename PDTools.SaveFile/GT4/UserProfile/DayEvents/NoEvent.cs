using System;
using System.Collections.Generic;
using System.Text;
using Syroot.BinaryData.Memory;

using PDTools.Structures;
using PDTools.Enums.PS2;

namespace PDTools.SaveFile.GT4.UserProfile.DayEvents;

public class NoEvent : IDayEvent
{
    public DayEventType EventType => DayEventType.NO_EVENT;

    public byte[] Data { get; set; }

    public void CopyTo(IDayEvent dest)
    {
        ((NoEvent)dest).Data = new byte[Data.Length];
        Array.Copy(Data, ((NoEvent)dest).Data, Data.Length);
    }

    public void Pack(GT4Save save, ref SpanWriter sw)
    {
        sw.WriteBytes(Data); // TODO: Fix this
    }

    public void Unpack(GT4Save save, ref SpanReader sr)
    {
        Data = sr.ReadBytes(0x0F);
    }
}
