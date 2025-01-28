using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.IO;
using System;

using CommandLine;

namespace PDTools.GrimSonyDLSBuilder;

internal class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(OptionsFunc);
    }

    public static void OptionsFunc(Options options)
    {
        try
        {
            var @params = new PaceFileInfoParams();
            @params.Name = options.Name;
            @params.TrackerUrl = options.TrackerUrl;

            Console.WriteLine($"Building DLS from: {options.TargetFile}");
            Console.WriteLine($"- Name: {@params.Name}");
            Console.WriteLine($"- Tracker URL: {@params.TrackerUrl}");

            if (string.IsNullOrEmpty(options.OutputFile))
                options.OutputFile = Path.ChangeExtension(options.TargetFile, ".dls");

            Console.WriteLine("Creating Pace File Info & hashing..");
            var fileInfo = PaceFileInfo.CreateFromTarget(options.TargetFile, @params);

            Console.WriteLine($"Saving file info as {options.OutputFile}");
            fileInfo.SaveFileInfo(options.OutputFile);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Errored while building DLS file: {e.Message}");
        }
    }

    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input target file")]
        public string TargetFile { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output .dls file")]
        public string OutputFile { get; set; }

        [Option("name", Required = true, HelpText = "File Name/ID for the DLS (TV File ID for GT5")]
        public string Name { get; set; }

        [Option("tracker-url", Required = true, HelpText = "Tracker URL (https://$1$2$3$4/tracker/register for GT5)")]
        public string TrackerUrl { get; set; }
    }
}