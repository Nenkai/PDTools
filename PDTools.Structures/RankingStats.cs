using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Memory;
using Syroot.BinaryData.Core;

using PDTools.Enums.PS3;

namespace PDTools.Structures;

public class RankingStats
{
    public byte SpecialGuest { get; set; }
    public byte Transmission { get; set; }
    public byte Professional { get; set; }
    public byte ASM { get; set; }
    public byte ActiveSteering { get; set; }
    public byte TCS { get; set; }
    public byte ABS { get; set; }
    public byte FrontTire { get; set; }
    public byte RearTire { get; set; }
    public string CountryCode { get; set; }
    public byte Paint { get; set; }
    public AutomobileControllerType ControllerType { get; set; }
    public byte Color { get; set; }
    public long CarCode { get; set; }
    public string SpecialGuestName { get; set; }

    // Based on "UnpackRankingStats(d[1])"
    public static RankingStats Read(Span<byte> stats)
    {
        SpanReader sr = new SpanReader(stats, Endian.Big);
        byte version = sr.ReadByte();
        if (version > 2)
            return null;

        RankingStats rStats = new RankingStats();

        byte blobSize = sr.ReadByte();
        byte versionAgain = sr.ReadByte();

        rStats.SpecialGuest = sr.ReadByte();
        rStats.Transmission = sr.ReadByte();
        rStats.Professional = sr.ReadByte();

        byte bits1 = sr.ReadByte();
        rStats.ASM = (byte)(bits1 & 0b_1111);
        rStats.ActiveSteering = (byte)(bits1 >> 4);

        byte bits2 = sr.ReadByte();
        rStats.TCS = (byte)(bits2 & 0b_1111);
        rStats.ASM = (byte)(bits2 >> 4);

        sr.ReadByte(); // Intentional

        rStats.FrontTire = sr.ReadByte();
        rStats.RearTire = sr.ReadByte();

        rStats.CountryCode = sr.ReadStringRaw(2);

        byte paintBits = sr.ReadByte();
        //rStats.painted = paintBits & 0b_01111111;
        rStats.Paint   = (byte)(paintBits & 0b_10000000);

        rStats.ControllerType = (AutomobileControllerType)sr.ReadByte();

        rStats.Color = sr.ReadByte(); // 15

        rStats.CarCode = sr.ReadInt64();

        if (rStats.SpecialGuest != 0)
        {
            // Originally read that way
            rStats.SpecialGuestName = sr.ReadStringRaw(blobSize - 24);
        }

        return rStats;
    }
}
