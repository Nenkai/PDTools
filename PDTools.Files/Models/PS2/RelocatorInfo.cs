using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS2
{
    /// <summary>
    /// Relocates offsets in a file. Used by ModelSet2
    /// </summary>
    public class RelocatorBase
    {
        public List<uint> OffsetsToRelocate = new List<uint>();

        public static RelocatorBase FromStream(BinaryStream bs, long mdlBasePos)
        {
            RelocatorBase info = new RelocatorBase();

            uint symbolsOffset = bs.ReadUInt32();
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
                info.OffsetsToRelocate.Add(currentOffset);
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
                            info.OffsetsToRelocate.Add(currentOffset);

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
                            info.OffsetsToRelocate.Add(currentOffset);

                            // updateX(this, currentOffset, MDLSPointer)
                        }
                    }
                }

            }

            return info;
        }

        public static int ReadCompressed7BitInt(BinaryStream bs)
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
}
