using System;
using System.Collections.Generic;
using System.Text;
using Syroot.BinaryData.Memory;

namespace PDTools.Structures.PS2;

public class AutomobileAccumulatedStatus
{
    public uint Odometer { get; set; }
    public float EngineLife { get; set; }
    public float OilLife { get; set; }
    public float Dirtiness { get; set; }
    public float BodyLife { get; set; }
    public byte Everlasting { get; set; }

    public void CopyTo(AutomobileAccumulatedStatus dest)
    {
        dest.Odometer = Odometer;
        dest.EngineLife = EngineLife;
        dest.OilLife = OilLife;
        dest.Dirtiness = Dirtiness;
        dest.BodyLife = BodyLife;
        dest.Everlasting = Everlasting;
    }

    public void Unpack(ref SpanReader sr)
    {
        Odometer = sr.ReadUInt32();
        EngineLife = sr.ReadSingle();
        OilLife = sr.ReadSingle();
        Dirtiness = sr.ReadSingle();
        BodyLife = sr.ReadSingle();
        Everlasting = sr.ReadByte();

        sr.Position += 0x0B;
    }

    public void Pack(ref SpanWriter sw)
    {
        sw.WriteUInt32(Odometer);
        sw.WriteSingle(EngineLife);
        sw.WriteSingle(OilLife);
        sw.WriteSingle(Dirtiness);
        sw.WriteSingle(BodyLife);
        sw.WriteByte(Everlasting);

        sw.Position += 0x0B;
    }
}
