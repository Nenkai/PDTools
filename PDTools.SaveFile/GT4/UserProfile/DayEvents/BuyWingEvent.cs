using System;
using System.Collections.Generic;
using System.Text;
using Syroot.BinaryData.Memory;

using PDTools.Structures;
using PDTools.Enums.PS2;

namespace PDTools.SaveFile.GT4.UserProfile.DayEvents
{
    public class BuyWingEvent : DayEvent, IGameSerializeBase
    {
        public override DayEventType EventType => DayEventType.BUY_WING;

        public DbCode WingCode { get; set; }

        public override void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteByte(0);
            sw.WriteByte(0);
            sw.WriteByte(0);
            sw.WriteInt32(0);
            sw.WriteInt32(WingCode.Code);
            sw.WriteInt32(WingCode.TableId);
        }

        public override void Unpack(GT4Save save, ref SpanReader sr)
        {
            sr.ReadByte();
            sr.ReadByte();
            sr.ReadByte();
            sr.ReadInt32();
            WingCode = new DbCode(sr.ReadInt32(), sr.ReadInt32());
        }
    }
}
