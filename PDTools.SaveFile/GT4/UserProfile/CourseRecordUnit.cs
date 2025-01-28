using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PDTools.Structures;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.UserProfile;

public class CourseRecordUnit : IGameSerializeBase<CourseRecordUnit>
{
    public DbCode CourseCode { get; set; }
    public CourseEntryUnit[] Entries { get; set; } = new CourseEntryUnit[10];
    public int[] SectionTimes { get; set; } = new int[16];
    public byte[] DriverExp { get; set; } = new byte[8];

    public void CopyTo(CourseRecordUnit dest)
    {
        dest.CourseCode = new DbCode(CourseCode.Code, CourseCode.TableId);

        for (var i = 0; i < Entries.Length; i++)
        {
            dest.Entries[i] = new CourseEntryUnit();
            Entries[i].CopyTo(dest.Entries[i]);
        }

        Array.Copy(SectionTimes, dest.SectionTimes, SectionTimes.Length);
        Array.Copy(DriverExp, dest.DriverExp, DriverExp.Length);
    }

    public void Pack(GT4Save save, ref SpanWriter sw)
    {
        sw.WriteInt32(CourseCode.Code);
        sw.WriteInt32(CourseCode.TableId);

        for (var i = 0; i < 10; i++)
            Entries[i].Pack(save, ref sw);

        for (var i = 0; i < 16; i++)
            sw.WriteInt32(SectionTimes[i]);

        for (var i = 0; i < 8; i++)
            sw.WriteByte(DriverExp[i]);

        sw.Position += 1;
        sw.Position += 7;
    }

    public void Unpack(GT4Save save, ref SpanReader sr)
    {
        CourseCode = new DbCode(sr.ReadInt32(), sr.ReadInt32());

        for (var i = 0; i < 10; i++)
        {
            Entries[i] = new CourseEntryUnit();
            Entries[i].Unpack(save, ref sr);
        }

        for (var i = 0; i < 16; i++)
            SectionTimes[i] = sr.ReadInt32();

        for (var i = 0; i < 8; i++)
            DriverExp[i] = sr.ReadByte();

        sr.ReadByte();
        sr.Position += 7;
    }
}
