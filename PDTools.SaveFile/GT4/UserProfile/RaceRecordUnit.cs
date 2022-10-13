using Syroot.BinaryData.Memory;

using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.SaveFile.GT4.UserProfile
{
    public class RaceRecordUnit : IGameSerializeBase
    {
        public int Unk { get; set; }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteInt32(Unk);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            Unk = sr.ReadInt32();
        }
    }
}
