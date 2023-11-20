using PDTools.Files.Models.VM.Instructions;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS2.Commands
{
    /// <summary>
    /// Base Model Set command.
    /// </summary>
    public abstract class ModelSetupPS2Command
    {
        /* Commands parsing has a bit of heuristics going on to make sure we can group them
         * i.e LODSelect & CallModelCallback */
        public abstract ModelSetupPS2Opcode Opcode { get; }

        public int Offset { get; set; }

        public abstract void Read(BinaryStream bs, int commandsBaseOffset);

        public abstract void Write(BinaryStream bs);

        public static ModelSetupPS2Command GetByOpcode(ModelSetupPS2Opcode opcode)
        {
            ModelSetupPS2Command cmd = opcode switch
            {
                ModelSetupPS2Opcode.pgluCallShape_Byte => new Cmd_pgluCallShapeByte(),
                ModelSetupPS2Opcode.CallModelCallback => new Cmd_CallModelCallback(),
                ModelSetupPS2Opcode.LODSelect => new Cmd_LODSelect(),
                ModelSetupPS2Opcode.Jump_Byte => new Cmd_JumpByte(),
                ModelSetupPS2Opcode.Jump_UShort => new Cmd_JumpUShort(),
                ModelSetupPS2Opcode.BBoxRender => new Cmd_BBoxRender(),
                ModelSetupPS2Opcode.pglEnableRendering => new Cmd_pglEnableRendering(),
                ModelSetupPS2Opcode.pglPushMatrix => new Cmd_pglPushMatrix(),
                ModelSetupPS2Opcode.pglPopMatrix => new Cmd_pglPopMatrix(),
                ModelSetupPS2Opcode.pglMatrixMode => new Cmd_pglMatrixMode(),
                ModelSetupPS2Opcode.pglLoadMatrix => new Cmd_pglLoadMatrix(),
                ModelSetupPS2Opcode.pglMultMatrix => new Cmd_pglMultMatrix(),
                ModelSetupPS2Opcode.pglTranslate => new Cmd_pglTranslate(),
                ModelSetupPS2Opcode.pglScale => new Cmd_pglScale(),
                ModelSetupPS2Opcode.pglRotate => new Cmd_pglRotate(),
                ModelSetupPS2Opcode.pglRotateX => new Cmd_pglRotateX(),
                ModelSetupPS2Opcode.pglRotateY => new Cmd_pglRotateY(),
                ModelSetupPS2Opcode.pglRotateZ => new Cmd_pglRotateZ(),
                ModelSetupPS2Opcode.pglEnableDepthTest => new Cmd_pglEnableDepthTest(),
                ModelSetupPS2Opcode.pglDisableDepthTest => new Cmd_pglDisableDepthTest(),
                ModelSetupPS2Opcode.pglDepthFunc => new Cmd_pglDepthFunc(),
                ModelSetupPS2Opcode.pglEnableAlphaTest => new Cmd_pglEnableAlphaTest(),
                ModelSetupPS2Opcode.pglDisableAlphaTest => new Cmd_pglDisableAlphaTest(),
                ModelSetupPS2Opcode.pglAlphaFunc => new Cmd_pglAlphaFunc(),
                ModelSetupPS2Opcode.pglEnableDestinationAlphaTest => new Cmd_pglEnableDestinationAlphaTest(),
                ModelSetupPS2Opcode.pglDisableDestinationAlphaTest => new Cmd_pglDisableDestinationAlphaTest(),
                ModelSetupPS2Opcode.pglSetDestinationAlphaFunc => new Cmd_pglSetDestinationAlphaFunc(),
                ModelSetupPS2Opcode.pglBlendFunc => new Cmd_pglBlendFunc(),
                ModelSetupPS2Opcode.pglSetFogColor => new Cmd_pglSetFogColor(),
                ModelSetupPS2Opcode.pglStoreFogColor => new Cmd_pglStoreFogColor(),
                ModelSetupPS2Opcode.pglCopyFogColor => new Cmd_pglCopyFogColor(),
                ModelSetupPS2Opcode.pglColorMask => new Cmd_pglColorMask(),
                ModelSetupPS2Opcode.pglDisableDepthMask => new Cmd_pglDisableDepthMask(),
                ModelSetupPS2Opcode.pglEnableDepthMask => new Cmd_pglEnableDepthMask(),
                ModelSetupPS2Opcode.pglDepthBias => new Cmd_pglDepthBias(),
                ModelSetupPS2Opcode.pglExternalTexIndex => new Cmd_pgluSetExternalTexIndex(),
                ModelSetupPS2Opcode.pglExternalMatIndex => new Cmd_pgluSetExternalMatIndex(),
                ModelSetupPS2Opcode.pgluSetTexTable_Byte => new Cmd_pgluSetTexTable_Byte(),
                ModelSetupPS2Opcode.pgluSetTexTable_UShort => new Cmd_pgluSetTexTable_UShort(),
                ModelSetupPS2Opcode.pglEnableCullFace => new Cmd_pglEnableCullFace(),
                ModelSetupPS2Opcode.pglDisableCullFace => new Cmd_pglDisableCullFace(),
                ModelSetupPS2Opcode.pglAlphaFail => new Cmd_pglAlphaFail(),
                ModelSetupPS2Opcode.pglGT3_1ui => new Cmd_GT3_1ui(),
                ModelSetupPS2Opcode.pglGT3_4f => new Cmd_GT3_4f(),
                ModelSetupPS2Opcode.pgl_53 => new Cmd_Unk53(),
                ModelSetupPS2Opcode.CallVM => new Cmd_CallVM(),
                ModelSetupPS2Opcode.VM_pglRotate => new Cmd_VM_pglRotate(),
                ModelSetupPS2Opcode.pglDisable19_14 => new Cmd_pglEnable19_14(),
                ModelSetupPS2Opcode.pgluTexTableFromExternalTexSetByte => new Cmd_pgluTexTableFromExternalTexSetByte(),
                ModelSetupPS2Opcode.VMCallback_Byte => new Cmd_VMCallbackByte(),
                ModelSetupPS2Opcode.VMCallback_UShort => new Cmd_VMCallbackByte(),

                _ => throw new Exception($"Unexpected opcode {opcode}")
            };

            return cmd;
        }
    }
}
