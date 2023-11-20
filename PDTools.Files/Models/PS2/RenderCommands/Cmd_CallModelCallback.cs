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
    public class Cmd_CallModelCallback : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.CallModelCallback;

        public List<ModelSetupPS2Command> Default = new List<ModelSetupPS2Command>();
        public List<List<ModelSetupPS2Command>> CommandsPerBranch = new List<List<ModelSetupPS2Command>>();

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

            if (branchCount > 0)
            {
                // Default
                long endOfAllOffset = 0;
                while (bs.Position < tableOffset + jumpOffsets[0])
                {
                    ModelSetupPS2Opcode opcode = (ModelSetupPS2Opcode)bs.Read1Byte();
                    if (opcode == ModelSetupPS2Opcode.End)
                        break;

                    var cmd = ModelSetupPS2Command.GetByOpcode(opcode);
                    cmd.Read(bs, 0);

                    if (opcode == ModelSetupPS2Opcode.Jump_UShort || opcode == ModelSetupPS2Opcode.Jump_Byte)
                    {
                        if (opcode == ModelSetupPS2Opcode.Jump_UShort)
                            endOfAllOffset = (bs.Position - 2) + (cmd as Cmd_JumpUShort).JumpOffset;
                        else if (opcode == ModelSetupPS2Opcode.Jump_Byte)
                            endOfAllOffset = (bs.Position - 1) + (cmd as Cmd_JumpByte).JumpOffset;
                        break;
                    }

                    Default.Add(cmd);
                }

                // Commands per returned parameter
                for (int i = 0; i < branchCount; i++)
                {
                    List<ModelSetupPS2Command> thisBranchCmds = new List<ModelSetupPS2Command>();
                    long startOffset = tableOffset + (i * sizeof(ushort));
                    long nextStartOffset = startOffset + sizeof(ushort);

                    bs.Position = startOffset + jumpOffsets[i];

                    // Attempt to use default jump offset as delimiter for last branch
                    long endOffset = (i == branchCount - 1 ? endOfAllOffset : nextStartOffset + jumpOffsets[i + 1]);

                    while (bs.Position < endOffset)
                    {
                        ModelSetupPS2Opcode opcode = (ModelSetupPS2Opcode)bs.Read1Byte();
                        if (opcode == ModelSetupPS2Opcode.End)
                            break;

                        var cmd = ModelSetupPS2Command.GetByOpcode(opcode);
                        cmd.Read(bs, 0);

                        // New jump = likely branch end
                        if (opcode == ModelSetupPS2Opcode.Jump_UShort || opcode == ModelSetupPS2Opcode.Jump_Byte)
                            break;

                        thisBranchCmds.Add(cmd);
                    }

                    CommandsPerBranch.Add(thisBranchCmds);
                }
            }
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt16(Parameter);
            bs.WriteByte((byte)(CommandsPerBranch.Count));

            long tableOffset = bs.Position;
            long dataOffset = bs.Position + (sizeof(ushort) * CommandsPerBranch.Count);
            bs.Position = dataOffset;

            foreach (var cmd in Default)
            {
                bs.WriteByte((byte)cmd.Opcode);
                cmd.Write(bs);
            }

            bs.WriteByte((byte)ModelSetupPS2Opcode.Jump_UShort);
            long skipAllJumpOffset = bs.Position;
            bs.WriteInt16(0);

            dataOffset = bs.Position;

            List<long> jumpOffsets = new List<long>();

            for (int i = 0; i < CommandsPerBranch.Count; i++)
            {
                long off = tableOffset + (sizeof(ushort) * i);
                bs.Position = off;
                bs.WriteUInt16((ushort)(dataOffset - off));
                bs.Position = dataOffset;

                foreach (var cmd in CommandsPerBranch[i])
                {
                    bs.WriteByte((byte)cmd.Opcode);
                    cmd.Write(bs);
                }

                if (i != CommandsPerBranch.Count - 1)
                {
                    bs.WriteByte((byte)ModelSetupPS2Opcode.Jump_UShort);
                    jumpOffsets.Add(bs.Position);
                    bs.WriteInt16(0);
                }

                dataOffset = bs.Position;
            }

            for (int i = 0; i < jumpOffsets.Count; i++)
            {
                bs.Position = jumpOffsets[i];
                bs.WriteUInt16((ushort)(dataOffset - jumpOffsets[i]));
            }

            bs.Position = skipAllJumpOffset;
            bs.WriteUInt16((ushort)(dataOffset - skipAllJumpOffset));

            bs.Position = dataOffset;
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_CallModelCallback)}";
        }
    }

    public enum ModelCallbackParameter
    {
        IsTailLampActive = 0,
    }
}
