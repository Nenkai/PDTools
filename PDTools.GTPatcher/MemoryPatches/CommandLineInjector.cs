using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libdebug;

namespace PDTools.GTPatcher.MemoryPatches
{
    public class CommandLineInjector : IMemoryPatch
    {
        public static bool _argvPatched;

        public const int Argc_Offset = 0x3196120;
        public const int Argv_Offset = 0x3196128;

        public const int PFSVolumePath_Offset = 0x2B72F80;

        private string[] _args;

        public CommandLineInjector(string[] args)
        {
            if (args.Length < 1)
                throw new Exception("Argument patch must have at least 1 argument (empty string)");

            _args = args;
        }

        public void OnAttach(GTPatcher dbg)
        {
            dbg.PS4.ChangeWatchpoint(0, true, WATCHPT_LENGTH.DBREG_DR7_LEN_4, WATCHPT_BREAKTYPE.DBREG_DR7_RDWR, dbg.ImageBase + Argv_Offset);
        }

        public async Task Patch(GTPatcher dbg, GeneralRegisters regs)
        {
            await dbg.PS4.Notify(222, "Caught main, injecting argc/argv...");
            await PatchArgcArgv(_args, dbg);

            foreach (var arg in _args)
            {
                if (arg.StartsWith("fsroot"))
                    await PatchFSRoot(dbg);
            }

            await dbg.PS4.ChangeWatchpoint(0, false, WATCHPT_LENGTH.DBREG_DR7_LEN_4, WATCHPT_BREAKTYPE.DBREG_DR7_RDWR, 0);

            await dbg.PS4.Notify(222, "Arguments injected!");
        }

        public static async Task PatchArgcArgv(string[] args, GTPatcher dbg)
        {
            // Update arg count
            int argCount = await dbg.ReadMemory<int>(Argc_Offset);
            await dbg.WriteMemory<int>(Argc_Offset, args.Length);

            ulong newArgvOffset = dbg.ImageBase + 0x1EDFD00;
            await dbg.WriteMemory<ulong>(Argv_Offset, newArgvOffset);

            ulong strPtr = newArgvOffset;
            ulong lastAlignedStrOffset = dbg.ImageBase + 0x1EE0200;

            await dbg.WriteMemory(0x1EDFD00, new byte[0x1000]);

            for (var i = 0; i < args.Length; i++)
            {
                await dbg.WriteMemoryAbsolute<ulong>(strPtr, lastAlignedStrOffset);
                await dbg.WriteMemoryAbsolute<string>(lastAlignedStrOffset, args[i]);

                strPtr += sizeof(ulong);

                if (i == 0)
                    lastAlignedStrOffset += 0x20;
                else
                    lastAlignedStrOffset = AlignValue(lastAlignedStrOffset + (ulong)args[i].Length, 0x20);
            }
        }

        private async Task PatchFSRoot(GTPatcher dbg)
        {
            // Requires command line argument to be set first i.e 'fsroot=/@/data/test'

            /* The game will first load the FileDeviceGTFS if the adhoc resource "pfsVolumePath" is set, which its value is /app0/gt.idx.
             * Make it not exist, and the device won't be loaded
             * Pseudo code:
             * 
             * string path = Adhoc::GetValue(value: "pfsVolumePath", default_value: NULL);
             * if (path != NULL)
             * {
             *     if (sceKernelStat(path, &stat))
             *     {
             *         string inputStr = "gtfs";
             *         PDISTD::GetDeviceFromType(device, keys, &thing, &inputStr);
             *         FileManager::AddDevice(g_Devices, device);
             *     }
             * }
             * 
             * CheckForAdditionalDevicesFromCommandLineArgs(device, keys, g_argc, g_argv); 
             */
            await dbg.WriteMemory<string>(PFSVolumePath_Offset, "/app0/doesnotexist.idx");

            /* There are a few devices available to be set:
             * - vol
             * - direct
             * - direct1
             * - mffs
             * 
             * gtfs is the regular volume system (FileDeviceGTFS). setting the commandline to "vol" will use gtfs.
             * direct is app_home/fsroot (FileDeviceKernel+FileDeviceKernelCachePTCheck), used by setting "fsroot" in the command line argument.
             * direct1 is actual direct (FileDeviceKernel).
             * mffs's use is unknown. (ManifestFileSystem), can't be set by command line arguments either. 
             * 
             * We do not want to use direct/app_home, therefore we need to change the strings required to set it. 
             * We can't change the string @ 0x1BF18B2 from "direct" to "direct1" because it's a stack string.
             * But it doesn't mean we can't change it to something else
             * 
             * direct -> direcc
             */
            // Do not write string. We do not want it null terminated
            await dbg.WriteMemory(0x1BF18B1, (byte)'c');
            await dbg.WriteMemory(0x1BF58BF, (byte)'c');

            // Patch instruction string lengths of strlen("direct1") to strlen("direcc")
            await dbg.WriteMemory(0x1BF3617, (byte)"direcc".Length);
            await dbg.WriteMemory(0x1BF3619, (byte)"direcc".Length);
            await dbg.WriteMemory(0x1BF364F, (byte)"direcc".Length);
            await dbg.WriteMemory(0x1BF365B, (byte)"direcc".Length);

            /* Patch actual check (direct1 -> direcc) */
            await dbg.WriteMemory<string>(0x1E905EF, "direcc");

            // Set mode of FileDeviceKernel from 1 to -1 (no idea what that does)
            await dbg.WriteMemory<int>(0x1BF381A, 1);
        }

        public static ulong AlignValue(ulong x, ulong alignment)
        {
            ulong mask = ~(alignment - 1);
            return (x + (alignment - 1)) & mask;
        }
    }
}
