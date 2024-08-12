using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2;

/// <summary>
/// Relocates offsets in a file. Used by ModelSet2
/// </summary>
public class RelocatorBase
{
    public List<RelocatableOffset> OffsetsToRelocate = new List<RelocatableOffset>();

    public static RelocatorBase FromStream(BinaryStream bs, long mdlBasePos)
    {
        RelocatorBase info = new RelocatorBase();

        uint userChunksDataOffset = bs.ReadUInt32();
        uint magicMaybe = bs.ReadUInt32(); // Not read

        // This is a loop incase we have mixed offset types (never happens in MDLS)
        while (true)
        {
            RelocatePointerType relocateType = (RelocatePointerType)ReadCompressed7BitInt(bs);
            if (relocateType == RelocatePointerType.Update0)
                break;

            int symbolIndex = ReadCompressed7BitInt(bs);
            int totalNumberOfOffsetsToRelocate = ReadCompressed7BitInt(bs);
            uint startOffsetForGroup = (uint)ReadCompressed7BitInt(bs);

            uint currentOffset = startOffsetForGroup;
            info.OffsetsToRelocate.Add(new RelocatableOffset(relocateType, currentOffset));
            totalNumberOfOffsetsToRelocate--;
            // updateX(this, currentOffset, MDLSPointer)

            while (totalNumberOfOffsetsToRelocate > 0)
            {
                byte typeAndCount = bs.Read1Byte();
                if (typeAndCount >= 0x80) // Array, all in a row with specified stride to skip per element
                {
                    int count = typeAndCount - 0x7F;
                    totalNumberOfOffsetsToRelocate -= count;

                    uint skipPerElement = (uint)ReadCompressed7BitInt(bs);
                    for (var j = 0; j < count; j++)
                    {
                        // Array, all in a row
                        currentOffset += skipPerElement;
                        info.OffsetsToRelocate.Add(new RelocatableOffset(relocateType, currentOffset));

                        // updateX(this, currentOffset, MDLSPointer)
                    }
                }
                else // Array with provided offsets to skip for each element
                {
                    int count = typeAndCount + 1;
                    totalNumberOfOffsetsToRelocate -= count;

                    for (var j = 0; j < count; j++)
                    {
                        currentOffset += (uint)ReadCompressed7BitInt(bs); // Size to skip
                        info.OffsetsToRelocate.Add(new RelocatableOffset(relocateType, currentOffset));

                        // updateX(this, currentOffset, MDLSPointer)
                    }
                }
            }
        }

        return info;
    }

    public void WriteRelocationData(BinaryStream bs)
    {
        OffsetsToRelocate.Sort((left, right) => left.Offset.CompareTo(right.Offset));

        long baseRelocatorInfoOffset = bs.Position;

        bs.WriteUInt32(0); // User data offset, skip for now
        bs.WriteUInt32(0xE4859D6D); // Would be nice to figure this value out - as in, it's not read at all

        var groups = MakeRelocatableGroups();
        foreach (var group in groups)
        {
            WriteCompressed7BitInt(bs, (int)group.Type);
            WriteCompressed7BitInt(bs, (int)0);
            int numOffsetsToRelocateForThisGroup = 0;
            for (int i = 0; i < group.OffsetGroups.Count; i++)
                numOffsetsToRelocateForThisGroup += group.OffsetGroups[i].Offsets.Count;

            WriteCompressed7BitInt(bs, numOffsetsToRelocateForThisGroup + 1);
            WriteCompressed7BitInt(bs, (int)group.Start.Offset);

            uint currentOffset = group.Start.Offset;

            for (int i = 0; i < group.OffsetGroups.Count; i++)
            {
                RelocationOffsetGroup offsetGroup = group.OffsetGroups[i];
                if (offsetGroup.Method == RelocationMethod.UpdateWithFixedDistancePerOffset)
                {
                    byte bits = (byte)((offsetGroup.Offsets.Count - 1) & 0b1111111);
                    bs.WriteByte(bits);

                    for (int j = 0; j < offsetGroup.Offsets.Count; j++)
                    {
                        uint next = offsetGroup.Offsets[j].Offset;

                        // Write distance
                        WriteCompressed7BitInt(bs, (int)(next - currentOffset));
                        currentOffset = next;
                    }
                }
                else
                    throw new NotImplementedException("Implement this");
            }
        }
        bs.WriteByte(0); // Terminator

        long userChunksDataOffset = bs.Position;
        bs.WriteUInt32(0); // Offset
        bs.WriteUInt32(0); // Has data?

        // offset point to stride of 0x08, which is processed by RelocatorBase::readUserChunks
        // never seen any model use it (so far)
        // there's no code for it either

        long lastOffset = bs.Position;
        bs.Position = baseRelocatorInfoOffset;
        bs.WriteUInt32((uint)(userChunksDataOffset - baseRelocatorInfoOffset));

        bs.Position = lastOffset;
    }

