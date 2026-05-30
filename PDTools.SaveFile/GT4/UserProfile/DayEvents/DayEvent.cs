// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Text;
using Syroot.BinaryData.Memory;

using PDTools.Enums.PS2;

using PDTools.Structures;

namespace PDTools.SaveFile.GT4.UserProfile.DayEvents;

public interface IDayEvent : IGameSerializeBase<IDayEvent>
{
    DayEventType EventType { get; }
}
