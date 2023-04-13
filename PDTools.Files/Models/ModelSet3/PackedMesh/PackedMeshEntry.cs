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
    public class PackedMeshEntry
    {
        public short StructDeclarationID { get; set; }
        public short ElementBitLayoutDefinitionID { get; set; }

        public PackedMeshEntryData Data { get; set; }

        public static PackedMeshEntry FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            PackedMeshEntry entry = new();

            entry.StructDeclarationID = bs.ReadInt16();
            entry.ElementBitLayoutDefinitionID = bs.ReadInt16();
            bs.ReadInt16();

            byte countOfUnk = bs.Read1Byte();
            bs.ReadByte(); // 1
            int unkOffset = bs.ReadInt32();
            float unk = bs.ReadSingle();
            int dataTocOffset = bs.ReadInt32();
            bs.Position += 0x18;

            // TODO read rest
            bs.Position = mdlBasePos + dataTocOffset;
            entry.Data = PackedMeshEntryData.FromStream(bs, mdlBasePos, mdl3VersionMajor);

            return entry;
        }

        public static int GetSize()
        {
            return 0x30;
        }
    }
}
