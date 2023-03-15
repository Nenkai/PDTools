using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    /// <summary>
    /// Not present in GT PSP
    /// </summary>
    public class Command_75_LoadMultipleMeshes2 : ModelCommand
    {
        public List<ushort> MeshIndices { get; set; } = new();
        public uint Unk { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            byte meshCount = bs.Read1Byte();
            for (var i = 0; i < meshCount; i++)
                MeshIndices.Add(bs.ReadUInt16());

            Unk = bs.ReadUInt32();
        }

        public override void Write(BinaryStream bs)
        {

        }

        public override string ToString()
        {
            return $"{nameof(Command_75_LoadMultipleMeshes2)} - Mesh Indices:{string.Join(", ", MeshIndices)}, Unk: {Unk}";
        }
    }
}
