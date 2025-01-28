using System;
using System.Collections.Generic;
using System.Text;

using PDTools.Structures;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.UserProfile;

public class Available : IGameSerializeBase<Available>
{
    public const int MAX_CAR_IDS = 1024;
    public const int MAX_COURSE_IDS = 128;

    public DbCode[] CarIdsAvailable { get; set; } = new DbCode[MAX_CAR_IDS];
    public DbCode[] CourseIdsAvailable { get; set; } = new DbCode[MAX_COURSE_IDS];
    public byte Unk { get; set; }

    public void CopyTo(Available dest)
    {
        for (var i = 0; i < MAX_CAR_IDS; i++)
        {
            dest.CarIdsAvailable[i] = new DbCode(CarIdsAvailable[i].Code, CarIdsAvailable[i].TableId);
        }

        for (var i = 0; i < MAX_COURSE_IDS; i++)
        {
            dest.CourseIdsAvailable[i] = new DbCode(CourseIdsAvailable[i].Code, CourseIdsAvailable[i].TableId);
        }

        dest.Unk = Unk;
    }

    public void Pack(GT4Save save, ref SpanWriter sw)
    {
        for (var i = 0; i < MAX_CAR_IDS; i++)
        {
            sw.WriteInt32(CarIdsAvailable[i].Code);
            sw.WriteInt32(CarIdsAvailable[i].TableId);
        }

        for (var i = 0; i < MAX_COURSE_IDS; i++)
        {
            sw.WriteInt32(CourseIdsAvailable[i].Code);
            sw.WriteInt32(CourseIdsAvailable[i].TableId);
        }

        sw.WriteByte(Unk);

        sw.Align(GT4Save.ALIGNMENT);
    }

    public void Unpack(GT4Save save, ref SpanReader sr)
    {
        for (var i = 0; i < MAX_CAR_IDS; i++)
            CarIdsAvailable[i] = new DbCode(sr.ReadInt32(), sr.ReadInt32());

        for (var i = 0; i < MAX_COURSE_IDS; i++)
            CourseIdsAvailable[i] = new DbCode(sr.ReadInt32(), sr.ReadInt32());

        Unk = sr.ReadByte();

        sr.Align(GT4Save.ALIGNMENT);
    }
}
