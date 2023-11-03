using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2
{
    public class PGLUshape
    {
        public byte Unk1 { get; set; }
        public byte Unk2 { get; set; }
        public byte Unk3 { get; set; }
        public ushort TotalStripVerts { get; set; }
        public ushort NumTriangles { get; set; }

        public List<VIFDescriptor> VIFDescriptors { get; set; } = new();
        public List<VIFPacket> VIFPackets { get; set; } = new();

        public void FromStream(BinaryStream bs, long mdlBasePos)
        {
            long shapeBasePos = bs.Position;

            bs.ReadInt32(); // Reloc ptr
            uint size = bs.ReadUInt32();

            byte bits = bs.Read1Byte();
            Unk1 = (byte)(bits & 0b11111);
            Unk2 = (byte)(bits >> 5);
            Unk3 = bs.Read1Byte();

            ushort vifChunksCount = bs.ReadUInt16();
            TotalStripVerts = bs.ReadUInt16();
            NumTriangles = bs.ReadUInt16();

            for (var i = 0; i < vifChunksCount; i++)
            {
                var desc = VIFDescriptor.FromStream(bs, mdlBasePos);
                VIFDescriptors.Add(desc);
            }

            for (var i = 0; i < vifChunksCount; i++)
            {
                bs.Position = shapeBasePos + VIFDescriptors[i].VIFDataOffset;
                var packet = VIFPacket.FromStream(bs, VIFDescriptors[i].DMATagQuadwordCount, mdlBasePos);
                VIFPackets.Add(packet);
            }
        }

        public void Write(BinaryStream bs, long mdlBasePos)
        {
            long baseShapeOffset = bs.Position;

            bs.WriteUInt32(0); // Reloc ptr
            bs.WriteUInt32(0); // Shape size write later
            bs.WriteByte((byte)((Unk2 & 0b111) << 5 | (Unk1 & 0b11111)));
            bs.WriteByte(Unk3);
            bs.WriteUInt16((ushort)VIFDescriptors.Count);
            bs.WriteUInt16(TotalStripVerts);
            bs.WriteUInt16(NumTriangles);

            // Skip descriptors for now
            long descriptorsOffset = bs.Position;
            bs.Position += (VIFDescriptors.Count * VIFDescriptor.GetSize());
            bs.Align(0x10, grow: true);

            // Write strips
            long lastPos = bs.Position;
            for (var i = 0; i < VIFPackets.Count; i++)
            {
                long packetStartOffset = bs.Position;
                VIFPacket packet = VIFPackets[i];
                packet.Write(bs);

                lastPos = bs.Position;
                long quadwordSize = (bs.Position - packetStartOffset) / 16;

                bs.Position = descriptorsOffset + (i * VIFDescriptor.GetSize());
                bs.WriteUInt32((uint)(packetStartOffset - baseShapeOffset));
                bs.WriteUInt16((ushort)quadwordSize);

                ushort flags = (ushort)((VIFDescriptors[i].pgluMaterialIndex << 9) | (VIFDescriptors[i].pgluTextureIndex & 0b111111111)); 
                bs.WriteUInt16(flags);

                bs.Position = lastPos;
            }

            uint shapeSize = (uint)(lastPos - baseShapeOffset);
            bs.Position = (baseShapeOffset + 4);
            bs.WriteUInt32(shapeSize);
            bs.Position = lastPos;
        }

        public void DumpShape(string objFileName)
        {
            using StreamWriter sw = new StreamWriter(objFileName);
            using StreamWriter matSw = new StreamWriter(objFileName + ".mtl");

            string fileName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(objFileName));
            sw.WriteLine($"mtllib {Path.GetFileNameWithoutExtension(objFileName)}.obj.mtl");

            int packetFaceStart = 1;

            for (var j = 0; j < VIFPackets.Count; j++)
            {
                sw.WriteLine($"#############");
                sw.WriteLine($"# Packet[{j}]");
                sw.WriteLine($"#############");

                int startFace = packetFaceStart;
                int faceI = 0;

                VIFPacket packet = VIFPackets[j];
                VIFCommand vertCommand = packet.Commands.First(e => e.VUAddr == 0xC000 || e.VUAddr == 0x8000);
                VIFCommand uvCommand = packet.Commands.FirstOrDefault(e => (e.VUAddr == 0xC040 && e.UnpackData.Any(a => a is int[])) ||
                                                                  (e.VUAddr == 0x8040 && e.UnpackData.Any(a => a is short[])) );

                VIFCommand resets = packet.Commands.Find(e => e.VUAddr == 0xC040 && e.UnpackData.Any(a => a is byte[]));

                VIFDescriptor desc = VIFDescriptors[j];
                matSw.WriteLine($"newmtl {fileName}.{0}.{desc.pgluTextureIndex}");
                matSw.WriteLine($"map_Kd {fileName}_textures/{fileName}.{0}.{(desc.pgluTextureIndex) - 1}.png");

                int resetIndex = 1;
                int nextVertReset = ((resets.UnpackData[resetIndex] as byte[])[0] + 6) / 3;

                Vector3[] verts = new Vector3[vertCommand.Num];

                for (var l = 0; l < vertCommand.UnpackData.Count; l++)
                {
                    if (vertCommand.UnpackData[l] is short[] vertShortArr)
                    {
                        float xf = vertShortArr[0] / 256f;
                        float yf = vertShortArr[1] / 256f;
                        float zf = vertShortArr[2] / 256f;
                        verts[l] = new Vector3(xf, yf, zf);
                    }
                    else if (vertCommand.UnpackData[l] is int[] vertFloatArr)
                    {
                        var xf = BitConverter.Int32BitsToSingle(vertFloatArr[0]);
                        var yf = BitConverter.Int32BitsToSingle(vertFloatArr[1]);
                        var zf = BitConverter.Int32BitsToSingle(vertFloatArr[2]);
                        verts[l] = new Vector3(xf, yf, zf);
                    }

                    sw.WriteLine($"v {verts[l].X} {verts[l].Y} {verts[l].Z}");
                }

                /*
                float xmin = 0, ymin = 0, zmin = 0, xmax = 0, ymax = 0, zmax = 0;
                for (var i = 0; i < verts.Length; i++)
                {
                    var vert = verts[i];
                    if (vert.X < xmin)
                        xmin = vert.X;

                    if (vert.Y < ymin)
                        ymin = vert.Y;

                    if (vert.Z < zmin)
                        zmin = vert.Z;

                    if (vert.X > xmax)
                        xmax = vert.X;

                    if (vert.Y > ymax)
                        ymax = vert.Y;

                    if (vert.Z > zmax)
                        zmax = vert.Z;
                }
                */

                sw.WriteLine();

                if (uvCommand is not null)
                {
                    for (var l = 0; l < uvCommand.UnpackData.Count; l++)
                    {
                        if (uvCommand.UnpackData[l] is int[] uvArr && uvArr.Length == 2) // GT3 float UVs
                        {
                            var u = BitConverter.Int32BitsToSingle(uvArr[0]);
                            var v = BitConverter.Int32BitsToSingle(uvArr[1]);
                            sw.WriteLine($"vt {u} {-v}");
                        }
                        else if (uvCommand.UnpackData[l] is short[] uvShortArr && uvShortArr.Length == 2) // GT4
                        {
                            var u = uvShortArr[0] / 4096f;
                            var v = uvShortArr[1] / 4096f;
                            sw.WriteLine($"vt {u} {-v}");
                        }
                    }
                }
                

                sw.WriteLine();
                sw.WriteLine($"usemtl {fileName}.{0}.{desc.pgluTextureIndex}");

                List<ushort> faces = new List<ushort>();
                for (var l = 0; l < verts.Length; l++)
                {
                    faces.Add((ushort)(packetFaceStart + faceI));
                    faces.Add((ushort)(packetFaceStart + faceI + 1));
                    faces.Add((ushort)(packetFaceStart + faceI + 2));

                    sw.WriteLine($"f {packetFaceStart + faceI}/{packetFaceStart + faceI} " +
                        $"{packetFaceStart + faceI + 1}/{packetFaceStart + faceI + 1} " +
                        $"{packetFaceStart + faceI + 2}/{packetFaceStart + faceI + 2}");

                    if (faceI + 3 == nextVertReset)
                    {
                        resetIndex++;
                        if (resetIndex < resets.UnpackData.Count)
                            nextVertReset += ((resets.UnpackData[resetIndex] as byte[])[0] + 6) / 3;
                        else
                            break;

                        faceI += 3;

                        sw.WriteLine("# Reset\n");
                    }
                    else
                    {
                        faceI++;
                    }
                }

                packetFaceStart += vertCommand.Num;
                sw.WriteLine();
            }
        }
    }
}
