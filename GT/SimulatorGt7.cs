using System.Net;
using System.Net.Sockets;
using GT.Models;
using PDTools.Crypto.SimulationInterface;
using PDTools.SimulatorInterface;

namespace GT;

public class SimulatorGt7 : IDisposable
{
    private readonly ISimulationInterfaceCryptor _cryptor;
    private readonly IPEndPoint _endpoint;
    private UdpClient _udpClient;
    private const int ReceivePortGt7 = 33739;
    private const int BindPortGt7 = 33740;

    public SimulatorGt7(string address)
    {
        if (!IPAddress.TryParse(address, out IPAddress addr))
            throw new ArgumentException("Could not parse IP Address.");

        _endpoint = new IPEndPoint(addr, ReceivePortGt7);
        _cryptor = new SimulatorInterfaceCryptorGT7();
    }

    public async Task<SimulatorPacket> GetData(CancellationToken token)
    {
        _udpClient = new UdpClient(BindPortGt7);

        SimulatorPacket packet = new();

        try
        {
            await _udpClient.SendAsync(new[] { (byte)'A' }, _endpoint, token);

            var result = await _udpClient.ReceiveAsync(token);

            if (result.Buffer.Length != 0x128)
                throw new InvalidDataException($"Expected packet size to be 0x128. Was {result.Buffer.Length:X4} bytes.");

            _cryptor.Decrypt(result.Buffer);
 
            packet.SetPacketInfo(SimulatorInterfaceGameType.GT7, result.RemoteEndPoint, DateTimeOffset.Now);
            packet.Read(result.Buffer);

            packet.MetersPerSecond = (float)Math.Round(packet.MetersPerSecond * 3.6, 2);
            packet.TurboBoost = (float)((packet.TurboBoost - 1.0) * 100.0);

        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Simulator Interface ending..");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Errored during simulation: {e.Message}");
        }
        finally
        {
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            Dispose();
        }

        return packet;
    }

    public static SimulatorPacket GetDataTest()
    {
        var gt = new SimulatorPacket();

        gt.OilTemperature = new Random().Next(0,1000);
        gt.OilPressure = new Random().Next(0,1000);
        gt.CurrentGear = 0;
        gt.GasCapacity = new Random().Next(0,1000);
        gt.GearRatios = new[] { (float)new Random().Next(0,1000) };
        gt.CarCode = 123;
        gt.BodyHeight = new Random().Next(0,1000);
        gt.Brake = 0;
        gt.Throttle = 0;
        gt.GasLevel = new Random().Next(0,1000);
        gt.BestLapTime = new TimeSpan();
        gt.TireFL_SurfaceTemperature = new Random().Next(0,1000);
        gt.TireFR_SurfaceTemperature = new Random().Next(0,1000);
        gt.TireRL_SurfaceTemperature = new Random().Next(0,1000);
        gt.TireRR_SurfaceTemperature = new Random().Next(0,1000);

        return gt;
    }

    public void Dispose()
    {
        _udpClient?.Dispose();
        _udpClient = null;
        GC.SuppressFinalize(this);
    }
}