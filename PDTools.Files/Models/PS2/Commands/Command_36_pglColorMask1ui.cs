using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Command_36_pglColorMask1ui : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglColorMask1ui;

        public uint ColorMask { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            ColorMask = bs.ReadUInt32();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt32(ColorMask);
        }

        public override string ToString()
        {
            return $"{nameof(Command_36_pglColorMask1ui)} - ColorMask: {ColorMask:X8}";
        }
    }
}
