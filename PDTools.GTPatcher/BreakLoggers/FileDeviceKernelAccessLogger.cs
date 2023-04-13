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
        public const ulong GTS_V168_SceKernelStatResult_Offset = 0x1C06416;
        public const ulong GT7_V100_SceKernelStatResult_Offset = 0x3A215E0;
        public const ulong GT7_V129_SceKernelStatResult_Offset = 0x2D59884;

        public ulong Offset { get; set; }
        public Breakpoint Breakpoint { get; set; }

        public bool LogOnlyOnMiss { get; set; }

        public FileDeviceKernelAccessLogger(bool logOnlyOnMiss = false)
        {
            LogOnlyOnMiss = logOnlyOnMiss;
        }

        public void Init(GTPatcher dbg)
        {
            switch (dbg.GameType)
            {
                case GameType.GTS_V168:
                    Offset = GTS_V168_SceKernelStatResult_Offset;
                    break;

                case GameType.GT7_V100:
                    Offset = 0x3A215E0;
                    break;

                case GameType.GT7_V129:
                    Offset = GT7_V129_SceKernelStatResult_Offset;
                    break;
            }

            // Why does the game crash when commenting this out?
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
            if (!LogOnlyOnMiss)
            {
                string fileName = dbg.ReadMemoryAbsolute<string>(registers.rbx);
                Console.WriteLine($"{fileName}: {registers.rax:X8}");
            }
            else
            {
                if (registers.rax != 0)
                {
                    string fileName = dbg.ReadMemoryAbsolute<string>(registers.rdi);
                    Console.WriteLine($"Missing: {fileName} (err: 0x{registers.rax:X8})");
                }
            }
        }
    }
}
