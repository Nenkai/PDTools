using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Files.Models.PS2.Commands;

namespace GTPS2ModelTool.Core;

public class RenderCommandContext
{
    // These are all the defaults from ModelSet2::begin (GT4O US: 0x2F7060)
    // Called before render_

    // Alpha Test
    public const bool DEFAULT_ALPHA_TEST = true;
    public bool AlphaTest { get; set; } = DEFAULT_ALPHA_TEST;

    // Alpha Fail
    public const AlphaFailMethod DEFAULT_ALPHA_FAIL = AlphaFailMethod.KEEP;
    public AlphaFailMethod AlphaFail { get; set; } = DEFAULT_ALPHA_FAIL;

    // Destination Alpha Test
    public const bool DEFAULT_DESTINATION_ALPHA_TEST = false;
    public bool DestinationAlphaTest { get; set; } = DEFAULT_DESTINATION_ALPHA_TEST;

    // Destination Alpha Test Func
    public const DestinationAlphaFunction DEFAULT_DESTINATION_ALPHA_FUNC = DestinationAlphaFunction.EQUAL_ONE;
    public DestinationAlphaFunction DestinationAlphaFunc { get; set; } = DEFAULT_DESTINATION_ALPHA_FUNC;

    // Cull Mode
    public const bool DEFAULT_CULL_MODE = true;
    public bool CullMode { get; set; } = DEFAULT_CULL_MODE;

    // Blend func
    public const byte DEFAULT_BLENDFUNC_A = 0;
    public const byte DEFAULT_BLENDFUNC_B = 1;
    public const byte DEFAULT_BLENDFUNC_C = 0;
    public const byte DEFAULT_BLENDFUNC_D = 1;
    public const byte DEFAULT_BLENDFUNC_FIX = 1;
    public byte BlendFunc_A { get; set; } = DEFAULT_BLENDFUNC_A;
    public byte BlendFunc_B { get; set; } = DEFAULT_BLENDFUNC_B;
    public byte BlendFunc_C { get; set; } = DEFAULT_BLENDFUNC_C;
    public byte BlendFunc_D { get; set; } = DEFAULT_BLENDFUNC_D;
    public byte BlendFunc_FIX { get; set; } = DEFAULT_BLENDFUNC_FIX;

    // Depth Bias
    public const float DEFAULT_DEPTH_BIAS = 0.0f;
    public float DepthBias { get; set; } = DEFAULT_DEPTH_BIAS;

    // Depth Mask
    public const bool DEFAULT_DEPTH_MASK = true;
    public bool DepthMask { get; set; } = DEFAULT_DEPTH_MASK;

    // Depth Mask
    public const uint DEFAULT_COLOR_MASK = unchecked(0xFFFFFFFF);
    public uint ColorMask { get; set; } = DEFAULT_COLOR_MASK;

    // Alpha Test
    public const AlphaTestFunc DEFAULT_ALPHA_TEST_FUNC = AlphaTestFunc.GREATER;
    public const byte DEFAULT_ALPHA_TEST_REF = 0x20;
    public AlphaTestFunc AlphaTestFunc { get; set; } = DEFAULT_ALPHA_TEST_FUNC;
    public byte AlphaTestRef { get; set; } = DEFAULT_ALPHA_TEST_REF;

    // GT3
    public const float DEFAULT_GT3_2_R = 0;
    public const float DEFAULT_GT3_2_G = 0;
    public const float DEFAULT_GT3_2_B = 0;
    public const float DEFAULT_GT3_2_A = 0;

    public float UnkGT3_2_R { get; set; } = DEFAULT_GT3_2_R;
    public float UnkGT3_2_G { get; set; } = DEFAULT_GT3_2_G;
    public float UnkGT3_2_B { get; set; } = DEFAULT_GT3_2_B;
    public float UnkGT3_2_A { get; set; } = DEFAULT_GT3_2_A;

    public uint? FogColor { get; set; }
    public bool FogColorWasSet { get; set; }

    public const byte DEFAULT_EXTERNAL_TEX_INDEX = 0;
    public byte ExternalTexIndex { get; set; } = DEFAULT_EXTERNAL_TEX_INDEX;

    public bool IsDefaultAlphaTest()
    {
        return AlphaTest == DEFAULT_ALPHA_TEST;
    }

    public bool IsDefaultAlphaFail()
    {
        return AlphaFail == DEFAULT_ALPHA_FAIL;
    }

    public bool IsDefaultDestinationAlphaTest()
    {
        return DestinationAlphaTest == DEFAULT_DESTINATION_ALPHA_TEST;
    }

    public bool IsDefaultDestinationAlphaFunc()
    {
        return DestinationAlphaFunc == DEFAULT_DESTINATION_ALPHA_FUNC;
    }

    public bool IsDefaultCullMode()
    {
        return CullMode == DEFAULT_CULL_MODE;
    }

