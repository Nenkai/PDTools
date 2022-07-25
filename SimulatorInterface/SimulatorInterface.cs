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

        public SimulatorInterface(string address)
        {
            if (!IPAddress.TryParse(address, out IPAddress addr))
                throw new ArgumentException("Could not parse IP Address.");

            _endpoint = new IPEndPoint(addr, ReceivePort);
            _cryptor = new SimulatorInterfaceCryptorGT7();
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

            // Will send a packet per tick - 60fps
            while (true)
            {
                if ((DateTime.UtcNow - lastSent).TotalSeconds > SendDelaySeconds)
                    udpClient.Send(new byte[1] { (byte)'A' }, _endpoint);

                byte[] data = udpClient.Receive(ref RemoteIpEndPoint);
                if (data.Length != 0x128)
                    throw new InvalidDataException($"Expected packet size to be 0x128. Was {data.Length:X4} bytes.");

                _cryptor.Decrypt(data);

                SpanReader sr = new SpanReader(data);
                int magic = sr.ReadInt32();
                if (magic != 0x47375330) // 0S7G - G7S0
                    throw new InvalidDataException($"Unexpected packet magic '{magic}'.");

                var dataPacket = new SimulatorPacketGT7();

                dataPacket.Position = new Vector3(sr.ReadSingle(), sr.ReadSingle(), sr.ReadSingle()); // Coords to track
                dataPacket.Acceleration = new Vector3(sr.ReadSingle(), sr.ReadSingle(), sr.ReadSingle());  // Accel in track pixels
                dataPacket.Rotation = new Vector3(sr.ReadSingle(), sr.ReadSingle(), sr.ReadSingle()); // Pitch/Yaw/Roll all -1 to 1
                dataPacket.Unknown_0x28 = sr.ReadSingle();
                dataPacket.Unknown_0x2C = new Vector3(sr.ReadSingle(), sr.ReadSingle(), sr.ReadSingle());
                dataPacket.Unknown_0x38 = sr.ReadSingle();
                dataPacket.RPM = sr.ReadSingle();

                // Skip IV
                sr.Position += 8;

                dataPacket.Unknown_0x48 = sr.ReadSingle();
                dataPacket.MetersPerSecond = sr.ReadSingle();
                dataPacket.TurboBoost = sr.ReadSingle();
                dataPacket.Unknown_0x54 = sr.ReadSingle();
                dataPacket.Unknown_Always85_0x58 = sr.ReadSingle();
                dataPacket.Unknown_Always110_0x5C = sr.ReadSingle();
                dataPacket.TireSurfaceTemperatureFL = sr.ReadSingle();
                dataPacket.TireSurfaceTemperatureFR = sr.ReadSingle();
                dataPacket.TireSurfaceTemperatureRL = sr.ReadSingle();
                dataPacket.TireSurfaceTemperatureRR = sr.ReadSingle();
                dataPacket.TotalTimeTicks = sr.ReadInt32(); // can't be more than MAX_LAPTIME1000 - which is 1209599999, or else it's set to -1
                dataPacket.CurrentLap = sr.ReadInt32();
                dataPacket.BestLapTime = TimeSpan.FromMilliseconds(sr.ReadInt32());
                dataPacket.LastLapTime = TimeSpan.FromMilliseconds(sr.ReadInt32());
                dataPacket.DayProgressionMS = sr.ReadInt32();
                dataPacket.PreRaceStartPositionOrQualiPos = sr.ReadInt16();
                dataPacket.NumCarsAtPreRace = sr.ReadInt16();
                dataPacket.MinAlertRPM = sr.ReadInt16();
                dataPacket.MaxAlertRPM = sr.ReadInt16();
                dataPacket.CalculatedMaxSpeed = sr.ReadInt16();
                dataPacket.Flags = (SimulatorFlags)sr.ReadInt16();

                int bits = sr.ReadByte();
                dataPacket.CurrentGear = (byte)(bits & 0b1111);
                dataPacket.SuggestedGear = (byte)(bits >> 4);

                dataPacket.Throttle = sr.ReadByte();
                dataPacket.Brake = sr.ReadByte();

                //short throttleAndBrake = sr.ReadInt16();
                byte unknown = sr.ReadByte();

                dataPacket.TireFL_Unknown0x94_0 = sr.ReadSingle();
                dataPacket.TireFR_Unknown0x94_1 = sr.ReadSingle();
                dataPacket.TireRL_Unknown0x94_2 = sr.ReadSingle();
                dataPacket.TireRR_Unknown0x94_3 = sr.ReadSingle();
                dataPacket.TireFL_Accel = sr.ReadSingle();
                dataPacket.TireFR_Accel = sr.ReadSingle();
                dataPacket.TireRL_Accel = sr.ReadSingle();
                dataPacket.TireRR_Accel = sr.ReadSingle();
                dataPacket.TireFL_UnknownB4 = sr.ReadSingle();
                dataPacket.TireFR_UnknownB4 = sr.ReadSingle();
                dataPacket.TireRL_UnknownB4 = sr.ReadSingle();
                dataPacket.TireRR_UnknownB4 = sr.ReadSingle();
                dataPacket.TireFL_UnknownC4 = sr.ReadSingle();
                dataPacket.TireFR_UnknownC4 = sr.ReadSingle();
                dataPacket.TireRL_UnknownC4 = sr.ReadSingle();
                dataPacket.TireRR_UnknownC4 = sr.ReadSingle();

                sr.Position += sizeof(int) * 8; // Seems to be reserved - server does not set that

                dataPacket.Unknown_0xF4 = sr.ReadSingle();
                dataPacket.Unknown_0xF8 = sr.ReadSingle();
                dataPacket.RPMUnknown_0xFC = sr.ReadSingle();

                for (var i = 0; i < 7; i++)
                    dataPacket.Unknown_0x100[i] = sr.ReadSingle();

                sr.Position += 8;
                dataPacket.CarCode = sr.ReadInt32();

                PrintStatus(dataPacket);
            }
        }

        private static void PrintStatus(SimulatorPacketGT7 packet)
        {
            Console.Clear();
            Console.WriteLine($"Simulator Interface Packet");
            Console.WriteLine("[Car Data]");
            Console.WriteLine($"- Car Code: {packet.CarCode}");
            Console.WriteLine($"- Throttle: {packet.Throttle}");
            Console.WriteLine($"- Brake: {packet.Brake}");
            Console.WriteLine($"- RPM: {packet.RPM} - KPH: {Math.Round(packet.MetersPerSecond * 3.6, 2)}");
            Console.WriteLine($"- Turbo Boost: {packet.TurboBoost}");

            if (packet.SuggestedGear == 15)
                Console.WriteLine($"- Gear: {packet.CurrentGear}");
            else
                Console.WriteLine($"- Gear: {packet.CurrentGear} (Suggested: {packet.SuggestedGear})");

            Console.WriteLine($"- Flags: {packet.Flags}");
            Console.WriteLine($"- Tires");
            Console.WriteLine($"    FL:{Math.Round(packet.TireSurfaceTemperatureFL, 2)} FR:{Math.Round(packet.TireSurfaceTemperatureFR, 2)}");
            Console.WriteLine($"    RL:{Math.Round(packet.TireSurfaceTemperatureRL, 2)} RR:{Math.Round(packet.TireSurfaceTemperatureRR, 2)}");

            Console.WriteLine();
            Console.WriteLine("[Race Data]");

            int a = (int)(packet.TotalTimeTicks * 0.16667);
            TimeSpan.FromSeconds(a / 10);
            Console.WriteLine($"- Total Session Time: {TimeSpan.FromSeconds(packet.TotalTimeTicks / 60)}");
            Console.WriteLine($"- Current Lap: {packet.CurrentLap}");
            Console.WriteLine($"- Best: {packet.BestLapTime}");
            Console.WriteLine($"- Last: {packet.LastLapTime}");
            Console.WriteLine($"- Time of Day: {TimeSpan.FromMilliseconds(packet.DayProgressionMS)}");

            Console.WriteLine();
            Console.WriteLine("[Positional Information]");
            Console.WriteLine($"- Position: {packet.Position}");
            Console.WriteLine($"- Accel: {packet.Acceleration}");
            Console.WriteLine($"- Rotation: {packet.Rotation}");
        }
    }
}
