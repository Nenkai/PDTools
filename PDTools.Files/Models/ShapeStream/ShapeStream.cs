using PDTools.Files.Models.ModelSet3.Meshes;
using PDTools.Files.Models.ModelSet3.ShapeStream;
using Syroot.BinaryData;
using System.Collections.Generic;
using System.IO;
using MDL3 = PDTools.Files.Models.ModelSet3.ModelSet3;

namespace PDTools.Files.Models.ShapeStream
{
    public class ShapeStream
    {
        public Dictionary<ushort, ShapeStreamMesh> Meshes;

        static public ShapeStream FromStream(Stream stream, MDL3 mdl)
        {
            ShapeStream ss = new()
            {
                Meshes = new()
            };

            ushort i = 0;
            foreach (MDL3ShapeStreamingInfo ssInfo in mdl.StreamingInfo.Infos)
            {
                var meshDictionary = ShapeStreamChunk.MeshesFromStream(stream, ssInfo);
                foreach (var meshItem in meshDictionary)
                {
                    ss.Meshes.Add(meshItem.Key, meshItem.Value);
                }
                i++;
            }

            return ss;
        }
    }
}
