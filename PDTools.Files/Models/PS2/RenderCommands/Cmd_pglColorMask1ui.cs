using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Cmd_pglColorMask : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglColorMask;

        public uint ColorMask { get; set; }

        public Cmd_pglColorMask()
        {

        }

        public Cmd_pglColorMask(uint mask)
        {
            ColorMask = mask;
        }

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
            return $"{nameof(Cmd_pglColorMask)} - ColorMask: {ColorMask:X8}";
        }
    }
}
