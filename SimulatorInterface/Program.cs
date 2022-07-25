
using System.Net.Sockets;
using System.Net;

namespace SimulatorInterface
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Simulator Interface GT7 - by Nenkai#9075");
            Console.WriteLine();

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: SimulatorInterface.exe <IP Address of PS4/PS5>");
                return;
            }


            SimulatorInterface simInterface = new SimulatorInterface(args[0]);
            simInterface.Start();
        }
    }
}