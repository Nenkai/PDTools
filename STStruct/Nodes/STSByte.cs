// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace PDTools.STStruct.Nodes;

[DebuggerDisplay("{Value} (SByte)")]
public class STSByte : NodeBase
{
    public STSByte(sbyte val)
    {
        Value = val;
    }

    public sbyte Value { get; set; }

    public override string ToString()
        => Value.ToString();
}
