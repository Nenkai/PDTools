using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.UserProfile
{
    public class GarageScratch : IGameSerializeBase
    {
        public const int MAX_CARS = 1000;

        public GarageCar[] Cars { get; set; } = new GarageCar[MAX_CARS];
        public int RidingCarIndex { get; set; }
        public uint UniqueID { get; set; }
        public byte[] Unk { get; set; } = new byte[0x14];
        public CarGarage CurrentCar { get; set; } = new CarGarage();

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            for (var i = 0; i < MAX_CARS; i++)
                Cars[i].Pack(save, ref sw);

            sw.WriteInt32(RidingCarIndex);
            sw.WriteUInt32(UniqueID);
            sw.WriteBytes(Unk);
            CurrentCar.Pack(save, ref sw);

            sw.Align(GT4Save.ALIGNMENT);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            for (var i = 0; i < MAX_CARS; i++)
            {
                Cars[i] = new GarageCar();
                Cars[i].Unpack(save, ref sr);
            }

            RidingCarIndex = sr.ReadInt32();
            UniqueID = sr.ReadUInt32();
            Unk = sr.ReadBytes(0x14);
            CurrentCar.Unpack(save, ref sr);

            sr.Align(GT4Save.ALIGNMENT);
        }
    }
}
