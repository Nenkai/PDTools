using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libdebug;

namespace PDTools.GTPatcher.MemoryPatches
{
    public class VersionEnvironmentPatcher : IMemoryPatch
    {
        public const int ImageBase = 0x400000;

        public const ulong GT7_V100_VersionEnvironment_Offset = 0x61298B0;
        public const ulong GT7_V125_VersionEnvironment_Offset = 0x51699B0;
        public const ulong GT7_V129_VersionEnvironment_Offset = 0x4FA2E00;

        public ulong Offset { get; set; }

        private string _environment;

        public VersionEnvironmentPatcher(string environment)
        {
            _environment = environment;
        }

        public void Init(GTPatcher dbg)
        {
            switch (dbg.GameType)
            {
                case GameType.GTS_V168:
                    break;

                case GameType.GT7_V100:
                    Offset = GT7_V100_VersionEnvironment_Offset;
                    break;

                case GameType.GT7_V125:
                    Offset = GT7_V125_VersionEnvironment_Offset;
                    break;

                case GameType.GT7_V129:
                    Offset = GT7_V129_VersionEnvironment_Offset;
                    break;
            }
        }

        public void OnAttach(GTPatcher dbg)
        {

        }

        public void Patch(GTPatcher dbg, GeneralRegisters regs)
        {
            SetVersionEnvironment(dbg, _environment);
        }

        public void SetVersionEnvironment(GTPatcher dbg, string env)
        {
            string oldEnv = dbg.ReadMemory<string>(Offset);
            dbg.WriteMemory<string>(Offset, env);

            dbg.Notify($"Environment: {oldEnv} -> {env}");
        }


    }
}
