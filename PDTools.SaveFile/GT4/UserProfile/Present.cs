using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.UserProfile
{
    public class Present : IGameSerializeBase
    {
        public byte[] Bits { get; set; } = new byte[0x74];

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
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
