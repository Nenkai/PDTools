using System;
using System.Collections.Generic;
using System.Text;
using Syroot.BinaryData.Memory;

using PDTools.Structures;
using PDTools.Enums.PS2;

namespace PDTools.SaveFile.GT4.UserProfile.DayEvents
{
    public class RunLicenseEvent : DayEvent, IGameSerializeBase
    {
        public override DayEventType EventType => DayEventType.RUN_LICENSE;

        public Result Result { get; set; }
        public byte Unk14 { get; set; }
        public byte Unk { get; set; }
        public int BestTime { get; set; }
        public DbCode RaceCode { get; set; }

        public override void Pack(GT4Save save, ref SpanWriter sw)
        {
            throw new NotImplementedException();
        }

        public override void Unpack(GT4Save save, ref SpanReader sr)
        {
            Result = (Result)sr.ReadByte();
            Unk14 = sr.ReadByte();
            sr.ReadByte();
            BestTime = sr.ReadInt32();
            RaceCode = new DbCode(sr.ReadInt32(), sr.ReadInt32());
        }
    }
}
