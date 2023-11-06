using PDTools.Files.Models.VM.Instructions;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS2.Commands
{
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
                ModelSetupPS2Opcode.pgluCallShape_1ub => new Command_4_pgluCallShape1ub(),
                ModelSetupPS2Opcode.CallModelCallback => new Command_6_CallModelCallback(),
                ModelSetupPS2Opcode.LODSelect => new Command_07_LODSelect(),
                ModelSetupPS2Opcode.Jump1us => new Command_09_Jump1us(),
                ModelSetupPS2Opcode.BBoxRender => new Command_10_BBoxRender(),
                ModelSetupPS2Opcode.pglEnable17_ => new Command_11_pglEnable17(),
                ModelSetupPS2Opcode.pglEnableAlphaTest => new Command_26_pglEnableAlphaTest(),
                ModelSetupPS2Opcode.pglDisableAlphaTest => new Command_27_pglDisableAlphaTest(),
                ModelSetupPS2Opcode.pglAlphaFunc => new Command_28_pglAlphaFunc(),
                ModelSetupPS2Opcode.pglEnableDestinationAlphaTest => new Command_29_pglEnableDestinationAlphaTest(),
                ModelSetupPS2Opcode.pglDisableDestinationAlphaTest => new Command_30_pglDisableDestinationAlphaTest(),
                ModelSetupPS2Opcode.pglSetDestinationAlphaFunc => new Command_31_pglSetDestinationAlphaFunc(),
                ModelSetupPS2Opcode.pglBlendFunc => new Command_32_pglBlendFunc(),
                ModelSetupPS2Opcode.pglSetFogColor => new Command_33_pglSetFogColor(),
                ModelSetupPS2Opcode.pglStoreFogColor => new Command_34_pglStoreFogColor(),
                ModelSetupPS2Opcode.pglCopyFogColor => new Command_35_pglCopyFogColor(),
                ModelSetupPS2Opcode.pglColorMask => new Command_36_pglColorMask(),
                ModelSetupPS2Opcode.pglDisableDepthMask => new Command_37_pglDisableDepthMask(),
                ModelSetupPS2Opcode.pglEnableDepthMask => new Command_38_pglEnableDepthMask(),
                ModelSetupPS2Opcode.pgluSetTexTable => new Command_43_pgluSetTexTable(),
                ModelSetupPS2Opcode.pglGT3_1ui => new Command_50_GT3_1ui(),
                ModelSetupPS2Opcode.pglGT3_4f => new Command_51_GT3_4f(),
                _ => throw new Exception($"Unexpected opcode {opcode}")
            };;

            return cmd;
        }
    }
}
