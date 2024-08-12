using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.CarModel1;

public class ExhaustData
{
    public Vector3 Position { get; set; }

    public void FromStream(BinaryStream bs)
    {
        Position = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        bs.Position += 0x14; // Empty
    }

    public void Write(BinaryStream bs)
    {
        bs.WriteSingle(Position.X); bs.WriteSingle(Position.Y); bs.WriteSingle(Position.Z);
        bs.Position += 0x14;
    }

    public static uint GetSize()
    {
        return 0x20;
    }
}
