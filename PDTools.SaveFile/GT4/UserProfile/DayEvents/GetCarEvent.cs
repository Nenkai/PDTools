using System;
using System.Collections.Generic;
using System.Text;
using Syroot.BinaryData.Memory;

using PDTools.Structures;
using PDTools.Enums.PS2;

namespace PDTools.SaveFile.GT4.UserProfile.DayEvents
{
    public class GetCarEvent : DayEvent, IGameSerializeBase
    {
        public override DayEventType EventType => DayEventType.GET_CAR;

        public byte Unk { get; set; }
        public GetCarReason Reason { get; set; }
        public byte Unk2 { get; set; }
        public int Unk3 { get; set; }
        public DbCode CarCode { get; set; }

        public override void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteByte(Unk);
            sw.WriteByte((byte)Reason);
            sw.WriteByte(Unk2);
            sw.WriteInt32(Unk3);
            sw.WriteInt32(CarCode.Code);
            sw.WriteInt32(CarCode.TableId);
        }

        public override void Unpack(GT4Save save, ref SpanReader sr)
        {
            Unk = sr.ReadByte();
            Reason = (GetCarReason)sr.ReadByte();
            Unk2 = sr.ReadByte();
            Unk3 = sr.ReadInt32();
            CarCode = new DbCode(sr.ReadInt32(), sr.ReadInt32());
        }
    }


}
