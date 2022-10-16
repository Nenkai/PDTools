using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PDTools.Utils;

namespace PDTools.Structures.PS3.MGameParameter
{
    public class EventAchieveCondition
    {
        public int Num { get; set; }
        public AchieveType Type { get; set; } = AchieveType.NONE;

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_A1_00);
            bs.WriteUInt32(1_00);
            bs.WriteInt32((int)Type);
            bs.WriteInt32(Num);
        }

        public void WriteToXml(XmlWriter xml)
        {
            if (Type == AchieveType.NONE)
                return;

            xml.WriteStartElement("achieve_condition");
            xml.WriteElementInt("num", Num);
            xml.WriteElementValue("type", Type.ToString());
            xml.WriteEndElement();
        }

        public void ReadFromCache(ref BitStream reader)
        {
            if (reader.ReadUInt32() != 0xE6_E6_A1_00)
                throw new Exception("Achieve Condition magic did not match expected one (E6 E6 A1 00)");

            uint version = reader.ReadUInt32();
            Type = (AchieveType)reader.ReadInt32();
            Num = reader.ReadInt32();
        }

        public void ParseAchieveCondition(XmlNode node)
        {
            foreach (XmlNode aNode in node.ChildNodes)
            {
                switch (aNode.Name)
                {
                    case "num":
                        Num = aNode.ReadValueInt(); break;

                    case "type":
                        Type = aNode.ReadValueEnum<AchieveType>(); break;
                }
            }
        }
    }

    public enum AchieveType
    {
        NONE,
        STOP,
        GOAL_V,
        TIME,
        ORDER,
        PYLON,
        SLIP_ANGLE,
        MORE_SPEED,
        MAX_GFORCE,
        OVERTAKE_NUM,
    }
}
