using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Syroot.BinaryData.Memory;
using System.Numerics;

using PDTools.Crypto.SimulationInterface;

namespace SimulatorInterface
{
    public class SimulatorInterface
    {
        private ISimulationInterfaceCryptor _cryptor;
        private IPEndPoint _endpoint;

        public const int SendDelaySeconds = 10;

        public const int ReceivePort = 33739;
        public const int BindPort = 33740;

        public bool _debugUnknowns = false;

        public SimulatorInterface(string address, bool debugUnknowns)
        {
            if (!IPAddress.TryParse(address, out IPAddress addr))
                throw new ArgumentException("Could not parse IP Address.");

            _endpoint = new IPEndPoint(addr, ReceivePort);
            _cryptor = new SimulatorInterfaceCryptorGT7();
            _debugUnknowns = debugUnknowns;
        }

        public bool Start()
        {
            Console.WriteLine($"- Starting Simulator Interface to connect at endpoint: {_endpoint}");
            const int SendDelaySeconds = 10;

            UdpClient udpClient;
            try
            {
                Console.WriteLine($"- Attempting to bind port: {BindPort}");
                udpClient = new UdpClient(BindPort);

                Console.WriteLine("- Sending heartbeat packet..");
                udpClient.Send(new byte[1] { (byte)'A' }, _endpoint);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return false;
            }

            DateTime lastSent = DateTime.UtcNow;
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Console.WriteLine("- Done. Waiting on packets.. (If this gets stuck, it failed to connect.)");

            SimulatorPacketGT7 packet = new SimulatorPacketGT7();

            bool firstPacket = true;

            // Will send a packet per tick - 60fps
            while (true)
            {
                if ((DateTime.UtcNow - lastSent).TotalSeconds > SendDelaySeconds)
                    udpClient.Send(new byte[1] { (byte)'A' }, _endpoint);

                byte[] data = udpClient.Receive(ref RemoteIpEndPoint);

                if (firstPacket)
                {
                    Console.Clear();
                    firstPacket = false;
                }

                if (data.Length != 0x128)
                    throw new InvalidDataException($"Expected packet size to be 0x128. Was {data.Length:X4} bytes.");

                _cryptor.Decrypt(data);
                packet.Read(data);

                PrintStatus(packet, _debugUnknowns);
            }
        }

        private static void PrintStatus(SimulatorPacketGT7 packet, bool showUnknown)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Simulator Interface Packet");
            Console.WriteLine("[Car Data]");
            Console.WriteLine($"- Car Code: {packet.CarCode}   ");
            Console.WriteLine($"- Throttle: {packet.Throttle}   ");
            Console.WriteLine($"- Brake: {packet.Brake}   ");
            Console.WriteLine($"- RPM: {packet.RPM} - KPH: {Math.Round(packet.MetersPerSecond * 3.6, 2)}     ");
            Console.WriteLine($"- Turbo Boost: {((packet.TurboBoost - 1.0) * 100.0):F2}kPa   ");

            if (packet.SuggestedGear == 15)
                Console.WriteLine($"- Gear: {packet.CurrentGear}                                    ");
            else
                Console.WriteLine($"- Gear: {packet.CurrentGear} (Suggested: {packet.SuggestedGear})");

            Console.WriteLine($"- Flags: {packet.Flags,-100}");
            Console.WriteLine($"- Tires");
            Console.WriteLine($"    FL:{packet.TireSurfaceTemperatureFL:F2} FR:{packet.TireSurfaceTemperatureFR:F2}");
            Console.WriteLine($"    RL:{packet.TireSurfaceTemperatureRL:F2} RR:{packet.TireSurfaceTemperatureRR:F2}");

            Console.WriteLine();
            Console.WriteLine("[Race Data]");

            Console.WriteLine($"- Total Session Time: {TimeSpan.FromSeconds(packet.TotalTimeTicks / 60)}     ");
            Console.WriteLine($"- Current Lap: {packet.LapCount}  ");

            if (packet.BestLapTime.TotalMilliseconds == -1)
                Console.WriteLine($"- Best: N/A      ");
            else
                Console.WriteLine($"- Best: {packet.BestLapTime:mm\\:ss\\.fff}     ");

            if (packet.LastLapTime.TotalMilliseconds == -1)
                Console.WriteLine($"- Last: N/A      ");
            else
                Console.WriteLine($"- Last: {packet.LastLapTime:mm\\:ss\\.fff}     ");

            Console.WriteLine($"- Time of Day: {TimeSpan.FromMilliseconds(packet.DayProgressionMS):hh\\:mm\\:ss}     ");

            Console.WriteLine();
            Console.WriteLine("[Positional Information]");
            Console.WriteLine($"- Position: {packet.Position:F3}     ");
            Console.WriteLine($"- Accel: {packet.Acceleration:F3}    ");
            Console.WriteLine($"- Rotation: {packet.Rotation:F3}     ");

            if (showUnknown)
            {
                Console.WriteLine();
                Console.WriteLine("[Unknowns]");
                Console.WriteLine($"0x2C (Vec3): {packet.Unknown_0x2C:F2}   ");
                Console.WriteLine($"0x38 (Float): {packet.Unknown_0x38:F2}   ");
                Console.WriteLine($"0x48 (Float): {packet.Unknown_0x48:F2}   ");
                Console.WriteLine($"0x54 (Float): {packet.Unknown_0x54:F2}   ");
                Console.WriteLine($"0x94 (Float): {packet.TireFL_Unknown0x94_0:F2} {packet.TireFR_Unknown0x94_1:F2} {packet.TireRL_Unknown0x94_2:F2} {packet.TireRR_Unknown0x94_3:F2}   ");
                Console.WriteLine($"0xB4 (Float): {packet.TireFL_UnknownB4:F2} {packet.TireFR_UnknownB4:F2} {packet.TireRL_UnknownB4:F2} {packet.TireRL_UnknownB4:F2}   ");
                Console.WriteLine($"0xC4 (Float): {packet.TireFL_UnknownC4:F2} {packet.TireFR_UnknownC4:F2} {packet.TireRL_UnknownC4:F2} {packet.TireRL_UnknownC4:F2}   ");

                Console.WriteLine($"0xF4 (Float): {packet.Unknown_0xF4:F2}   ");
                Console.WriteLine($"0xF8 (Float): {packet.Unknown_0xF8:F2}   ");
                Console.WriteLine($"0xFC (Float): {packet.RPMUnknown_0xFC:F2}   ");

            }
        }
    }
}
