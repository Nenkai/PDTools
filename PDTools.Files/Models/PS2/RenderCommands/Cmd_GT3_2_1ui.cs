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
    // Refer to 0x250688 with param 2 (GT3 EU)
    public class Cmd_GT3_2_1ui : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglGT3_2_1ui;

        public float Color { get; set; }

        public Cmd_GT3_2_1ui()
        {

        }

        public Cmd_GT3_2_1ui(float color)
        {
            Color = color;
        }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Color = bs.ReadSingle();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteSingle(Color);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_GT3_2_1ui)}";
        }
    }
}
