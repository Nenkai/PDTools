using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.Option
{
    public class ColorCorrection : IGameSerializeBase
    {
        public int brightness { get; set; }
        public int contrast { get; set; }
        public int saturation { get; set; }
        public int color_balance { get; set; }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteInt32(brightness);
            sw.WriteInt32(contrast);
            sw.WriteInt32(saturation);
            sw.WriteInt32(color_balance);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            brightness = sr.ReadInt32();
            contrast = sr.ReadInt32();
            saturation = sr.ReadInt32();
            color_balance = sr.ReadInt32();
        }
    }
}
