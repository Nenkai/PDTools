using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Threading;
using System.IO;

using Syroot.BinaryData.Memory;

using PDTools.Crypto.SimulationInterface;

namespace PDTools.SimulatorInterface
{
    /// <summary>
    /// Simulator Interface for GT7. (Disposable object).
    /// </summary>
    public class SimulatorInterfaceClient : IDisposable
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

        public SimulatorPacket Packet { get; set; }
        public delegate void SimulatorDelegate(SimulatorPacket packet);

        /// <summary>
        /// Fired from a packet from the GT Engine Simulation is sent.
        /// </summary>
        public event SimulatorDelegate OnReceive;

        public bool Started { get; private set; }
        public SimulatorInterfaceGameType SimulatorGameType { get; private set; }

        /// <summary>
        /// Creates a new simulator interface.
        /// </summary>
        /// <param name="address">Target address.</param>
        /// <exception cref="ArgumentException"></exception>
        public SimulatorInterfaceClient(string address, SimulatorInterfaceGameType gameType)
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
        /// This can be started anytime - during the game's boot process or in a race.
        /// </summary>
        /// <param name="cts">Cancellation token to stop the interface.</param>
        /// <returns></returns>
        public async Task Start(CancellationToken token = default,bool loop = true)
        {
            if (Started)
                throw new InvalidOperationException("Simulator Interface already started.");

            Started = true;

            _lastSentHeartbeat = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(1));
            _udpClient = new UdpClient(BindPort);

            // Will send a packet per tick - 60fps
            
            // while (loop)
            // {
                if ((DateTime.UtcNow - _lastSentHeartbeat).TotalSeconds > SendDelaySeconds)
                    await SendHeartbeat(token);

#if NET6_0_OR_GREATER
                UdpReceiveResult result = await _udpClient.ReceiveAsync(token);
#else
                UdpReceiveResult result = await _udpClient.ReceiveAsync().WithCancellation(token);
#endif

                if (result.Buffer.Length != 0x128)
                    throw new InvalidDataException($"Expected packet size to be 0x128. Was {result.Buffer.Length:X4} bytes.");

                _cryptor.Decrypt(result.Buffer);

                Packet = new SimulatorPacket();
                Packet.SetPacketInfo(SimulatorGameType, result.RemoteEndPoint, DateTimeOffset.Now);
                Packet.Read(result.Buffer);

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
            // }
        }

        private async Task SendHeartbeat(CancellationToken ct)
        {
#if NET6_0_OR_GREATER
            await _udpClient.SendAsync(new byte[1] { (byte)'A' }, _endpoint, ct);
#else
            await _udpClient.SendAsync(new byte[1] { (byte)'A' }, 1, _endpoint).WithCancellation(ct);
#endif
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
                _cryptor = new SimulatorInterfaceCryptorGT6();
            }
            else
            {
                throw new NotSupportedException($"'{gameType}' is not supported.");
            }
        }

        public void Dispose()
        {
            _udpClient?.Dispose();
            _udpClient = null;
            GC.SuppressFinalize(this);
        }
    }
}
