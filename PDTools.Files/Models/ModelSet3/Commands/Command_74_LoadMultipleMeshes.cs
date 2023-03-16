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
    public class Command_74_LoadMultipleMeshes : ModelSetupCommand
    {
        public List<ushort> MeshIndices { get; set; } = new();

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            byte meshCount = bs.Read1Byte();
            for (var i = 0; i < meshCount; i++)
                MeshIndices.Add(bs.ReadUInt16());
        }

        public override void Write(BinaryStream bs)
        {

        }

        public override string ToString()
        {
            return $"{nameof(Command_74_LoadMultipleMeshes)} - Mesh Indices:{string.Join(", ", MeshIndices)}";
        }
    }
}
