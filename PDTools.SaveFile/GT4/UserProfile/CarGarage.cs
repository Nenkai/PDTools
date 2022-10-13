using System;
using System.Collections.Generic;
using System.Text;

using PDTools.Structures;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.UserProfile
{
    public class CarGarage : IGameSerializeBase
    {
        public byte[] Parameter { get; set; }
        public byte[] Unk { get; set; } = new byte[0x20];
        public int Unk2 { get; set; }
        public byte[] Unk3 { get; set; } = new byte[0x0C];
        public byte[] Unk4 { get; set; } = new byte[0x20];

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteBytes(Parameter);
            sw.WriteBytes(Unk);
            sw.WriteInt32(Unk2);
            sw.WriteBytes(Unk3);
            sw.WriteBytes(Unk4);

            sw.Align(GT4Save.ALIGNMENT);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            if (save.IsGT4Retail())
                Parameter = sr.ReadBytes(0x468);
            else
                Parameter = sr.ReadBytes(0x4B0);

            Unk = sr.ReadBytes(0x20);
            Unk2 = sr.ReadInt32();
            Unk3 = sr.ReadBytes(0x0C);
            Unk4 = sr.ReadBytes(0x20);

            sr.Align(GT4Save.ALIGNMENT);
        }
    }
}
