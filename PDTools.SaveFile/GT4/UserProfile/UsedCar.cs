using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.UserProfile
{
    public class UsedCar : IGameSerializeBase<UsedCar>
    {
        public const int _80sCars_StartID = 0;
        public const int Early90sCars_StartID = 80;
        public const int Late90sCars_StartID = 160;

        public byte[] Bits { get; set; } = new byte[0x20];
        public int Week { get; set; }

        public void CopyTo(UsedCar dest)
        {
            Array.Copy(Bits, dest.Bits, Bits.Length);
            dest.Week = Week;
        }

        public void SetUsedCarStatus(int idx, bool soldout)
        {
            if (soldout)
                Bits[idx / 8] |= (byte)(1 << (idx % 8));
            else
                Bits[idx / 8] &= (byte)~(1 << (idx % 8));
        }

        public bool IsCarSoldout(int idx)
        {
            return ((Bits[idx / 8] >> (idx % 8)) & 1) == 1;
        }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteBytes(Bits);
            sw.WriteInt32(Week);

            sw.Align(GT4Save.ALIGNMENT);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            Bits = sr.ReadBytes(0x20);
            Week = sr.ReadInt32();

            sr.Align(GT4Save.ALIGNMENT);
        }
    }
}
