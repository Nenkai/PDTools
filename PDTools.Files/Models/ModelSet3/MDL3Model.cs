using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

using PDTools.Files.Models.ModelSet3.Commands;
using System.IO;

namespace PDTools.Files.Models.ModelSet3
{
    public class ModelSet3Model
    {
        public string Name { get; set; }

        public float Unk { get; set; }
        public Vector3 Origin { get; set; }
        public List<Vector3> Bounds = new();
        public List<ModelCommand> Commands { get; set; } = new();
        public int InitInstance_VMInstructionPtr { get; set; }
        public int OnUpdate_VMInstructionPtr { get; set; }
        public int Unk_VMInstructionPtr { get; set; }
        public short Unk_0x2C { get; set; }
        public short Flags_0x2E { get; set; }

        public static ModelSet3Model FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            ModelSet3Model model = new();

            model.Unk = bs.ReadSingle();
            float x = bs.ReadSingle();
            float y = bs.ReadSingle();
            float z = bs.ReadSingle();
            model.Origin = new(x, y, z);

            bs.ReadByte(); // Unk
            bs.ReadByte(); // Unk
            int boundsCount = bs.ReadInt16();
            int boundsOffset = bs.ReadInt32();

            uint commandsOffset = bs.ReadUInt32();
            uint commandsSize = bs.ReadUInt32();

            // Possibly indices
            model.InitInstance_VMInstructionPtr = bs.ReadInt32();
            model.OnUpdate_VMInstructionPtr = bs.ReadInt32();
            model.Unk_VMInstructionPtr = bs.ReadInt32();

            model.Unk_0x2C = bs.ReadInt16(); // Unk
            model.Flags_0x2E = bs.ReadInt16(); // Unk

            for (int i = 0; i < boundsCount; i++)
            {
                bs.Position = mdlBasePos + boundsOffset + i * 0xC;
                float bx = bs.ReadSingle();
                float by = bs.ReadSingle();
                float bz = bs.ReadSingle();
                model.Bounds.Add(new(bx, by, bz));
            }

            
            bs.Position = commandsOffset;
            while (bs.Position < commandsOffset + commandsSize)
            {
                byte opcode = bs.Read1Byte();
                var cmd = ModelCommand.GetByOpcode(opcode);
                cmd.Offset = (int)(bs.Position - commandsOffset) - 1;
                cmd.Read(bs, (int)commandsOffset);
                model.Commands.Add(cmd);
            }

            /*
            StreamWriter sw = new StreamWriter("test.txt");
            foreach (var cmd in model.Commands)
                sw.WriteLine($"{cmd.Offset,5:X2} | {cmd}");
            sw.Close();
            */

            return model;
        }

        public static int GetSize()
        {
            return 0x30;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
