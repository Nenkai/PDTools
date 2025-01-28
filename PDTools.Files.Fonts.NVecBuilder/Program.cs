using PDTools.Files.Fonts;

using System;
using System.IO;

namespace PDTools.Files.Fonts.NVecBuilder;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("NVecBuilder by Nenkai");

        if (args.Length != 2)
        {
            Console.WriteLine("This tool is intended for GT5/6 (older PS3 GTs uses older versions of vector fonts)");
            Console.WriteLine("  Usage: nvec_builder.exe <input_ttf_file> <output_vec_file>");
            return;
        }

        Console.WriteLine();

        if (!File.Exists(args[0]))
        {
            Console.WriteLine("Input TTF file does not exist");
            return;
        }

        args[1] = Path.GetFullPath(args[1]);
        Directory.CreateDirectory(Path.GetDirectoryName(args[1]));

        try
        {
            TrueTypeToNVecConverter.Convert(args[0], args[1]);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to convert: {ex}");
            return;
        }

        Console.WriteLine($"TTF -> NVEC -> {args[1]}");

        Console.WriteLine("Make sure to add the font to all \"font/vec/fontset_*.txt\" if you are going to use it!");
        Console.WriteLine("Fonts are loaded at boot and stay/consume memory, if your font is large try to remove some characters from it with a ttf editor first!");
    }
}