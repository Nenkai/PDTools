using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace PDTools.STStruct.Nodes
{
    [DebuggerDisplay("{Name} (String)")]
    public class STString : NodeBase
    {
        public string Name { get; set; }
    }
}
