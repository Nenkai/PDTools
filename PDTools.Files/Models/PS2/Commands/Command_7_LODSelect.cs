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
    /// Whether to render this model based on bbox, seeks past commands if not
    /// </summary>
    public class Command_07_LODSelect : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.LODSelect;

        public Vector3 Unk { get; set; }
        public float Unk2 { get; set; }
        public List<List<ModelSetupPS2Command>> CommandsPerLOD { get; set; } = new();

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Unk = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
            Unk2 = bs.ReadSingle();
            byte lodCount = bs.Read1Byte();

            long tableOffset = bs.Position;
            ushort[] jumpOffsets = bs.ReadUInt16s(lodCount);

            for (int i = 0; i < lodCount; i++)
            {
                List<ModelSetupPS2Command> lodCommands = new List<ModelSetupPS2Command>();
                long startOffset = tableOffset + (i * sizeof(ushort));
                long nextStartOffset = startOffset + sizeof(ushort);

                bs.Position = startOffset + jumpOffsets[i];
                long endOffset = nextStartOffset + (i == jumpOffsets.Length - 1 ? bs.Length : jumpOffsets[i + 1]);

                while (bs.Position < endOffset)
                {
                    ModelSetupPS2Opcode opcode = (ModelSetupPS2Opcode)bs.Read1Byte();
                    if (opcode == ModelSetupPS2Opcode.End)
                        break;

                    var cmd = ModelSetupPS2Command.GetByOpcode(opcode);

                    cmd.Read(bs, 0);
                    lodCommands.Add(cmd);
                }

                CommandsPerLOD.Add(lodCommands);
            }
        }

        public override void Write(BinaryStream bs)
        {
            
        }

        public override string ToString()
        {
            return $"{nameof(Command_07_LODSelect)}";
        }
    }
}
