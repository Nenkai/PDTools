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
    /// Calls a model callback - different per type of model
    /// </summary>
    public class Command_6_CallModelCallback : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.BBoxRender;

        /* CarCallback_GetTailLampActive = 0,
         * 
           CarCallback_SetWheelSpeed = 1,
           // 2 same
           // 3 same
           // 4 same
           
           CarCallback_SetSteering = 5,
           CarCallback_SetUnk = 6,
           CarCallback_GetTimezone = 7,
           CarCallback_RotateZ = 8,
           
           CarCallback_RenderTire = 15,
           // 16 same
           // 17 same
           // 18 same
           
           CarCallback_RenderWheel = 36,
           // 37 same
           // 38 same
           // 39 same
        */
        public ushort Parameter { get; set; }
        
        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Parameter = bs.ReadUInt16();

            ushort branchCount = bs.Read1Byte();

            long tableOffset = bs.Position;
            ushort[] jumpOffsets = bs.ReadUInt16s(branchCount); // Last is always skip?

            for (int i = 0; i < branchCount - 1; i++)
            {
                List<ModelSetupPS2Command> lodCommands = new List<ModelSetupPS2Command>();
                long startOffset = tableOffset + (i * sizeof(ushort));
                long nextStartOffset = startOffset + sizeof(ushort);

                bs.Position = startOffset + jumpOffsets[i];
                long endOffset = nextStartOffset + jumpOffsets[i + 1];

                while (bs.Position < endOffset)
                {
                    ModelSetupPS2Opcode opcode = (ModelSetupPS2Opcode)bs.Read1Byte();
                    if (opcode == ModelSetupPS2Opcode.End)
                        break;

                    var cmd = ModelSetupPS2Command.GetByOpcode(opcode);
                    cmd.Read(bs, 0);
                    lodCommands.Add(cmd);

                    if (opcode == ModelSetupPS2Opcode.Jump1us)
                        break;
                }
            }
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt16(Parameter);
            throw new NotImplementedException("Finish this");
        }

        public override string ToString()
        {
            return $"{nameof(Command_6_CallModelCallback)}";
        }
    }
}
