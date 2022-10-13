using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.UserProfile
{
    public class ChampionshipContext : IGameSerializeBase
    {
        public byte[] Data { get; set; } = new byte[0x88];

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteBytes(Data);

            sw.Align(GT4Save.ALIGNMENT);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            Data = sr.ReadBytes(0x88);

            sr.Align(GT4Save.ALIGNMENT);
        }
    }
}
