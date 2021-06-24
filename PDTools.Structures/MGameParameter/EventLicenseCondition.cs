using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    public class EventLicenseCondition
    {
        public bool UseBasicFinish { get; set; }
        public bool StopOnFinish { get; set; }
        public LicenseDisplayModeType DisplayMode { get; set; }
        public List<string> GadgetNames { get; set; } = new List<string>();
        public List<LicenseConditionData> FinishCondition { get; set; } = new List<LicenseConditionData>();
        public List<LicenseConditionData> FailureCondition { get; set; } = new List<LicenseConditionData>();
        public List<LicenseConditionData> SuccessCondition { get; set; } = new List<LicenseConditionData>();

        public void ParseFromXml(XmlNode licenseCondNode)
        {
            foreach (XmlNode node in licenseCondNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "use_basic_finish":
                        UseBasicFinish = node.ReadValueBool(); break;

                    case "stop_on_finish":
                        StopOnFinish = node.ReadValueBool(); break;

                    case "display_mode":
                        DisplayMode = node.ReadValueEnum<LicenseDisplayModeType>(); break;

                    case "gadget_names":
                        foreach (XmlNode gadget in node.SelectNodes("gadget"))
                            GadgetNames.Add(gadget.ReadValueString());
                        break;

                    case "failure_condition":
                        ParseConditionList(node, FailureCondition); break;
                    case "finish_condition":
                        ParseConditionList(node, FinishCondition); break;
                    case "success_condition":
                        ParseConditionList(node, SuccessCondition); break;
                }
            }
        }

        public void ParseConditionList(XmlNode condNode, List<LicenseConditionData> list)
        {
            foreach (XmlNode node in condNode.SelectNodes("data"))
            {
                var cond = new LicenseConditionData();
                switch (node.Name)
                {
                    case "check_type":
                        cond.CheckType = node.ReadValueEnum<LicenseCheckType>(); break;

                    case "condition":
                        cond.Condition = node.ReadValueEnum<LicenseConditionType>(); break;

                    case "connection":
                        cond.Connection = node.ReadValueEnum<LicenseConnectionType>(); break;

                    case "result_type":
                        cond.ResultType = (LicenseResultType)node.ReadValueInt(); break;

                    case "float_value":
                        cond.FloatValue = node.ReadValueSingle(); break;

                    case "uint_value":
                        cond.UIntValue = node.ReadValueUInt(); break;

                    case "int_value":
                        cond.IntValue = node.ReadValueInt(); break;
                }
                list.Add(cond);
            }
        }

        public void WriteToXml(XmlWriter xml)
        {
            if (!FinishCondition.Any() && !FailureCondition.Any() && !SuccessCondition.Any())
                return;

            xml.WriteStartElement("license_condition");
            xml.WriteElementBool("use_basic_finish", UseBasicFinish);
            xml.WriteElementBool("stop_on_finish", StopOnFinish);
            xml.WriteElementValue("display_mode", DisplayMode.ToString());

            if (GadgetNames.Any())
            {
                xml.WriteStartElement("gadget_names");
                foreach (var gadget in GadgetNames)
                    xml.WriteElementValue("gadget", gadget);
                xml.WriteEndElement();
            }

            WriteConditions(xml, "failure_condition", FailureCondition);
            WriteConditions(xml, "finish_condition", FinishCondition);
            WriteConditions(xml, "success_condition", SuccessCondition);

            xml.WriteEndElement();
        }

        private void WriteConditions(XmlWriter xml, string nodeName, List<LicenseConditionData> conditions)
        {
            xml.WriteStartElement(nodeName);

            foreach (var cond in conditions)
            {
                xml.WriteElementValue("check_type", cond.CheckType.ToString());
                xml.WriteElementValue("condition", cond.Condition.ToString());
                xml.WriteElementValue("connection", cond.Connection.ToString());
                xml.WriteElementEnumInt("result_type", cond.ResultType);
                xml.WriteElementFloat("float_value", cond.FloatValue);
                xml.WriteElementUInt("uint_value", cond.UIntValue);
                xml.WriteElementInt("int_value", cond.IntValue);
            }

            xml.WriteEndElement();
        }

        public void ReadFromCache(ref BitStream reader)
        {
            if (reader.ReadUInt32() != 0xE6_E8_DC_CF)
                throw new Exception("License condition magic did not match expected (0xE6_E8_DC_CF)");

            uint version = reader.ReadUInt32();

            throw new NotImplementedException("Implement license condition reading");
        }

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E8_DC_CF);
            bs.WriteUInt32(1_01);

            bs.WriteBool(UseBasicFinish);
            bs.WriteBool(StopOnFinish);
            bs.WriteByte((byte)DisplayMode);

            bs.WriteInt32(GadgetNames.Count);
            foreach (var gadget in GadgetNames)
                bs.WriteNullStringAligned4(gadget);

            WriteConditionListToCache(ref bs, FinishCondition);
            WriteConditionListToCache(ref bs, FailureCondition);
            WriteConditionListToCache(ref bs, SuccessCondition);
        }

        private void WriteConditionListToCache(ref BitStream bs, List<LicenseConditionData> list)
        {
            bs.WriteInt32(list.Count);
            foreach (var data in list)
            {
                bs.WriteUInt32(0xE6_E8_DC_CD);
                bs.WriteUInt32(1_00);

                bs.WriteSByte((sbyte)data.CheckType);
                bs.WriteSByte((sbyte)data.Condition);
                bs.WriteSByte((sbyte)data.Connection);
                bs.WriteSByte((sbyte)data.ResultType);
                bs.WriteSingle(data.FloatValue);
                bs.WriteUInt32(data.UIntValue);
                bs.WriteInt32(data.IntValue);
            }
        }
    }

    public class LicenseConditionData
    {
        public LicenseCheckType CheckType { get; set; }
        public LicenseConditionType Condition { get; set; }
        public LicenseConnectionType Connection { get; set; }
        public LicenseResultType ResultType { get; set; }
        public float FloatValue { get; set; }
        public uint UIntValue { get; set; }
        public int IntValue { get; set; }
    }

    public enum LicenseResultType
    {
        EMPTY,
        FAILURE,
        CLEAR,
        BRONZE,
        SILVER,
        GOLD
    }

    public enum LicenseDisplayModeType
    {
        NONE,
        PYLON_NUM,
        FUEL_DIST,
        FUEL_TIME,
        DRIFT_SCORE,

    }

    public enum LicenseConnectionType
    {
        OR,
        AND,
        XOR
    }

    public enum LicenseConditionType
    {
        EQUAL,
        NOTEQUAL,
        GREATER,
        LESS,
        GREATER_EQUAL,
        LESS_EQUAL,
    }

    public enum LicenseCheckType
    {
        RANK,
        OTHER_SUBMODE,
        TOTAL_TIME,
        LAP_TIME,
        BEST_LAP_TIME,
        LAP_COUNT,
        VELOCITY,
        V_POSITION,
        GADGET_COUNT,
        COURSE_OUT,
        HIT_COUNT,
        HIT_POWER,
        HIT_WALL,
        FUEL_AMOUNT,
        COMPLETE_FLAG,
        WRONG_WAY_COUNT,
        ROAD_DISTANCE,
        STANDING_TIME,
        COURSE_OUT_TIME,
        FUEL_CONSUMPTION,
        FLOATING_TIME,
        ILLEGAL,
    }
}
