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
        public static bool _argvPatched;

        public const int ImageBase = 0x400000;

        public const int VersionBuild_Offset = 0x2B72B10;

        private string _branch;

        public VersionBuildPatcher(string branch)
        {
            _branch = branch;
        }

        public void OnAttach(GTPatcher dbg)
        {

        }

        public async Task Patch(GTPatcher dbg, GeneralRegisters regs)
        {
            await SetVersionBranch(dbg, _branch);
        }

        public async Task SetVersionBranch(GTPatcher dbg, string branch)
        {
            string oldBranch = await dbg.ReadMemory<string>(VersionBuild_Offset);
            await dbg.WriteMemory<string>(VersionBuild_Offset, branch);

            await dbg.PS4.Notify(36, $"Branch: {oldBranch} -> {branch}");
        }

    }
}
