using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;

using PDTools.Files.Models.PS2.Commands;
using PDTools.Utils;
using System.Numerics;

namespace PDTools.Files.Models.PS2.ModelSet;

/// <summary>
/// Represents a model within a ModelSet0.
/// </summary>
public class ModelSet0Model : ModelPS2Base
{
    public PGLUshape UnkModel0 { get; set; }
    public PGLUshape MainModel { get; set; }
    public PGLUshape UnkModel2 { get; set; }
    public PGLUshape ReflectionModel { get; set; }
    public PGLUshape UnkModel4 { get; set; }

    public Vector3[] BoundaryBox = new Vector3[8];

    public void FromStream(Stream stream, uint basePos)
    {
        var bs = new BinaryStream(stream, ByteConverter.Little);

        uint empty = bs.ReadUInt32();
        uint relocPtr = bs.ReadUInt32();

        uint[] lodOffsetsMaybe = bs.ReadUInt32s(5);
        bs.ReadUInt32(); // Empty

        for (int i = 0; i < 8; i++)
        {
            BoundaryBox[i] = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        }

        for (int i = 0; i < lodOffsetsMaybe.Length; i++)
        {
            if (lodOffsetsMaybe[i] == 0)
                continue;

            bs.Position = basePos + lodOffsetsMaybe[i];

            var shape = new PGLUshape();
            shape.FromStream(bs);

            switch (i)
            {
                case 0:
                    UnkModel0 = shape; break;
                case 1:
                    MainModel = shape; break;
                case 2:
                    UnkModel2 = shape; break;
                case 3:
                    ReflectionModel = shape; break;
                case 4:
                    UnkModel4 = shape; break;
            }
        }


    }

    public static int GetSize()
    {
        return 0x80;
    }
}
