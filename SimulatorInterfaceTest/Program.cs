using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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

            Console.WriteLine("Simulator Interface GT7/GTSport - Nenkai#9075");
            Console.WriteLine();

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: SimulatorInterface.exe <IP address of PS4/PS5> ('--gtsport' for GT Sport support, optional: '--debug' to show unknown values)");
                return;
            }

            _showUnknown = args.Contains("--debug");
            bool gtsport = args.Contains("--gtsport");

            Console.WriteLine("Starting interface..");

            SimulatorInterface simInterface = new SimulatorInterface(args[0], !gtsport ? SimulatorInterfaceGameType.GT7 : SimulatorInterfaceGameType.GTSport);
            simInterface.OnReceive += SimInterface_OnReceive;

            var cts = new CancellationTokenSource();

            // Cancel token from outside source to end simulator
            
            var task = simInterface.Start(cts.Token);
            
            try
            {
                await task;
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"Simulator Interface ending..");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Errored during simulation: {e.Message}");
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

            // Print the packet contents to the console
            packet.PrintPacket(_showUnknown);

            if (packet.GameType == SimulatorInterfaceGameType.GT7 || packet.GameType == SimulatorInterfaceGameType.GTSport)
            {
                // Do something with game specific packet
                var gt7Packet = packet as SimulatorPacketG7S0;
            }
            
        }
    }
}