    public bool IsDefaultAlphaTestFunc()
    {
        return AlphaTestFunc == DEFAULT_ALPHA_TEST_FUNC &&
            AlphaTestRef == DEFAULT_ALPHA_TEST_REF;
    }

    public bool IsDefaultDepthBias()
    {
        return DepthBias == DEFAULT_DEPTH_BIAS;
    }

    public bool IsDefaultDepthMask()
    {
        return DepthMask == DEFAULT_DEPTH_MASK;
    }

    public bool IsDefaultColorMask()
    {
        return ColorMask == DEFAULT_COLOR_MASK;
    }

    public bool IsDefaultBlendFunc()
    {
        return BlendFunc_A == DEFAULT_BLENDFUNC_A &&
            BlendFunc_B == DEFAULT_BLENDFUNC_B &&
            BlendFunc_C == DEFAULT_BLENDFUNC_C &&
            BlendFunc_D == DEFAULT_BLENDFUNC_D &&
            BlendFunc_FIX == DEFAULT_BLENDFUNC_FIX;
    }

    public bool IsDefaultFogColor()
    {
        return FogColor is null;
    }

    public bool IsDefaultGT3_2()
    {
        return UnkGT3_2_R == DEFAULT_GT3_2_R &&
            UnkGT3_2_G == UnkGT3_2_G &&
            UnkGT3_2_B == UnkGT3_2_B &&
            UnkGT3_2_A == UnkGT3_2_A;
    }

    public bool IsDefaultExternalTexSetIndex()
        => ExternalTexIndex == DEFAULT_EXTERNAL_TEX_INDEX;

    public List<ModelSetupPS2Command> GetCurrentCommandsForContext()
    {
        var cmds = new List<ModelSetupPS2Command>();

        if (!IsDefaultExternalTexSetIndex())
            cmds.Add(new Cmd_pgluSetExternalTexIndex(ExternalTexIndex));

        if (!IsDefaultAlphaFail())
            cmds.Add(new Cmd_pglAlphaFail(AlphaFail));

        if (!IsDefaultAlphaTestFunc())
            cmds.Add(new Cmd_pglAlphaFunc(this.AlphaTestFunc, this.AlphaTestRef));

        if (!IsDefaultAlphaTest())
            cmds.Add(new Cmd_pglDisableAlphaTest());

        if (!IsDefaultDestinationAlphaTest())
            cmds.Add(new Cmd_pglEnableDestinationAlphaTest());

        if (!IsDefaultDestinationAlphaFunc())
            cmds.Add(new Cmd_pglSetDestinationAlphaFunc(DestinationAlphaFunc));

        if (!IsDefaultBlendFunc())
            cmds.Add(new Cmd_pglBlendFunc(BlendFunc_A, BlendFunc_B, BlendFunc_C, BlendFunc_D, BlendFunc_FIX));

        if (!IsDefaultDepthMask())
            cmds.Add(new Cmd_pglDisableDepthMask());

        if (!IsDefaultColorMask())
            cmds.Add(new Cmd_pglColorMask(ColorMask));

        if (!IsDefaultDepthBias())
            cmds.Add(new Cmd_pglDepthBias(DepthBias));

        if (!IsDefaultFogColor())
            cmds.Add(new Cmd_pglSetFogColor((uint)FogColor));

        if (!IsDefaultGT3_2())
            cmds.Add(new Cmd_GT3_2_4f(UnkGT3_2_R, UnkGT3_2_G, UnkGT3_2_B, UnkGT3_2_A));


        return cmds;
    }

