using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS2.Commands
{
    public enum ModelSetupPS2Opcode : byte
    {
        // End of setup commands.
        End = 0,

        RenderModel_1ub = 2,
        RenderModel_1us = 3,

        // Calls a shape (mesh) to use (1 unsigned byte)
        pgluCallShape_1ub = 4,

        // Calls a shape (mesh) to use (1 unsigned short)
        pgluCallShape_1us = 5,

        // Calls the model's callback, passes parameter to it, returns value
        // Used for stuff like brake lights in Car models!
        // Will be used for which branch to jump to
        // Callbacks depends on the kind of model:
        // - GT4Model::CarModel::Callback::callback(int) at 2F3AC0 (GT4O US),
        // CarModel::Callback::callback 21FD10 (GT3 EU)
        CallModelCallback = 6,

        /// <summary>
        /// Jumps to lod command data based on the current lod.
        /// </summary>
        LODSwitchTable = 7,

        // Jumps to a relative offset (byte)
        JumpByte = 8,

        // Jumps to a relative offset (short)
        JumpShort = 9,

        BBoxRender = 10,

        // Calls pglEnable(17)
        pglEnable17_ = 11,

        // Pushes a matrix. similar to glPushMatrix
        pglPushMatrix = 12,

        // Pops a matrix. similar to glPopMatrix
        pglPopMatrix = 13,

        // Sets the matrix mode. Similar to glMatrixMode
        pglMatrixMode = 14,

        // Calls pglInverse with a matrix
        pglInverse = 15,

        // Calls a pgl command with a matrix. Could be similar in operation to command 15
        pglUnk16 = 16,

        // Calls pglTranslate. Similar to glTranslate
        pglTranslate = 17,

        // Calls pglScale. Similar to glScale
        pglScale = 18,

        // Calls pglRotate. Similar to glRotate
        pglRotate = 19,

        // Calls pglRotateX. Custom, rotates on X axis
        pglRotateX = 20,

        // Calls pglRotateX. Custom, rotates on Y axis
        pglRotateY = 21,

        // Calls pglRotateX. Custom, rotates on Z axis
        pglRotateZ = 22,

        // Presumably enables depth test, pglEnable(5) is called.
        pglEnableDepthTest = 23,

        // Presumably disables depth test, pglDisable(5) is called.
        pglDisableDepthTest = 24,

        // Sets the depth func to use. Similar to glDepthFunc
        pglDepthFunc = 25,

        // Presumably enables alpha test. Calls pglEnable(4)
        pglEnableAlphaTest = 26,

        // Presumably disables alpha test. Calls pglDisable(4)
        pglDisableAlphaTest = 27,

        // Sets the alpha function to use (1 unsigned byte) - similar to glAlphaFunc
        pglAlphaFunc_1ub = 28,

        // Presumably enables destination alpha testing. Calls pglEnable(3)
        pglEnableDestinationAlphaTest = 29,

        // Presumably enables destination alpha testing. Calls pglDisable(3)
        pglDisableDestinationAlphaTest = 30,

        // Calls pglDestinationAlphaFunc(value) - 1 unsigned byte - should be 2 or 5
        pglDestinationAlphaFunc = 31,

        // Sets the blend function to use - similar to glBlendFunc
        pglBlendFunc1ub = 32,

        // Sets the fog color. 1 unsigned int
        pglFogColor1ui = 33,

        // Gets the fog color, to be used with command 35 later
        pglGetFogColor = 34,

        // Sets fog color to default value
        pglFogColor1ui_default = 35,

        // Sets the color mask with pglColorMask1ui(~value) (1 unsigned int)
        pglColorMask1ui = 36,

        // Disables depth mask
        pglDisableDepthMask = 37,

        // Enables depth mask
        pglEnableDepthMask = 38,

        // Sets the depth bias.
        pglDepthBias = 39,

        // Does something with 4 float. Not used above GT3. Other games's code skips 4 floats.
        pglGT3_Unk40 = 40,

        // Calls pgluExternalTexIndex
        pglExternalTexIndex = 41,

        // Calls pgluExternalMatIndex
        pglExternalMatIndex = 42,

        // Sets the tex table to use (1 unsigned byte)
        // Calls: 
        // - pgluCacheTexSetPath3(PGLUTexture*)
        // - pgluTexTable(v55->PGLUTextureMapOffset_0x18)
        pgluTexTable_1ub = 43,

        // Same as command 43, but index is a short.
        pgluTexTable_1us = 44,

        // Calls pglEnable(17), may be related to alpha fail
        pglEnable17 = 45,

        // Calls pglDisable(17), may be related to alpha fail
        pglDisable17 = 46,

        pglAlphaFail = 47,

        // Calls pglCylinderMapHint - 3 floats
        pglCylinderMapHint = 49,

        // Does something with 1 float. Not used above GT3. Other games's code skips 1 float.
        // Operates similarly to command 51.
        pglGT3_1Int = 50,

        // Does something with 4 floats. Not used above GT3. Other games's code skips 4 floats.
        pglGT3_4Float = 51,

        // GT4 and above. Calls ModelSet2::setShapeTweenRatio
        ModelSet_setShapeTweenRatio = 52,

        // Calls pgluShapeTweenRatio() and pgluCallShape()
        pgl_53 = 53,

        // Unknown. Used only in GT3, otherwise skips 1 float
        pgl_54 = 54,

        // Calls pglGetCullFace, and pglCullFace
        pglCullFace = 64,

        // GT4 and above. Unknown. Skips 3 bytes
        Unk_65 = 65,

        // GT4 and above. Unknown. Skips 3 bytes
        Unk_66 = 66,

        // GT4 and above. Calls model set VM.
        ModelSetVM_callVM = 67,

        // GT4 and above.
        Unk_68 = 68,

        // GT4 and above. Calls these with a short (rescale?) - VM Instance related?
        pglTranslate2 = 69,
        pglScale2 = 70,
        pglRotate2 = 72,
        pglRotateX2 = 73,
        pglRotateY2 = 74,
        pglRotateZ2 = 75,
        pgluShapeTweenRatio2 = 76,

        UnkBranch = 77,

        // GT4 and above. Calls pglCullFace(2)
        pglCullFace_2 = 78,

        // GT4 and above. Calls pglCullFace(1)
        pglCullFace_1 = 79,

        // GT4 and above. Calls pgluTexSet using external tex set (up to 16). Uses 1 unsigned byte as index
        pgluTexTableFromExternalTexSet1ub = 80,

        // GT4 and above. Calls pgluTexSet using external tex set (up to 16). Uses 1 unsigned short as index
        pgluTexTableFromExternalTexSet1us = 81,

        // GT4 and above. Calls pglVariableColorScale
        pglVariableColorScale = 82,

        // GT4 and above. Calls pglVariableColorOffset
        pglVariableColorOffset = 83,

        // GT4 and above. Calls pglVariableColorScale using shorts (vm instance related?)
        pglVariableColorScale_Shorts = 84,

        // GT4 and above. Calls pglVariableColorOffset using shorts (vm instance related?)
        pglVariableColorOffset_Shorts = 85,

        // GT4 and above. Calls pglTexGenf(3, facing_attenuation_) and pglTexGenf(2, facing_bias_)
        // Both of these globals are set through ModelSet2::setFacingAttenuation and ModelSet2::setFacingBias
        // Which are called by course env ptr code, it seems
        pglTexGenf_WithCurrentFacingParameters = 86,

        // GT4 and above. Calls pglTexGenf(3, 0.0) and pglTexGenf(2, 1.0)
        pglTexGenf_Default = 87,

        // GT4 and above. Calls pglDisable(19) and pglDisable(14)
        pglDisable19_14 = 88,

        // GT4 and above. Calls modelset callback. takes unsigned byte parameter
        ModelSetCallback_89_1ub = 89,

        // GT4 and above. Same as 89, with unsigned short index
        ModelSetCallback_90_1us = 90,
    }
}
