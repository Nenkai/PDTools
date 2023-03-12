using System;
using System.IO;
using PDTools.Files.Models.ModelSet3.ShapeStream;
using Syroot.BinaryData;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System.Collections.Generic;

namespace PDTools.Files.Models.ShapeStream
{
    public class ShapeStreamChunk
    {
        public byte[] DeflatedData;
        public BinaryStream Stream;

        public static Dictionary<ushort, ShapeStreamMesh> MeshesFromStream(Stream stream, MDL3ShapeStreamingInfo shapeStreamInfo)
        {
            BinaryStream bs = new BinaryStream(stream);

            ShapeStreamChunk ssChunk = new();

            bs.Seek(shapeStreamInfo.DataOffset, SeekOrigin.Begin);
            byte[] data = bs.ReadBytes((int)shapeStreamInfo.DataSize);

            byte[] bigBuffer = new byte[0x100000];
            var inflater = new Inflater(noHeader: true);
            inflater.SetInput(data, 0, data.Length);
            var dataLength = inflater.Inflate(bigBuffer);

            ssChunk.DeflatedData = new byte[dataLength];
            Array.Copy(bigBuffer, ssChunk.DeflatedData, dataLength);

            MemoryStream ms = new MemoryStream(ssChunk.DeflatedData);
            ssChunk.Stream = new BinaryStream(ms, ByteConverter.Big);
            
            Dictionary<ushort, ShapeStreamMesh> meshes = new();
            foreach (MDL3ShapeStreamingInfoMeshEntry entry in shapeStreamInfo.Entries.Values)
            {
                uint mdlbasepos = entry.OffsetWithinShapeStream;
                ssChunk.Stream.Position = mdlbasepos;

                ShapeStreamMesh mesh = new();
                mesh.ShapeStreamChunk = ssChunk;
                mesh.InfoMeshEntry = entry;

                mesh.MeshDataSize = ssChunk.Stream.ReadUInt32();
                ssChunk.Stream.Position += 0x8;
                mesh.VerticesOffset = ssChunk.Stream.ReadUInt32();
                mesh.TriOffset = ssChunk.Stream.ReadUInt32();
                ssChunk.Stream.Position += 0x8;
                mesh.BBoxOffset = ssChunk.Stream.ReadUInt32();

                meshes.Add(entry.MeshIndex, mesh);
            }

            return meshes;
        }
    }
}
