using PDTools.Files.Models.VM.Instructions;
using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.VM
{
    public class VMBytecode
    {
        public Dictionary<int, VMInstruction> Instructions { get; set; } = new Dictionary<int, VMInstruction>();

        public void ReadBytecode(BinaryStream bs)
        {
            var originalEndian = bs.ByteConverter;
            bs.ByteConverter = ByteConverter.Little;

            int startPos = (int)bs.Position;

            int i = 0;
            while (true)
            {
                VMInstructionOpcode opcode = (VMInstructionOpcode)bs.Read1Byte();
                if (opcode == 0)
                    break;

                var ins = VMInstruction.GetByOpcode(opcode);
                ins.Offset = (int)bs.Position - startPos - 1;

                ins.Read(bs, 0);
                Instructions.Add(i++, ins);
            }

            bs.ByteConverter = originalEndian;
        }

        public void Print(Dictionary<short, VMHostMethodEntry> values)
        {
            StreamWriter sw = new StreamWriter("test.txt");
            foreach (var ins in Instructions)
            {
                sw.WriteLine($" {ins.Value.Offset,3:X2} | {ins.Value.Disassemble(values)}");
            }
            sw.Flush();
        }
    }

}
