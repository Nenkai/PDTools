using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;
using System.IO;
using PDTools.Files.Models.PS3.PGLCommands;

namespace PDTools.Files.Models.PS3.ModelSet3
{
    public class ModelSet3Model
    {
        public string Name { get; set; }

        public float Unk { get; set; }
        public Vector3 Origin { get; set; }
        public List<Vector3> Bounds = new();
        public List<ModelSetupCommand> Commands { get; set; } = new();
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

            long commandsOffsetActual = mdlBasePos + commandsOffset;
            bs.Position = commandsOffsetActual;
            while (bs.Position < commandsOffsetActual + commandsSize)
            {
                byte opcode = bs.Read1Byte();
                var cmd = ModelSetupCommand.GetByOpcode(opcode);

                cmd.Offset = (int)(bs.Position - commandsOffsetActual) - 1;
                cmd.Read(bs, (int)commandsOffsetActual);
                model.Commands.Add(cmd);
            }

            // For each command that can jump, translate offsets to command index
            model.TranslateJumpCommandOffsetsToIndices();

            /*
            StreamWriter sw = new StreamWriter("test.txt");
            foreach (var cmd in model.Commands)
                sw.WriteLine($"{cmd.Offset,5:X2} | {cmd}");
            sw.Close();
            */

            return model;
        }

        private void TranslateJumpCommandOffsetsToIndices()
        {
            for (var i = 0; i < Commands.Count; i++)
            {
                ModelSetupCommand setupCommand = Commands[i];
                switch (setupCommand.Opcode)
                {
                    case ModelSetupOpcode.Command_5_Switch:
                        var switchCmd = setupCommand as Command_5_Switch;
                        switchCmd.BranchJumpIndices = new int[switchCmd.BranchOffsets.Length];

                        for (int i1 = 0; i1 < switchCmd.BranchOffsets.Length; i1++)
                        {
                            ushort offset = switchCmd.BranchOffsets[i1];
                            for (var j = i; j < Commands.Count; j++)
                            {
                                if (Commands[j].Offset == offset)
                                {
                                    switchCmd.BranchJumpIndices[i1] = j;
                                }
                            }
                        }

                        break;

                    case ModelSetupOpcode.Command_9_JumpToByte:
                        var jumpCmd = setupCommand as Command_9_JumpToByte;
                        int byteOffset = jumpCmd.AbsoluteJumpToOffset;
                        for (var j = i; j < Commands.Count; j++)
                        {
                            if (Commands[j].Offset == byteOffset)
                            {
                                jumpCmd.JumpToIndex = j;
                            }
                        }
                        break;

                    case ModelSetupOpcode.Command_10_JumpToShort:
                        var shortJumpCmd = setupCommand as Command_10_JumpToShort;
                        int shortOffset = shortJumpCmd.AbsoluteJumpOffset;
                        for (var j = i; j < Commands.Count; j++)
                        {
                            if (Commands[j].Offset == shortOffset)
                            {
                                shortJumpCmd.JumpToIndex = j;
                            }
                        }
                        break;
                }
            }
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
