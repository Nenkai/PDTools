using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PDTools.Utils;

using PDTools.Enums;

namespace PDTools.Structures.MGameParameter;

public class AchieveCondition
{
    /// <summary>
    /// Defaults to 0.
    /// </summary>
    public int Num { get; set; } = 0;

    /// <summary>
    /// Defaults to <see cref="AchieveType.NONE"/>.
    /// </summary>
    public AchieveType Type { get; set; } = AchieveType.NONE;

    public bool IsDefault()
    {
        var defaultAchieveCondition = new AchieveCondition();
        return Num == defaultAchieveCondition.Num &&
            Type == defaultAchieveCondition.Type;
    }

    public void CopyTo(AchieveCondition other)
    {
        other.Num = Num;
        other.Type = Type;
    }

    public void Serialize(ref BitStream bs)
    {
        bs.WriteUInt32(0xE6_E6_A1_00);
        bs.WriteUInt32(1_00);
        bs.WriteInt32((int)Type);
        bs.WriteInt32(Num);
    }

    public void WriteToXml(XmlWriter xml)
    {
        xml.WriteElementInt("num", Num);
        xml.WriteElementValue("type", Type.ToString());
    }

    public void Deserialize(ref BitStream reader)
    {
        if (reader.ReadUInt32() != 0xE6_E6_A1_00)
            throw new Exception("Achieve Condition magic did not match expected one (E6 E6 A1 00)");

        uint version = reader.ReadUInt32();
        Type = (AchieveType)reader.ReadInt32();
        Num = reader.ReadInt32();
    }

    public void ParseFromXml(XmlNode node)
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
