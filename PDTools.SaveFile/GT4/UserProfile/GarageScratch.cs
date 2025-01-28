using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

using PDTools.Structures.PS2;

namespace PDTools.SaveFile.GT4.UserProfile;

public class GarageScratch : IGameSerializeBase<GarageScratch>
{
    public const int MAX_CARS = 1000;

    public GarageScratchUnit[] Cars { get; set; } = new GarageScratchUnit[MAX_CARS];
    public int RidingCarIndex { get; set; }
    public uint UniqueID { get; set; }
    public byte[] Unk { get; set; } = new byte[0x14];
    public CarGarage CurrentCar { get; set; } = new CarGarage();

    public void CopyTo(GarageScratch dest)
    {
        for (var i = 0; i < Cars.Length; i++)
        {
            dest.Cars[i] = new GarageScratchUnit();
            Cars[i].CopyTo(dest.Cars[i]);
        }

        dest.RidingCarIndex = RidingCarIndex;
        dest.UniqueID = UniqueID;
        Array.Copy(Unk, dest.Unk, Unk.Length);
        CurrentCar.CopyTo(dest.CurrentCar);
    }

    public void Pack(GT4Save save, ref SpanWriter sw)
    {
        for (var i = 0; i < MAX_CARS; i++)
            Cars[i].Pack(save, ref sw);

        sw.WriteInt32(RidingCarIndex);
        sw.WriteUInt32(UniqueID);
        sw.WriteBytes(Unk);
        CurrentCar.Pack(ref sw, GT4Save.IsGT4Online(save.Type));

        sw.Align(GT4Save.ALIGNMENT);
    }

    public void Unpack(GT4Save save, ref SpanReader sr)
    {
        for (var i = 0; i < MAX_CARS; i++)
        {
            Cars[i] = new GarageScratchUnit();
            Cars[i].Unpack(save, ref sr);
        }

        RidingCarIndex = sr.ReadInt32();
        UniqueID = sr.ReadUInt32();
        Unk = sr.ReadBytes(0x14);
        CurrentCar.Unpack(ref sr, GT4Save.IsGT4Online(save.Type));

        sr.Align(GT4Save.ALIGNMENT);
    }

    /// <summary>
    /// Clears all the cars.
    /// </summary>
    public void Clear()
    {
        for (var i = 0; i < Cars.Length; i++)
            Cars[i].IsSlotTaken = false;
    }

    /// <summary>
    /// Clears all the cars's data.
    /// </summary>
    public void ClearAllCarData()
    {
        for (var i = 0; i < Cars.Length; i++)
        {
            Cars[i].GarageDataExists = false;
        }
    }

    public bool IsFull()
    {
        for (var i = 0; i < MAX_CARS; i++)
        {
            if (!Cars[i].IsSlotTaken)
                return false;
        }

        return true;
    }

    public int GetCarCount()
    {
        int count = 0;
        for (var i = 0; i < MAX_CARS; i++)
        {
            if (Cars[i].IsSlotTaken)
                count++;
        }

        return count;
    }

    public int GetFirstUnusedSlotIndex()
    {
        for (var i = 0; i < MAX_CARS; i++)
        {
            if (!Cars[i].IsSlotTaken)
                return i;
        }

        return -1;
    }

    public bool HasCarCode(int code)
    {
        for (var i = 0; i < MAX_CARS; i++)
        {
            if (Cars[i].IsSlotTaken && Cars[i].CarCode.Code == code)
                return true;
        }

        return false;
    }
}
