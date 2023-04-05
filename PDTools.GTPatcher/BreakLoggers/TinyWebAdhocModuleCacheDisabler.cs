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
    public class TinyWebAdhocModuleCacheDisabler : IBreakLogger
    {
        public const ulong GT7_V129_AdhocExceptionHandler_Offset = 0x1A7C6C0; // AdhocModule::executeRequest

        public ulong Offset { get; set; }
        public Breakpoint Breakpoint { get; set; }

        public TinyWebAdhocModuleCacheDisabler()
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
           // Fix adhoc cache 1.29
           // Skip caching conditions
           dbg.WriteMemory(0x1A7C70C, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 });
           dbg.WriteMemory(0x1A7C74B, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 });
           return;
        }
    }
}
