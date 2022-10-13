using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

using PDTools.Utils;

namespace PDTools.SaveFile.GT4.Option
{
    public class OptionLANBattle : IGameSerializeBase
    {
        public int Unk { get; set; }
        public string Name1 { get; set; }
        public int Style { get; set; }
        public string Name2 { get; set; }
        public int Style2 { get; set; }

        public byte[] Data { get; set; }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteInt32(Unk);

            sw.WriteStringFix(Name1, 0x40);
            sw.WriteInt32(Style);

            sw.WriteStringFix(Name2, 0x40);
            sw.WriteInt32(Style2);

            sw.WriteBytes(Data);

            // No align
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            Unk = sr.ReadInt32();

            Name1 = sr.ReadFixedString(0x40);
            Style = sr.ReadInt32();

            Name2 = sr.ReadFixedString(0x40);
            Style2 = sr.ReadInt32();

            Data = sr.ReadBytes(0x24);

            // No align
        }
    }
}
