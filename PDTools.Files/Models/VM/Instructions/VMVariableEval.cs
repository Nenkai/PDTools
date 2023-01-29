using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.VM.Instructions
{
    public class VMVariableEval : VMInstruction
    {
        public bool IsHostMethod;
        public short Index;

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            short bits = bs.ReadInt16();
            IsHostMethod = bits >> 15 != 0;
            Index = (short)(bits & 0b1111111_11111111);
        }

        public override void Write(BinaryStream bs)
        {
            short bits = 0;
            bits |= (short)(IsHostMethod ? 1 << 15 : 0);
            bits |= (short)(Index & 0b1111111_11111111);
            bs.WriteInt16(bits);
        }

        public override string Disassemble(Dictionary<short, VMHostMethodEntry> values)
        {
            if (IsHostMethod)
                return $"VARIABLE_EVAL: Host Method:{values[Index].Name} (ID:{Index})";
            else
                return $"VARIABLE_EVAL: Local:{Index}";
        }
    }
}
