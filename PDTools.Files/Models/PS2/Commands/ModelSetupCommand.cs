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
                ModelSetupPS2Opcode.pgluCallShape_1ub => new Cmd_pgluCallShape1ub(),
                ModelSetupPS2Opcode.CallModelCallback => new Cmd_CallModelCallback(),
                ModelSetupPS2Opcode.LODSelect => new Cmd_LODSelect(),
                ModelSetupPS2Opcode.JumpShort => new Cmd_Jump1us(),
                ModelSetupPS2Opcode.BBoxRender => new Cmd_BBoxRender(),
                ModelSetupPS2Opcode.pglEnable17_ => new Cmd_pglEnable17(),
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
                ModelSetupPS2Opcode.pglExternalTexIndex => new Cmd_pgluSetExternalTexIndex(),
                ModelSetupPS2Opcode.pgluSetTexTable => new Cmd_pgluSetTexTable(),
                ModelSetupPS2Opcode.pglGT3_1ui => new Cmd_GT3_1ui(),
                ModelSetupPS2Opcode.pglGT3_4f => new Cmd_GT3_4f(),
                _ => throw new Exception($"Unexpected opcode {opcode}")
            };;

            return cmd;
        }
    }
}
