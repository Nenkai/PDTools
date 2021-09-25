using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using System.IO;
using PDTools.Crypto;
using PDTools.Utils;

namespace PDTools.GrimPFS
{
    public class GrimPatch
    {
        public const uint PatchSequenceSeed = 0;
        public const uint HeaderSeed = 1;
        public const uint TOCSeed = 2;
        public const uint UpdateNodeInfoSeed = 3;

        public ulong BaseVolumeSerial { get; set; }
        public ulong TargetVolumeSerial { get; set; }

        public string TitleID { get; set; }

        public Dictionary<string, GrimPatchFile> Files = new();

        private GrimPatch()
        {

        }

        public GrimPatch(string titleId, ulong baseVolumeSerial, ulong targetSerial)
        {
            TitleID = titleId;
            BaseVolumeSerial = baseVolumeSerial;
            TargetVolumeSerial = targetSerial;
        }

        public ulong DefaultChunkSize { get; set; } = 0x800000; // 8 MB
        public void AddFile(string gamePath, string pfsPath, uint fileIndex, ulong fileSize, ulong chunkSizeOverride = 0)
        {
            uint chunkId = 0;

            ulong fileChunkSize = chunkSizeOverride == 0 ? DefaultChunkSize : chunkSizeOverride;

            while (fileSize > 0)
            {
                uint titleIdCrc = ~CRC32.CRC32_0x04C11DB7(TitleID, 0);
                ulong encodedValue = MiscCrypto.UpdateShiftValue(((TargetVolumeSerial << 0x4 | fileIndex) << 0x14 | chunkId) ^ titleIdCrc);
                string hashStr = PDIPFSPathResolver.GetRandomStringFromValue(encodedValue);

                var file = new GrimPatchFile()
                {
                    GamePath = gamePath,
                    DownloadPath = hashStr,
                    ChunkId = chunkId,
                    PFSPath = pfsPath,
                };

                Files.Add(hashStr, file);

                if (fileChunkSize > fileSize)
                    fileChunkSize = fileSize;

                fileSize -= fileChunkSize;
                chunkId++;
            }
        }

        public void Save(string output, uint headerSeed, uint tocIndex)
        {
            uint titleIdCrc = ~CRC32.CRC32_0x04C11DB7(TitleID, 0);

            using var sw = new StreamWriter(output);
            sw.WriteLine($"PatchTarget {TitleID} {BaseVolumeSerial} {TargetVolumeSerial}");
            sw.WriteLine($"ChunkSize {DefaultChunkSize:X8}");

            ulong psEncodedValue = MiscCrypto.UpdateShiftValue(((TargetVolumeSerial << 0x4 | PatchSequenceSeed) << 0x14 | 0) ^ titleIdCrc);
            string psHashStr = PDIPFSPathResolver.GetRandomStringFromValue(psEncodedValue);
            sw.WriteLine($"PATCHSEQUENCE {psHashStr}");

            ulong uniEncodedValue = MiscCrypto.UpdateShiftValue(((TargetVolumeSerial << 0x4 | UpdateNodeInfoSeed) << 0x14 | 0) ^ titleIdCrc);
            string uniHashStr = PDIPFSPathResolver.GetRandomStringFromValue(uniEncodedValue);
            sw.WriteLine($"UPDATENODEINFO {uniHashStr}");

            ulong headerEncodedValue = MiscCrypto.UpdateShiftValue(((TargetVolumeSerial << 0x4 | HeaderSeed) << 0x14 | 0) ^ titleIdCrc);
            string headerHashStr = PDIPFSPathResolver.GetRandomStringFromValue(headerEncodedValue);
            sw.WriteLine($"Header {PDIPFSPathResolver.GetPathFromSeed(tocIndex)} {headerHashStr}");

            ulong tocEncodedValue = MiscCrypto.UpdateShiftValue(((tocIndex << 0x4 | TOCSeed) << 0x14 | 0) ^ titleIdCrc);
            string tocHashStr = PDIPFSPathResolver.GetRandomStringFromValue(tocEncodedValue);
            sw.WriteLine($"TOC {PDIPFSPathResolver.GetPathFromSeed(tocIndex)} {tocIndex} {tocHashStr}");

            foreach (var file in Files.Values)
                sw.WriteLine($"File {file.GamePath} {file.PFSPath} {file.ChunkId} {file.DownloadPath}");
        }

