using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using PDTools.Files.Models.PS2.Commands;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2;

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

    public void FromStream(BinaryStream bs)
    {
        long basePos = bs.Position;

        bs.ReadInt32(); // Reloc ptr
        uint unkSizeMaybe = bs.ReadUInt32(); // TODO: Investigate this - might actually be a pointer to something!

        byte bits = bs.Read1Byte();
        Unk1 = (byte)(bits & 0b11111);
        Unk2 = (byte)(bits >> 5);
        Unk3 = bs.Read1Byte();

        ushort vifChunksCount = bs.ReadUInt16();
        TotalStripVerts = bs.ReadUInt16();
        NumTriangles = bs.ReadUInt16();

        for (var i = 0; i < vifChunksCount; i++)
        {
            var desc = VIFDescriptor.FromStream(bs);
            VIFDescriptors.Add(desc);
        }

        for (var i = 0; i < vifChunksCount; i++)
        {
            bs.Position = basePos + VIFDescriptors[i].VIFDataOffset;
            var packet = VIFPacket.FromStream(bs, VIFDescriptors[i].DMATagQuadwordCount);
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

        PGLUshapeConverted shapeData = new();

        for (var j = 0; j < VIFPackets.Count; j++)
        {
            int faceI = 0;

            VIFPacket packet = VIFPackets[j];
            VIFCommand vertCommand = packet.Commands.FirstOrDefault(e => e.VUAddr == 0xC000 || // Regular
                                                                         e.VUAddr == 0x8000);  // GT4 (compressed)

            VIFCommand uvCommand = packet.Commands.FirstOrDefault(e => (e.VUAddr == 0xC040 && e.UnpackData.Any(a => a is int[])) ||  // Regular 
                                                                       (e.VUAddr == 0x8040 && e.UnpackData.Any(a => a is short[]))); // GT4 (compressed)

            VIFCommand normalsCommand = packet.Commands.FirstOrDefault(e => (e.VUAddr == 0xC040 ||   // Regular
                                                                             e.VUAddr == 0xC080) &&  // External Texture
                                                                             e.UnpackData.Any(a => a is int[] arr && arr.Length == 3));

            VIFCommand vertColors = packet.Commands.FirstOrDefault(e => e.VUAddr == 0xC080 && e.UnpackData.Any(a => a is byte[]));


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
                    float xf = BitConverter.Int32BitsToSingle(vertFloatArr[0]);
                    float yf = BitConverter.Int32BitsToSingle(vertFloatArr[1]);
                    float zf = BitConverter.Int32BitsToSingle(vertFloatArr[2]);
                    shapeData.Vertices.Add(new Vector3(xf, yf, zf));
                }
            }

            if (uvCommand is not null)
            {
                for (var l = 0; l < uvCommand.UnpackData.Count; l++)
                {
                    if (uvCommand.UnpackData[l] is int[] uvArr && uvArr.Length == 2) // GT3 float UVs
                    {
                        float u = BitConverter.Int32BitsToSingle(uvArr[0]);
                        float v = BitConverter.Int32BitsToSingle(uvArr[1]);

                        shapeData.UVs.Add(new Vector2(u, 1.0f - v));
                    }
                    else if (uvCommand.UnpackData[l] is short[] uvShortArr && uvShortArr.Length == 2) // GT4
                    {
                        float u = uvShortArr[0] / 4096f;
                        float v = uvShortArr[1] / 4096f;
                        shapeData.UVs.Add(new Vector2(u, 1.0f - v));
                    }
                }
            }

            if (normalsCommand is not null)
            {
                for (var l = 0; l < normalsCommand.UnpackData.Count; l++)
                {
                    var norms = (int[])normalsCommand.UnpackData[l];
                    float xf = BitConverter.Int32BitsToSingle(norms[0]);
                    float yf = BitConverter.Int32BitsToSingle(norms[1]);
                    float zf = BitConverter.Int32BitsToSingle(norms[2]);
                    shapeData.Normals.Add(new Vector3(xf, yf, zf));
                }
            }

            if (vertColors is not null)
            {
                for (var l = 0; l < vertColors.UnpackData.Count; l++)
                {
                    var colors = (byte[])vertColors.UnpackData[l];
                    shapeData.Colors.Add([colors[0], colors[1], colors[2], colors[3]]);
                }
            }

            bool oddTriangle = false;
            for (var l = 0; l < shapeData.Vertices.Count; l++)
            {
                if (!oddTriangle)
                {
                    shapeData.Faces.Add((
                        (ushort)(packetFaceStart + faceI),
                        (ushort)(packetFaceStart + faceI + 1),
                        (ushort)(packetFaceStart + faceI + 2),
                        desc.pgluMaterialIndex,
                        desc.pgluTextureIndex)
                    );
                    oddTriangle = true;
                }
                else
                {
                    shapeData.Faces.Add((
                        (ushort)(packetFaceStart + faceI + 2),
                        (ushort)(packetFaceStart + faceI + 1),
                        (ushort)(packetFaceStart + faceI),
                        desc.pgluMaterialIndex,
                        desc.pgluTextureIndex)
                    );
                    oddTriangle = false;
                }

                if (faceI + 3 == nextVertReset)
                {
                    resetIndex++;
                    if (resetIndex < resets.UnpackData.Count)
                        nextVertReset += ((resets.UnpackData[resetIndex] as byte[])[0] + 6) / 3;
                    else
                        break;

                    faceI += 3;
                    oddTriangle = false;
                }
                else
                {
                    faceI++;
                }
            }

            packetFaceStart += vertCommand.Num;
        }

        shapeData.UsesExternalTexture = VIFDescriptors.Any(e => e.pgluTextureIndex == VIFDescriptor.EXTERNAL_TEXTURE);
        return shapeData;
    }
}

