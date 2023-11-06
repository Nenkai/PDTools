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
    /// Sets all 4 values linked to shape unk1 value 2 to the specified uint - GT3 only.
    /// </summary>
    public class Command_50_GT3_1ui : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglGT3_1ui;

        public float Value { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Value = bs.ReadSingle();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteSingle(Value);
        }

        public override string ToString()
        {
            return $"{nameof(Command_50_GT3_1ui)}";
        }
    }
}
