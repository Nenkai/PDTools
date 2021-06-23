using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace PDTools.STStruct.Nodes
{
    [DebuggerDisplay("{Value} (UInt)")]
    public class STUInt : NodeBase
    {
        public STUInt(uint val)
        {
            Value = val;
        }

        public uint Value { get; set; }

        public override string ToString()
            => Value.ToString();
    }
}
