using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libdebug;

namespace PDTools.GTPatcher.MemoryPatches;

public interface IMemoryPatch
{
    void Init(GTPatcher dbg);

    void OnAttach(GTPatcher dbg);

    void Patch(GTPatcher dbg, GeneralRegisters regs);
}
