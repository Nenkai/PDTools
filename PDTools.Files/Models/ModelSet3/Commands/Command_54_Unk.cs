using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class Command_54_Unk : ModelSetupCommand
    {
        public ushort Value { get; set; }
        public ushort[] BranchOffsets { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Value = bs.ReadUInt16();

            byte arrLen = bs.Read1Byte();
            BranchOffsets = new ushort[arrLen];

            for (var i = 0; i < arrLen; i++)
            {
                BranchOffsets[i] = bs.ReadUInt16();

                // Translate relative to absolute
                long currentOffset = bs.Position - commandsBaseOffset;
                BranchOffsets[i] += (ushort)currentOffset;
            }
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt16(Value);

            bs.WriteByte((byte)BranchOffsets.Length);
            bs.WriteUInt16s(BranchOffsets);
        }

        public override string ToString()
        {
            return $"{nameof(Command_54_Unk)}: Value:{Value} - Jump offsets: {string.Join(",", BranchOffsets.Select(e => e.ToString("X2")))}";
        }
    }
}