public class PGLUshapeConverted
{
    /// <summary>
    /// Original shape index
    /// </summary>
    public int ShapeIndex { get; set; }

    public bool UsesExternalTexture { get; set; }
    public int TextureSetIndex { get; set; }
    public List<Vector3> Vertices { get; set; } = [];
    public List<(int A, int B, int C, int MatIdx, int TexId)> Faces { get; set; } = [];
    public List<Vector2> UVs { get; set; } = [];
    public List<Vector3> Normals { get; set; } = [];
    public List<byte[]> Colors { get; set; } = [];
    public List<ModelSetupPS2Command> RenderCommands { get; set; }

    /// <summary>
    /// Dumps the shape to a obj file
    /// </summary>
    /// <param name="file"></param>
    public void DumpToObjFile(string file, List<PGLUmaterial> materials = null)
    {
        using var objWriter = new StreamWriter(Path.ChangeExtension(file, ".obj"));
        using var mtlWriter = new StreamWriter(Path.ChangeExtension(file, ".mtl"));

        DumpToObj(objWriter, mtlWriter, 0, 0, 0, 0, materials);
    }

    /// <summary>
    /// Dumps the shape to a obj/mtl writer
    /// </summary>
    /// <param name="objWriter"></param>
    /// <param name="matWriter"></param>
    /// <param name="texSetIndex"></param>
    /// <param name="faceIdxStart"></param>
    /// <param name="vtIdxStart"></param>
    public void DumpToObj(StreamWriter objWriter, StreamWriter matWriter, int texSetIndex, int faceIdxStart, int vnIdxStart, int vtIdxStart, List<PGLUmaterial> materials)
    {
        for (int i = 0; i < Vertices.Count; i++)
        {
            objWriter.Write($"v {Vertices[i].X} {Vertices[i].Y} {Vertices[i].Z}");
            if (Colors.Count == Vertices.Count)
            {
                objWriter.Write($" {Colors[i][0] * (1.0f / 255.0f)} {Colors[i][1] * (1.0f / 255.0f)} {Colors[i][2] * (1.0f / 255.0f)}"); // Alpha not supported
            }

            objWriter.WriteLine();
        }
        objWriter.WriteLine();

        for (int i = 0; i < UVs.Count; i++)
            objWriter.WriteLine($"vt {UVs[i].X} {UVs[i].Y}");

        for (int i = 0; i < Normals.Count; i++)
            objWriter.WriteLine($"vn {Normals[i].X} {Normals[i].Y} {Normals[i].Z}");

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

                if (currentMatId != 0 && materials is not null)
                {
                    var material = materials[currentMatId - 1];
                    matWriter.WriteLine($"Ka {material.Ambient.R} {material.Ambient.G} {material.Ambient.B} {material.Ambient.A}");
                    matWriter.WriteLine($"Kd {material.Diffuse.R} {material.Diffuse.G} {material.Diffuse.B} {material.Diffuse.A}");
                    matWriter.WriteLine($"Ks {material.Specular.R} {material.Specular.G} {material.Specular.B} {material.Specular.A}");
                    matWriter.WriteLine($"Ke {material.UnkColor.R} {material.UnkColor.G} {material.UnkColor.B} {material.UnkColor.A}");
                }

                if (Faces[i].TexId != 0 && Faces[i].TexId != 511) // Not External
                {
                    matWriter.WriteLine($"map_Kd Textures/{texSetIndex}.{currentTexId - 1}.png");
                    matWriter.WriteLine($"map_Ka Textures/{texSetIndex}.{currentTexId - 1}.png");
                    matWriter.WriteLine($"map_Ks Textures/{texSetIndex}.{currentTexId - 1}.png");
                }

                matWriter.WriteLine();
            }

            objWriter.Write($"f {faceIdxStart + Faces[i].A}");
            if (UVs.Count > 0)
                objWriter.Write($"/{vtIdxStart + Faces[i].A}");
            if (Normals.Count > 0)
            {
                if (UVs.Count == 0)
                    objWriter.Write('/');
                objWriter.Write($"/{vnIdxStart + Faces[i].A}");
            }
            objWriter.Write(" ");

            objWriter.Write($"{faceIdxStart + Faces[i].B}");
            if (UVs.Count > 0)
                objWriter.Write($"/{vtIdxStart + Faces[i].B}");
            if (Normals.Count > 0)
            {
                if (UVs.Count == 0)
                    objWriter.Write('/');
                objWriter.Write($"/{vnIdxStart + Faces[i].B}");
            }
            objWriter.Write(" ");

            objWriter.Write($"{faceIdxStart + Faces[i].C}");
            if (UVs.Count > 0)
                objWriter.Write($"/{vtIdxStart + Faces[i].C}");
            if (Normals.Count > 0)
            {
                if (UVs.Count == 0)
                    objWriter.Write('/');
                objWriter.Write($"/{vnIdxStart + Faces[i].C}");
            }
            objWriter.WriteLine();
        }
    }
}
