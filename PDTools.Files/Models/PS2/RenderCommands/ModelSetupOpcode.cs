using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS2.RenderCommands
{
    public enum ModelSetupPS2Opcode : byte
    {
        // End of setup commands.
        End = 0,

        RenderModel_Byte = 2,
        RenderModel_UShort = 3,

        /// <summary>
        /// Calls a shape (mesh) to use (1 unsigned byte)
        /// </summary>
        pgluCallShape_Byte = 4,

        /// <summary>
        /// Calls a shape (mesh) to use (1 unsigned short)
        /// </summary>
        pgluCallShape_UShort = 5,

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
        Jump_Byte = 8,

        /// <summary>
        /// Jumps to a relative offset (short)
        /// </summary>
        Jump_UShort = 9,

        /// <summary>
        /// Render commands, takes a bbox.
        /// </summary>
        BBoxRender = 10,

        /// <summary>
        /// Calls pglEnable(17) - Not entirely figured out, it may be just enabling rendering
        /// It's always at the bottom of commands, so probably it?
        /// </summary>
        pglEnableRendering = 11,

        /// <summary>
        /// Pushes the current matrix stack down by one, duplicating the current matrix. Similar to glPushMatrix
        /// </summary>
        pglPushMatrix = 12,

        /// <summary>
        /// Pops the current matrix stack, replacing the current matrix with the one below it on the stack. Similar to glPopMatrix
        /// </summary>
        pglPopMatrix = 13,

        /// <summary>
        /// Sets the matrix mode. Similar to glMatrixMode
        /// </summary>
        pglMatrixMode = 14,

        /// <summary>
        /// Calls pglLoadMatrix, same as glLoadMatrix. Replaces the current matrix with the one whose elements are specified by m. 
        /// </summary>
        pglLoadMatrix = 15,

        /// <summary>
        /// Calls pglMultMatrix, same as glMultMatrix. Multiplies the current matrix with the one specified using m, and replaces the current matrix with the product.
        /// </summary>
        pglMultMatrix = 16,

        /// <summary>
        /// Calls pglTranslate, same as glTranslate. Multiplies the current matrix by the specified translation vector
        /// </summary>
        pglTranslate = 17,

        /// <summary>
        /// Calls pglScale, same as glScale. Multiplies the current matrix by the specified scale vector.
        /// </summary>
        pglScale = 18,

        /// <summary>
        /// Calls pglRotate, same as glRotate. Multiplies the current matrix by the specified rotation vector.
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
        /// Sets the current external texture to use. Calls pgluExternalTexIndex
        /// </summary>
        pglExternalTexIndex = 41,

        /// <summary>
        /// Calls pgluExternalMatIndex
        /// </summary>
        pglExternalMatIndex = 42,

        /// <summary>
        /// Sets the tex table to use (1 unsigned byte)
        /// </summary>
        pgluSetTexTable_Byte = 43,

        /// <summary>
        /// Sets the tex table to use (1 unsigned short)
        /// </summary>
        pgluSetTexTable_UShort = 44,

        /// <summary>
        /// Calls glEnable(15). Enables face culling
        /// </summary>
        pglEnableCullFace = 45,

        /// <summary>
        /// Calls pglDisable(15). Disables face culling
        /// </summary>
        pglDisableCullFace = 46,

        /// <summary>
        /// Calls pglAlphaFail. Sets the alpha fail by setting GS TEST's AFAIL value.
        /// </summary>
        pglAlphaFail = 47,

        // 48 not present at all in GT3 or 4

        // Calls pglCylinderMapHint - 3 floats
        pglCylinderMapHint = 49,

        // Does something with 4 floats. Not used above GT3. Other games's code skips 4 floats.
        // Refer to 0x2505E8 with param 3 (GT3 EU)
        pglGT3_2_1ui = 50,

        // Does something with 1 float. Not used above GT3. Other games's code skips 1 float.
        // Operates similarly to command 51.
        // Refer to 0x250688 with param 3 (GT3 EU)
        pglGT3_2_4f = 51,

        // GT4 and above. Calls ModelSet2::setShapeTweenRatio
        ModelSet_setShapeTweenRatio = 52,

        /// <summary>
        /// Calls pgluShapeTweenRatio() and pgluCallShape()
        /// </summary>
        pgl_53 = 53,

        // Does something with 1 float. Not used above GT3. Other games's code skips 1 float.
        // Operates similarly to command 51.
        // Refer to 0x250688 with param 2 (GT3 EU)
        pglGT3_3_1ui = 54,

        // Does something with 1 float. Not used above GT3. Other games's code skips 1 float.
        // Operates similarly to command 54.
        // Refer to 0x2505E8 with param 2 (GT3 EU)
        pglGT3_3_4f = 55,

        /// <summary>
        /// GT4 and above. Calls pglGetCullFace, and pglCullFace
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
        CallVM = 67,

        /// <summary>
        /// GT4 and above. Calls pglMultMatrix using VM output registers.
        /// </summary>
        VM_MultMatrix = 68,

        /// <summary>
        /// GT4 and above. Calls pglTranslate using VM output registers.
        /// </summary>
        VM_pglTranslate = 69,

        /// <summary>
        /// GT4 and above. Calls pglScale using VM output registers.
        /// </summary>
        VM_pglScale = 70,

        /// <summary>
        /// GT4 and above. Calls pglRotate using VM output registers.
        /// </summary>
        VM_pglRotate = 72,

        /// <summary>
        /// GT4 and above. Calls pglRotateX using VM output registers.
        /// </summary>
        VM_pglRotateX = 73,

        /// <summary>
        /// GT4 and above. Calls pglRotateY using VM output registers.
        /// </summary>
        VM_pglRotateY = 74,

        /// <summary>
        /// GT4 and above. Calls pglRotateZ using VM output registers.
        /// </summary>
        VM_pglRotateZ = 75,

        /// <summary>
        /// GT4 and above. Calls pgluShapeTweenRatio using VM output registers.
        /// </summary>
        VM_pgluShapeTweenRatio = 76,

        /// <summary>
        /// GT4 and above. Branches depending on parameter.
        /// </summary>
        VM_Branch = 77,

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
        pgluTexTableFromExternalTexSetByte = 80,

        /// <summary>
        /// GT4 and above. Calls pgluTexSet using external tex set (up to 16). Uses 1 unsigned short as index
        /// </summary>
        pgluTexTableFromExternalTexSetUShort = 81,

        /// <summary>
        /// GT4 and above. Calls pglVariableColorScale
        /// </summary>
        pglVariableColorScale = 82,

        /// <summary>
        /// GT4 and above. Calls pglVariableColorOffset
        /// </summary>
        pglVariableColorOffset = 83,

        /// <summary>
        /// GT4 and above. Calls pglVariableColorScale using VM output registers.
        /// </summary>
        VM_pglVariableColorScale = 84,

        /// <summary>
        /// GT4 and above. Calls pglVariableColorOffset using VM output registers.
        /// </summary>
        VM_pglVariableColorOffset = 85,

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
        VMCallback_Byte = 89,

        /// <summary>
        /// GT4 and above. Same as 89, with unsigned short index
        /// </summary>
        VMCallback_UShort = 90,
    }
}
