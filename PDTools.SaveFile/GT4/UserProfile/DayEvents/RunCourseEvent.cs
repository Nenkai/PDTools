using System;
using System.Collections.Generic;
using System.Text;
using Syroot.BinaryData.Memory;

using PDTools.Structures;
using PDTools.Enums.PS2;

namespace PDTools.SaveFile.GT4.UserProfile.DayEvents
{
    public class RunCourseEvent : DayEvent, IGameSerializeBase
    {
        public override DayEventType EventType => DayEventType.RUN_COURSE;

        public byte Unk { get; set; }
        public Result Result { get; set; }
        public RunCourseMode CourseMode { get; set; }
        public int BestTime { get; set; }
        public DbCode CourseCode { get; set; }

        public override void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteByte(Unk);
            sw.WriteByte((byte)Result);
            sw.WriteByte((byte)CourseMode);
            sw.WriteInt32(BestTime);
            sw.WriteInt32(CourseCode.Code);
            sw.WriteInt32(CourseCode.TableId);
        }

        public override void Unpack(GT4Save save, ref SpanReader sr)
        {
            Unk = sr.ReadByte();
            Result = (Result)sr.ReadByte();
            CourseMode = (RunCourseMode)sr.ReadByte();
            BestTime = sr.ReadInt32();
            CourseCode = new DbCode(sr.ReadInt32(), sr.ReadInt32());
        }
    }
}
