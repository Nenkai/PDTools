// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace PDTools.STStruct.Nodes;

[DebuggerDisplay("byte[{Data.Length}]")]
public class MBlob : NodeBase
{
    public MBlob(Memory<byte> data)
    {
        Data = data;
    }
    public Memory<byte> Data { get; set; }
}
