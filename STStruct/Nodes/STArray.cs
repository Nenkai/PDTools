// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace PDTools.STStruct.Nodes;

[DebuggerDisplay("Array[{Elements.Count}]")]
public class STArray : NodeBase
{
    public List<NodeBase> Elements { get; set; }
}
