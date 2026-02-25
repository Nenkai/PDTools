using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Files.Models.PS2.Commands;
using PDTools.Files.Textures.PS2;

namespace PDTools.Files.Models.PS2.ModelSet;

public abstract class ModelSetPS2Base
{
    /// <summary>
    /// Models in this model set. The game will iterate through all of these & their commands to render on every tick.
    /// </summary>
    public List<ModelPS2Base> Models { get; set; } = [];

    /// <summary>
    /// Shapes aka meshes.
    /// </summary>
    public List<PGLUshape> Shapes { get; set; } = [];

    public abstract int GetNumModels();

    public abstract List<TextureSet1> GetTextureSetList();

    public abstract uint AddShape(PGLUshape shape);

    public abstract PGLUshape GetShape(int shapeIndex);

    public abstract int GetNumShapes();

    public abstract int AddMaterial(PGLUmaterial material);

    public abstract int GetMaterialCount();

    public abstract List<PGLUmaterial> GetVariationMaterials(int varIndex);

    public abstract int GetNumVariations();

    //////////////////////////////////
    /// Interpreter for dumping
    //////////////////////////////////

    public List<DumpedLOD> DumpModelLODs(int modelIndex, string outdir)
    {
        ModelCommandShapeExtractor context = new()
        {
            ModelIndex = modelIndex,
            OutputDir = outdir
        };

        ModelPS2Base model = Models[modelIndex];

        ProcessCommands(context, model.Commands);
        return context.LODToShapes;
    }

