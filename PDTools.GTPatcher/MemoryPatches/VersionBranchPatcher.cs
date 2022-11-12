﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libdebug;

namespace PDTools.GTPatcher.MemoryPatches
{
    public class VersionBranchPatcher : IMemoryPatch
    {
        public const int ImageBase = 0x400000;

        public const ulong GT7_V100_VersionBranch_Offset = 0x6129890;
        public const ulong GT7_V125_VersionBranch_Offset = 0x5169990;

        public ulong Offset { get; set; }

        private string _branch;

        public VersionBranchPatcher(string branch)
        {
            _branch = branch;
        }

        public void Init(GTPatcher dbg)
        {
            switch (dbg.GameType)
            {
                case GameType.GTS_V168:
                    break;

                case GameType.GT7_V100:
                    Offset = GT7_V100_VersionBranch_Offset;
                    break;

                case GameType.GT7_V125:
                    Offset = GT7_V125_VersionBranch_Offset;
                    break;
            }
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
            string oldBranch = await dbg.ReadMemory<string>(Offset);
            await dbg.WriteMemory<string>(Offset, branch);

            await dbg.Notify($"Branch: {oldBranch} -> {branch}");
        }


    }
}
