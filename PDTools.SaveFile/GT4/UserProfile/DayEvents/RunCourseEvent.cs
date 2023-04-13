using System;
using System.Collections.Generic;
using System.Text;
using Syroot.BinaryData.Memory;

using PDTools.Structures;
using PDTools.Enums.PS2;

namespace PDTools.SaveFile.GT4.UserProfile.DayEvents
{
    public class RunCourseEvent : IDayEvent
    {
        public DayEventType EventType => DayEventType.RUN_COURSE;

        public byte Unk { get; set; }
        public Result Result { get; set; }
        public RunCourseMode CourseMode { get; set; }
        public int BestTime { get; set; }
        public DbCode CourseCode { get; set; }

        public void CopyTo(IDayEvent dest)
        {
            ((RunCourseEvent)dest).Unk = Unk;
            ((RunCourseEvent)dest).Result = Result;
            ((RunCourseEvent)dest).CourseMode = CourseMode;
            ((RunCourseEvent)dest).BestTime = BestTime;
            ((RunCourseEvent)dest).CourseCode = new DbCode(CourseCode.Code, CourseCode.TableId);
        }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteByte(Unk);
            sw.WriteByte((byte)Result);
            sw.WriteByte((byte)CourseMode);
            sw.WriteInt32(BestTime);
            sw.WriteInt32(CourseCode.Code);
            sw.WriteInt32(CourseCode.TableId);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            Unk = sr.ReadByte();
            Result = (Result)sr.ReadByte();
            CourseMode = (RunCourseMode)sr.ReadByte();
            BestTime = sr.ReadInt32();
            CourseCode = new DbCode(sr.ReadInt32(), sr.ReadInt32());
        }
    }
}
