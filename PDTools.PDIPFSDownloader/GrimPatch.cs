using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using PDTools.Crypto;
using PDTools.Utils;

namespace PDTools.GrimPFS
{
    public class GrimPatch
    {
        public const uint PatchSequenceSeed = 0;
        public const uint HeaderSeed = 1;
        public const uint UpdateNodeInfoSeed = 3;

        public ulong BaseVolumeSerial { get; set; }
        public ulong TargetVolumeSerial { get; set; }

        public string TitleID { get; set; }

        public Dictionary<string, GrimPatchFile> Files = new();

        public GrimPatch(string titleId, ulong baseVolumeSerial, ulong targetSerial)
        {
            TitleID = titleId;
            BaseVolumeSerial = baseVolumeSerial;
            TargetVolumeSerial = targetSerial;
        }

        public ulong DefaultChunkSize { get; } = 0x800000; // 8 MB
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

        public void Save(string output, uint tocIndex, uint tocSize)
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
            sw.WriteLine($"Header {headerHashStr}");

            ulong tocEncodedValue = MiscCrypto.UpdateShiftValue(((TargetVolumeSerial << 0x4 | tocIndex) << 0x14 | 0) ^ titleIdCrc);
            string tocHashStr = PDIPFSPathResolver.GetRandomStringFromValue(tocEncodedValue);
            sw.WriteLine($"TOC {PDIPFSPathResolver.GetPathFromSeed(tocIndex)} {tocSize} {tocHashStr}");

            foreach (var file in Files.Values)
                sw.WriteLine($"File {file.GamePath} {file.PFSPath} {file.ChunkId} {file.DownloadPath}");
        }
    }
}
