using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS2.ModelSet
{
    public class ModelCommandContext
    {
        public int CallbackParameter { get; set; } = -1;
        public string ExtraName { get; set; }

        public int ModelIndex { get; set; }
        public int LOD { get; set; } = -1;

        private int _texSetIndex { get; set; }

        public string OutputDir { get; set; }
        public StreamWriter ObjWriter { get; set; }
        public StreamWriter MatWriter { get; set; }

        public int VertIdxStart { get; set; }
        public int VTIdxStart { get; set; }

        public void SetTexTable(int index)
        {
            _texSetIndex = index;
        }

        public void SetLOD(int lod)
        {
            LOD = lod;
            ResetIndices();
        }

        public void ResetIndices()
        {
            VertIdxStart = 0;
            VTIdxStart = 0;
        }

        public void DumpShapeToObj(PGLUshapeConverted shape, int shapeIndex, string shapeName)
        {
            HashSet<int> texIndices = new HashSet<int>();
            for (int i = 0; i < shape.Faces.Count; i++)
            {
                if (!texIndices.Contains(shape.Faces[i].TexId))
                    texIndices.Add(shape.Faces[i].TexId);
            }

            bool isExternalTextureReflection = texIndices.All(e => e == VIFDescriptor.EXTERNAL_TEXTURE);

            ObjWriter.WriteLine($"# Shape {shapeIndex}");

            if (isExternalTextureReflection)
                ObjWriter.WriteLine($"o {shapeName}_reflection");
            else
                ObjWriter.WriteLine($"o {shapeName}");

            shape.Dump(ObjWriter, MatWriter, _texSetIndex, VertIdxStart, VTIdxStart);
            ObjWriter.WriteLine();

            VertIdxStart += shape.Vertices.Count;
            VTIdxStart += shape.UVs.Count;
        }

        public void SetupObjWriter(string name)
        {
            ObjWriter = new StreamWriter(Path.Combine(OutputDir, $"{name}.obj"));
            MatWriter = new StreamWriter(Path.Combine(OutputDir, $"{name}.mtl"));
        }

        public void FinishModel()
        {
            if (ObjWriter is not null)
            {
                ObjWriter.Flush();
                ObjWriter.Dispose();
                ObjWriter = null;
            }

            if (MatWriter is not null)
            {
                MatWriter.Flush();
                MatWriter.Dispose();
                MatWriter = null;
            }
        }
    }
}
