using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using Syroot.BinaryData.Memory;

using PDTools.Utils;
using PDTools.Structures.MGameParameter;

namespace PDTools.Structures.PS3
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

        public short[] FinishOrders { get; set; }
        public int[] TotalTimes { get; set; }
        public int[] BestTimes { get; set; }

        public ReplayEntry[] ReplayEntries;

        public byte[] GameParameterBuffer { get; set; }
        public GameParameter GameParameter { get; set; } = new GameParameter();

        public static MReplayInfo LoadFromFile(string fileName)
        {
            byte[] file = File.ReadAllBytes(fileName);
            BitStream bs = new BitStream(BitStreamMode.Read, file);

            string magic = bs.ReadStringRaw(4);
            bs.Position += 4;
            uint replayVersion = bs.ReadUInt32();
            bs.Position += 20;

            var rply = new MReplayInfo();
            rply.GTBehaviorVersion = bs.ReadUInt32();
            rply.SpecDBVersion = bs.ReadUInt32();
            rply.RecordedDateTime = PDIDATETIME.JulianToDateTime_64(bs.ReadUInt64());
            rply.SpecDBName = bs.ReadStringRaw(0x30).TrimEnd('\0');

            bs.Position = 0x70;
            rply.Unk = bs.ReadUInt32();
            rply.RaceCompleted = bs.ReadBool4();
            rply.OneLap = bs.ReadBool4();
            rply.Score = bs.ReadUInt32();

            rply.FinishOrders = new short[32];
            for (int i = 0; i < 32; i++)
                rply.FinishOrders[i] = bs.ReadInt16();

            rply.TotalTimes = new int[32];
            for (int i = 0; i < 32; i++)
                rply.TotalTimes[i] = bs.ReadInt32();

            rply.BestTimes = new int[32];
            for (int i = 0; i < 32; i++)
                rply.BestTimes[i] = bs.ReadInt32();

            bs.Position = 0x200;
            uint raceParameterBufferSize = bs.ReadUInt32();
            bs.Position += 4;
            uint raceParameterOffset = bs.ReadUInt32();
            bs.Position += 4; // gameParameterOffset, provided again after anyway
            int gameParameterBufferSize = bs.ReadInt32();
            int gameParameterOffset = bs.ReadInt32();
            bs.Position += 12; // Empty
            uint carParameterOffset = bs.ReadUInt32();
            uint carParameterBufferSize = bs.ReadUInt32();
            bs.Position += 8;
            uint driverParameterOffset = bs.ReadUInt32();
            uint driverParameterBufferSize = bs.ReadUInt32();
            bs.Position += 4;
            rply.EntryMax = bs.ReadUInt32();
            uint entriesOffset = bs.ReadUInt32();
            rply.TotalFrameCount = bs.ReadUInt32();
            bs.Position += 4;
            uint frameDataInfoCount = bs.ReadUInt32();
            uint frameDataInfoOffset = bs.ReadUInt32();

            uint unkCount = bs.ReadUInt32();
            uint unkOffset = bs.ReadUInt32();

            uint unk = bs.ReadUInt32();
            uint replayChunksOffset = bs.ReadUInt32();

            rply.GeometryStreamSize = bs.ReadUInt32();
            rply.GeometryQualityLevel = bs.ReadUInt32();
            rply.FilesystemVersion = bs.ReadUInt64();
            bs.Position += 4;

            rply.EntryNum = bs.ReadUInt32();
            bs.Position += 2;
            ushort entryBufferSize = bs.ReadUInt16();
            bs.Position += 4;
            uint unkBits = bs.ReadUInt32();
            uint replayChunkCount = bs.ReadUInt32();
            bs.Position += 8;

            rply.ReplayEntries = new ReplayEntry[rply.EntryMax];
            for (int i = 0; i < rply.EntryMax; i++)
            {
                var entry = new ReplayEntry();
                bs.Position = (int)entriesOffset + entryBufferSize * i;
                entry.ParseEntry(ref bs, (int)carParameterBufferSize);

                entry.FinishOrder = rply.FinishOrders[i];
                entry.TotalTime = TimeSpan.FromMilliseconds(rply.TotalTimes[i]);
                entry.BestTime = TimeSpan.FromMilliseconds(rply.BestTimes[i]);
                rply.ReplayEntries[i] = entry;
            }

            bs.Position = gameParameterOffset;
            rply.GameParameterBuffer = bs.GetSpan().Slice(0, gameParameterBufferSize).ToArray();
            rply.GameParameter.ReadFromBuffer(ref bs);
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
        public void ParseEntry(ref BitStream sr, int bufferSize)
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
            EntryName = sr.ReadNullTerminatedString();
            sr.Position = basePos + 0x10;
            sr.Position += 4;

            sr.Position = carParameterOffset;

            DriverParameter = new MCarDriverParameter[carDriverParameterCount];
            byte[] carBlob = new byte[bufferSize];
            sr.ReadIntoByteArray(bufferSize, carBlob, 8);
            Car = MCarParameter.ImportFromBlob(carBlob);

            for (int i = 0; i < carDriverParameterCount; i++)
            {
                sr.Position = (int)carDriverParameterOffset + i * 0x10;
                int paramOffset = sr.ReadInt32();
                sr.Position = paramOffset;
                DriverParameter[i] = MCarDriverParameter.Read(ref sr);
            }
        }
    }
}
