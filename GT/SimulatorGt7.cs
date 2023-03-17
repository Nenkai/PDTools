using System.Net;
using System.Net.Sockets;
using System.Text;
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

    public async Task<GranTurismoData> GetData()
    {
        var token = new CancellationTokenSource().Token;

        _udpClient = new UdpClient(BindPortGt7);

        GranTurismoData data = new();

        try
        {
            await _udpClient.SendAsync(new[] { (byte)'A' }, _endpoint, token);

            var result = await _udpClient.ReceiveAsync(token);

            if (result.Buffer.Length != 0x128)
                throw new InvalidDataException($"Expected packet size to be 0x128. Was {result.Buffer.Length:X4} bytes.");

            _cryptor.Decrypt(result.Buffer);

            var packet = new SimulatorPacket();
            packet.SetPacketInfo(SimulatorInterfaceGameType.GT7, result.RemoteEndPoint, DateTimeOffset.Now);
            packet.Read(result.Buffer);

            data = new GranTurismoData(packet);
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

        return data;
    }

    public void Dispose()
    {
        _udpClient?.Dispose();
        _udpClient = null;
        GC.SuppressFinalize(this);
    }
}