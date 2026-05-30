// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using System.Diagnostics;

namespace PDTools.STStruct.Nodes;

[DebuggerDisplay("{Value} (UShort)")]
public class STUShort : NodeBase
{
    public STUShort(ushort val)
    {
        Value = val;
    }

    public ushort Value { get; set; }

    public override string ToString()
        => Value.ToString();
}
