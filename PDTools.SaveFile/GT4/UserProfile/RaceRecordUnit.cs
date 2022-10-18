using Syroot.BinaryData.Memory;

using System;
using System.Collections.Generic;
using System.Text;

using PDTools.Enums.PS2;

namespace PDTools.SaveFile.GT4.UserProfile
{
    public class RaceRecordUnit : IGameSerializeBase
    {
        public ushort Bits { get; set; }
        public byte ASpecPoints { get; set; }

        public EventType GetEventType()
        {
            return (EventType)(Bits & 0b1111); // 4 bits
        }

        public Result GetPermanentResult()
        {
            return (Result)((Bits >> 4) & 0b1111); // 4 bits
        }

        public Result GetCurrentResult()
        {
            return (Result)((Bits >>8) & 0b1111); // 4 bits
        }

        public void SetPermanentResult(Result res)
        {
            Bits = (ushort)((Bits & 0xFF0F) | (ushort)(((byte)res & 0b1111) << 4)); // Permanent Result
        }

        public void SetCurrentResult(Result res)
        {
            Bits = (ushort)((Bits & 0xF0FF) | (ushort)(((byte)res & 0b1111) << 8)); 
        }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteUInt16(Bits);
            sw.WriteByte(ASpecPoints);
            sw.Position += 1;
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            Bits = sr.ReadUInt16();
            ASpecPoints = sr.ReadByte();
            sr.Position += 1;
        }
    }
}
