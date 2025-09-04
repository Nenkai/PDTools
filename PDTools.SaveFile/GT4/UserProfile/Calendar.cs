using System;
using System.Collections.Generic;
using System.Text;
using Syroot.BinaryData.Memory;

using PDTools.SaveFile.GT4.UserProfile.DayEvents;
using PDTools.Enums.PS2;
using PDTools.Structures;

namespace PDTools.SaveFile.GT4.UserProfile;

public class Calendar : IGameSerializeBase<Calendar>
{
    public const int MAX_EVENTS = 2922;

    public int Days { get; set; }
    public IDayEvent[] Events { get; set; } = new IDayEvent[MAX_EVENTS];
    public DateTime Date { get; set; }

    public void CopyTo(Calendar dest)
    {
        dest.Days = Days;

        for (var i = 0; i < dest.Events.Length; i++)
        {
            dest.Events[i] = (IDayEvent)Activator.CreateInstance(Events[i].GetType())!;
            Events[i].CopyTo(dest.Events[i]);
        }

        dest.Date = Date;
    }

    public void Pack(GT4Save save, ref SpanWriter sw)
    {
        sw.WriteInt32(Days);

        for (var i = 0; i < MAX_EVENTS; i++)
        {
            IDayEvent @event = Events[i];
            sw.WriteByte((byte)@event.EventType);
            @event.Pack(save, ref sw);

        }

        sw.WriteInt32(PDIDATETIME.DateTimeToDay(Date));

        sw.Align(GT4Save.ALIGNMENT);
    }

    public static DateTime GetOriginDate()
    {
        return new DateTime(2005, 4, 2);
    }

    public void Unpack(GT4Save save, ref SpanReader sr)
    {
        Days = sr.ReadInt32();

        for (var i = 0; i < MAX_EVENTS; i++)
        {
            DayEventType type = (DayEventType)sr.ReadByte();
            IDayEvent @event;
            switch (type)
            {
                case DayEventType.NO_EVENT:
                    @event = new NoEvent();
                    @event.Unpack(save, ref sr);
                    break;

                case DayEventType.GET_CAR:
                    @event = new GetCarEvent();
                    @event.Unpack(save, ref sr);
                    break;
                case DayEventType.SELL_CAR:
                    @event = new SellCarEvent();
                    @event.Unpack(save, ref sr);
                    break;
                case DayEventType.RUN_RACE:
                    @event = new RunRaceEvent();
                    @event.Unpack(save, ref sr);
                    break;
                case DayEventType.RUN_LICENSE:
                    @event = new RunLicenseEvent();
                    @event.Unpack(save, ref sr);
                    break;
                case DayEventType.RUN_MISSION:
                    @event = new RunMissionEvent();
                    @event.Unpack(save, ref sr);
                    break;
                case DayEventType.BUY_WHEEL:
                    @event = new BuyWheelEvent();
                    @event.Unpack(save, ref sr);
                    break;
                case DayEventType.BUY_WING:
                    @event = new BuyWingEvent();
                    @event.Unpack(save, ref sr);
                    break;
                case DayEventType.RUN_COURSE:
                    @event = new RunCourseEvent();
                    @event.Unpack(save, ref sr);
                    break;

                default:
                    throw new Exception($"Unsupported day event type: {type}");
            }

            Events[i] = @event;
        }

        Date = PDIDATETIME.DayToDateTime(sr.ReadInt32());

        if (Date < GetOriginDate())
            Date = GetOriginDate(); // Reset to origin

        sr.Align(GT4Save.ALIGNMENT);
    }
}
