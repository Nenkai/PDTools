using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.UserProfile
{
    public class Present : IGameSerializeBase
    {
        public const int CourseIndexStart = 866;

        public byte[] Bits { get; set; } = new byte[0x74];

        public void SetUnlocked(int idx, bool unlocked)
        {
            if (unlocked)
                Bits[idx / 8] |= (byte)(1 << (idx % 8));
            else
                Bits[idx / 8] &= (byte)~(1 << (idx % 8));
        }

        public bool IsUnlocked(int idx)
        {
            return ((Bits[idx / 8] >> (idx % 8)) & 1) == 1;
        }

        public void SetCarUnlocked(int idx, bool unlocked)
        {
            SetUnlocked(idx, unlocked);
        }

        public bool IsCourseUnlocked(int idx)
        {
            return IsUnlocked(CourseIndexStart + idx);
        }

        public void SetCourseUnlocked(int idx, bool unlocked)
        {
            SetUnlocked(CourseIndexStart + idx, unlocked);
        }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            Bits.AsSpan().Fill(0xFF);
            sw.WriteBytes(Bits);

            sw.Align(GT4Save.ALIGNMENT);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            Bits = sr.ReadBytes(0x74);

            sr.Align(GT4Save.ALIGNMENT);
        }
    }
}
