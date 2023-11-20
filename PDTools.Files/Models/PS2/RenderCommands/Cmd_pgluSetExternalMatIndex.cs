using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Cmd_pgluSetExternalMatIndex : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglExternalMatIndex;

        public byte MatIndex { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            MatIndex = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(MatIndex);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_pgluSetExternalMatIndex)} - Mat: {MatIndex}";
        }
    }
}
