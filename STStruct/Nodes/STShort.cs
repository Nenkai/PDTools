using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace PDTools.STStruct.Nodes
{
    [DebuggerDisplay("{Value} (Short)")]
    public class STShort : NodeBase
    {
        public STShort(short val)
        {
            Value = val;
        }

        public short Value { get; set; }

        public override string ToString()
            => Value.ToString();
    }
}
