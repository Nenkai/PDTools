using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    /// <summary>
    /// GT4 and above. Calls pglRotate using VM output registers.
    /// </summary>
    public class Cmd_VM_Branch : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.VM_Branch;

        public ushort OutRegisterIndex { get; set; }
        public List<List<ModelSetupPS2Command>> CommandsPerBranch = new();

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            OutRegisterIndex = bs.ReadUInt16();
            byte numBranches = bs.Read1Byte();
            long tableOffset = bs.Position;
            short[] offs = bs.ReadInt16s(numBranches);

            for (int i = 0; i < numBranches; i++)
            {
                List<ModelSetupPS2Command> thisBranchCmds = new();
                long startOffset = tableOffset + (i * sizeof(short));
                long nextStartOffset = startOffset + sizeof(short);

                if (offs[i] == 0 && i > 0)
                {
                    // Empty or invalid branch offset
                    CommandsPerBranch.Add(thisBranchCmds);
                    continue;
                }

                bs.Position = startOffset + offs[i];

                while (true)
                {
                    ModelSetupPS2Opcode opcode = (ModelSetupPS2Opcode)bs.Read1Byte();
                    if (opcode == ModelSetupPS2Opcode.End)
                        break;

                    var cmd = ModelSetupPS2Command.GetByOpcode(opcode);
                    cmd.Read(bs, 0);

                    // Stop branch at unconditional jump
                    if (opcode == ModelSetupPS2Opcode.Jump_UShort || opcode == ModelSetupPS2Opcode.Jump_Byte)
                        break;

                    thisBranchCmds.Add(cmd);
                }

                CommandsPerBranch.Add(thisBranchCmds);
            }
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt16(OutRegisterIndex);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_VM_Branch)}";
        }
    }
}
