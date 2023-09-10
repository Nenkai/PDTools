using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

using PDTools.Crypto;
using PDTools.Utils;
using PDTools.Hashing;

namespace PDTools.GrimPFS
{
    public class GrimPatch
    {
        public const uint PatchSequenceSeed = 0;
        public const uint HeaderSeed = 1;
        public const uint GameFileSeed = 2;
        public const uint UpdateNodeInfoSeed = 3;

        public ulong BaseVolumeSerial { get; set; }
        public ulong TargetVolumeSerial { get; set; }

        public string TitleID { get; set; }

        public Dictionary<string, GrimPatchFile> Files = new Dictionary<string, GrimPatchFile>();

        /* \!/ Major note regarding UpdateNodeInfo \!/
         * If a file index specified in the file, and its scrambled path (i.e 9/AB/CD) is already present in the player's PDIPFS
         * i.e person reverted their toc to vanilla, the game will check if the current iterated entry in the list exists and if so, it will be removed
         * from the person's download list of files as it assumes its all good
         * This can lead to improper patching, and should be used strictly starting from 1.22.
         * 
         * From BCES01893 1.22 EBOOT, at 0x0ABA240 (calls ab79d0 - CheckFileExists) - Nenkai
         */

        private GrimPatch()
        {

        }

        public GrimPatch(string titleId, ulong baseVolumeSerial, ulong targetSerial)
        {
            TitleID = titleId;
            BaseVolumeSerial = baseVolumeSerial;
            TargetVolumeSerial = targetSerial;
        }

        public ulong DefaultChunkSize { get; set; } = 0x200000; // 2 MB, can't be more (game allocates 0x210000)
        public void AddFile(string gamePath, string pfsPath, uint fileIndex, ulong fileSize, ulong chunkSizeOverride = 0)
        {
            uint chunkId = 0;

            ulong fileChunkSize = chunkSizeOverride == 0 ? DefaultChunkSize : chunkSizeOverride;

            uint titleIdCrc = ~CRC32.CRC32_0x04C11DB7(TitleID, 0);
            while (fileSize > 0)
            {
                ulong encodedValue = MiscCrypto.UpdateShiftValue((((ulong)fileIndex << 0x4 | GameFileSeed) << 0x14 | chunkId) ^ titleIdCrc);
                string hashStr = PDIPFSPathResolver.GetRandomStringFromValue(encodedValue);

                var file = new GrimPatchFile()
                {
                    GamePath = gamePath,
                    DownloadPath = hashStr,
                    ChunkId = chunkId,
                    PFSPath = pfsPath,
                    FileIndex = fileIndex,
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

            using (var sw = new StreamWriter(output))
            {
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
                sw.WriteLine($"Header {PDIPFSPathResolver.GetPathFromSeed(headerSeed)} {headerSeed} {headerHashStr}");

                ulong tocEncodedValue = MiscCrypto.UpdateShiftValue((((ulong)tocIndex << 0x4 | GameFileSeed) << 0x14 | 0) ^ titleIdCrc);
                string tocHashStr = PDIPFSPathResolver.GetRandomStringFromValue(tocEncodedValue);
                sw.WriteLine($"TOC {PDIPFSPathResolver.GetPathFromSeed(tocIndex)} {tocIndex} {tocHashStr}");

                foreach (var file in Files.Values)
                    sw.WriteLine($"File {file.GamePath} {file.PFSPath} {file.FileIndex} {file.ChunkId} {file.DownloadPath}");
            }
        }

        public static bool TryRead(string inputFile, out GrimPatch patch)
        {
            patch = null;

            var tmpPatch = new GrimPatch();
            using (var sw = new StreamReader(inputFile))
            {
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

                while (!sw.EndOfStream)
                {
                    string line = sw.ReadLine();
                    if (string.IsNullOrEmpty(line) || line.StartsWith("//"))
                        continue;

                    tmpPatch.ReadFile(line);
                }
            }

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
            if (headerSpl.Length != 4 || headerSpl[0] != "Header")
                return false;

            if (!uint.TryParse(headerSpl[2], out uint headerFileIndex))
                return false;

            Files.Add(headerSpl[3], new GrimPatchFile()
            {
                DownloadPath = headerSpl[3],
                PFSPath = headerSpl[1],
                FileIndex = headerFileIndex,
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

        private bool ReadFile(string line)
        {
            string[] fileSpl = line.Split(' ');
            if (fileSpl.Length != 6 || fileSpl[0] != "File")
                return false;

            if (!uint.TryParse(fileSpl[3], out uint fileIndex))
                return false;

            if (!uint.TryParse(fileSpl[4], out uint chunkId))
                return false;

            Files.Add(fileSpl[5], new GrimPatchFile()
            {
                PFSPath = fileSpl[2],
                FileIndex = fileIndex,
                ChunkId = chunkId,
                GamePath = fileSpl[1],
                DownloadPath = fileSpl[5],
                FileType = GrimPatchFileType.TOC,
            });

            return true;
        }
    }
}
