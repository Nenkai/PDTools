using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

using PDTools.Enums.PS2;

namespace PDTools.Structures.PS2
{
    public class CarGarage
    {
        public const int MAX_SHEETS = 3;
        public CarEquipments[] Sheets { get; set; } = new CarEquipments[MAX_SHEETS];
        public byte[] UnkData { get; set; }
        public int Unk { get; set; }
        public byte[] Unk2 { get; set; }
        public AutomobileAccumulatedStatus Status { get; set; } = new AutomobileAccumulatedStatus();

        public void Unpack(ref SpanReader sr)
        {
            for (var i = 0; i < MAX_SHEETS; i++)
            {
                Sheets[i] = new CarEquipments();
                Sheets[i].Unpack(ref sr);
            }

            UnkData = sr.ReadBytes(0x20);
            Unk = sr.ReadInt32();
            Unk2 = sr.ReadBytes(0x0C);
            Status.Unpack(ref sr);
        }

        public void Pack(ref SpanWriter sw)
        {
            for (var i = 0; i < MAX_SHEETS; i++)
                Sheets[i].Pack(ref sw);

            sw.WriteBytes(UnkData);
            sw.WriteInt32(Unk);
            sw.WriteBytes(Unk2);
            Status.Pack(ref sw);
        }

        public int partsTypeToTrunkTopNum(PartsTypeGT4 partsType)
        {
            switch (partsType)
            {
                case PartsTypeGT4.GENERIC_CAR:
                case PartsTypeGT4.CHASSIS:
                case PartsTypeGT4.RACINGMODIFY:
                case PartsTypeGT4.STEER:
                case PartsTypeGT4.ENGINE:
                case PartsTypeGT4.WHEEL:
                case PartsTypeGT4.WING:
                    return -1;
                case PartsTypeGT4.BRAKE:
                    return 0;
                case PartsTypeGT4.BRAKECONTROLLER:
                    return 2;
                case PartsTypeGT4.SUSPENSION:
                    return 4;
                case PartsTypeGT4.ASCC:
                    return 9;
                case PartsTypeGT4.TCSC:
                    return 11;
                case PartsTypeGT4.LIGHTWEIGHT:
                    return 13;
                case PartsTypeGT4.DRIVETRAIN:
                    return 17;
                case PartsTypeGT4.GEAR:
                    return 19;
                case PartsTypeGT4.NATUNE:
                    return 23;
                case PartsTypeGT4.TURBINEKIT:
                    return 27;
                case PartsTypeGT4.PORTPOLISH:
                    return 33;
                case PartsTypeGT4.ENGINEBALANCE:
                    return 35;
                case PartsTypeGT4.DISPLACEMENT:
                    return 37;
                case PartsTypeGT4.COMPUTER:
                    return 39;
                case PartsTypeGT4.INTERCOOLER:
                    return 41;
                case PartsTypeGT4.MUFFLER:
                    return 44;
                case PartsTypeGT4.CLUTCH:
                    return 48;
                case PartsTypeGT4.FLYWHEEL:
                    return 52;
                case PartsTypeGT4.PROPELLERSHAFT:
                    return 56;
                case PartsTypeGT4.LSD:
                    return 58;
                case PartsTypeGT4.FRONTTIRE:
                    return 64;
                case PartsTypeGT4.REARTIRE:
                    return 77;
                case PartsTypeGT4.NOS:
                    return 90;
                case PartsTypeGT4.SUPERCHARGER:
                    return 92;
                case PartsTypeGT4.TIRESIZE:
                    return 94;

                default:
                    return -1;
            }
        }
    }
}
