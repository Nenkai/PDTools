using ExifLibrary;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.ScapesDataHelper;

public class ExifInfo
{
    public ImageFileDirectory ImageFileDirectory { get; set; }

    public ExifProperty this[ushort tag] => ImageFileDirectory.Properties[tag];

    public static ExifInfo FromStream(Stream stream)
    {
        long basePos = stream.Position;

        var bs = new BinaryStream(stream, ByteConverter.Little);
        uint sig = bs.ReadUInt32();
        if (sig == 0x2A004D4D)
            bs.ByteConverter = ByteConverter.Big;
        else if (sig == 0x002A4949)
            bs.ByteConverter = ByteConverter.Little;

        var exif = new ExifInfo();

        uint offset = bs.ReadUInt32();

        bs.Position = basePos + offset;
        exif.ImageFileDirectory = ImageFileDirectory.FromStream(bs, basePos);

        return exif;
    }

    public bool TryGetProperty(ushort tag, out ExifProperty prop)
    {
        return ImageFileDirectory.Properties.TryGetValue(tag, out prop);
    }
}

public class ImageFileDirectory // IFD
{
    public Dictionary<ushort, ExifProperty> Properties = [];

    public static ImageFileDirectory FromStream(BinaryStream bs, long baseExifOffset)
    {
        var ifd = new ImageFileDirectory();

        long baseIfdPos = bs.Position;
        ushort numTags = bs.ReadUInt16();
        for (int i = 0; i < numTags; i++)
        {
            var prop = ExifProperty.FromStream(bs, baseExifOffset);
            if (prop.Tag == 0x8769)
            {
                bs.Position = baseExifOffset + (uint)prop.Value;
                prop.Value = ImageFileDirectory.FromStream(bs, baseExifOffset);
            }
            ifd.Properties.Add(prop.Tag, prop);
        }

        return ifd;
    }
}

public class ExifProperty
{
    public ushort Tag { get; set; }
    public ushort Format { get; set; }
    public object Value { get; set; }

    public static ExifProperty FromStream(BinaryStream bs, long baseExifOffset)
    {
        var prop = new ExifProperty();
        prop.Tag = bs.ReadUInt16();
        prop.Format = bs.ReadUInt16();
        uint numComponents = bs.ReadUInt32();

        switch (prop.Tag)
        {
            case 0x8769:
                prop.Value = bs.ReadUInt32();
                break;
            case 0xD00B:
                prop.Value = bs.ReadUInt32(); break;
            case 0xD00C:
                {
                    uint offset = bs.ReadUInt32();
                    bs.Position = baseExifOffset + offset;
                    prop.Value = bs.ReadBytes((int)numComponents);
                }
                break;
        }

        return prop;
    }
}


public enum ExifTag : ushort
{
    ExifOffset = 0x8769,
    PDIDate = 0xD00B,
    PDIExif = 0xD00C,
};