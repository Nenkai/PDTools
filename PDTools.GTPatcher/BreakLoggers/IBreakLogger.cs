// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libdebug;

namespace PDTools.GTPatcher.BreakLoggers;

public interface IBreakLogger
{
    void Init(GTPatcher PS4);

    bool CheckHit(GTPatcher dbg, GeneralRegisters registers);

    void OnBreak(GTPatcher dbg, GeneralRegisters registers);
}
