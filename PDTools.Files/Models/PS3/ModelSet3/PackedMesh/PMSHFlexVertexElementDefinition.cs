using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

using PDTools.Files.Models.PS3.ModelSet3.FVF;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh;

public class PMSHFlexVertexElementDefinition
{
    public string Name { get; set; }
    public byte NameID { get; set; }
    public byte OutputFlexOffset { get; set; }
    public byte ElementCount { get; set; }
    public CELL_GCM_VERTEX_TYPE Type { get; set; }

    public bool IsPacked { get; set; }

    public static PMSHFlexVertexElementDefinition FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        PMSHFlexVertexElementDefinition declaration = new();

        declaration.NameID = bs.Read1Byte();
        declaration.OutputFlexOffset = bs.Read1Byte();
        declaration.ElementCount = bs.Read1Byte();
        declaration.Type = (CELL_GCM_VERTEX_TYPE)bs.Read1Byte();

        return declaration;
    }

    public static int GetSize()
    {
        return 0x04;
    }
}
