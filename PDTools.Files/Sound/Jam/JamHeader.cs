using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;

using PDTools.Files.Sound.Se;

namespace PDTools.Files.Sound.Jam;

/* "Jam" is a reference to the sound authoring tool provided in the SDK
 * -> PS2SDK/P-sound/atools/Sndtool111/mac/SoundPreview111/Doc/html/jam.htm (japanese)
 * 
 * The original jam format is described in
 * -> PS2SDK/P-sound/atools/Sndtool111/mac/SoundPreview111/Doc/html/sformat.htm (japanese)
 * 
 * However this header is quite different and simplifies some contents & arrangement
 * Note that the magic is the same and not at position 0 either
 * 
 */
// GTSOUNDINSTRUMENTJAM::open (GT4O US: 0x2E20B0)
// SDDRV::Jam::open (GT4O US: 0x2E20B0)
// SDDRV::Jam::bdLink (GT4O US: 0x533998)
public class JamHeader
{
    /// <summary>
    /// Program chunks for SqSequencer (sqt files)
    /// </summary>
    // SDDRV::Jam::getProgChunk (GT4O US: 0x533878)
    public List<JamProgChunk> ProgramChunks { get; set; } = [];

    /// <summary>
    /// Sound effect midi sequences, handled by SeSequencer
    /// </summary>
    // SDDRV::Jam::getSeSeq (GT4O US: 0x533760)
    public List<SeSeq> SeSequences { get; set; } = [];

    /// <summary>
    /// Program chunks for SeSequencer
    /// </summary>
    // SDDRV::Jam::getSeProgChunk (GT4O US: 0x533678)
    public List<JamProgChunk> SeProgramChunks { get; set; } = [];

    // SDDRV::Jam::getLfoTable (GT4O US: 0x5337F0)
    // SDDRV::Jam::getVelocity (GT4O US: 0x533760)

    public void Read(BinaryStream bs)
    {
        long basePos = bs.Position;

        uint jamHeaderSize = bs.ReadUInt32();
        uint bdSize = bs.ReadUInt32(); // Body size
        bs.Position += 0x04;

        uint spuStreamHeaderMagic = bs.ReadUInt32(); // 'SShd'
        if (spuStreamHeaderMagic != 0x64685353)
            throw new InvalidDataException();

        int programChunkPhysicalOffset = bs.ReadInt32();
        int velocityChunkPhysicalOffset = bs.ReadInt32();
        int lfoTableChunkPhysicalOffset = bs.ReadInt32();
        int seSeqChunkPhysicalOffset = bs.ReadInt32();
        int unkPhysicalOffset = bs.ReadInt32(); // Unknown, set in se files. Never seen actually read
        int seProgChunkPhysicalOffset = bs.ReadInt32();

        if (programChunkPhysicalOffset != -1)
        {
            bs.Position = basePos + programChunkPhysicalOffset;

            long baseChunkPos = bs.Position;
            short chunkCount = bs.ReadInt16();
            short[] chunkOffsets = bs.ReadInt16s(chunkCount + 1);

            for (int i = 0; i < chunkCount + 1; i++)
            {
                bs.Position = baseChunkPos + chunkOffsets[i];

                var chunk = new JamProgChunk();
                chunk.Read(bs);
                ProgramChunks.Add(chunk);
            }
        }

        if (seSeqChunkPhysicalOffset != -1)
        {
            bs.Position = basePos + seSeqChunkPhysicalOffset;

            long baseChunkPos = bs.Position;
            short chunkCount = bs.ReadInt16();
            short[] chunkOffsets = bs.ReadInt16s(chunkCount + 1);

            List<(short, short Offset)> entries = [];
            for (int i = 0; i < chunkCount + 1; i++)
            {
                bs.Position = baseChunkPos + chunkOffsets[i];

                entries.Add((bs.ReadInt16(), bs.ReadInt16()));
            }

            // TODO: Arrange this better
            for (int i = 0; i < chunkCount + 1; i++)
            {
                bs.Position = baseChunkPos + entries[i].Offset;

                SeSeq seSeq = new SeSeq();
                seSeq.Read(bs);
                SeSequences.Add(seSeq);
            }
        }

        if (seProgChunkPhysicalOffset != -1)
        {
            bs.Position = basePos + seProgChunkPhysicalOffset;

            long baseChunkPos = bs.Position;
            short chunkCount = bs.ReadInt16();
            short[] chunkOffsets = bs.ReadInt16s(chunkCount + 1);

            for (int i = 0; i < chunkCount + 1; i++)
            {
                bs.Position = baseChunkPos + chunkOffsets[i];

                var chunk = new JamProgChunk();
                chunk.Read(bs);
                SeProgramChunks.Add(chunk);
            }
        }
    }
}
