
using System;
using System.Collections.Generic;
using System.Text;

using PDTools.Enums.PS2;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.UserProfile.DayEvents
{
    public abstract class DayEvent : IGameSerializeBase
    {
        public abstract DayEventType EventType { get; }

        public abstract void Pack(GT4Save save, ref SpanWriter sw);
        public abstract void Unpack(GT4Save save, ref SpanReader sr);
    }
}
