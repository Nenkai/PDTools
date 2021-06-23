using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using Syroot.BinaryData.Memory;

using PDTools;
using PDTools.Structures;

using System.Linq;

namespace GTReplayInfo
{
    public class MReplayInfo
    {
        public uint Version { get; set; }
        public uint GTBehaviorVersion { get; set; }
        public uint SpecDBVersion { get; set; }
        public DateTime RecordedDateTime { get; set; }
        public string SpecDBName { get; set; }
        public uint Unk { get; set; }
        public bool RaceCompleted { get; set; }
        public bool OneLap { get; set; }
        public uint Score { get; set; }
        public uint EntryMax { get; set; }

        public uint TotalFrameCount { get; set; }
        public uint GeometryStreamSize { get; set; }
        public uint GeometryQualityLevel { get; set; }
        public ulong FilesystemVersion { get; set; }
        public uint EntryNum { get; set; }

        public ReplayEntry[] ReplayEntries;

        public static MReplayInfo LoadFromFile(string fileName)
        {
            byte[] file = File.ReadAllBytes(fileName);
            SpanReader sr = new SpanReader(file, Syroot.BinaryData.Core.Endian.Big);

            string magic = sr.ReadStringRaw(4);
            sr.Position += 4;
            uint replayVersion = sr.ReadUInt32();
            sr.Position += 20;

            var rply = new MReplayInfo();
            rply.GTBehaviorVersion = sr.ReadUInt32();
            rply.SpecDBVersion = sr.ReadUInt32();
            rply.RecordedDateTime = JulianTime.FromJulianDateValue(sr.ReadUInt64());
            rply.SpecDBName = sr.ReadString0();

            sr.Position = 0x70;
            rply.Unk = sr.ReadUInt32();
            rply.RaceCompleted = sr.ReadBoolean4();
            rply.OneLap = sr.ReadBoolean4();
            rply.Score = sr.ReadUInt32();

            short[] finishOrders = new short[32];
            for (int i = 0; i < 32; i++)
                finishOrders[i] = sr.ReadInt16();

            int[] totalTimes = new int[32];
            for (int i = 0; i < 32; i++)
                totalTimes[i] = sr.ReadInt32();

            int[] bestTimes = new int[32];
            for (int i = 0; i < 32; i++)
                bestTimes[i] = sr.ReadInt32();

            sr.Position = 0x200;
            uint raceParameterBufferSize = sr.ReadUInt32();
            sr.Position += 4;
            uint raceParameterOffset = sr.ReadUInt32();
            sr.Position += 4; // gameParameterOffset, provided again after anyway
            uint gameParameterBufferSize = sr.ReadUInt32();
            uint gameParameterOffset = sr.ReadUInt32();
            sr.Position += 12;
            uint carParameterOffset = sr.ReadUInt32();
            uint carParameterBufferSize = sr.ReadUInt32();
            sr.Position += 8;
            uint driverParameterOffset = sr.ReadUInt32();
            uint driverParameterBufferSize = sr.ReadUInt32();
            sr.Position += 4;
            rply.EntryMax = sr.ReadUInt32();
            uint entriesOffset = sr.ReadUInt32();
            rply.TotalFrameCount = sr.ReadUInt32();
            sr.Position += 4;
            uint frameDataInfoCount = sr.ReadUInt32();
            uint frameDataInfoOffset = sr.ReadUInt32();

            uint unkCount = sr.ReadUInt32();
            uint unkOffset = sr.ReadUInt32();

            uint unk = sr.ReadUInt32();
            uint replayChunksOffset = sr.ReadUInt32();

            rply.GeometryStreamSize = sr.ReadUInt32();
            rply.GeometryQualityLevel = sr.ReadUInt32();
            rply.FilesystemVersion = sr.ReadUInt64();
            sr.Position += 4;

            rply.EntryNum = sr.ReadUInt32();
            sr.Position += 2;
            ushort entryBufferSize = sr.ReadUInt16();
            sr.Position += 4;
            uint unkBits = sr.ReadUInt32();
            uint replayChunkCount = sr.ReadUInt32();
            sr.Position += 8;

            rply.ReplayEntries = new ReplayEntry[rply.EntryMax];
            for (int i = 0; i < rply.EntryMax; i++)
            {
                var entry = new ReplayEntry();
                sr.Position = (int)entriesOffset + (entryBufferSize * i);
                entry.ParseEntry(ref sr, (int)carParameterBufferSize);

                entry.FinishOrder = finishOrders[i];
                entry.TotalTime = TimeSpan.FromMilliseconds(totalTimes[i]);
                entry.BestTime = TimeSpan.FromMilliseconds(bestTimes[i]);
                rply.ReplayEntries[i] = entry;
            }

            return rply;
        }
    }

    public class ReplayEntry
    {
        public int EntryIndex { get; set; }
        public TimeSpan[] SectorTimes = new TimeSpan[16];
        public string EntryName { get; set; }

        public short FinishOrder;
        public TimeSpan TotalTime { get; set; }
        public TimeSpan BestTime { get; set; }

        public MCarParameter Car { get; set; }
        public MCarDriverParameter[] DriverParameter { get; set; }
        public void ParseEntry(ref SpanReader sr, int bufferSize)
        {
            EntryIndex = sr.ReadInt32();
            if (EntryIndex == -1)
                return;

            int carParameterOffset = sr.ReadInt32();
            sr.Position += 8;

            uint carDriverParameterCount = sr.ReadUInt32();
            uint carDriverParameterOffset = sr.ReadUInt32();
            sr.Position += 8;
            for (int i = 0; i < 16; i++)
            {
                var ms = sr.ReadUInt32();
                if (ms != 1209599999)
                    SectorTimes[i] = TimeSpan.FromMilliseconds(ms);
            }

            int basePos = sr.Position;
            EntryName = sr.ReadString0();
            sr.Position = basePos + 0x10;
            sr.Position += 4;

            sr.Position = carParameterOffset;
            Car = MCarParameter.ImportFromBlob(sr.Span.Slice(sr.Position, bufferSize));
            DriverParameter = new MCarDriverParameter[carDriverParameterCount];
            for (int i = 0; i < carDriverParameterCount; i++)
            {
                sr.Position = (int)carDriverParameterOffset + (i * 0x10);
                int paramOffset = sr.ReadInt32();
                sr.Position = paramOffset;
                DriverParameter[i] = MCarDriverParameter.Read(ref sr);
            }
        }
    }
}
