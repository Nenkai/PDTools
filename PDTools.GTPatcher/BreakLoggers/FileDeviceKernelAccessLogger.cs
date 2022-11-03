using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libdebug;

namespace PDTools.GTPatcher.BreakLoggers
{
    public class FileDeviceKernelAccessLogger : IBreakLogger
    {
        public const int SceKernelStatResult_Offset = 0x1C06416;

        public Breakpoint Breakpoint { get; set; }

        public bool LogOnlyOnMiss { get; set; }

        public FileDeviceKernelAccessLogger(bool logOnlyOnMiss = false)
        {
            LogOnlyOnMiss = logOnlyOnMiss;
        }

        public void Init(GTPatcher dbg)
        {
            // Why does the game crash when commenting this out?
            Breakpoint = dbg.SetBreakpoint(dbg.ImageBase + SceKernelStatResult_Offset);
        }

        public bool CheckHit(GTPatcher dbg, GeneralRegisters registers)
        {
            if (registers.rip == dbg.ImageBase + SceKernelStatResult_Offset)
                return true;

            return false;
        }

        public async Task OnBreak(GTPatcher dbg, GeneralRegisters registers)
        {
            if (!LogOnlyOnMiss)
            {
                string fileName = await dbg.ReadMemoryAbsolute<string>(registers.rdi);
                Console.WriteLine($"{fileName}");
            }
            else
            {
                if (registers.rax != 0)
                {
                    string fileName = await dbg.ReadMemoryAbsolute<string>(registers.rdi);
                    Console.WriteLine($"Missing: {fileName} (err: 0x{registers.rax:X8})");
                }
            }
        }
    }
}
