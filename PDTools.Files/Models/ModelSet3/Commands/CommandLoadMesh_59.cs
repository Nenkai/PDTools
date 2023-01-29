using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class CommandLoadMesh_59 : ModelCommand
    {
        public byte MeshID { get; set; }
        public byte Unk { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            MeshID = bs.Read1Byte();
            Unk = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(MeshID);
            bs.WriteByte(Unk);
        }

        public override string ToString()
        {
            return $"{nameof(CommandLoadMesh_59)}: {MeshID} {Unk}";
        }
    }
}
