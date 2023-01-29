using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class CommandUnk_4 : ModelCommand
    {
        public short MeshIndex;

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            MeshIndex = bs.ReadInt16();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteInt16(MeshIndex);
        }

        public override string ToString()
        {
            return $"{nameof(CommandUnk_4)}: {MeshIndex}";
        }
    }
}
