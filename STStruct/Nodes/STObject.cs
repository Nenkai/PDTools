using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace PDTools.STStruct.Nodes;

[DebuggerDisplay("Object")]
public class STObject : NodeBase
{
    public NodeBase Child;
}
