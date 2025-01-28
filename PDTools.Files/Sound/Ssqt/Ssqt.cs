using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;

using PDTools.Files.Sound.Ssqt.Meta;

namespace PDTools.Files.Sound.Ssqt;

// Sqt is handled by game code through one of the two sequencers - SqSequencer, for menu bgm.
// Sq is somewhat named in the JAM sdk docs.

// Spu sequence Table? Sound sequence table?
public class Ssqt
{
    /// <summary>
    /// 'Ssqt' - SsqtHeaderChunk::MAGIC
    /// </summary>
    public const uint MAGIC = 0x74717353;

    public List<Sssq> Tracks { get; set; } = [];

    public void Read(string fileName)
    {
        using var fs = new FileStream(fileName, FileMode.Open);
        using var bs = new BinaryStream(fs, ByteConverter.Little);

        uint magic = bs.ReadUInt32();
        if (magic != MAGIC)
            throw new InvalidDataException("Invalid Ssqt header chunk magic");

        uint count_ = bs.ReadUInt32(); // number of sequences
        uint[] sequenceOffsets = bs.ReadUInt32s((int)count_ + 1);

        for (int i = 0; i < count_ + 1; i++)
        {
            bs.Position = sequenceOffsets[i];
            var sssq = new Sssq();
            sssq.Read(bs);

            Tracks.Add(sssq);
        }
    }
}