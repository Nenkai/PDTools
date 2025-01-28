using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.ModelSet;

/// <summary>
/// Bounding (?)
/// </summary>
public class ModelSet1Bounding
{
    public Vector4 Value { get; set; } // Only W is used, for computing LOD width? see: GT3 EU 0x225f08 (defaultLOD)

    public void FromStream(BinaryStream bs)
    {
        Value = new Vector4(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
    }

    public void Write(BinaryStream bs)
    {
        bs.WriteSingle(Value.X); bs.WriteSingle(Value.Y); bs.WriteSingle(Value.Z); bs.WriteSingle(Value.W);
    }

    public static int GetSize()
    {
        return 0x10;
    }
}
