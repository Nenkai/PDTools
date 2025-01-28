using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Sound;

/// <summary>
/// General header
/// </summary>

// SDDRV::EsHeader::remap (GT4O US: 0x533278)
public class EsHeader
{
    public uint magic { get;set; }
    public uint reloc_ptr { get; set; }

    /// <summary>
    /// Header size
    /// </summary>
    public uint hsize { get; set; }
    public uint RuntimePtr2 { get; set; }

    /// <summary>
    /// Sound size
    /// </summary>
    public uint ssize { get; set; }
    public uint sptr { get; set; }

    public static EsHeader FromStream(BinaryStream bs)
    {
        var hdr = new EsHeader();
        hdr.magic = bs.ReadUInt32();
        hdr.reloc_ptr = bs.ReadUInt32();
        hdr.hsize = bs.ReadUInt32();
        hdr.RuntimePtr2 = bs.ReadUInt32();
        hdr.ssize = bs.ReadUInt32();
        hdr.sptr = bs.ReadUInt32();
        bs.Position += 0x08;

        return hdr;
    }
}
