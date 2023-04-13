using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libdebug;

namespace PDTools.GTPatcher.BreakLoggers
{
    public class FileDeviceMPHAccessLogger : IBreakLogger
    {
        public const ulong GT7_V100_FileOpen_Offset = 0x3A21740;
        public const ulong GT7_V125_FileOpen_Offset = 0x312F240;
        public const ulong GT7_V129_FileOpen_Offset = 0x2D62577;

        public ulong Offset { get; set; }
        public Breakpoint Breakpoint { get; set; }

        public bool LogOnlyOnMiss { get; set; }

        public FileDeviceMPHAccessLogger(bool logOnlyOnMiss = false)
        {
            LogOnlyOnMiss = logOnlyOnMiss;
        }

        public void Init(GTPatcher dbg)
        {
            switch (dbg.GameType)
            {
                case GameType.GT7_V100:
                    Offset = GT7_V100_FileOpen_Offset;
                    break;

                case GameType.GT7_V125:
                    Offset = GT7_V125_FileOpen_Offset;
                    break;

                case GameType.GT7_V129:
                    Offset = GT7_V129_FileOpen_Offset;
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
            if (!LogOnlyOnMiss)
            {
                string fileName = dbg.ReadMemoryAbsolute<string>(registers.rsi);
                Console.WriteLine($"{fileName}");
            }
            else
            {
                if (registers.rax != 0)
                {
                    string fileName = dbg.ReadMemoryAbsolute<string>(registers.rsi);
                    Console.WriteLine($"Missing: {fileName} (err: 0x{registers.rax:X8})");
                }
            }
        }
    }
}
