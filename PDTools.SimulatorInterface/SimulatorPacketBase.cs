using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace PDTools.SimulatorInterface
{
    public abstract class SimulatorPacketBase
    {
        public IPEndPoint RemoteEndPoint { get; private set; }
        public DateTimeOffset DateReceived { get; private set; }

        public void SetPacketInfo(IPEndPoint remoteEndPoint, DateTimeOffset dateReceived)
        {
            RemoteEndPoint = remoteEndPoint;
            DateReceived = dateReceived;
        }

        public abstract void Read(Span<byte> data);

        public abstract void PrintPacket(bool debug = false);
    }
}
