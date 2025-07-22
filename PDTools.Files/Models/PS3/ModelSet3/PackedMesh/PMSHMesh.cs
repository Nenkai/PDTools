using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh;

public class PMSHMesh
{
    public short FlexVertexDeclarationID { get; set; }
    public short ElementBitLayoutDefinitionID { get; set; }

    public List<PMSHSmoothMeshData> DataList { get; set; } = [];

    public static PMSHMesh FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        PMSHMesh entry = new();

        entry.FlexVertexDeclarationID = bs.ReadInt16();
        entry.ElementBitLayoutDefinitionID = bs.ReadInt16();
        short unk_0x04 = bs.ReadInt16();
        byte smoothMeshDataCountMaybe = bs.Read1Byte(); // Not sure on this. car\0001\0001\hq\body has this as 1 but data offset is 0.
        byte unk_0x07 = bs.Read1Byte();
        int unkOffset_0x08 = bs.ReadInt32();
        float unk_0x0C = bs.ReadSingle();
        int smoothMeshDataOffset = bs.ReadInt32();
        int bboxesOffset = bs.ReadInt32();
        float field_0x18 = bs.ReadSingle();
        float field_0x1C = bs.ReadSingle();
        short field_0x20 = bs.ReadInt16();
        short field_0x22 = bs.ReadInt16();
        short field_0x24 = bs.ReadInt16();
        short field_0x26 = bs.ReadInt16();
        short field_0x28 = bs.ReadInt16();
        short field_0x2A = bs.ReadInt16();
        bs.Position += 4; // Padding

        if (smoothMeshDataOffset != 0)
        {
            for (int i = 0; i < smoothMeshDataCountMaybe; i++)
            {
                bs.Position = mdlBasePos + smoothMeshDataOffset + (i * PMSHSmoothMeshData.GetSize());
                var data = PMSHSmoothMeshData.FromStream(bs, mdlBasePos, mdl3VersionMajor);
                entry.DataList.Add(data);
            }
        }

        return entry;
    }

    public static int GetSize()
    {
        return 0x30;
    }
}
