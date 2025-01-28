using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libdebug;

namespace PDTools.GTPatcher.BreakLoggers;

public class EvalExpressionStringLogger : IBreakLogger
{
    // Address for debugging adhoc compilation exceptions from EvalExpressionString
    public const ulong GT7_V129_Offset = 0x26B5A4B;

    public ulong Offset { get; set; }
    public Breakpoint Breakpoint { get; set; }

    public bool LogOnlyOnMiss { get; set; }

    public EvalExpressionStringLogger()
    {

    }

    public void Init(GTPatcher dbg)
    {
        switch (dbg.GameType)
        { 
            case GameType.GT7_V129:
                Offset = GT7_V129_Offset;
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
        string err = dbg.ReadMemoryAbsolute<string>(dbg.ReadMemoryAbsolute<ulong>(registers.rsi + 8));
        Console.WriteLine(err);

        /*
        if (!dbg._breakLoggers.Any(e => e is EvalExpressionCompilationTokenTypeLogger))
        {
            var log = new EvalExpressionCompilationTokenTypeLogger();
            log.Init(dbg);
            dbg._breakLoggers.Add(log);
        }
        */
    }
}
