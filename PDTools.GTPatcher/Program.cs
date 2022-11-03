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
            var dbg = new GTPatcher(options.IPAddress);

            if (options.Arguments.Count() > 0)
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

            if (options.LogMissingFiles)
            {
                Console.WriteLine($"Will log accessed missing files");
                dbg.AddBreakLogger(new FileDeviceKernelAccessLogger(logOnlyOnMiss: true));
            }

            Console.CancelKeyPress += delegate {
                _cts.Cancel();
            };

            Console.WriteLine();

            await dbg.Start(_cts.Token);
            await dbg.DisposeAsync();
        }
    }

    public class Options
    {
        [Option('i', "ip", Required = true, HelpText = "PS4 IP Address")]
        public string IPAddress { get; set; }

        [Option('a', "args", Required = false, HelpText = "Command Line Arguments for the game")]
        public IEnumerable<string> Arguments { get; set; }

        [Option('b', "build", Required = false, HelpText = "Sets the game's build")]
        public string Build { get; set; }

        [Option('l', "log-missing-files", Required = false, HelpText = "Whether to log missing files from the game (makes it run slower)")]
        public bool LogMissingFiles { get; set; }
    }
}