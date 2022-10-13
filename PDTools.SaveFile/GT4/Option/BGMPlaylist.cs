using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.Option
{
    public class BGMPlaylist : IGameSerializeBase
    {
        public const int MAX_TRACKS = 128;

        public int NumActiveEntries;
        public ushort Unk;
        public ushort Unk2;
        public ushort ShuffleRandom;
        public ushort Unk3;
        public uint Unk4_MaybeUnused { get; set; }

        public BGMPlayData[] Tracks { get; set; } = new BGMPlayData[MAX_TRACKS];

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteInt32(NumActiveEntries);
            sw.WriteUInt16(Unk);
            sw.WriteUInt16(Unk2);
            sw.WriteUInt16(ShuffleRandom);
            sw.WriteUInt16(Unk3);
            sw.WriteUInt32(Unk4_MaybeUnused);

            for (var i = 0; i < MAX_TRACKS; i++)
                Tracks[i].Pack(save, ref sw);
            
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            NumActiveEntries = sr.ReadInt32();
            Unk = sr.ReadUInt16();
            Unk2 = sr.ReadUInt16();
            ShuffleRandom = sr.ReadUInt16();
            Unk3 = sr.ReadUInt16();
            Unk4_MaybeUnused = sr.ReadUInt32();

            for (var i = 0; i < MAX_TRACKS; i++)
            {
                Tracks[i] = new BGMPlayData();
                Tracks[i].Unpack(save, ref sr);
            }
        }

        public ulong stringsToUniqueID(string str)
        {
            ulong hash = 0;

            for (var i = 0; i < str.Length; i++)
                hash += (byte)str[i];

            hash &= 0xFFFF;

            for (var i = 0; i < str.Length; i++)
                hash = (hash << 7 | hash >> 57) + (byte)str[i];

            return hash;
        }

    }
}
