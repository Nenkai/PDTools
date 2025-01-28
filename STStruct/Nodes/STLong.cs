using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace PDTools.STStruct.Nodes;

[DebuggerDisplay("{Value} (Long)")]
public class STLong : NodeBase
{
    public STLong(long val)
    {
        Value = val;
    }

    public long Value { get; set; }

    public override string ToString()
        => Value.ToString();
}
