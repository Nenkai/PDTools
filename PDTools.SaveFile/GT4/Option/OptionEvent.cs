using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

using PDTools.Utils;

namespace PDTools.SaveFile.GT4.Option
{
    public class OptionEvent : IGameSerializeBase
    {
        public string EventName { get; set; }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteStringFix(EventName, 0x50);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            EventName = sr.ReadFixedString(0x50);

            // No align
        }
    }
}
