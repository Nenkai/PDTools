using Syroot.BinaryData.Memory;

using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.SaveFile.GT4.Option
{
    public class GameZone : IGameSerializeBase
    {
        public int GameZoneType { get; set; }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteInt32(GameZoneType);

            sw.Align(GT4Save.ALIGNMENT);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            GameZoneType = sr.ReadInt32();

            sr.Align(GT4Save.ALIGNMENT);
        }
    }
}
