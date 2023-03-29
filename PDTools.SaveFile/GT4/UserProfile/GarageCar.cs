using System;
using System.Collections.Generic;
using System.Text;

using PDTools.Structures;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.UserProfile
{
    public class GarageScratchUnit : IGameSerializeBase<GarageScratchUnit>
    {
        public DbCode CarCode { get; set; }
        public ulong Flags { get; set; }
        public int Unk { get; set; }
        public int Odometer { get; set; }
        public long Flags2 { get; set; }

        private const ulong FreeSlotMask = unchecked((ulong)~0b1); // 1st bit
        public bool IsSlotTaken
        {
            get => (Flags & 1) != 0;
            set => Flags = (Flags & FreeSlotMask) | (value ? 1ul : 0ul);
        }

        private const ulong GarageDataExistsMask = unchecked((ulong)~0b10); // 2nd bit
        public bool GarageDataExists
        {
            get => ((Flags >> 1) & 1) != 0;
            set => Flags = (Flags & GarageDataExistsMask) | ((value ? 1ul : 0ul) << 1);
        }

        private const ulong VariationIndexMask = unchecked(~0b1_11111100ul); // 3 to 9, 7 bits
        public uint VariationIndex
        {
            get => (uint)((Flags >> 2) & 0b1111111ul);
            set => Flags = (Flags & VariationIndexMask) | ((value & 0b1111111ul) << 2);
        }

        private const ulong ShowroomPowerMask = unchecked(~0b1111_11111111_11111110_00000000ul); // 10 to 27, 18 bits
        public uint ShowroomPower
        {
            get => (uint)((Flags >> 9) & 0b11_11111111_11111111ul); // 18 bits
            set => Flags = (Flags & ShowroomPowerMask) | ((value & 0b11_11111111_11111111ul) << 9);
        }

        public uint ShowroomPowerPS
        {
            get => ShowroomPower / 100;
        }

        private const ulong ShowroomWeightMask = unchecked(~0b111_11111111_00000000_00000000_00000000_00000000ul); // 32 to 43, 11 bits
        public uint ShowroomWeight // 28 to 39, 11 bits
        {
            get => (uint)((Flags >> 32) & 0b111_11111111ul);
            set => Flags = (Flags & ShowroomWeightMask) | ((value & 0b111_11111111ul) << 32);
        }

        public uint RideHistory
        {
            get => (uint)(Flags2 & 0b11_11111111); // 10 bits
        }

        public void CopyTo(GarageScratchUnit dest)
        {
            dest.CarCode = new DbCode(CarCode.Code, CarCode.TableId);
            dest.Flags = Flags;
            dest.Unk = Unk;
            dest.Odometer = Odometer;
            dest.Flags2 = Flags2;
        }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteInt32(CarCode.Code);
            sw.WriteInt32(CarCode.TableId);
            sw.WriteUInt64(Flags);
            sw.WriteInt32(Unk);
            sw.WriteInt32(Odometer);
            sw.WriteInt64(Flags2);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            CarCode = new DbCode(sr.ReadInt32(), sr.ReadInt32());
            Flags = sr.ReadUInt64();
            Unk = sr.ReadInt32();
            Odometer = sr.ReadInt32();
            Flags2 = sr.ReadInt64();
        }

    }
}
