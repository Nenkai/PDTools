﻿using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

using PDTools.Utils;

namespace PDTools.SaveFile.GT4.Option
{
    public class OptionLogger : IGameSerializeBase
    {
        public byte[] Loggers { get; set; }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteBytes(Loggers);

            sw.Align(GT4Save.ALIGNMENT);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            Loggers = sr.ReadBytes(5);

            sr.Align(GT4Save.ALIGNMENT);
        }
    }
}
