﻿using System;
using System.Collections.Generic;
using System.Text;
using Syroot.BinaryData.Memory;

using PDTools.Structures;
using PDTools.Enums.PS2;

namespace PDTools.SaveFile.GT4.UserProfile.DayEvents;

public class RunMissionEvent : IDayEvent
{
    public DayEventType EventType => DayEventType.RUN_MISSION;

    public Result Result { get; set; }
    public byte Unk14 { get; set; }
    public byte Unk { get; set; }
    public int BestTime { get; set; }
    public DbCode RaceCode { get; set; }

    public void CopyTo(IDayEvent dest)
    {
        ((RunMissionEvent)dest).Result = Result;
        ((RunMissionEvent)dest).Unk14 = Unk14;
        ((RunMissionEvent)dest).Unk = Unk;
        ((RunMissionEvent)dest).BestTime = BestTime;
        ((RunMissionEvent)dest).RaceCode = new DbCode(RaceCode.Code, RaceCode.TableId);
    }

    public void Pack(GT4Save save, ref SpanWriter sw)
    {
        sw.WriteByte((byte)Result);
        sw.WriteByte(Unk14);
        sw.WriteByte(Unk);
        sw.WriteInt32(BestTime);
        sw.WriteInt32(RaceCode.Code);
        sw.WriteInt32(RaceCode.TableId);
    }

    public void Unpack(GT4Save save, ref SpanReader sr)
    {
        Result = (Result)sr.ReadByte();
        Unk14 = sr.ReadByte();
        Unk = sr.ReadByte();
        BestTime = sr.ReadInt32();
        RaceCode = new DbCode(sr.ReadInt32(), sr.ReadInt32());
    }
}
