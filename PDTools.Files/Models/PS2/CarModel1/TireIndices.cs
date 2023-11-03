using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.CarModel1
{
    public class TireIndices
    {
        public TireCompoundFileIndices FrontTires { get; set; } = new();
        public TireCompoundFileIndices RearTires { get; set; } = new();

        public void FromStream(BinaryStream bs)
        {
            FrontTires.FromStream(bs);
            RearTires.FromStream(bs);
        }

        public void Write(BinaryStream bs)
        {
            FrontTires.Write(bs);
            RearTires.Write(bs);
        }
    }

    public class TireCompoundFileIndices
    {
        public uint NormalTire { get; set; }
        public uint SportsTire { get; set; }
        public uint RacingHardTire { get; set; }
        public uint RacingMediumTire { get; set; }
        public uint RacingSoftTire { get; set; }
        public uint RacingSuperSoftTire { get; set; }
        public uint Control_SimulationTire { get; set; }
        public uint DirtTire { get; set; }

        public void FromStream(BinaryStream bs)
        {
            NormalTire = bs.ReadUInt32();
            SportsTire = bs.ReadUInt32();
            RacingHardTire = bs.ReadUInt32();
            RacingMediumTire = bs.ReadUInt32();
            RacingSoftTire = bs.ReadUInt32();
            RacingSuperSoftTire = bs.ReadUInt32();
            Control_SimulationTire = bs.ReadUInt32();
            DirtTire = bs.ReadUInt32();
        }

        public void Write(BinaryStream bs)
        {
            bs.WriteUInt32(NormalTire);
            bs.WriteUInt32(SportsTire);
            bs.WriteUInt32(RacingHardTire);
            bs.WriteUInt32(RacingMediumTire);
            bs.WriteUInt32(RacingSoftTire);
            bs.WriteUInt32(RacingSuperSoftTire);
            bs.WriteUInt32(Control_SimulationTire);
            bs.WriteUInt32(DirtTire);
        }
    }
}
