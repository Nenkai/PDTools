using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libdebug;

namespace PDTools.GTPatcher.BreakLoggers
{
    /// <summary>
    /// For GT7/Vegas
    /// </summary>
    public class AdhocExceptionFixer : IBreakLogger
    {
        public const ulong GT7_V129_AdhocExceptionHandler_Offset = 0x26B6090;

        public ulong Offset { get; set; }
        public Breakpoint Breakpoint { get; set; }

        public AdhocExceptionFixer()
        {
            
        }

        public void Init(GTPatcher dbg)
        {
            switch (dbg.GameType)
            {
                case GameType.GT7_V129:
                    Offset = GT7_V129_AdhocExceptionHandler_Offset;
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
            if (Offset == GT7_V129_AdhocExceptionHandler_Offset)
            {
                // Set report callback to vegas to null, otherwise it crashes 
                dbg.WriteMemory<ulong>(0x5CE4028, 0);
                return;
            }

        }
    }
}
