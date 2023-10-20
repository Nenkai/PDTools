using PDTools.Files.Models.ModelSet3;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using PDTools.Files.Textures.PS2;

namespace PDTools.Files.Models.ModelSet2
{
    public class ModelSet2
    {
        const string MAGIC = "MDLS";

        public RelocatorInfo Relocator { get; set; }

        public List<Model> Models { get; set; } = new List<Model>();
        public List<PGLUshape> Shapes { get; set; } = new List<PGLUshape>();

        /// <summary>
        /// Textures. Each texture set within a list represents seemingly one lod level.
        /// </summary>
        public List<List<TextureSet1>> TextureSetLists { get; set; } = new List<List<TextureSet1>>();

        public static ModelSet2 FromStream(Stream stream)
        {
            using var bs = new BinaryStream(stream);
            long basePos = bs.Position;

            string magic = bs.ReadString(4);
            if (magic != MAGIC)
                throw new InvalidDataException("Not a valid ModelSet2 file.");

            /* HEADER - 0xE4 */
            ModelSet2 modelSet = new();

            int relocatorInfoOffset = bs.ReadInt32();
            int relocatorDataSize = bs.ReadInt32();
            int relocationBase = bs.ReadInt32();
            int fileSize = bs.ReadInt32();

            ushort unk = bs.ReadUInt16();
            ushort modelCount = bs.ReadUInt16();
            ushort shapeCount = bs.ReadUInt16();
            ushort pgluMatTableCount = bs.ReadUInt16();
            ushort textureSetLodLevelCount = bs.ReadUInt16();
            ushort textureSetListCount = bs.ReadUInt16();

            bs.Position = basePos + 0x38;
            uint modelsOffset = bs.ReadUInt32();
            uint shapesOffset = bs.ReadUInt32();
            uint pgluMatTableOffset = bs.ReadUInt32();
            uint pgluTexSetsOffset = bs.ReadUInt32();

            bs.Position = basePos + shapesOffset;

            bs.Position = basePos + relocatorInfoOffset;
            modelSet.Relocator = RelocatorInfo.FromStream(bs, basePos);

            modelSet.ReadModels(bs, basePos, modelsOffset, modelCount);
            modelSet.ReadShapes(bs, basePos, shapesOffset, shapeCount);
            modelSet.ReadTextureSets(bs, basePos, pgluTexSetsOffset, textureSetListCount, textureSetLodLevelCount);
            

            return modelSet;
        }

        private void ReadModels(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * Model.GetSize());

                var model = Model.FromStream(bs, baseMdlPos);
                Models.Add(model);
            }
        }

        private void ReadShapes(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * sizeof(int));
                uint shapeOffset = bs.ReadUInt32();

                bs.Position = baseMdlPos + shapeOffset;

                var shape = PGLUshape.FromStream(bs, baseMdlPos);
                Shapes.Add(shape);
            }
        }

        private void ReadTextureSets(BinaryStream bs, long baseMdlPos, uint offset, uint listCount, uint textureLodLevels)
        {
            for (int i = 0; i < listCount; i++)
            {
                bs.Position = baseMdlPos + offset + (i * sizeof(int));
                int entriesOffset = bs.ReadInt32();

                // One texture set per lod level
                List<TextureSet1> list = new List<TextureSet1>();
                for (int j = 0; j < textureLodLevels; j++)
                {
                    bs.Position = baseMdlPos + entriesOffset + (j * sizeof(int));

                    int off = bs.ReadInt32();
                    bs.Position = baseMdlPos + off;

                    TextureSet1 textureSet = new TextureSet1();
                    textureSet.FromStream(bs);
                    list.Add(textureSet);
                }

                TextureSetLists.Add(list);
            }
        }

        public void DumpShape(int shapeIndex)
        {

            PGLUshape shape = Shapes[shapeIndex];
            using StreamWriter sw = new StreamWriter($"shape{shapeIndex}.obj");

            int packetFaceStart = 1;

            for (var j = 0; j < shape.VIFPackets.Count; j++)
            {
                sw.WriteLine($"# Packet[{j}]\n");

                int startFace = packetFaceStart;
                int faceI = 0;

                var packet = shape.VIFPackets[j];
                var vertCommand = packet.Commands.First(e => e.VUAddr == 0xC000 || e.VUAddr == 0x8000);
                var resets = packet.Commands.Find(e => e.VUAddr == 0xC040 && e.UnpackData.Any(a => a is byte[]));

                int resetIndex = 1;
                int nextVertReset = ((resets.UnpackData[resetIndex] as byte[])[0] + 6) / 3;

                Vector3[] verts = new Vector3[vertCommand.Num];

                for (var l = 0; l < vertCommand.UnpackData.Count; l++)
                {
                    if (vertCommand.UnpackData[l] is short[])
                    {
                        float xf = (vertCommand.UnpackData[l] as short[])[0] / 256f;
                        float yf = (vertCommand.UnpackData[l] as short[])[1] / 256f;
                        float zf = (vertCommand.UnpackData[l] as short[])[2] / 256f;
                        verts[l] = new Vector3(xf, yf, zf);
                    }
                    else
                    {
                        var xf = BitConverter.Int32BitsToSingle((vertCommand.UnpackData[l] as int[])[0]) * 16;
                        var yf = BitConverter.Int32BitsToSingle((vertCommand.UnpackData[l] as int[])[1]) * 16;
                        var zf = BitConverter.Int32BitsToSingle((vertCommand.UnpackData[l] as int[])[2]) * 16;
                        verts[l] = new Vector3(xf, yf, zf);
                    }

                    sw.WriteLine($"v {verts[l].X} {verts[l].Y} {verts[l].Z}");
                }

                sw.WriteLine();

                for (var l = 0; l < verts.Length; l++)
                {
                    sw.WriteLine($"f {packetFaceStart + faceI} {packetFaceStart + faceI + 1} {packetFaceStart + faceI + 2}");

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
            }
        }
    }
}
