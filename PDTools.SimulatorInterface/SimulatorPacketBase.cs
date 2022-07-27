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
        /// <summary>
        /// Peer address.
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; private set; }

        /// <summary>
        /// Date when this packet was received.
        /// </summary>
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
