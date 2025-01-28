using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace PDTools.STStruct.Nodes;

[DebuggerDisplay("{Value} (Byte)")]
public class STByte : NodeBase
{
    public STByte(byte val)
    {
        Value = val;
    }

    public byte Value { get; set; }

    public override string ToString()
        => Value.ToString();
}
