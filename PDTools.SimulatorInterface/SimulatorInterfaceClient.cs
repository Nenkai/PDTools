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

namespace PDTools.SimulatorInterface;

/// <summary>
/// Simulator Interface for GT7. (Disposable object).
/// </summary>
public class SimulatorInterfaceClient : IDisposable
{
    private ISimulationInterfaceCryptor _cryptor;
    private readonly IPEndPoint _endpoint;
    private UdpClient _udpClient;
    private DateTime _lastSentHeartbeat;

    public const int SendDelaySeconds = 10;

    public const int ReceivePortDefault = 33339;
    public const int BindPortDefault = 33340;

    public const int ReceivePortGT7 = 33739;
    public const int BindPortGT7 = 33740;

    public int ReceivePort { get; }
    public int BindPort { get; }

    public SimInterfacePacketType PacketType { get; set; } = SimInterfacePacketType.PacketType3;

    public delegate void SimulatorDelegate(SimulatorPacket packet);

    /// <summary>
    /// Fired from a packet from the GT Engine Simulation is sent.
    /// </summary>
    public event SimulatorDelegate OnReceive;

    public bool Started { get; private set; }
    public SimulatorInterfaceGameType SimulatorGameType { get; private set; }
    private ReadOnlyMemory<byte> _heartbeatBytes;

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

        _heartbeatBytes = (PacketType switch
        {
            SimInterfacePacketType.PacketType1 => "A"u8,
            SimInterfacePacketType.PacketType2 => "B"u8,
            SimInterfacePacketType.PacketType3 => "~"u8,
            _ => "A"u8, // We should default to "~".
        }).ToArray();

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
    public async Task Start(CancellationToken token = default)
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
                await SendHeartbeat(token);

            UdpReceiveResult result = await _udpClient.ReceiveAsync(token);

            if (result.Buffer.Length != GetExpectedPacketSize())
                throw new InvalidDataException($"Expected packet size to be 0x{GetExpectedPacketSize():X} bytes. Was {result.Buffer.Length:X4} bytes.");

            _cryptor.Decrypt(result.Buffer);

            SimulatorPacket packet = new SimulatorPacket();
            packet.SetPacketInfo(SimulatorGameType, result.RemoteEndPoint, DateTimeOffset.Now);
            packet.Read(result.Buffer);

            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            this.OnReceive(packet);

            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();
        }
    }

    private async Task SendHeartbeat(CancellationToken ct)
    {
        await _udpClient.SendAsync(_heartbeatBytes, _endpoint, ct);
        _lastSentHeartbeat = DateTime.UtcNow;
    }

    private void InitCryptor(SimulatorInterfaceGameType gameType)
    {
        if (gameType == SimulatorInterfaceGameType.GT7)
        {
            var cryptor = new SimulatorInterfaceCryptorGT7();
            switch (PacketType)
            {
                case SimInterfacePacketType.PacketType2:
                    cryptor.XorKey = 0xDEADBEEF;
                    break;

                case SimInterfacePacketType.PacketType3:
                    cryptor.XorKey = 0x55FABB4F;
                    break;
            }

            _cryptor = cryptor;
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

    public uint GetExpectedPacketSize()
    {
        // TODO: Game might send a packet of 0x94 if not using 'A' type heartbeat?
        // Might need checking. Might also be exclusive to GT7 >= 1.42

        return PacketType switch
        {
            SimInterfacePacketType.PacketType1 => 0x128,
            SimInterfacePacketType.PacketType2 => 0x13C,
            SimInterfacePacketType.PacketType3 => 0x158,
            _ => 0x128,
        };
    }
    public void Dispose()
    {
        _udpClient?.Dispose();
        _udpClient = null;
        GC.SuppressFinalize(this);
    }

    public enum SimInterfacePacketType
    {
        PacketType1,

        // Both of these were added in GT7 1.42.
        // Initial signs of expansions can be seen in 1.40 (possibly earlier even?) - 3 switch cases (+ default)
        // can already be seen, all using 'A' and the same amount of bytes.

        /// <summary>
        /// GT7 >= 1.42
        /// </summary>
        PacketType2,

        /// <summary>
        /// GT7 >= 1.42
        /// </summary>
        PacketType3
    }
}