    /// <summary>
    /// Brute-force dumps ALL shapes from the shape table, bypassing the command tree.
    /// This ensures shapes behind unhandled VM commands or callbacks are not missed.
    /// Shapes already found via command-tree walking can be passed in to provide naming context.
    /// </summary>
    public List<DumpedLOD> DumpAllShapes(int modelIndex, HashSet<int> alreadyDumpedShapeIndices)
    {
        var lod = new DumpedLOD();

        for (int i = 0; i < Shapes.Count; i++)
        {
            if (alreadyDumpedShapeIndices.Contains(i))
                continue; // Already dumped via normal command-tree walk

            try
            {
                PGLUshapeConverted shapeData = Shapes[i].GetShapeData();
                shapeData.ShapeIndex = i;

                string name = $"shape{i}_extra";
                if (shapeData.UsesExternalTexture)
                    name += "_reflection";

                lod.Add(name, shapeData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Warning: Could not extract shape {i}: {ex.Message}");
            }
        }

        if (lod.Shapes.Count > 0)
            return [lod];

        return [];
    }

    /// <summary>
    /// Gets a friendly name for a callback parameter for shape naming.
    /// </summary>
    private static string GetCallbackName(ModelCallbackParameter param)
    {
        return param switch
        {
            ModelCallbackParameter.IsTailLampActive => "tail_lamp",
            ModelCallbackParameter.TweenShapeSpeedRandom_1 => "aero_tween1",
            ModelCallbackParameter.TweenShapeSpeedRandom_2 => "aero_tween2",
            ModelCallbackParameter.TweenShapeSpeedRandom_3 => "aero_tween3",
            ModelCallbackParameter.TweenShapeSpeedRandom_4 => "aero_tween4",
            ModelCallbackParameter.SetSteering => "steering",
            ModelCallbackParameter.SetActiveWingShapeTweenRatio => "active_wing",
            ModelCallbackParameter.GetTimeZone => "timezone",
            ModelCallbackParameter.RotateZ => "rotate_z",
            _ => $"callback_{(int)param}",
        };
    }

    private void ProcessCommands(ModelCommandShapeExtractor extractor, List<ModelSetupPS2Command> cmds)
    {
        foreach (ModelSetupPS2Command command in cmds)
        {
            switch (command.Opcode)
            {
                case ModelSetupPS2Opcode.BBoxRender:
                    ProcessCommands(extractor, (command as Cmd_BBoxRender).CommandsOnRender);
                    break;

                case ModelSetupPS2Opcode.LODSelect:
                    var lodSel = command as Cmd_LODSelect;
                    for (int i = 0; i < lodSel.CommandsPerLOD.Count; i++)
                    {
                        extractor.SetLOD(i);

                        ProcessCommands(extractor, lodSel.CommandsPerLOD[i]);
                    }
                    break;

                case ModelSetupPS2Opcode.CallModelCallback:
                    var callbackCmd = command as Cmd_CallModelCallback;
                    {
                        string callbackName = GetCallbackName(callbackCmd.Parameter);

                        // Process default commands (always present)
                        extractor.CurrentCallback = callbackCmd.Parameter;
                        ProcessCommands(extractor, callbackCmd.Default);

                        // Process ALL branches regardless of parameter type
                        for (int i = 0; i < callbackCmd.CommandsPerBranch.Count; i++)
                        {
                            extractor.CallbackBranchIndex = i;

                            // Use specific names for known parameters
                            if (callbackCmd.Parameter == ModelCallbackParameter.IsTailLampActive)
                                extractor.ExtraShapeName = i == 0 ? "tail_lamp_off" : "tail_lamp_on";
                            else if (callbackCmd.Parameter == ModelCallbackParameter.GetTimeZone)
                                extractor.ExtraShapeName = $"timezone_b{i}";
                            else
                                extractor.ExtraShapeName = $"{callbackName}_b{i}";

                            ProcessCommands(extractor, callbackCmd.CommandsPerBranch[i]);
                        }

                        extractor.ExtraShapeName = null;
                        extractor.CurrentCallback = null;
                    }
                    break;

                case ModelSetupPS2Opcode.VM_Branch:
                    var vmBranchCmd = command as Cmd_VM_Branch;
                    for (int i = 0; i < vmBranchCmd.CommandsPerBranch.Count; i++)
                    {
                        extractor.CallbackBranchIndex = i;
                        extractor.ExtraShapeName = $"vmbranch{vmBranchCmd.OutRegisterIndex}_b{i}";

                        ProcessCommands(extractor, vmBranchCmd.CommandsPerBranch[i]);
                    }

                    extractor.ExtraShapeName = null;
                    extractor.CallbackBranchIndex = -1;
                    break;

                case ModelSetupPS2Opcode.pgluSetTexTable_Byte:
                    byte index = (command as Cmd_pgluSetTexTable_Byte).TexSetTableIndex;
                    extractor.SetTexTable(index);
                    break;

                case ModelSetupPS2Opcode.pgluSetTexTable_UShort:
                    ushort indexShort = (command as Cmd_pgluSetTexTable_UShort).TexSetTableIndex;
                    extractor.SetTexTable(indexShort);
                    break;

                case ModelSetupPS2Opcode.pgluCallShape_Byte:
                    {
                        if (extractor.CurrentLOD == -1)
                        {
                            extractor.ModelName = $"model{extractor.ModelIndex}";
                        }
                        else
                        {
                            extractor.ModelName = $"model{extractor.ModelIndex}.lod{extractor.CurrentLOD}";
                        }


                        var callShape = (command as Cmd_pgluCallShapeByte);
                        int shapeIndex = callShape.ShapeIndex;
                        PGLUshapeConverted shapeData = Shapes[shapeIndex].GetShapeData();
                        shapeData.ShapeIndex = shapeIndex;
                        shapeData.RenderCommands = extractor.RenderCommandContext.GetCurrentCommandsForContext();

                        string name = $"shape{shapeIndex}";
                        if (!string.IsNullOrEmpty(extractor.ExtraShapeName))
                            name += $"_{extractor.ExtraShapeName}";

                        if (shapeData.UsesExternalTexture)
                            name += "_reflection";

                        extractor.AddShape(name, shapeData);
                    }
                    break;

                case ModelSetupPS2Opcode.pgluCallShape_UShort:
                    {
                        if (extractor.CurrentLOD == -1)
                        {
                            extractor.ModelName = $"model{extractor.ModelIndex}";
                        }
                        else
                        {
                            extractor.ModelName = $"model{extractor.ModelIndex}.lod{extractor.CurrentLOD}";
                        }

                        var callShape = (command as Cmd_pgluCallShape_UShort);
                        int shapeIndex = callShape.ShapeIndex;
                        PGLUshapeConverted shapeData = Shapes[shapeIndex].GetShapeData();
                        shapeData.ShapeIndex = shapeIndex;
                        shapeData.RenderCommands = extractor.RenderCommandContext.GetCurrentCommandsForContext();

                        string name = $"shape{shapeIndex}";
                        if (!string.IsNullOrEmpty(extractor.ExtraShapeName))
                            name += $"_{extractor.ExtraShapeName}";

                        if (shapeData.UsesExternalTexture)
                            name += "_reflection";

                        extractor.AddShape(name, shapeData);
                    }
                    break;

                case ModelSetupPS2Opcode.pgl_53:
                    {
                        var callShape = (command as Cmd_Unk53);
                        int shapeIndex = callShape.ShapeIndex;
                        PGLUshapeConverted shapeData = Shapes[shapeIndex].GetShapeData();
                        shapeData.ShapeIndex = shapeIndex;
                        shapeData.RenderCommands = extractor.RenderCommandContext.GetCurrentCommandsForContext();

                        string name = $"shape{shapeIndex}";
                        if (!string.IsNullOrEmpty(extractor.ExtraShapeName))
                            name += $"_{extractor.ExtraShapeName}";

                        if (shapeData.UsesExternalTexture)
                            name += "_reflection";

                        extractor.AddShape(name, shapeData);
                    }
                    break;

                default:
                    bool handled = extractor.RenderCommandContext.ApplyCommand(command);
                    if (!handled && command.Opcode != ModelSetupPS2Opcode.End)
                    {
                        Console.WriteLine($"[ProcessCommands] Unhandled opacity/render opcode: {command.Opcode}");
                    }
                    break;
            }
        }
    }
}
