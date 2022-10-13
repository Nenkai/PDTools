using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.UserProfile
{
    public class UsedCar
    {
        public byte[] Bits { get; set; } = new byte[0x20];
        public int Week { get; set; }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteBytes(Bits);
            sw.WriteInt32(Week);

            sw.Align(GT4Save.ALIGNMENT);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            Bits = sr.ReadBytes(0x20);
            Week = sr.ReadInt32();

            sr.Align(GT4Save.ALIGNMENT);
        }
    }
}
