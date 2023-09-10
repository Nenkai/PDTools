using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PDTools.Enums;
using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    public class StageResetData
    {
        public string Code { get; set; }
        public StageCoordType Coord { get; set; }
        public sbyte TargetID { get; set; }
        public sbyte ResourceID { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float RotYDeg { get; set; }
        public float VCoord { get; set; }

        public void CopyTo(StageResetData other)
        {
            other.Code = Code;
            other.Coord = Coord;
            other.TargetID = TargetID;
            other.ResourceID = ResourceID;
            other.X = X;
            other.Y = Y;
            other.Z = Z;
            other.RotYDeg = RotYDeg;
            other.VCoord = VCoord;
        }

        public void ParseFromXml(XmlNode stageDataResetNode)
        {
            foreach (XmlNode node in stageDataResetNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "code":
                        Code = node.ReadValueString(); break;
                    case "coord":
                        if (node.ReadValueString() == "NONE") // gt6 r180, kinda weird
                            break;

                        Coord = node.ReadValueEnum<StageCoordType>(); break;
                    case "x":
                        X = node.ReadValueSingle(); break;
                    case "y":
                        Y = node.ReadValueSingle(); break;
                    case "z":
                        Z = node.ReadValueSingle(); break;
                    case "rotydeg":
                        Z = node.ReadValueSingle(); break;
                    case "vcoord":
                        VCoord = node.ReadValueSingle(); break;
                }
            }
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteElementValue("code", Code);
            xml.WriteElementValue("coord", Coord.ToString());
            xml.WriteElementFloat("x", X);
            xml.WriteElementFloat("y", X);
            xml.WriteElementFloat("z", X);
            xml.WriteElementFloat("rotydeg", X);
            xml.WriteElementFloat("vcoord", X);
        }

        public void Serialize(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_0D_DD);
            bs.WriteUInt32(1_00);
            bs.WriteNullStringAligned4(Code);
            bs.WriteSByte((sbyte)Coord);
            bs.WriteSByte(TargetID);
            bs.WriteSByte(ResourceID);
            bs.WriteSByte(0); // Unk field_0x1f
            bs.WriteSingle(X);
            bs.WriteSingle(Y);
            bs.WriteSingle(Z);
            bs.WriteSingle(RotYDeg);
            bs.WriteSingle(VCoord);
        }
    }
}
