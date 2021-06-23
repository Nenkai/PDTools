using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace PDTools.STStruct.Nodes
{
    [DebuggerDisplay("{Value} (ULong)")]
    public class STULong : NodeBase
    {
        public STULong(ulong val)
        {
            Value = val;
        }

        public ulong Value { get; set; }

        public override string ToString()
            => Value.ToString();
    }
}
