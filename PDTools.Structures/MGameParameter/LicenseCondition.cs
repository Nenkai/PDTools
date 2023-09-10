using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    public class LicenseCondition
    {
        /// <summary>
        /// Defaults to false.
        /// </summary>
        public bool UseBasicFinish { get; set; } = false;

        /// <summary>
        /// Defaults to false.
        /// </summary>
        public bool StopOnFinish { get; set; }

        /// <summary>
        /// Defautls to <see cref="LicenseDisplayModeType.NONE"/>
        /// </summary>
        public LicenseDisplayModeType DisplayMode { get; set; } = LicenseDisplayModeType.NONE;

        public List<string> GadgetNames { get; set; } = new List<string>();
        public List<LicenseConditionData> FinishCondition { get; set; } = new List<LicenseConditionData>();
        public List<LicenseConditionData> FailureCondition { get; set; } = new List<LicenseConditionData>();
        public List<LicenseConditionData> SuccessCondition { get; set; } = new List<LicenseConditionData>();

        public bool IsDefault()
        {
            var defaultLicenseCondition = new LicenseCondition();
            return UseBasicFinish == defaultLicenseCondition.UseBasicFinish &&
                StopOnFinish == defaultLicenseCondition.StopOnFinish &&
                DisplayMode == defaultLicenseCondition.DisplayMode &&
                FinishCondition.Count == 0 &&
                FailureCondition.Count == 0 &&
                SuccessCondition.Count == 0;
        }

        public void CopyTo(LicenseCondition other)
        {
            other.UseBasicFinish = UseBasicFinish;
            other.StopOnFinish = StopOnFinish;
            other.DisplayMode = DisplayMode;

            foreach (var gadget in GadgetNames)
                other.GadgetNames.Add(gadget);

            foreach (var thisData in FinishCondition)
            {
                var data = new LicenseConditionData();
                thisData.CopyTo(data);
                other.FinishCondition.Add(data);
            }

            foreach (var thisData in FailureCondition)
            {
                var data = new LicenseConditionData();
                thisData.CopyTo(data);
                other.FailureCondition.Add(data);
            }

            foreach (var thisData in SuccessCondition)
            {
                var data = new LicenseConditionData();
                thisData.CopyTo(data);
                other.SuccessCondition.Add(data);
            }
        }

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
                        foreach (XmlNode childNode in node.SelectNodes("data"))
                            ParseConditionList(childNode, FailureCondition); break;
                    case "finish_condition":
                        foreach (XmlNode childNode in node.SelectNodes("data"))
                            ParseConditionList(childNode, FinishCondition); break;
                    case "success_condition":
                        foreach (XmlNode childNode in node.SelectNodes("data"))
                            ParseConditionList(childNode, SuccessCondition); break;
                }
            }
        }

        public void ParseConditionList(XmlNode condNode, List<LicenseConditionData> list)
        {
            var cond = new LicenseConditionData();
            foreach (XmlNode node in condNode.ChildNodes)
            {
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
            }

            list.Add(cond);
        }

        public void WriteToXml(XmlWriter xml)
        {
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
        }

        private void WriteConditions(XmlWriter xml, string nodeName, List<LicenseConditionData> conditions)
        {
            xml.WriteStartElement(nodeName);

            foreach (var cond in conditions)
            {
                xml.WriteStartElement("data");
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

            xml.WriteEndElement();
        }

        public void ReadFromCache(ref BitStream reader)
        {
            if (reader.ReadUInt32() != 0xE6_E8_DC_CF)
                throw new Exception("License condition magic did not match expected (0xE6_E8_DC_CF)");

            uint version = reader.ReadUInt32();

            throw new NotImplementedException("Implement license condition reading");
        }

        public void Serialize(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E8_DC_CF);
            bs.WriteUInt32(1_01);

            bs.WriteBool(UseBasicFinish);
            bs.WriteBool(StopOnFinish);
            bs.WriteByte((byte)DisplayMode);

            bs.WriteInt32(GadgetNames.Count);
            foreach (var gadget in GadgetNames)
                bs.WriteNullStringAligned4(gadget);

            bs.WriteInt32(FinishCondition.Count);
            foreach (var data in FinishCondition)
                data.Serialize(ref bs);

            bs.WriteInt32(FailureCondition.Count);
            foreach (var data in FailureCondition)
                data.Serialize(ref bs);

            bs.WriteInt32(SuccessCondition.Count);
            foreach (var data in SuccessCondition)
                data.Serialize(ref bs);
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
        
        public void CopyTo(LicenseConditionData other)
        {
            other.CheckType = CheckType;
            other.Condition = Condition;
            other.Connection = Connection;
            other.ResultType = ResultType;
            other.FloatValue = FloatValue;
            other.UIntValue = UIntValue;
            other.IntValue = IntValue;
        }

        public void Serialize(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E8_DC_CD);
            bs.WriteUInt32(1_00);

            bs.WriteSByte((sbyte)CheckType);
            bs.WriteSByte((sbyte)Condition);
            bs.WriteSByte((sbyte)Connection);
            bs.WriteSByte((sbyte)ResultType);
            bs.WriteSingle(FloatValue);
            bs.WriteUInt32(UIntValue);
            bs.WriteInt32(IntValue);
        }

        public override string ToString()
        {
            return $"[{ResultType}] {CheckType} is {Condition} to ({IntValue}/{UIntValue}/{FloatValue}) {Connection}";
        }
    }
}
