using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public abstract class ModelCommand
    {
        public byte Opcode { get; private set; }

        public int Offset { get; set; }

        public abstract void Read(BinaryStream bs, int commandsBaseOffset);

        public abstract void Write(BinaryStream bs);

        public static ModelCommand GetByOpcode(byte opcode)
        {
            ModelCommand cmd = opcode switch
            {
                0 => new CommandEnd_0(),
                2 => new CommandUnk_2(),
                3 => new CommandUnk_3(),
                4 => new CommandUnk_4(),
                5 => new CommandSwitch_5(),
                6 => new CommandUnk_6(),
                7 => new CommandUnk_7(),
                8 => new CommandUnk_8(),
                9 => new CommandJumpToByte_9(),
                10 => new CommandJumpToShort_10(),
                14 => new CommandUnk_14(),
                15 => new CommandUnk_15(),
                44 => new CommandUnk_44(),
                53 => new CommandUnk_53(),
                54 => new CommandUnk_54(),
                59 => new CommandLoadMesh_59(),
                60 => new CommandUnk_60(),
                61 => new CommandUnk_61(),
                _ => new CommandUnk_Hack()
                //_ => throw new Exception($"Unexpected opcode {opcode}")
            };

            cmd.Opcode = opcode;
            return cmd;
        }
    }
}
