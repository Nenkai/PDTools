using System;
using System.Collections.Generic;
using System.Text;

using PDTools.Structures;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.UserProfile
{
    public class GarageCar : IGameSerializeBase
    {
        public DbCode CarCode { get; set; }
        public ulong Flags { get; set; }
        public int Unk { get; set; }
        public int Odometer { get; set; }
        public ulong Flags2 { get; set; }

        public bool IsValid()
        {
            return (Flags & 1) != 0; // First bit
        }

        public bool GetUnkBit()
        {
            return ((Flags >> 1) & 1) != 0;
        }

        public uint GetVariationIndex()
        {
            return (uint)((Flags >> 2) & 0b1111111); // 7 bits
        }

        public uint GetShowroomPower()
        {
            return (uint)((Flags >> 9) & 0b11_11111111_11111111); // 18 bits
        }

        public uint GetShowroomWeight()
        {
            return (uint)((Flags >> 32) & 0b111_11111111); // 11 bits
        }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteInt32(CarCode.Code);
            sw.WriteInt32(CarCode.TableId);
            sw.WriteUInt64(Flags);
            sw.WriteInt32(Unk);
            sw.WriteInt32(Odometer);
            sw.WriteUInt64(Flags2);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            CarCode = new DbCode(sr.ReadInt32(), sr.ReadInt32());
            Flags = sr.ReadUInt64();
            Unk = sr.ReadInt32();
            Odometer = sr.ReadInt32();
            Flags2 = sr.ReadUInt64();
        }

    }
}
