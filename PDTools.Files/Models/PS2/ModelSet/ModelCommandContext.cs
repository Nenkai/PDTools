using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS2.ModelSet;

public class ModelCommandContext
{
    public string ModelName { get; set; }

    public int CallbackParameter { get; set; } = -1;
    public string ExtraShapeName { get; set; }

    public int ModelIndex { get; set; }
    public int CurrentLOD { get; set; } = -1;
    public List<Dictionary<string, PGLUshapeConverted>> LODToShapes { get; set; } = [];

    private int _texSetIndex { get; set; }

    public string OutputDir { get; set; }


    public void SetTexTable(int index)
    {
        _texSetIndex = index;
    }

    public void SetLOD(int lod)
    {
        CurrentLOD = lod;

        if (LODToShapes.Count - 1 < lod)
            LODToShapes.Add([]);
    }

    public void AddShape(string name, PGLUshapeConverted shape)
    {
        if (CurrentLOD == -1)
        {
            CurrentLOD = 0;
            LODToShapes.Add([]);
        }

        shape.TextureSetIndex = _texSetIndex;
        LODToShapes[CurrentLOD].Add(name, shape);
    }
}
