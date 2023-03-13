using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libdebug;

namespace PDTools.GTPatcher.MemoryPatches
{
    public class VersionBuildPatcher : IMemoryPatch
    {
        public const int ImageBase = 0x400000;

        public const ulong GTSP_V168_VersionBuild_Offset = 0x2B72B10;
        public const ulong GT7_V100_VersionBuild_Offset = 0x61298D0;
        public const ulong GT7_V125_VersionBuild_Offset = 0x51699D0;
        public const ulong GT7_V129_VersionBuild_Offset = 0x4FA2E20;

        public ulong Offset { get; set; }
        private string _build;

        public VersionBuildPatcher(string build)
        {
            _build = build;
        }

        public void Init(GTPatcher dbg)
        {
            switch (dbg.GameType)
            {
                case GameType.GTS_V168:
                    Offset = GTSP_V168_VersionBuild_Offset;
                    break;

                case GameType.GT7_V100:
                    Offset = GT7_V100_VersionBuild_Offset;
                    break;

                case GameType.GT7_V125:
                    Offset = GT7_V125_VersionBuild_Offset;
                    break;

                case GameType.GT7_V129:
                    Offset = GT7_V129_VersionBuild_Offset;
                    break;
            }
        }

        public void OnAttach(GTPatcher dbg)
        {
            
        }

        public async Task Patch(GTPatcher dbg, GeneralRegisters regs)
        {
            await SetVersionBuild(dbg, _build);
        }

        public async Task SetVersionBuild(GTPatcher dbg, string build)
        {
            string oldBuild = await dbg.ReadMemory<string>(Offset);
            await dbg.WriteMemory<string>(Offset, build);

            await dbg.Notify($"Build: {oldBuild} -> {build}");
        }


    }
}
