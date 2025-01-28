using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Structures;

public struct MatchingEventId
{
    private ulong _id;

    public MatchingEventId(ulong id)
        => _id = id;

    public readonly ulong GetRawId()
        => _id;

    public void SetRawId(ulong id)
        => _id = id;

    public readonly uint GetEventId()
        => (uint)((_id >> 32) & 0b_11111111_11111111_11111111);

    public void SetEventId(uint eventId)
        => _id |= (eventId << 32);

    public readonly uint GetEventId32()
        => (uint)((_id >> 32) & 0b_11111111_11111111_11111111_11111111);

    public readonly byte GetSpaceBit()
        => (byte)((_id >> 59) & 0b1);

    public void SetSpaceBit(byte space)
    {
        if (space > 1)
            space = 1;
        _id |= ((ulong)space << 59);
    }

    public readonly int GetRegion()
        => (int)((_id >> 56) & 0b111);

    public void SetRegion(byte region)
    {
        if (region > 7)
            region = 7;

        _id |= ((ulong)region << 56);
    }

    public readonly uint GetRoomVersion()
        => (uint)(_id & 0b_11111111_11111111_11111111_11111111);

    public void SetRoomVersion(uint version)
        => _id |= version;

    public readonly uint GetRoomSpecialVersion()
    {
        uint versionRaw = GetRoomVersion();
        return ((versionRaw >> 21) & 0b11111111111);
    }

    public readonly uint GetRoomApplicationVersion()
    {
        uint versionRaw = GetRoomVersion();
        return ((versionRaw >> 10) & 0b111111);
    }

    public readonly uint GetRoomServerVersion()
    {
        uint versionRaw = GetRoomVersion();
        return ((versionRaw >> 16) & 0b11111); // Should be 2
    }

    public readonly uint GetRoomElfVersion()
    {
        uint versionRaw = GetRoomVersion();
        return versionRaw & 0b1111111111; // Should be 58
    }
}
