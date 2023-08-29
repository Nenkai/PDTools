using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libdebug;

namespace PDTools.GTPatcher.BreakLoggers
{
    // Prints each token lexing token numbers when using EvalExpressionString
    // Should be applied after the game has gone after the PS Studio movie - as tons of scripts are compiled before that moment
    public class EvalExpressionCompilationTokenTypeLogger : IBreakLogger
    {
        public const ulong GT7_V129_Offset = 0xEB7F40;

        public ulong Offset { get; set; }
        public Breakpoint Breakpoint { get; set; }

        public bool LogOnlyOnMiss { get; set; }

        public EvalExpressionCompilationTokenTypeLogger()
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
            if (registers.rip == dbg.ImageBase + Offset )
                return true;

            return false;
        }

        public void OnBreak(GTPatcher dbg, GeneralRegisters registers)
        {
            string tokType = "Token: " + dbg.ReadMemoryAbsolute<ulong>(registers.rdi);
            Console.WriteLine(tokType);
        }
    }
}