        public static bool TryRead(string inputFile, out GrimPatch patch)
        {
            patch = null;

            var tmpPatch = new GrimPatch();
            using var sw = new StreamReader(inputFile);
            if (sw.EndOfStream)
                return false;

            string patchTarget = sw.ReadLine();
            if (!tmpPatch.ReadPatchTarget(patchTarget))
                return false;
            if (sw.EndOfStream)
                return false;

            string chunkSize = sw.ReadLine();
            if (!tmpPatch.ReadChunkSize(chunkSize))
                return false;
            if (sw.EndOfStream)
                return false;

            string patchSeq = sw.ReadLine();
            if (!tmpPatch.ReadPatchSequence(patchSeq))
                return false;
            if (sw.EndOfStream)
                return false;

            string uni = sw.ReadLine();
            if (!tmpPatch.ReadUpdateNodeInfo(uni))
                return false;
            if (sw.EndOfStream)
                return false;

            string header = sw.ReadLine();
            if (!tmpPatch.ReadHeader(header))
                return false;
            if (sw.EndOfStream)
                return false;

            string toc = sw.ReadLine();
            if (!tmpPatch.ReadToC(toc))
                return false;
            if (sw.EndOfStream)
                return false;

            patch = tmpPatch;
            return true;
        }

        private bool ReadPatchTarget(string line)
        {
            var patchTargetSpl = line.Split(' ');
            if (patchTargetSpl.Length != 4 || patchTargetSpl[0] != "PatchTarget")
                return false;

            TitleID = patchTargetSpl[1];
            if (!ulong.TryParse(patchTargetSpl[2], out ulong baseSerial))
                return false;

            if (!ulong.TryParse(patchTargetSpl[3], out ulong targetSerial))
                return false;

            BaseVolumeSerial = baseSerial;
            TargetVolumeSerial = targetSerial;

            return true;
        }

        private bool ReadChunkSize(string line)
        {
            var chunkSizeSpl = line.Split(' ');
            if (chunkSizeSpl.Length != 2 || chunkSizeSpl[0] != "ChunkSize")
                return false;

            if (!uint.TryParse(chunkSizeSpl[1], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out uint chunkSize))
                return false;

            DefaultChunkSize = chunkSize;
            return true;
        }

        private bool ReadPatchSequence(string line)
        {
            string[] patchSeqSpl = line.Split(' ');
            if (patchSeqSpl.Length != 2 || patchSeqSpl[0] != "PATCHSEQUENCE")
                return false;

            Files.Add(patchSeqSpl[1], new GrimPatchFile()
            {
                DownloadPath = patchSeqSpl[1],
                FileType = GrimPatchFileType.PatchSequence,
            });

            return true;
        }

        private bool ReadUpdateNodeInfo(string line)
        {
            string[] uniSpl = line.Split(' ');
            if (uniSpl.Length != 2 || uniSpl[0] != "UPDATENODEINFO")
                return false;

            Files.Add(uniSpl[1], new GrimPatchFile()
            {
                DownloadPath = uniSpl[1],
                FileType = GrimPatchFileType.UpdateNodeInfo,
            });

            return true;
        }

        private bool ReadHeader(string line)
        {
            string[] headerSpl = line.Split(' ');
            if (headerSpl.Length != 3 || headerSpl[0] != "Header")
                return false;

            Files.Add(headerSpl[2], new GrimPatchFile()
            {
                DownloadPath = headerSpl[2],
                PFSPath = headerSpl[1],
                FileType = GrimPatchFileType.Header,
            });

            return true;
        }

        private bool ReadToC(string line)
        {
            string[] tocSpl = line.Split(' ');
            if (tocSpl.Length != 4 || tocSpl[0] != "TOC")
                return false;

            if (!uint.TryParse(tocSpl[2], out uint tocFileIndex))
                return false;

            Files.Add(tocSpl[3], new GrimPatchFile()
            {
                PFSPath = tocSpl[1],
                FileIndex = tocFileIndex,
                DownloadPath = tocSpl[3],
                FileType = GrimPatchFileType.GameFile,
            });

            return true;
        }
    }
}
