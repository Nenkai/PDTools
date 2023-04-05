using PDTools.Files.Models.ModelSet3.Meshes;
using PDTools.Utils;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.ModelSet3.PackedMesh
{
    public class PackedMeshElementBitLayoutArray
    {
        public List<PackedMeshElementBitLayout> Layouts { get; set; } = new();

        public static PackedMeshElementBitLayoutArray FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            var arr = new PackedMeshElementBitLayoutArray();
            int count = bs.ReadInt32();
            int entriesOffset = bs.ReadInt32();
            bs.Position = entriesOffset;

            for (var i = 0; i < count; i++)
            {
                bs.Position = mdlBasePos + entriesOffset + (i * PackedMeshElementBitLayout.GetSize());
                var def = PackedMeshElementBitLayout.FromStream(bs, mdlBasePos, mdl3VersionMajor);
                arr.Layouts.Add(def);
            }

            return arr;
        }

        public static int GetSize()
        {
            return 0x08;
        }
    }
}
