// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.VM
{
    public class RegisterInfo
    {
        public string? Name { get; set; }
        public ushort ArrayLength { get; set; }
        public ushort RegisterIndex { get; set; }

        public void FromStream(BinaryStream bs, long baseMdlPos)
        {
            uint nameOffset = bs.ReadUInt32();
            ArrayLength = bs.ReadUInt16();
            RegisterIndex = bs.ReadUInt16();

            Debug.Assert(nameOffset != 0);
            bs.Position = baseMdlPos + nameOffset;
            Name = bs.ReadString(StringCoding.ZeroTerminated);
        }

        public static int GetSize()
        {
            return 0x08;
        }

        public override string ToString()
        {
            return $"{Name} (Index: {RegisterIndex}, ArrayLength: {ArrayLength})";
        }
    }
}
