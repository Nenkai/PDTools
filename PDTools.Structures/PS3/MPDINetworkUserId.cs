using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Syroot.BinaryData.Memory;

using PDTools.Utils;

namespace PDTools.Structures.PS3
{
    public class MPDINetworkUserId
    {
        public string Name { get; set; }
        public int Unk { get; set; }
        public string Domain { get; set; }
        public string Region { get; set; }
        public string NpIdResource { get; set; }
        public int Unk2 { get; set; }
        public int Unk3 { get; set; }
        public int UserNumber { get; set; }
        public CellNetCtlEtherAddr Addr { get; set; }

        public static MPDINetworkUserId Read(Span<byte> data)
        {
            if (data.Length != 0x30)
                return null;

            MPDINetworkUserId id = default;
            SpanReader sr = new SpanReader(data);
            id.Name = sr.ReadFixedString(0x10);
            id.Unk = sr.ReadInt32();
            id.Domain = sr.ReadFixedString(2);
            id.Region = sr.ReadFixedString(2);
            id.NpIdResource = sr.ReadFixedString(4);
            id.Unk2 = sr.ReadInt32();
            id.Unk3 = sr.ReadInt32();
            id.UserNumber = sr.ReadInt32();

            ReadOnlySpan<byte> ether_addr = sr.Span.Slice(sr.Position, 8);
            id.Addr = MemoryMarshal.Read<CellNetCtlEtherAddr>(ether_addr);
            return id;
        }
    }

    public unsafe struct CellNetCtlEtherAddr
    {
        const int CELL_NET_CTL_ETHER_ADDR_LEN = 6;

        private fixed byte _data[CELL_NET_CTL_ETHER_ADDR_LEN];
        public Span<byte> Data
        {
            get
            {
                fixed (byte* fT = _data)
                    return new Span<byte>(fT, CELL_NET_CTL_ETHER_ADDR_LEN);
            }
        }

        private short _padding;
    }
}
