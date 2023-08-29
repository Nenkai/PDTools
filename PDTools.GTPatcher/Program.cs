using System;
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
            Console.WriteLine("GTDebugger by Nenkai#9075 for GT Sport");

            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync<Options>(Options);
        }

        public static async Task Options(Options options)
        {
            var dbg = new GTPatcher(options.IPAddress, GameType.GT7_V136);

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


            dbg.AddBreakLogger(new EvalExpressionStringLogger());
            dbg.AddBreakLogger(new EvalExpressionCompilationTokenTypeLogger());

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