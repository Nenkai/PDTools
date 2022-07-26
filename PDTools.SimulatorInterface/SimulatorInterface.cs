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

namespace PDTools.SimulatorInterface
{
    /// <summary>
    /// Simulator Interface for GT7. (Disposable object).
    /// </summary>
    public class SimulatorInterface : IDisposable
    {
        private ISimulationInterfaceCryptor _cryptor;
        private IPEndPoint _endpoint;
        private UdpClient _udpClient;
        private DateTime _lastSentHeartbeat;

        public const int SendDelaySeconds = 10;

        public const int ReceivePortDefault = 33339;
        public const int BindPortDefault = 33340;

        public const int ReceivePortGT7 = 33739;
        public const int BindPortGT7 = 33740;

        public int ReceivePort { get; }
        public int BindPort { get; }

        public delegate void SimulatorDeletate(SimulatorPacketBase packet);
        public event SimulatorDeletate OnReceive;

        public bool Started { get; private set; }
        public SimulatorInterfaceGameType SimulatorGameType { get; private set; }

        /// <summary>
        /// Creates a new simulator interface.
        /// </summary>
        /// <param name="address">Target address.</param>
        /// <exception cref="ArgumentException"></exception>
        public SimulatorInterface(string address, SimulatorInterfaceGameType gameType)
        {
            if (!IPAddress.TryParse(address, out IPAddress addr))
                throw new ArgumentException("Could not parse IP Address.");

            switch (gameType)
            {
                case SimulatorInterfaceGameType.GT7:
                    ReceivePort = ReceivePortGT7;
                    BindPort = BindPortGT7;
                    break;

                case SimulatorInterfaceGameType.GT6:
                case SimulatorInterfaceGameType.GTSport:
                    ReceivePort = ReceivePortDefault;
                    BindPort = BindPortDefault;
                    break;

                default:
                    throw new ArgumentException("Invalid game type.");
            }

            _endpoint = new IPEndPoint(addr, ReceivePort);
            InitCryptor(gameType);

            SimulatorGameType = gameType;
        }

        /// <summary>
        /// Starts the simulator interface.
        /// </summary>
        /// <param name="cts">Cancellation token to stop the interface.</param>
        /// <returns></returns>
        public async Task Start(CancellationTokenSource cts = default)
        {
            if (Started)
                throw new InvalidOperationException("Simulator Interface already started.");

            Started = true;

            _lastSentHeartbeat = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(1));
            _udpClient = new UdpClient(BindPort);

            // Will send a packet per tick - 60fps
            while (true)
            {
                if ((DateTime.UtcNow - _lastSentHeartbeat).TotalSeconds > SendDelaySeconds)
                    await SendHeartbeat(cts.Token);
  
                UdpReceiveResult result = await _udpClient.ReceiveAsync(cts.Token);
                if (result.Buffer.Length != 0x128)
                    throw new InvalidDataException($"Expected packet size to be 0x128. Was {result.Buffer.Length:X4} bytes.");

                _cryptor.Decrypt(result.Buffer);

                SimulatorPacketBase packet = InitPacket(SimulatorGameType);
                packet.Read(result.Buffer);

                if (cts.IsCancellationRequested)
                    cts.Token.ThrowIfCancellationRequested();

                this.OnReceive(packet);

                if (cts.IsCancellationRequested)
                    cts.Token.ThrowIfCancellationRequested();
            }
        }

        private async Task SendHeartbeat(CancellationToken ct)
        {
            await _udpClient.SendAsync(new byte[1] { (byte)'A' }, _endpoint, ct);
            _lastSentHeartbeat = DateTime.UtcNow;
        }

        private void InitCryptor(SimulatorInterfaceGameType gameType)
        {
            if (gameType == SimulatorInterfaceGameType.GT7)
            {
                _cryptor = new SimulatorInterfaceCryptorGT7();
            }
            else if (gameType == SimulatorInterfaceGameType.GTSport)
            {
                _cryptor = new SimulatorInterfaceCryptorGTSport();
            }
            else if (gameType == SimulatorInterfaceGameType.GT6)
            {
                throw new NotSupportedException($"'{gameType}' is not supported yet.");

                _cryptor = new SimulatorInterfaceCryptorGT6();
            }
            else
            {
                throw new NotSupportedException($"'{gameType}' is not supported yet.");
            }
        }

        private SimulatorPacketBase InitPacket(SimulatorInterfaceGameType gameType)
        {
            var packet = gameType switch
            {
                SimulatorInterfaceGameType.GT7 => new SimulatorPacketG7S0(),
                SimulatorInterfaceGameType.GTSport => new SimulatorPacketG7S0(),
                _ => throw new NotSupportedException($"'{gameType}' is not supported yet."),
            };

            return packet;
        }

        public void Dispose()
        {
            _udpClient?.Dispose();
            _udpClient = null;
            GC.SuppressFinalize(this);
        }
    }
}
