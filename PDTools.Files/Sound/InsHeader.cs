using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;

using PDTools.Files.Sound.Jam;

namespace PDTools.Files.Sound;

// GTSOUNDINSTRUMENT::AttachData (GT4O US: 0x2E1E18)
// InsHeader::getJamHeader (GT4O US: 0x2E1D58)
public class InsHeader
{
    public EsHeader EsHeader { get; set; }
    public JamHeader JamHeader { get; set; }

    public void Read(Stream stream)
    {
        var bs = new BinaryStream(stream, ByteConverter.Little);

        // NOTE: EsHeader used in both ENGN (.es) engine sounds & .ins files
        EsHeader = EsHeader.FromStream(bs);
        if (EsHeader.magic != 0x54534E49) // INST (Instrument), can also be ENGN
            throw new InvalidDataException();

        bs.Position = 0x20;

        // Starting from here this is unique to instrument files
        JamHeader = new JamHeader();
        JamHeader.Read(bs);
    }
}
