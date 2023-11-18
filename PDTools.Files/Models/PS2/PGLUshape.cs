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
        /* Valid values are 1-2-3-4-5, GT4 only supports 1 & 4. 
         * 2 uses a value from command 51 (gt3 only command). */
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
            //bs.Position = (baseShapeOffset + 4);
            //bs.WriteUInt32(shapeSize);
            bs.Position = lastPos;
        }

        public PGLUshapeConverted GetShapeData()
        {
            int packetFaceStart = 1;

            PGLUshapeConverted shapeData = new PGLUshapeConverted();

            for (var j = 0; j < VIFPackets.Count; j++)
            {
                int startFace = packetFaceStart;
                int faceI = 0;

                VIFPacket packet = VIFPackets[j];
                VIFCommand vertCommand = packet.Commands.First(e => e.VUAddr == 0xC000 || e.VUAddr == 0x8000);
                VIFCommand uvCommand = packet.Commands.FirstOrDefault(e => (e.VUAddr == 0xC040 && e.UnpackData.Any(a => a is int[])) ||
                                                                  (e.VUAddr == 0x8040 && e.UnpackData.Any(a => a is short[])) );

                VIFCommand resets = packet.Commands.Find(e => e.VUAddr == 0xC040 && e.UnpackData.Any(a => a is byte[]));

                VIFDescriptor desc = VIFDescriptors[j];

                int resetIndex = 1;
                int nextVertReset = ((resets.UnpackData[resetIndex] as byte[])[0] + 6) / 3;

                for (var l = 0; l < vertCommand.UnpackData.Count; l++)
                {
                    if (vertCommand.UnpackData[l] is short[] vertShortArr)
                    {
                        float xf = vertShortArr[0] / 4096f;
                        float yf = vertShortArr[1] / 4096f;
                        float zf = vertShortArr[2] / 4096f;
                        shapeData.Vertices.Add(new Vector3(xf, yf, zf));
                    }
                    else if (vertCommand.UnpackData[l] is int[] vertFloatArr)
                    {
                        var xf = BitConverter.Int32BitsToSingle(vertFloatArr[0]);
                        var yf = BitConverter.Int32BitsToSingle(vertFloatArr[1]);
                        var zf = BitConverter.Int32BitsToSingle(vertFloatArr[2]);
                        shapeData.Vertices.Add(new Vector3(xf, yf, zf));
                    }
                }

                if (uvCommand is not null)
                {
                    for (var l = 0; l < uvCommand.UnpackData.Count; l++)
                    {
                        if (uvCommand.UnpackData[l] is int[] uvArr && uvArr.Length == 2) // GT3 float UVs
                        {
                            var u = BitConverter.Int32BitsToSingle(uvArr[0]);
                            var v = BitConverter.Int32BitsToSingle(uvArr[1]);
                            shapeData.UVs.Add(new Vector2(u, 1.0f - v));
                        }
                        else if (uvCommand.UnpackData[l] is short[] uvShortArr && uvShortArr.Length == 2) // GT4
                        {
                            var u = uvShortArr[0] / 4096f;
                            var v = uvShortArr[1] / 4096f;
                            shapeData.UVs.Add(new Vector2(u, 1.0f - v));
                        }
                    }
                }
                
                for (var l = 0; l < shapeData.Vertices.Count; l++)
                {
                    shapeData.Faces.Add((
                        (ushort)(packetFaceStart + faceI),
                        (ushort)(packetFaceStart + faceI + 1), 
                        (ushort)(packetFaceStart + faceI + 2),
                        desc.pgluMaterialIndex,
                        desc.pgluTextureIndex)
                    );

                    if (faceI + 3 == nextVertReset)
                    {
                        resetIndex++;
                        if (resetIndex < resets.UnpackData.Count)
                            nextVertReset += ((resets.UnpackData[resetIndex] as byte[])[0] + 6) / 3;
                        else
                            break;

                        faceI += 3;
                    }
                    else
                    {
                        faceI++;
                    }
                }

                packetFaceStart += vertCommand.Num;
            }

            return shapeData;
        }
    }

    public class PGLUshapeConverted
    {
        public int MaterialIndex { get; set; }
        public int TextureIndex { get; set; }
        public List<Vector3> Vertices { get; set; } = new();
        public List<(int A, int B, int C, int MatIdx, int TexId)> Faces { get; set; } = new();
        public List<Vector2> UVs { get; set; } = new();
        public List<Vector3> Normals { get; set; } = new();

        public void Dump(StreamWriter objWriter, StreamWriter matWriter, int texSetIndex, int faceIdxStart, int vtIdxStart)
        {
            for (int i = 0; i < Vertices.Count; i++)
                objWriter.WriteLine($"v {Vertices[i].X} {Vertices[i].Y} {Vertices[i].Z}");
            objWriter.WriteLine();

            for (int i = 0; i < UVs.Count; i++)
                objWriter.WriteLine($"vt {UVs[i].X} {UVs[i].Y}");

            objWriter.WriteLine();

            int currentTexId = -1;
            int currentMatId = -1;
            for (int i = 0; i < Faces.Count; i++)
            {
                if (Faces[i].MatIdx != currentMatId || Faces[i].TexId != currentTexId)
                {
                    currentMatId = Faces[i].MatIdx;
                    currentTexId = Faces[i].TexId;

                    objWriter.WriteLine($"usemtl Mat{currentMatId - 1}_Tex{currentTexId - 1}");

                    matWriter.WriteLine($"newmtl Mat{currentMatId - 1}_Tex{currentTexId - 1}");

                    if (Faces[i].TexId != 511) // External
                        matWriter.WriteLine($"map_Kd {texSetIndex}.{currentTexId - 1}.png");

                    matWriter.WriteLine();
                }

                objWriter.Write($"f {faceIdxStart + Faces[i].A}");
                if (UVs.Count > 0)
                    objWriter.Write($"/{vtIdxStart + Faces[i].A}");
                objWriter.Write(" ");

                objWriter.Write($"{faceIdxStart + Faces[i].B}");
                if (UVs.Count > 0)
                    objWriter.Write($"/{vtIdxStart + Faces[i].B}");
                objWriter.Write(" ");

                objWriter.Write($"{faceIdxStart + Faces[i].C}");
                if (UVs.Count > 0)
                    objWriter.Write($"/{vtIdxStart + Faces[i].C}");
                objWriter.WriteLine();
            }
        }
    }
}
