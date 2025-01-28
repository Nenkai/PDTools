using System;
using System.Collections.Generic;
using System.Text;

using PDTools.Enums.PS2;
using PDTools.Utils;
using PDTools.Structures;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.UserProfile;

public class CourseEntryUnit : IGameSerializeBase<CourseEntryUnit>
{
    public int RaceTime { get; set; }
    public int Date { get; set; }
    public DbCode CarCode { get; set; }

    public const int MAX_PASSCODE_LEN = 32;
    public string PassCode { get; set; }
    public byte[] Data = new byte[3];

    public void CopyTo(CourseEntryUnit dest)
    {
        dest.RaceTime = RaceTime;
        dest.Date = Date;
        dest.CarCode = new DbCode(CarCode.Code, CarCode.TableId);
        dest.PassCode = PassCode;
        Array.Copy(Data, dest.Data, Data.Length);
    }

    public void Pack(GT4Save save, ref SpanWriter sw)
    {
        sw.WriteInt32(RaceTime);
        sw.WriteInt32(Date);
        sw.WriteInt32(CarCode.Code);
        sw.WriteInt32(CarCode.TableId);
        sw.WriteStringFix(PassCode, MAX_PASSCODE_LEN);
    }

    public void Unpack(GT4Save save, ref SpanReader sr)
    {
        RaceTime = sr.ReadInt32();
        Date = sr.ReadInt32();
        CarCode = new DbCode(sr.ReadInt32(), sr.ReadInt32());
        PassCode = sr.ReadFixedString(MAX_PASSCODE_LEN);
    }


}
