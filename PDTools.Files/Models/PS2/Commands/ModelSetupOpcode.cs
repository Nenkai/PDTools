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

        /// <summary>
        /// Calls a shape (mesh) to use (1 unsigned byte)
        /// </summary>
        pgluCallShape_1ub = 4,

        /// <summary>
        /// Calls a shape (mesh) to use (1 unsigned short)
        /// </summary>
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
        LODSelect = 7,

        /// <summary>
        /// Jumps to a relative offset (byte)
        /// </summary>
        JumpByte = 8,

        /// <summary>
        /// Jumps to a relative offset (short)
        /// </summary>
        Jump1us = 9,

        BBoxRender = 10,

        /// <summary>
        /// Calls pglEnable(17)
        /// </summary>
        pglEnable17_ = 11,

        /// <summary>
        /// Pushes a matrix. similar to glPushMatrix
        /// </summary>
        pglPushMatrix = 12,

        /// <summary>
        /// Pops a matrix. similar to glPopMatrix
        /// </summary>
        pglPopMatrix = 13,

        /// <summary>
        /// Sets the matrix mode. Similar to glMatrixMode
        /// </summary>
        pglMatrixMode = 14,

        /// <summary>
        /// Calls pglInverse with a matrix
        /// </summary>
        pglInverse = 15,

        /// <summary>
        /// Calls a pgl command with a matrix. Could be similar in operation to command 15
        /// </summary>
        pglUnk16 = 16,

        /// <summary>
        /// Calls pglTranslate. Similar to glTranslate
        /// </summary>
        pglTranslate = 17,

        /// <summary>
        /// Calls pglScale. Similar to glScale
        /// </summary>
        pglScale = 18,

        /// <summary>
        /// Calls pglRotate. Similar to glRotate
        /// </summary>
        pglRotate = 19,

        /// <summary>
        /// Calls pglRotateX. Custom, rotates on X axis
        /// </summary>
        pglRotateX = 20,

        /// <summary>
        /// Calls pglRotateX. Custom, rotates on Y axis
        /// </summary>
        pglRotateY = 21,

        /// <summary>
        /// Calls pglRotateX. Custom, rotates on Z axis
        /// </summary>
        pglRotateZ = 22,

        /// <summary>
        /// Enables Depth Test by calling pglEnable(5), which sets GS ZBUF's ZMSK (Z Value Drawing Mask) & GS TEST's ZTE (Depth Test) bits
        /// </summary>
        pglEnableDepthTest = 23,

        /// <summary>
        /// Disables Depth Test by calling pglDisable(5), which unsets GS ZBUF's ZMSK (Z Value Drawing Mask) & GS TEST's ZTE (Depth Test) bits
        /// </summary>
        pglDisableDepthTest = 24,

        /// <summary>
        /// Sets the depth func to use. Sets GS TEST's ZTST value
        /// 0 = NEVER, 1 = ALWAYS, 2 = GEQUAL, 3 = GREATER
        /// </summary>
        pglDepthFunc = 25,

        /// <summary>
        /// Enables Alpha Test by calling pglEnable(4), which sets GS TEST's ATE (Alpha Test) bit
        /// </summary>
        pglEnableAlphaTest = 26,

        /// <summary>
        /// Disables Alpha Test by calling pglDisable(4), which unsets GS TEST's ATE (Alpha Test) bit
        /// </summary>
        pglDisableAlphaTest = 27,

        /// <summary>
        /// Sets the alpha function to use by setting GS TEST's ATST (Alpha Test Method) value
        /// 0 = NEVER, 1 = ALWAYS, 2 = LESS, 3 = LEQUAL, 4 = EQUAL, 5 = GEQUAL, 6 = GREATER, 7 = NOTEQUAL
        /// </summary>
        pglAlphaFunc = 28,

        /// <summary>
        /// Enables Destination Alpha Test by calling pglEnable(3), which toggles on GS TEST register bit 14 (DATE).
        /// </summary>
        pglEnableDestinationAlphaTest = 29,

        /// <summary>
        /// Disables Destination Alpha Test by calling pglDisable(3), which untoggles GS TEST register bit 14 (DATE).
        /// </summary>
        pglDisableDestinationAlphaTest = 30,

        /// <summary>
        /// Calls pglDestinationAlphaFunc(value) - 1 unsigned byte - 2 unsets GS TEST register bit 15 (DATM), 5 sets it
        /// </summary>
        pglSetDestinationAlphaFunc = 31,

        /// <summary>
        /// Sets the blend function to use by setting GS ALPHA_1 & ALPHA_2 (FIX)
        /// </summary>
        pglBlendFunc = 32,

        /// <summary>
        /// Sets the fog color. 1 unsigned int to GS FOGCOL register
        /// </summary>
        pglSetFogColor = 33,

        /// <summary>
        /// Stores the fog color from current FOGCOL register
        /// </summary>
        pglStoreFogColor = 34,

        /// <summary>
        /// Copies/Restores fog color from fog color value
        /// </summary>
        pglCopyFogColor = 35,

        /// <summary>
        /// Sets the color mask with pglColorMask1ui(~value) (1 unsigned int)
        /// </summary>
        pglColorMask = 36,

        /// <summary>
        /// Disables depth mask
        /// </summary>
        pglDisableDepthMask = 37,

        /// <summary>
        /// Enables depth mask
        /// </summary>
        pglEnableDepthMask = 38,

        /// <summary>
        /// Sets the depth bias.
        /// </summary>
        pglDepthBias = 39,

        // Does something with 4 float. Not used above GT3. Other games's code skips 4 floats.
        pglGT3_Unk40 = 40,

        /// <summary>
        /// Calls pgluExternalTexIndex
        /// </summary>
        pglExternalTexIndex = 41,

        /// <summary>
        /// Calls pgluExternalMatIndex
        /// </summary>
        pglExternalMatIndex = 42,

        /// <summary>
        /// Sets the tex table to use (1 unsigned byte)
        /// </summary>
        pgluSetTexTable = 43,

        /// <summary>
        /// Sets the tex table to use (1 unsigned short)
        /// </summary>
        pgluTexTable_1us = 44,

        /// <summary>
        /// Calls pglEnable(17), may be related to alpha fail
        /// </summary>
        pglEnable17 = 45,

        /// <summary>
        /// Calls pglDisable(17), may be related to alpha fail
        /// </summary>
        pglDisable17 = 46,

        /// <summary>
        /// Sets the alpha fail by setting GS TEST's AFAIL value.
        /// 0 = KEEP, 1 = FB_ONLY, 2 = ZB_ONLY, 3 = RGB_ONLY
        /// </summary>
        pglAlphaFail = 47,

        // Calls pglCylinderMapHint - 3 floats
        pglCylinderMapHint = 49,

        // Does something with 1 float. Not used above GT3. Other games's code skips 1 float.
        // Operates similarly to command 51.
        pglGT3_1ui = 50,

        // Does something with 4 floats. Not used above GT3. Other games's code skips 4 floats.
        pglGT3_4f = 51,

        // GT4 and above. Calls ModelSet2::setShapeTweenRatio
        ModelSet_setShapeTweenRatio = 52,

        /// <summary>
        /// Calls pgluShapeTweenRatio() and pgluCallShape()
        /// </summary>
        pgl_53 = 53,

        /// <summary>
        /// Unknown. Used only in GT3, otherwise skips 1 float
        /// </summary>
        pgl_54 = 54,

        /// <summary>
        /// Calls pglGetCullFace, and pglCullFace
        /// </summary>
        pglCullFace = 64,

        /// <summary>
        /// GT4 and above. Unknown. Skips 3 bytes
        /// </summary>
        Unk_65 = 65,

        /// <summary>
        /// GT4 and above. Unknown. Skips 3 bytes
        /// </summary>
        Unk_66 = 66,

        /// <summary>
        /// GT4 and above. Calls model set VM.
        /// </summary>
        ModelSetVM_callVM = 67,

        /// <summary>
        /// GT4 and above.
        /// </summary>
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

        /// <summary>
        /// GT4 and above. Calls pglCullFace(2)
        /// </summary>
        pglCullFace_2 = 78,

        /// <summary>
        /// GT4 and above. Calls pglCullFace(1)
        /// </summary>
        pglCullFace_1 = 79,

        /// <summary>
        /// GT4 and above. Calls pgluTexSet using external tex set (up to 16). Uses 1 unsigned byte as index
        /// </summary>
        pgluTexTableFromExternalTexSet1ub = 80,

        /// <summary>
        /// GT4 and above. Calls pgluTexSet using external tex set (up to 16). Uses 1 unsigned short as index
        /// </summary>
        pgluTexTableFromExternalTexSet1us = 81,

        /// <summary>
        /// GT4 and above. Calls pglVariableColorScale
        /// </summary>
        pglVariableColorScale = 82,

        /// <summary>
        /// GT4 and above. Calls pglVariableColorOffset
        /// </summary>
        pglVariableColorOffset = 83,

        /// <summary>
        /// GT4 and above. Calls pglVariableColorScale using shorts (vm instance related?)
        /// </summary>
        pglVariableColorScale_Shorts = 84,

        /// <summary>
        /// GT4 and above. Calls pglVariableColorOffset using shorts (vm instance related?)
        /// </summary>
        pglVariableColorOffset_Shorts = 85,

        // GT4 and above. Calls pglTexGenf(3, facing_attenuation_) and pglTexGenf(2, facing_bias_)
        // Both of these globals are set through ModelSet2::setFacingAttenuation and ModelSet2::setFacingBias
        // Which are called by course env ptr code, it seems
        pglTexGenf_WithCurrentFacingParameters = 86,

        /// <summary>
        /// GT4 and above. Calls pglTexGenf(3, 0.0) and pglTexGenf(2, 1.0)
        /// </summary>
        pglTexGenf_Default = 87,

        /// <summary>
        /// GT4 and above. Calls pglDisable(19) and pglDisable(14)
        /// </summary>
        pglDisable19_14 = 88,

        /// <summary>
        /// GT4 and above. Calls modelset callback. takes unsigned byte parameter
        /// </summary>
        ModelSetCallback_89_1ub = 89,

        /// <summary>
        /// GT4 and above. Same as 89, with unsigned short index
        /// </summary>
        ModelSetCallback_90_1us = 90,
    }
}
