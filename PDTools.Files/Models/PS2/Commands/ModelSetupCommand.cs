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
        public abstract ModelSetupPS2Opcode Opcode { get; }

        public int Offset { get; set; }

        public abstract void Read(BinaryStream bs, int commandsBaseOffset);

        public abstract void Write(BinaryStream bs);

        public static ModelSetupPS2Command GetByOpcode(ModelSetupPS2Opcode opcode)
        {
            ModelSetupPS2Command cmd = opcode switch
            {
                ModelSetupPS2Opcode.pgluCallShape_1ub => new Command_4_pgluCallShape1ub(),
                ModelSetupPS2Opcode.BBoxRender => new Command_10_BBoxRender(),
                ModelSetupPS2Opcode.pglEnable17_ => new Command_11_pglEnable17(),
                ModelSetupPS2Opcode.pglColorMask1ui => new Command_36_pglColorMask1ui(),
                ModelSetupPS2Opcode.pglEnableDepthMask => new Command_38_pglEnableDepthMask(),
                ModelSetupPS2Opcode.pgluTexTable_1ub => new Command_43_pgluTexTable1ub(),

                _ => new Command_11_pglEnable17() //throw new Exception($"Unexpected opcode {opcode}")
            };;

            return cmd;
        }
    }
}