    public bool ApplyCommand(ModelSetupPS2Command cmd)
    {
        bool diff = false;
        switch (cmd.Opcode)
        {
            case ModelSetupPS2Opcode.pglStoreFogColor:
            case ModelSetupPS2Opcode.pglEnableRendering:
                // don't care
                break;

            case ModelSetupPS2Opcode.pglAlphaFunc:
                {
                    var alphaFunc = cmd as Cmd_pglAlphaFunc;
                    if (alphaFunc.TST != AlphaTestFunc ||
                        alphaFunc.REF != AlphaTestRef)
                        diff = true;

                    AlphaTestFunc = alphaFunc.TST;
                    AlphaTestRef = alphaFunc.REF;
                    break;
                }

            case ModelSetupPS2Opcode.pglAlphaFail:
                {
                    var alphaFail = cmd as Cmd_pglAlphaFail;
                    if (alphaFail.FailMethod != AlphaFail)
                        diff = true;

                    alphaFail.FailMethod = AlphaFail;
                    break;
                }

            case ModelSetupPS2Opcode.pglDisableAlphaTest:
                if (AlphaTest)
                    diff = true;

                AlphaTest = false;
                break;
            case ModelSetupPS2Opcode.pglEnableDestinationAlphaTest:
                if (!DestinationAlphaTest)
                    diff = true;

                DestinationAlphaTest = true;
                break;
            case ModelSetupPS2Opcode.pglSetDestinationAlphaFunc:
                {
                    var dest = cmd as Cmd_pglSetDestinationAlphaFunc;
                    if (dest.Func != DestinationAlphaFunc)
                        diff = true;
                    
                    DestinationAlphaFunc = dest.Func;
                    break;
                }

            case ModelSetupPS2Opcode.pglBlendFunc:
                {
                    var blend = cmd as Cmd_pglBlendFunc;
                    if (blend.A != BlendFunc_A ||
                        blend.B != BlendFunc_B ||
                        blend.C != BlendFunc_C ||
                        blend.D != BlendFunc_D ||
                        blend.FIX != BlendFunc_FIX)
                        diff = true;

                    BlendFunc_A = blend.A;
                    BlendFunc_B = blend.B;
                    BlendFunc_C = blend.C;
                    BlendFunc_D = blend.D;
                    BlendFunc_FIX = blend.FIX;
                    break;
                }

            case ModelSetupPS2Opcode.pglDisableDepthMask:
                if (DepthMask)
                    diff = true;

                DepthMask = false;
                break;
            case ModelSetupPS2Opcode.pglDepthBias:
                {
                    var bias = cmd as Cmd_pglDepthBias;
                    if (bias.Value != 0.0f)
                        diff = true;

                    DepthBias = bias.Value;
                    break;
                }

            case ModelSetupPS2Opcode.pglColorMask:
                {
                    var colorMask = cmd as Cmd_pglColorMask;
                    if ((uint)(int)~colorMask.ColorMask != ColorMask)
                        diff = true;

                    ColorMask = (uint)(int)~colorMask.ColorMask;
                    break;
                }

            case ModelSetupPS2Opcode.pglSetFogColor:
                {
                    var fogCol = cmd as Cmd_pglSetFogColor;
                    if (fogCol.Color != FogColor)
                        diff = true;

                    FogColorWasSet = true;
                    FogColor = fogCol.Color;
                    break;
                }

            case ModelSetupPS2Opcode.pglCopyFogColor:
                FogColor = null;
                return true;
            case ModelSetupPS2Opcode.pglGT3_2_1ui:
            case ModelSetupPS2Opcode.pglGT3_2_4f:
                {
                    if (cmd.Opcode == ModelSetupPS2Opcode.pglGT3_2_4f)
                    {
                        var gt3_2_4f = cmd as Cmd_GT3_2_4f;
                        if (gt3_2_4f.R != UnkGT3_2_R ||
                            gt3_2_4f.G != UnkGT3_2_G ||
                            gt3_2_4f.B != UnkGT3_2_B ||
                            gt3_2_4f.A != UnkGT3_2_A)
                            diff = true;

                        UnkGT3_2_R = gt3_2_4f.R;
                        UnkGT3_2_G = gt3_2_4f.G;
                        UnkGT3_2_B = gt3_2_4f.B;
                        UnkGT3_2_A = gt3_2_4f.A;
                    }
                    else if (cmd.Opcode == ModelSetupPS2Opcode.pglGT3_2_1ui)
                    {
                        var gt3_2_4f = cmd as Cmd_GT3_2_1ui;
                        if (gt3_2_4f.Color != UnkGT3_2_R ||
                            gt3_2_4f.Color != UnkGT3_2_G ||
                            gt3_2_4f.Color != UnkGT3_2_B ||
                            gt3_2_4f.Color != UnkGT3_2_A)
                            diff = true;

                        UnkGT3_2_R = gt3_2_4f.Color;
                        UnkGT3_2_G = gt3_2_4f.Color;
                        UnkGT3_2_B = gt3_2_4f.Color;
                        UnkGT3_2_A = 0.0f; // As per original code
                    }

                    break;
                }

            case ModelSetupPS2Opcode.pglEnableAlphaTest:
                if (!AlphaTest)
                    diff = true;
                AlphaTest = true;
                break;

            case ModelSetupPS2Opcode.pglDisableDestinationAlphaTest:
                if (DestinationAlphaTest)
                    diff = true;

                DestinationAlphaTest = false;
                break;
            case ModelSetupPS2Opcode.pglEnableDepthMask:
                if (!DepthMask)
                    diff = true;
                
                DepthMask = true;
                break;

            case ModelSetupPS2Opcode.pglExternalTexIndex:
                {
                    var cmd_ = cmd as Cmd_pgluSetExternalTexIndex;
                    if (cmd_.TexIndex != ExternalTexIndex)
                        diff = true;

                    ExternalTexIndex = cmd_.TexIndex;
                    break;
                }
            default:
                break;
                throw new NotImplementedException();
        }

        return diff;
    }
}
