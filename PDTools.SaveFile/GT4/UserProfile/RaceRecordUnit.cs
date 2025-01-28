using Syroot.BinaryData.Memory;

using System;
using System.Collections.Generic;
using System.Text;

using PDTools.Enums.PS2;

namespace PDTools.SaveFile.GT4.UserProfile;

public class RaceRecordUnit : IGameSerializeBase<RaceRecordUnit>
{
    // 4 bits type
    // 4 bits permanent result
    // 4 bits unknown
    // 4 bits current result
    public ushort Bits { get; set; }
    public byte ASpecScore { get; set; }

    public EventType GetEventType()
    {
        return (EventType)(Bits & 0b1111); // 4 bits
    }

    public void SetEventType(EventType type)
    {
        Bits = (ushort)((Bits & 0xFFF0) | ((byte)type & 0b1111)); // 4 bits
    }

    public Result GetPermanentResult()
    {
        return (Result)((Bits >> 4) & 0b1111); // 4 bits
    }

    public Result GetUnknownLicenseOrMissionResult()
    {
        return (Result)((Bits >> 8) & 0b1111); // 4 bits
    }

    public Result GetCurrentResult()
    {
        return (Result)((Bits >> 12) & 0b1111); // 4 bits
    }

    public void SetPermanentResult(Result res)
    {
        Bits = (ushort)((Bits & 0xFF0F) | (ushort)(((byte)res & 0b1111) << 4)); // Permanent Result
    }

    public void SetLicenseOrMissionResult(Result res)
    {
        Bits = (ushort)((Bits & 0xF0FF) | (ushort)(((byte)res & 0b1111) << 8)); // Permanent Result
    }

    public void SetCurrentResult(Result res)
    {
        Bits = (ushort)((Bits & 0x0FFF) | (ushort)(((byte)res & 0b1111) << 12)); 
    }

    public void CopyTo(RaceRecordUnit dest)
    {
        dest.Bits = Bits;
        dest.ASpecScore = ASpecScore;
    }

    public void Pack(GT4Save save, ref SpanWriter sw)
    {
        sw.WriteUInt16(Bits);
        sw.WriteByte(ASpecScore);
        sw.Position += 1;
    }

    public void Unpack(GT4Save save, ref SpanReader sr)
    {
        Bits = sr.ReadUInt16();
        ASpecScore = sr.ReadByte();
        sr.Position += 1;
    }
}
