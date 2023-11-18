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
    /// GT4
    /// </summary>
    public class Cmd_VMCallbackUShort : ModelSetupPS2Command
    {
        public ushort Param { get; set; }

        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.VMCallback_UShort;

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Param = bs.ReadUInt16();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt16(Param);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_VMCallbackUShort)}";
        }
    }
}
