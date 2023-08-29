using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet2
{
    public class PGLUshape
    {
        public float Scale { get; set; }
        public byte Unk1 { get; set; }
        public byte Unk2 { get; set; }
        public byte Unk3 { get; set; }

        public List<VIFDescriptor> VIFDescriptors { get; set; } = new();
        public List<VIFPacket> VIFPackets { get; set; } = new();

        public static PGLUshape FromStream(BinaryStream bs, long mdlBasePos)
        {
            PGLUshape shape = new PGLUshape();
            long shapeBasePos = bs.Position;

            bs.ReadInt32(); // Reloc ptr
            shape.Scale = bs.ReadSingle();

            byte bits = bs.Read1Byte();
            shape.Unk1 = (byte)(bits & 0b11111);
            shape.Unk2 = (byte)(bits >> 5);
            shape.Unk3 = bs.Read1Byte();

            short vifChunksCount = bs.ReadInt16();
            short numVerts = bs.ReadInt16();
            short numTriangles = bs.ReadInt16();

            for (var i = 0; i < vifChunksCount; i++)
            {
                var desc = VIFDescriptor.FromStream(bs, mdlBasePos);
                shape.VIFDescriptors.Add(desc);
            }

            for (var i = 0; i < vifChunksCount; i++)
            {
                bs.Position = shapeBasePos + shape.VIFDescriptors[i].VIFDataOffset;
                var packet = VIFPacket.FromStream(bs, shape.VIFDescriptors[i].DMATagQuadwordCount, mdlBasePos);
                shape.VIFPackets.Add(packet);
            }

            return shape;
        }
    }
}
