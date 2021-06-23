using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.STStruct
{
    public abstract class NodeBase
    {
        public NodeType Type { get; set; }
    }

    public enum NodeType
    {
        Null,
        SByte,
        Short,
        Int,
        Long,
        Float,
        MBlob,
        String,
        Array,
        Map,
        Object,
        Bool,
        UByte,
        UShort,
        UInt = 0x0E,
        Double = 0x0F,

    }
}
