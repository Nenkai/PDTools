using PDTools.Utils;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PDTools.Structures.MGameParameter;

public class FailureCondition
{
    public List<FailCondition> FailConditions { get; set; } = [];
    public List<int> DataList { get; set; } = [];

    /// <summary>
    /// Defaults to false.
    /// </summary>
    public bool NoFailureAtResult { get; set; } = false;

    public bool IsDefault()
    {
        var defaultFailureCondition = new FailureCondition();
        return FailConditions.Count == 0 &&
            DataList.Count == 0 &&
            NoFailureAtResult == defaultFailureCondition.NoFailureAtResult;
    }

    public void CopyTo(FailureCondition other)
    {
        for (int i = 0; i < FailConditions.Count; i++)
            other.FailConditions.Add(FailConditions[i]);

        for (int i = 0; i < DataList.Count; i++)
            other.DataList.Add(DataList[i]);

        other.NoFailureAtResult = NoFailureAtResult;
    }

    public void WriteToXml(XmlWriter xml)
    {

        xml.WriteStartElement("type_list");
        {
            foreach (var value in FailConditions)
                xml.WriteElementValue("type", value.ToString());
        }
        xml.WriteEndElement();

        if (DataList.Count != 0)
        {
            xml.WriteStartElement("data_list");
            foreach (var data in DataList)
                xml.WriteElementInt("data", data);
            xml.WriteEndElement();
        }

        xml.WriteElementBool("no_failure_at_result", NoFailureAtResult);
    }

    public void Deserialize(ref BitStream reader)
    {
        if (reader.ReadUInt32() != 0xE6_E6_DC_CE)
            throw new Exception("Failure condition magic did not match expected (E6 E6 DC CE)");

        uint version = reader.ReadUInt32();

        int type_list_count = reader.ReadInt32();
        for (int i = 0; i < type_list_count; i++)
            FailConditions.Add((FailCondition)reader.ReadInt32());

        int data_list_count = reader.ReadInt32();
        for (int i = 0; i < data_list_count; i++)
            DataList.Add(reader.ReadInt32());

        NoFailureAtResult = reader.ReadBool();
    }

    public void Serialize(ref BitStream bs)
    {
        bs.WriteUInt32(0xE6_E6_DC_CE);
        bs.WriteUInt32(1_00);

        bs.WriteInt32(FailConditions.Count);
        foreach (var value in FailConditions)
            bs.WriteInt32((int)value);

        bs.WriteInt32(DataList.Count);
        foreach (var value in DataList)
            bs.WriteInt32(value);
        bs.WriteBool(NoFailureAtResult);
    }

    public void ParseFromXml(XmlNode node)
    {
        foreach (XmlNode pNode in node.ChildNodes)
        {
            switch (pNode.Name)
            {
                case "type_list":
                    {
                        var list = pNode.SelectNodes("type");
                        if (list is not null)
                        {
                            foreach (XmlNode? type in list)
                            {
                                if (type is null)
                                    continue;

                                FailCondition cond = type.ReadValueEnum<FailCondition>();
                                if (cond == FailCondition.NONE || FailConditions.Contains(cond))
                                    continue;
                                FailConditions.Add(cond);
                            }
                        }
                    }
                    break;

                case "data_list":
                    {
                        var list = pNode.SelectNodes("data");
                        if (list is not null)
                        {
                            foreach (XmlNode? data in list)
                            {
                                if (data is not null)
                                    DataList.Add(data.ReadValueInt());
                            }
                        }
                    }
                    break;

                case "no_failure_at_result":
                    NoFailureAtResult = pNode.ReadValueBool();
                    break;

            }
        }
    }
}

