using PDTools.Files.Models.PS2.RenderCommands;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDTools.Files.Models.PS2.ModelSet;

public class ModelCommandShapeExtractor
{
    public RenderCommandContext RenderCommandContext { get; set; } = new();

    public string ModelName { get; set; }
    public string ExtraShapeName { get; set; }

    public int ModelIndex { get; set; }
    public int CurrentLOD { get; set; } = -1;

    public List<DumpedLOD> LODToShapes { get; set; } = [];

    private int _texSetIndex { get; set; }

    public string OutputDir { get; set; }

    public ModelCallbackParameter? CurrentCallback { get; set; }
    public int CallbackBranchIndex { get; set; } = -1;

    public List<ModelSetupPS2Command> CommandQueue { get; set; } = [];

    public void SetTexTable(int index)
    {
        _texSetIndex = index;
    }

    public void SetLOD(int lod)
    {
        CurrentLOD = lod;

        if (LODToShapes.Count - 1 < lod)
            LODToShapes.Add(new DumpedLOD());
    }

    public void AddShape(string name, PGLUshapeConverted shape)
    {
        if (CurrentLOD == -1)
        {
            CurrentLOD = 0;
            LODToShapes.Add(new DumpedLOD());
        }

        shape.TextureSetIndex = _texSetIndex;
        LODToShapes[CurrentLOD].Add(name, shape);

        if (CurrentCallback is not null)
        {
            LODToShapes[CurrentLOD].AddCallbackShape(CurrentCallback.Value, CallbackBranchIndex, name, shape);
        }
    }
}

public class DumpedLOD
{
    public Dictionary<string, PGLUshapeConverted> Shapes { get; set; } = [];
    public Dictionary<ModelCallbackParameter, List<List<string>>> Callbacks { get; set; } = [];

    public void Add(string name, PGLUshapeConverted shape)
    {
        // sometimes a shape can be called again? it's weird, needs investigation (gt4: ac_0014, model index 0)
        Shapes.TryAdd(name, shape);
    }

    public void AddCallbackShape(ModelCallbackParameter parameter, int idx, string name, PGLUshapeConverted shape)
    {
        Callbacks.TryAdd(parameter, []);

        while (Callbacks[parameter].Count <= idx)
            Callbacks[parameter].Add([]);

        Callbacks[parameter][idx].Add(name);
    }
}
