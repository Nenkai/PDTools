using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

using PDTools.Utils;

namespace PDTools.SaveFile.GT4.Option
{
    public class OptionNetConfig : IGameSerializeBase<OptionNetConfig>
    {
        public byte[] Data { get; set; }

        public void CopyTo(OptionNetConfig dest)
        {
            dest.Data = new byte[Data.Length];
            Array.Copy(Data, dest.Data, Data.Length);
        }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteBytes(Data);
            sw.Position += 8;

            // No align
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            Data = sr.ReadBytes(0x258);
            sr.Position += 0x08;

            // No align
        }
    }
}
