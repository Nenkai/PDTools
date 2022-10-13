using Syroot.BinaryData.Memory;

using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.SaveFile.GT4.UserProfile
{
    public class CourseRecordBase : IGameSerializeBase
    {
        public CourseRecordUnit[] Records { get; set; }

        public CourseRecordBase(int size)
        {
            Records = new CourseRecordUnit[size];
        }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            for (var i = 0; i < Records.Length; i++)
                Records[i].Pack(save, ref sw);

            sw.Align(GT4Save.ALIGNMENT);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            for (var i = 0; i < Records.Length; i++)
            {
                Records[i] = new CourseRecordUnit();
                Records[i].Unpack(save, ref sr);
            }

            sr.Align(GT4Save.ALIGNMENT);
        }
    }
}
