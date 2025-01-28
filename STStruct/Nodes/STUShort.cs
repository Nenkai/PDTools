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
