using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libdebug;

namespace PDTools.GTPatcher.BreakLoggers;

public class AdhocExceptionLogger : IBreakLogger
{
    public const ulong GTS_V168_AdhocExceptionObject_Ctor_Offset = 0x1712AC0;
    public const ulong GT7_V100_AdhocExceptionObject_Ctor_Offset = 0x30BDBA0;
    

    public ulong Offset { get; set; }

    public Breakpoint Breakpoint { get; set; }

    public bool LogOnlyOnMiss { get; set; }

    public AdhocExceptionLogger()
    {
        
    }

    public void Init(GTPatcher dbg)
    {
        switch (dbg.GameType)
        {
            case GameType.GTS_V168:
                Offset = GTS_V168_AdhocExceptionObject_Ctor_Offset;
                break;

            case GameType.GT7_V100:
                Offset = GT7_V100_AdhocExceptionObject_Ctor_Offset;
                break;
        }

        Breakpoint = dbg.SetBreakpoint(dbg.ImageBase + Offset);
    }

    public bool CheckHit(GTPatcher dbg, GeneralRegisters registers)
    {
        if (registers.rip == dbg.ImageBase + Offset)
            return true;

        return false;
    }

    public void OnBreak(GTPatcher dbg, GeneralRegisters registers)
    {
        if (registers.rax != 0)
        {
            string message = dbg.ReadMemoryAbsolute<string>(registers.rax);
            Console.WriteLine(message);
        }
        
    }
}
