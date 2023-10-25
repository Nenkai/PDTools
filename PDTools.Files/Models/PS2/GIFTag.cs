using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2
{
    public class GIFTag
    {
        public ushort NLoop { get; set; }
        public bool EndOfPacket { get; set; }
        public ushort Id { get; set; }
        public bool Pre { get; set; }
        public ushort Prim { get; set; }
        public byte Flag { get; set; }
        public List<byte> Regs { get; set; } = new List<byte>();

        public void FromStream(BinaryStream bs)
        {
            ushort flags = bs.ReadUInt16();
            NLoop = (ushort)(flags & 0b111_1111_1111_1111);
            EndOfPacket = ((flags >> 15) & 1) != 0;

            bs.ReadUInt16(); // pad

            uint flags2 = bs.ReadUInt32();
            Id = (ushort)(flags2 & 0b11_1111_1111_1111);
            Pre = ((flags2 >> 14) & 1) != 0;
            Prim = (ushort)((flags2 >> 15) & 0b111_1111_1111);
            Flag = (byte)((flags2 >> 26) & 0b11);

            byte nreg = (byte)((flags2 >> 28) & 0b1111);

            uint regs = bs.ReadUInt32();
            int bitIndex = 0;
            for (int i = 0; i < 4; i++)
            {
                Regs.Add((byte)((regs >> bitIndex) & 0b1111));
                bitIndex += 4;
            }
        }

        public void Write(BinaryStream bs)
        {
            bs.WriteUInt16((ushort)(((EndOfPacket ? 1 : 0) << 15) | (NLoop & 0b111_1111_1111_1111)));
            bs.WriteUInt16(0); // pad
            bs.WriteUInt32((uint)(((3 & 0b1111) << 28) |
                ((Flag & 0b11) << 26) |
                ((Prim & 0b111_1111_1111) << 15) |
                ((Pre ? 1 : 0) << 14) |
                (Id & 0b11_1111_1111_1111)));

            uint regs = 0;
            int bitIndex = 0;
            for (int i = 0; i < 4; i++)
            {
                regs |= (uint)((Regs[i] & 0b1111) << bitIndex);
                bitIndex += 4;
            }
            bs.WriteUInt32(regs);
        }
    }
}
