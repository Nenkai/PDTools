using PDTools.Enums.PS2;

using Syroot.BinaryData.Memory;

using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.SaveFile.GT4.Option
{
    public class GameZone : IGameSerializeBase<GameZone>
    {
        public GameZoneType GameZoneType { get; set; }

        public void CopyTo(GameZone dest)
        {
            dest.GameZoneType = GameZoneType;
        }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteInt32((int)GameZoneType);

            sw.Align(GT4Save.ALIGNMENT);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            GameZoneType = (GameZoneType)sr.ReadInt32();

            sr.Align(GT4Save.ALIGNMENT);
        }
    }
}
