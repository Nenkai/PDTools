using System.Collections.Generic;
using System.IO;
using MDL3 = PDTools.Files.Models.PS3.ModelSet3.ModelSet3;

using Syroot.BinaryData;

using PDTools.Files.Models.PS3.ModelSet3.ShapeStream;


namespace PDTools.Files.Models.ShapeStream
{
    public class ShapeStream
    {
        public List<ShapeStreamChunk> Chunks = new List<ShapeStreamChunk>();

        static public ShapeStream FromStream(Stream stream, MDL3 mdl)
        {
            ShapeStream ss = new();

            ushort i = 0;
            foreach (MDL3ShapeStreamingChunkInfo ssInfo in mdl.StreamingInfo.ChunkInfos)
            {
                var chunk = ShapeStreamChunk.FromStream(stream, ssInfo);
                ss.Chunks.Add(chunk);
                i++;
            }

            return ss;
        }

        public ShapeStreamMesh GetMeshByIndex(ushort meshIndex)
        {
            foreach (var chunk in Chunks)
            {
                if (chunk.Meshes.TryGetValue(meshIndex, out ShapeStreamMesh mesh))
                    return mesh;
            }

            return null;
        }
    }
}
