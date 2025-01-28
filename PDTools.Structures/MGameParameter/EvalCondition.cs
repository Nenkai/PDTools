using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using System.ComponentModel;
using PDTools.Utils;

using PDTools.Enums;

namespace PDTools.Structures.MGameParameter;

public class EvalCondition
{
    /// <summary>
    /// Defaults to <see cref="EvalConditionType.NONE"/>.
    /// </summary>
    public EvalConditionType ConditionType { get; set; } = EvalConditionType.NONE;
    public int Gold { get; set; } = 0;
    public int Silver { get; set; } = 0;
    public int Bronze { get; set; } = 0;
    public string GhostDataPath { get; set; }

    public bool IsDefault()
    {
        var defaultEvalCondition = new EvalCondition();
        return ConditionType == defaultEvalCondition.ConditionType &&
            Gold == defaultEvalCondition.Gold &&
            Silver == defaultEvalCondition.Silver &&
            Bronze == defaultEvalCondition.Bronze &&
            GhostDataPath == defaultEvalCondition.GhostDataPath;
    }

    public void CopyTo(EvalCondition other)
    {
        other.ConditionType = ConditionType;
        other.Gold = Gold;
        other.Silver = Silver;
        other.Bronze = Bronze;
        other.GhostDataPath = GhostDataPath;
    }

    public void WriteToXml(XmlWriter xml)
    {
        xml.WriteElementValue("type", ConditionType.ToString());
        xml.WriteElementInt("gold", Gold);
        xml.WriteElementInt("silver", Silver);
        xml.WriteElementInt("bronze", Bronze);

        if (!string.IsNullOrEmpty(GhostDataPath))
            xml.WriteElementValue("ghost_data_path", GhostDataPath);
    }

    public void Deserialize(ref BitStream reader)
    {
        if (reader.ReadUInt32() != 0xE6_E6_A1_00)
            throw new Exception("Eval Condition did not match expected magic (E6 E6 A1 00)");

        var version = reader.ReadUInt32();
        ConditionType = (EvalConditionType)reader.ReadInt32();
        Gold = reader.ReadInt32();
        Silver = reader.ReadInt32();
        Bronze = reader.ReadInt32();
        GhostDataPath = reader.ReadString4();
    }

    public void Serialize(ref BitStream bs)
    {
        bs.WriteUInt32(0xE6_E6_A1_00);
        bs.WriteUInt32(1_00);
        bs.WriteInt32((int)ConditionType);
        bs.WriteInt32(Gold);
        bs.WriteInt32(Silver);
        bs.WriteInt32(Bronze);
        bs.WriteNullStringAligned4(GhostDataPath);
    }

    public void ParseFromXml(XmlNode node)
    {
        foreach (XmlNode evalNode in node.ChildNodes)
        {
            switch (evalNode.Name)
            {
                case "bronze":
                    Bronze = evalNode.ReadValueInt();
                    break;
                case "silver":
                    Silver = evalNode.ReadValueInt();
                    break;
                case "gold":
                    Gold = evalNode.ReadValueInt();
                    break;

                case "type":
                    ConditionType = evalNode.ReadValueEnum<EvalConditionType>();
                    break;

                case "ghost_data_path":
                    GhostDataPath = evalNode.ReadValueString();
                    break;
            }
        }
    }
}
