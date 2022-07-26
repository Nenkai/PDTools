
using System.Net.Sockets;
using System.Net;
using PDTools.SimulatorInterface;

namespace SimulatorInterfaceTest
{
    internal class Program
    {
        private static bool _showUnknown = false;

        static async Task Main(string[] args)
        {
            /* Mostly a test sample for using the Simulator Interface library */

            Console.WriteLine("Simulator Interface GT7 - Nenkai#9075");
            Console.WriteLine();

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: SimulatorInterface.exe <IP address of PS4/PS5>");
                return;
            }

            _showUnknown = args.Contains("--debug");

            Console.WriteLine("Starting interface..");

            SimulatorInterface simInterface = new SimulatorInterface(args[0], SimulatorInterfaceGameType.GT7);
            simInterface.OnReceive += SimInterface_OnReceive;

            var cts = new CancellationTokenSource();

            // Cancel token from outside source to end simulator
            
            var task = simInterface.Start(cts);
            
            try
            {
                await task;
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"Simulator Interface ending..");
            }
            finally
            {
                // Important to clear up underlaying socket
                simInterface.Dispose();
            }
        }

        private static void SimInterface_OnReceive(SimulatorPacketBase packet)
        {
            Console.SetCursorPosition(0, 0);
            packet.PrintPacket(_showUnknown);
        }
    }
}