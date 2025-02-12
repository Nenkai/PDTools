using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.ScapesDataHelper;

public class JxlBoxInfoMap
{
    public Dictionary<uint, (long Offset, uint Size)> Boxes = [];

    public static JxlBoxInfoMap FromStream(Stream stream)
    {
        var bs = new BinaryStream(stream, ByteConverter.Big);

        var inf = new JxlBoxInfoMap();
        while (stream.Position != stream.Length)
        {
            uint boxSize = bs.ReadUInt32();
            uint sig = bs.ReadUInt32();
            inf.Boxes.Add(sig, (bs.Position, boxSize - 8));

            bs.Position += boxSize - 8;
        }

        return inf;
    }

    public static JxlBoxInfoMap FromFile(byte[] file)
    {
        using var ms = new MemoryStream(file);
        return FromStream(ms);
    }
}
