﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;

using CommandLine;
using CommandLine.Text;

using libdebug;

using PDTools.GTPatcher.MemoryPatches;
using PDTools.GTPatcher.BreakLoggers;

namespace PDTools.GTPatcher
{
    public class Program
    {
        public static CancellationTokenSource _cts = new CancellationTokenSource();

        public static async Task Main(string[] args)
        {
            using var fs = new FileStream(@"D:\Modding_Research\Gran_Turismo\Gran_Turismo_7\Package\GT7-1.29\Image0\eboot.elf", FileMode.Open);
            using var bs = new BinaryReader(fs);

            fs.Position = 0x3B7DDD0;
            byte[] arr = new byte[350];
            for (var i = 0; i < 350; i++)
                arr[i] = bs.ReadByte();

            byte test = arr[274];

            fs.Position = 0x3B7DF30;
            short[] arr2 = new short[2080];
            for (var i = 0; i < 2080; i++)
                arr2[i] = bs.ReadInt16();


            fs.Position = 0x3B7F050;

            short[] arr3 = new short[2190];
            for (var i = 0; i < 2190; i++)
                arr3[i] = bs.ReadInt16();

            short test2 = arr3[test];

            fs.Position = 0x3B80170;
            short[] arr4 = new short[542];
            for (var i = 0; i < 542; i++)
                arr4[i] = bs.ReadInt16();

            int aa = Array.IndexOf(arr4, (short)18);
            Console.WriteLine("GTDebugger by Nenkai#9075 for GT Sport");

            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync<Options>(Options);
        }

        public static async Task Options(Options options)
        {
            var dbg = new GTPatcher(options.IPAddress, GameType.GT7_V129);

            if (options.Arguments.Any())
            {
                string[] args = new string[1 + options.Arguments.Count()];
                args[0] = "";
                for (var i = 0; i < options.Arguments.Count(); i++)
                    args[i + 1] = options.Arguments.ElementAt(i);
                dbg.AddPatch(new CommandLineInjector(args));

                Console.WriteLine($"Command Line Arguments to Inject:");
                foreach (var arg in options.Arguments)
                    Console.WriteLine($"- {arg}");
                Console.WriteLine();
            }

            if (!string.IsNullOrEmpty(options.Build))
            {
                Console.WriteLine($"Setting game build to: {options.Build}");
                dbg.AddPatch(new VersionBuildPatcher(options.Build));
            }

            if (!string.IsNullOrEmpty(options.Branch))
            {
                Console.WriteLine($"Setting game branch to: {options.Branch}");
                dbg.AddPatch(new VersionBranchPatcher(options.Branch));
            }

            if (!string.IsNullOrEmpty(options.Environment))
            {
                Console.WriteLine($"Setting game environment to: {options.Environment}");
                dbg.AddPatch(new VersionEnvironmentPatcher(options.Environment));
            }

            if (options.KernelLogFiles || options.KernelLogMissingFiles)
            {
                Console.WriteLine($"Will log accessed files from FileDeviceKernel/sceKernelStat");
                dbg.AddBreakLogger(new FileDeviceKernelAccessLogger(options.KernelLogMissingFiles));
            }

            if (options.MPHLogFiles || options.MPHLogMissingFiles)
            {
                Console.WriteLine($"Will log accessed files from FileDeviceMPH");
                dbg.AddBreakLogger(new FileDeviceMPHAccessLogger(options.MPHLogMissingFiles));
            }

            if (options.FixAdhocExceptionCrash)
            {
                Console.WriteLine($"Will fix GT7 adhoc exceptions");
                dbg.AddBreakLogger(new AdhocExceptionFixer());
            }

            if (options.DisableTinyWebCaching)
            {
                Console.WriteLine($"Will disable tinyweb adhoc module caching");
                dbg.AddBreakLogger(new TinyWebAdhocModuleCacheDisabler());
            }


            dbg.AddBreakLogger(new TestLogger());

            Console.CancelKeyPress += delegate {
                _cts.Cancel();
            };

            Console.WriteLine();

            dbg.Start(new CancellationTokenSource().Token);
            dbg.Dispose();
        }
    }

    public class Options
    {
        [Option('i', "ip", Required = true, HelpText = "PS4 IP Address")]
        public string IPAddress { get; set; }

        [Option('a', "args", Required = false, HelpText = "Command Line Arguments for the game")]
        public IEnumerable<string> Arguments { get; set; }

        [Option("build", Required = false, HelpText = "Sets the game's build")]
        public string Build { get; set; }

        [Option("branch", Required = false, HelpText = "Sets the game's branch")]
        public string Branch { get; set; }

        [Option("environment", Required = false, HelpText = "Sets the game's version environment")]
        public string Environment { get; set; }

        [Option("mph-log-files", Required = false, HelpText = "Whether to log files from GT7 (makes it run slower)")]
        public bool MPHLogFiles { get; set; }

        [Option("mph-log-missing-files", Required = false, HelpText = "Whether to log missing files from GT7 (makes it run slower)")]
        public bool MPHLogMissingFiles { get; set; }

        [Option("kernel-log-files", Required = false, HelpText = "Whether to log files from the game (makes it run slower)")]
        public bool KernelLogFiles { get; set; }

        [Option("kernel-log-missing-files", Required = false, HelpText = "Whether to log missing files from the game (makes it run slower)")]
        public bool KernelLogMissingFiles { get; set; }

        [Option("fix-gt7-adhoc-exceptions", Required = false, HelpText = "Fixes adhoc exceptions crashing GT7 (removes vegas reporting)")]
        public bool FixAdhocExceptionCrash { get; set; }

        [Option("disable-tinyweb-caching", Required = false, HelpText = "Disables TinyWeb ADC/Adhoc caching")]
        public bool DisableTinyWebCaching { get; set; }
    }
}