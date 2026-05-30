// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace PDTools.STStruct.Nodes;

[DebuggerDisplay("{Value} (Float)")]
public class STFloat : NodeBase
{
    public STFloat(float val)
    {
        Value = val;
    }

    public float Value { get; set; }

    public override string ToString()
        => Value.ToString();
}
