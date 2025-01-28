using System;
using System.Collections.Generic;
using System.Text;
using Syroot.BinaryData.Memory;

using PDTools.Structures;
using PDTools.Enums.PS2;

namespace PDTools.SaveFile.GT4.UserProfile.DayEvents;

public class SellCarEvent : IDayEvent
{
    public DayEventType EventType => DayEventType.SELL_CAR;

    public byte ColorIndex { get; set; }
    public short Price { get; set; }
    public DbCode CarCode { get; set; }

    public void CopyTo(IDayEvent dest)
    {
        ((SellCarEvent)dest).ColorIndex = ColorIndex;
        ((SellCarEvent)dest).Price = Price;
        ((SellCarEvent)dest).CarCode = new DbCode(CarCode.Code, CarCode.TableId);
    }

    public void Pack(GT4Save save, ref SpanWriter sw)
    {
        sw.WriteByte(ColorIndex);
        sw.WriteInt16(Price);
        sw.WriteInt32(0);
        sw.WriteInt32(CarCode.Code);
        sw.WriteInt32(CarCode.TableId);
    }

    public void Unpack(GT4Save save, ref SpanReader sr)
    {
        ColorIndex = sr.ReadByte();
        Price = sr.ReadInt16();
        sr.ReadInt32();
        CarCode = new DbCode(sr.ReadInt32(), sr.ReadInt32());
    }
}
