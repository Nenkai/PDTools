using System;
using System.Collections.Generic;
using System.Text;

using PDTools.Enums.PS2;
using PDTools.Utils;
using PDTools.Structures;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.UserProfile;

public class LicenseEntryUnit : IGameSerializeBase<LicenseEntryUnit>
{
    public int RaceTime { get; set; }
    public int Date { get; set; }
    public Result Result { get; set; }

    public const int MAX_PASSCODE_LEN = 32;
    public string PassCode { get; set; }
    public byte[] Data = new byte[3];

    public void CopyTo(LicenseEntryUnit dest)
    {
        dest.RaceTime = RaceTime;
        dest.Date = Date;
        dest.Result = Result;
        dest.PassCode = PassCode;

        Array.Copy(Data, dest.Data, Data.Length);
    }

    public void Pack(GT4Save save, ref SpanWriter sw)
    {
        sw.WriteInt32(RaceTime);
        sw.WriteInt32(Date);
        sw.WriteByte((byte)Result);
        sw.WriteStringFix(PassCode, MAX_PASSCODE_LEN);
        sw.Position += 3;
    }

    public void Unpack(GT4Save save, ref SpanReader sr)
    {
        RaceTime = sr.ReadInt32();
        Date = sr.ReadInt32();
        Result = (Result)sr.ReadByte();
        PassCode = sr.ReadFixedString(MAX_PASSCODE_LEN);
        sr.Position += 3;
    }


}
