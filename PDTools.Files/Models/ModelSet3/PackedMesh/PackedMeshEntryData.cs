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
    public class PackedMeshEntryData
    {
        public int PackedFlexVertsOffset { get; set; }
        public ushort FlexVertCount { get; set; }
        public int NonPackedFlexVertsOffset { get; set; }

        public static PackedMeshEntryData FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            PackedMeshEntryData data = new();

            data.PackedFlexVertsOffset = bs.ReadInt32();
            int unkOffset_0x04 = bs.ReadInt32();
            int unkOffset_0x08 = bs.ReadInt32();
            data.NonPackedFlexVertsOffset = bs.ReadInt32();
            int unkOffset_0x10 = bs.ReadInt32();
            int unkOffset_0x14 = bs.ReadInt32();
            int unkOffset_0x18 = bs.ReadInt32();
            bs.ReadInt32(); // Unk 0
            bs.ReadInt16(); // Unk
            data.FlexVertCount = bs.ReadUInt16();

            // TODO read rest
            /*
            bs.Position = vertsOffset;

            BitStream bits = new BitStream(BitStreamMode.Read, bs.ReadBytes(18));
            ulong b = bits.ReadBits(13);
            ulong b2 = bits.ReadBits(13);
            ulong b3 = bits.ReadBits(14);
            ulong b4 = bits.ReadBits(32);

            ulong c = bits.ReadBits(13);
            ulong c2 = bits.ReadBits(13);
            ulong c3 = bits.ReadBits(14);
            ulong c4 = bits.ReadBits(32);
            */
            return data;
        }

        public int GetOffsetOfPackedElement(PackedMeshElementBitLayoutArray bitLayouts, PackedMeshFlexVertexDefinition vertDefinition, string type)
        {
            int byteOffset = 0;

            int currentLayoutIndex = 0;
            foreach (var elem in vertDefinition.PackedElements)
            {
                if (elem.Key == "colorSet1")
                    continue;

                if (elem.Key == type)
                    break;

                byteOffset += ((FlexVertCount * bitLayouts.Layouts[currentLayoutIndex].TotalBitCount) + 7) / 8;
                currentLayoutIndex++;
            }

            return byteOffset;
        }

        public int GetTotalByteSizeOfPackedElement(PackedMeshElementBitLayoutArray bitLayouts, PackedMeshFlexVertexDefinition vertDefinition, string type)
        {
            int currentLayoutIndex = 0;
            foreach (var elem in vertDefinition.Elements)
            {
                if (elem.Key == "colorSet1")
                    continue;

                if (elem.Key == type)
                    return 4 * FlexVertCount;

                currentLayoutIndex++;
            }

            currentLayoutIndex = 0;
            foreach (var elem in vertDefinition.PackedElements)
            {
                if (elem.Key == "colorSet1")
                    continue;

                if (elem.Key == type)
                    return ((bitLayouts.Layouts[currentLayoutIndex].TotalBitCount * FlexVertCount) + 7) / 8;

                currentLayoutIndex++;
            }

            return -1;
        }

        public static int GetSize()
        {
            return 0x30;
        }
    }
}
