using System;
using System.Collections.Generic;
using System.Text;
using Syroot.BinaryData.Memory;

using PDTools.Structures;

namespace PDTools.SaveFile.GT4.UserProfile
{
    public class Favorite : IGameSerializeBase<Favorite>
    {
        public const int MAX_ENTRIES = 32;

        public int Max { get; set; }
        public int CurrentCount { get; set; }
        public DbCode[] Codes { get; set; } = new DbCode[MAX_ENTRIES];

        public void CopyTo(Favorite dest)
        {
            dest.Max = Max;
            dest.CurrentCount = CurrentCount;

            for (var i = 0; i < MAX_ENTRIES; i++)
                dest.Codes[i] = new DbCode(Codes[i].Code, Codes[i].TableId);
        }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteInt32(Max);
            sw.WriteInt32(CurrentCount);

            for (var i = 0; i < MAX_ENTRIES; i++)
            {
                sw.WriteInt32(Codes[i].Code);
                sw.WriteInt32(Codes[i].TableId);
            }

            sw.Align(GT4Save.ALIGNMENT);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            Max = sr.ReadInt32();
            CurrentCount = sr.ReadInt32();

            for (var i = 0; i < MAX_ENTRIES; i++)
                Codes[i] = new DbCode(sr.ReadInt32(), sr.ReadInt32());

            sr.Align(GT4Save.ALIGNMENT);
        }
    }
}