    // Could be improved by using fixed distance arrays
    public List<RelocationTypeGroup> MakeRelocatableGroups()
    {
        List<RelocationTypeGroup> typeGroups = new List<RelocationTypeGroup>();

        RelocationTypeGroup currentTypeGroup = null;
        RelocationOffsetGroup offsetGroup = null;

        int typeGroupStartIndex = 0;
        while (typeGroupStartIndex < OffsetsToRelocate.Count)
        {
            if (typeGroupStartIndex == 0)
            {
                currentTypeGroup = new RelocationTypeGroup(OffsetsToRelocate[typeGroupStartIndex].Type);
                currentTypeGroup.Start = OffsetsToRelocate[typeGroupStartIndex];

                offsetGroup = new RelocationOffsetGroup();
                offsetGroup.Method = RelocationMethod.UpdateWithFixedDistancePerOffset;
                typeGroupStartIndex++;
            }

            int i;
            for (i = typeGroupStartIndex; i < OffsetsToRelocate.Count; i++)
            {
                if (offsetGroup.Offsets.Count >= 0x7F)
                {
                    currentTypeGroup.OffsetGroups.Add(offsetGroup);
                    offsetGroup = new RelocationOffsetGroup();
                }

                offsetGroup.Offsets.Add(OffsetsToRelocate[i]);
            }
            typeGroupStartIndex += (i - typeGroupStartIndex);

            if (typeGroupStartIndex >= OffsetsToRelocate.Count)
            {
                typeGroups.Add(currentTypeGroup);
                currentTypeGroup.OffsetGroups.Add(offsetGroup);
            }
        }

        return typeGroups;
    }

    public void Add(RelocatePointerType type, uint offset)
    {
        if (OffsetsToRelocate.FindIndex(e => e.Type == type && e.Offset == offset) != -1)
            return;

        OffsetsToRelocate.Add(new RelocatableOffset(type, offset));
    }

    private static int ReadCompressed7BitInt(BinaryStream bs)
    {
        byte firstVal = (byte)bs.ReadByte();
        if (firstVal >> 7 != 0)
        {
            if ((firstVal & 0x40) != 0)
                return (firstVal & 0x3F) << 24 | bs.Read1Byte() << 16 | bs.Read1Byte() << 8 | bs.Read1Byte();
            else
                return (firstVal & 0x7F) << 8 | bs.Read1Byte();
        }
        else
            return firstVal;
    }

    // ...thanks ChatGPT
    private static void WriteCompressed7BitInt(BinaryStream bs, int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be non-negative.");

        if (value < 0x80)
        {
            // Single-byte encoding for values less than 128
            bs.Write((byte)value);
        }
        else if (value < 0x4000)
        {
            // Two-byte encoding for values less than 16384
            bs.Write((byte)((value >> 8) | 0x80));
            bs.Write((byte)(value & 0xFF));
        }
        else
        {
            // Four-byte encoding for other values
            bs.Write((byte)((value >> 24) | 0xC0));
            bs.Write((byte)((value >> 16) & 0xFF));
            bs.Write((byte)((value >> 8) & 0xFF));
            bs.Write((byte)(value & 0xFF));
        }
    }
}

public class RelocationOffsetGroup
{
    public List<RelocatableOffset> Offsets = new List<RelocatableOffset>();
    public RelocationMethod Method { get; set; }
}

public class RelocationTypeGroup
{
    public RelocatePointerType Type { get; set; }
    public RelocatableOffset Start { get; set; }
    public List<RelocationOffsetGroup> OffsetGroups { get; set; } = new List<RelocationOffsetGroup>();

    public RelocationTypeGroup(RelocatePointerType type)
    {
        Type = type;
    }
}

public class RelocatableOffset
{
    public RelocatePointerType Type { get; set; }
    public uint Offset { get; set; }

    public RelocatableOffset(RelocatePointerType type, uint offset)
    {
        Type = type;
        Offset = offset;
    }

    public override string ToString()
    {
        return $"0x{Offset:X8} ({Type})";
    }
}

/// <summary>
/// Represents the size of a pointer for relocation
/// </summary>
public enum RelocatePointerType : byte
{
    /// <summary>
    /// No update.
    /// </summary>
    Update0 = 0,

    /// <summary>
    /// Update a byte as a pointer
    /// </summary>
    Update1 = 1,

    /// <summary>
    /// Update a short as a pointer
    /// </summary>
    Update2 = 2,

    /// <summary>
    /// Update an int as a pointer
    /// </summary>
    Update4 = 4,
}

public enum RelocationMethod
{
    UpdateWithFixedDistancePerOffset,
    UpdateWithDistanceForNextOffsets,
}
