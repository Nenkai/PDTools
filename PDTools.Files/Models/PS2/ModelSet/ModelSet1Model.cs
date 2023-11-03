using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;

using PDTools.Files.Models.PS2.Commands;
using PDTools.Utils;

namespace PDTools.Files.Models.PS2.ModelSet
{
    public class ModelSet1Model
    {
        public List<ModelSetupPS2Command> Commands { get; set; } = new();

        public void FromStream(BinaryStream bs)
        {
            while (true)
            {
                ModelSetupPS2Opcode opcode = (ModelSetupPS2Opcode)bs.Read1Byte();
                if (opcode == ModelSetupPS2Opcode.End)
                    break;

                var cmd = ModelSetupPS2Command.GetByOpcode(opcode);

                cmd.Read(bs, 0);
                Commands.Add(cmd);
            }
        }

        public void Write(BinaryStream bs)
        {
            foreach (var cmd in Commands)
            {
                bs.Write((byte)cmd.Opcode);
                cmd.Write(bs);
            }

            bs.WriteByte(0); // End
        }
    }
}
