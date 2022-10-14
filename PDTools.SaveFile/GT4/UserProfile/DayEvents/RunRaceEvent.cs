using System;
using System.Collections.Generic;
using System.Text;
using Syroot.BinaryData.Memory;

using PDTools.Structures;
using PDTools.Enums.PS2;

namespace PDTools.SaveFile.GT4.UserProfile.DayEvents
{
    public class RunRaceEvent : DayEvent, IGameSerializeBase
    {
        public override DayEventType EventType => DayEventType.RUN_RACE;

        public byte Unk { get; set; }
        public Result Result { get; set; }
        public byte Unk2 { get; set; }
        public int BestTime { get; set; }
        public DbCode RaceCode { get; set; }

        public override void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteByte(Unk);
            sw.WriteByte((byte)Result);
            sw.WriteByte(Unk2);
            sw.WriteInt32(BestTime);
            sw.WriteInt32(RaceCode.Code);
            sw.WriteInt32(RaceCode.TableId);
        }

        public override void Unpack(GT4Save save, ref SpanReader sr)
        {
            Unk = sr.ReadByte();
            Result = (Result)sr.ReadByte();
            Unk2 = sr.ReadByte();
            BestTime = sr.ReadInt32();
            RaceCode = new DbCode(sr.ReadInt32(), sr.ReadInt32());
        }
    }
}
