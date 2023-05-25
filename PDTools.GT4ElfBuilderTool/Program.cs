using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.GT4ElfBuilderTool
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Input: <CORE.GT4> <output elf file>");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Input file does not exist");
                return;
            }

            var image = new GTImageLoader();
            if (!image.Load(File.ReadAllBytes(args[0])))
            {
                Console.WriteLine("Failed to load CORE file");
                return;
            }

            image.BuildELF(args[1]);
        }
    }
}
