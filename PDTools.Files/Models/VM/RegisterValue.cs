// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.VM
{
    public struct RegisterVal
    {
        public int Value { get; set; }

        public RegisterVal(int value)
        {
            Value = value;
        }
    }
}
