// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Sound.Ssqt.Meta;

public class SqSetTempoEvent : ISqMeta
{
    public uint UsecPerQuarterNote { get; set; }

    public void Read(BinaryStream bs)
    {
        UsecPerQuarterNote = (uint)(bs.ReadByte() << 16 | bs.Read1Byte() << 8 | bs.Read1Byte());
    }
}
