using Syroot.BinaryData.Memory;

using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.SaveFile.GT4.Option
{
    public class BGMPlayData : IGameSerializeBase
    {
        public ulong UniqueID { get; set; }
        public byte Unk { get; set; }
        public byte UnkCount { get; set; }
        public byte UnkIndex1 { get; set; }
        public byte UnkIndex2 { get; set; }
        public uint Unused { get; set; }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteUInt64(UniqueID);
            sw.WriteByte(Unk);
            sw.WriteByte(UnkCount);
            sw.WriteByte(UnkIndex1);
            sw.WriteByte(UnkIndex2);
            sw.WriteUInt32(Unused);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            UniqueID = sr.ReadUInt64();
            Unk = sr.ReadByte();
            UnkCount = sr.ReadByte();
            UnkIndex1 = sr.ReadByte();
            UnkIndex2 = sr.ReadByte();
            Unused = sr.ReadUInt32();
        }
    }
}
