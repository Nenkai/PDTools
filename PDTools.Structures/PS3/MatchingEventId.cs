
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Structures.PS3
{
    public struct MatchingEventId
    {
        private ulong _id;

        public MatchingEventId(ulong id)
            => _id = id;

        public ulong GetRawId()
            => _id;

        public void SetRawId(ulong id)
            => _id = id;

        public uint GetEventId()
            => (uint)(_id >> 32 & 0b_11111111_11111111_11111111);

        public void SetEventId(uint eventId)
            => _id |= eventId << 32;

        public uint GetEventId32()
            => (uint)(_id >> 32 & 0b_11111111_11111111_11111111_11111111);

        public byte GetSpaceBit()
            => (byte)(_id >> 59 & 0b1);

        public void SetSpaceBit(byte space)
        {
            if (space > 1)
                space = 1;
            _id |= (ulong)space << 59;
        }

        public int GetRegion()
            => (int)(_id >> 56 & 0b111);

        public void SetRegion(byte region)
        {
            if (region > 7)
                region = 7;

            _id |= (ulong)region << 56;
        }

        public uint GetRoomVersion()
            => (uint)(_id & 0b_11111111_11111111_11111111_11111111);

        public void SetRoomVersion(uint version)
            => _id |= version;

        public uint GetRoomSpecialVersion()
        {
            uint versionRaw = GetRoomVersion();
            return versionRaw >> 21 & 0b11111111111;
        }

        public uint GetRoomApplicationVersion()
        {
            uint versionRaw = GetRoomVersion();
            return versionRaw >> 10 & 0b111111;
        }

        public uint GetRoomServerVersion()
        {
            uint versionRaw = GetRoomVersion();
            return versionRaw >> 16 & 0b11111; // Should be 2
        }

        public uint GetRoomElfVersion()
        {
            uint versionRaw = GetRoomVersion();
            return versionRaw & 0b1111111111; // Should be 58
        }
    }
}
