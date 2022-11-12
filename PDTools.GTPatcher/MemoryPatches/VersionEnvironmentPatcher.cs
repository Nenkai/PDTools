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
            }
        }

        public void OnAttach(GTPatcher dbg)
        {

        }

        public async Task Patch(GTPatcher dbg, GeneralRegisters regs)
        {
            await SetVersionEnvironment(dbg, _environment);
        }

        public async Task SetVersionEnvironment(GTPatcher dbg, string env)
        {
            string oldEnv = await dbg.ReadMemory<string>(Offset);
            await dbg.WriteMemory<string>(Offset, env);

            await dbg.Notify($"Environment: {oldEnv} -> {env}");
        }


    }
}
