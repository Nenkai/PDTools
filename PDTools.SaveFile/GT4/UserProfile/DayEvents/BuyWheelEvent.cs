using System;
using System.Collections.Generic;
using System.Text;
using Syroot.BinaryData.Memory;

using PDTools.Enums.PS2;

using PDTools.Structures;

namespace PDTools.SaveFile.GT4.UserProfile.DayEvents
{
    public class BuyWheelEvent : DayEvent
    {
        public override DayEventType EventType => DayEventType.BUY_WHEEL;

        public WheelCategoryType WheelCategory { get; set; }
        public byte Unk { get; set; }
        public byte Unk2 { get; set; }
        public DbCode WheelCode { get; set; }

        public override void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteByte((byte)WheelCategory);
            sw.WriteByte(Unk);
            sw.WriteByte(Unk2);
            sw.WriteInt32(0);
            sw.WriteInt32(WheelCode.Code);
            sw.WriteInt32(WheelCode.TableId);
        }

        public override void Unpack(GT4Save save, ref SpanReader sr)
        {
            WheelCategory = (WheelCategoryType)sr.ReadByte();
            Unk = sr.ReadByte();
            Unk2 = sr.ReadByte();
            sr.ReadInt32();
            WheelCode = new DbCode(sr.ReadInt32(), sr.ReadInt32());
        }
    }

    
}
