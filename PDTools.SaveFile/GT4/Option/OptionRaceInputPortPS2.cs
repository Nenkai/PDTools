using Syroot.BinaryData.Memory;

using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.SaveFile.GT4.Option
{
    public class OptionRaceInputPortPS2 : IGameSerializeBase
    {
        public byte[] Data { get; set; }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteBytes(Data);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            Data = sr.ReadBytes(0xB8);
        }
    }
}
