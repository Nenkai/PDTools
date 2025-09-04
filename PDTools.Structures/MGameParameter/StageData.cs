using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PDTools.Utils;
using PDTools.Enums;

namespace PDTools.Structures.MGameParameter;

public class StageData
{
    public List<StageResetData> AtQuickBack { get; set; } = [];
    public List<StageResetData> BeforeStart { get; set; } = [];
    public List<StageResetData> Countdown { get; set; } = [];
    public List<StageResetData> RaceEnd { get; set; } = [];

    /// <summary>
    /// Defaults to <see cref="StageLayoutType.DEFAULT"/>
    /// </summary>
    public StageLayoutType LayoutTypeAtQuick { get; set; } = StageLayoutType.DEFAULT;

    /// <summary>
    /// Defaults to <see cref="StageLayoutType.DEFAULT"/>
    /// </summary>
    public StageLayoutType LayoutTypeBeforeStart { get; set; }

    /// <summary>
    /// Defaults to <see cref="StageLayoutType.DEFAULT"/>
    /// </summary>
    public StageLayoutType LayoutTypeCountdown { get; set; }

    /// <summary>
    /// Defaults to <see cref="StageLayoutType.DEFAULT"/>
    /// </summary>
    public StageLayoutType LayoutTypeRaceEnd { get; set; }

    public bool IsDefault()
    {
        var defaultStageData = new StageData();
        return AtQuickBack.Count == 0 &&
            BeforeStart.Count == 0 &&
            Countdown.Count == 0 &&
            RaceEnd.Count == 0 &&
            LayoutTypeAtQuick == defaultStageData.LayoutTypeAtQuick &&
            LayoutTypeBeforeStart == defaultStageData.LayoutTypeBeforeStart &&
            LayoutTypeCountdown == defaultStageData.LayoutTypeCountdown &&
            LayoutTypeRaceEnd == defaultStageData.LayoutTypeRaceEnd;
    }

    public void CopyTo(StageData other)
    {
        for (int i = 0; i < AtQuickBack.Count; i++)
        {
            var resetData = new StageResetData();
            AtQuickBack[i].CopyTo(resetData);
            other.AtQuickBack.Add(resetData);
        }

        for (int i = 0; i < BeforeStart.Count; i++)
        {
            var resetData = new StageResetData();
            BeforeStart[i].CopyTo(resetData);
            other.BeforeStart.Add(resetData);
        }

        for (int i = 0; i < Countdown.Count; i++)
        {
            var resetData = new StageResetData();
            Countdown[i].CopyTo(resetData);
            other.Countdown.Add(resetData);
        }

        for (int i = 0; i < RaceEnd.Count; i++)
        {
            var resetData = new StageResetData();
            RaceEnd[i].CopyTo(resetData);
            other.RaceEnd.Add(resetData);
        }

        other.LayoutTypeAtQuick = LayoutTypeAtQuick;
        other.LayoutTypeBeforeStart = LayoutTypeBeforeStart;
        other.LayoutTypeCountdown = LayoutTypeCountdown;
        other.LayoutTypeRaceEnd = LayoutTypeRaceEnd;
    }

    public void ParseFromXml(XmlNode node)
    {
        foreach (XmlNode pNode in node.ChildNodes)
        {
            switch (pNode.Name)
            {
                case "at_quick_back":
                case "at_quick":
                    AtQuickBack = ParseStageDataResetList(pNode);
                    break;

                case "before_start":
                    BeforeStart = ParseStageDataResetList(pNode);
                    break;

                case "countdown":
                case "race_start":
                    Countdown = ParseStageDataResetList(pNode);
                    break;
                case "race_end":
                    RaceEnd = ParseStageDataResetList(pNode);
                    break;

                case "layout_type_at_quick":
                    LayoutTypeAtQuick = pNode.ReadValueEnum<StageLayoutType>();
                    break;
                case "layout_type_before_start":
                    LayoutTypeBeforeStart = pNode.ReadValueEnum<StageLayoutType>();
                    break;
                case "layout_type_countdown":
                    LayoutTypeCountdown = pNode.ReadValueEnum<StageLayoutType>();
                    break;
                case "layout_type_race_end":
                    LayoutTypeRaceEnd = pNode.ReadValueEnum<StageLayoutType>();
                    break;

            }
        }
    }

    public void WriteToXml(XmlWriter xml)
    {
        xml.WriteStartElement("at_quick_back");
        foreach (var stage in AtQuickBack)
            stage.WriteToXml(xml);
        xml.WriteEndElement();

        xml.WriteStartElement("before_start");
        foreach (var stage in BeforeStart)
            stage.WriteToXml(xml);
        xml.WriteEndElement();

        xml.WriteStartElement("race_start");
        foreach (var stage in Countdown)
            stage.WriteToXml(xml);
        xml.WriteEndElement();

        xml.WriteStartElement("race_end");
        foreach (var stage in RaceEnd)
            stage.WriteToXml(xml);
        xml.WriteEndElement();

        if (LayoutTypeAtQuick != StageLayoutType.DEFAULT)
            xml.WriteElementValue("layout_type_at_quick", LayoutTypeAtQuick.ToString());

        if (LayoutTypeBeforeStart != StageLayoutType.DEFAULT)
            xml.WriteElementValue("layout_type_before_start", LayoutTypeBeforeStart.ToString());

        if (LayoutTypeCountdown != StageLayoutType.DEFAULT)
            xml.WriteElementValue("layout_type_countdown", LayoutTypeCountdown.ToString());

        if (LayoutTypeRaceEnd != StageLayoutType.DEFAULT)
            xml.WriteElementValue("layout_type_race_end", LayoutTypeRaceEnd.ToString());
    }

    public static List<StageResetData> ParseStageDataResetList(XmlNode node)
    {
        var list = new List<StageResetData>();
        var stageResetDataNodes = node.SelectNodes("stage_reset_data");
        if (stageResetDataNodes is not null)
        {
            foreach (XmlNode? pNode in stageResetDataNodes)
            {
                if (pNode is null)
                    continue;

                var data = new StageResetData();
                data.ParseFromXml(pNode);
                list.Add(data);
            }
        }

        return list;
    }


    public void Serialize(ref BitStream bs)
    {
        bs.WriteUInt32(0xE6_E6_04_DD);
        bs.WriteUInt32(1_02); // Version

        bs.WriteSByte((sbyte)LayoutTypeAtQuick);
        bs.WriteInt32(AtQuickBack.Count);
        foreach (var stageResetData in AtQuickBack)
            stageResetData.Serialize(ref bs);

        bs.WriteSByte((sbyte)LayoutTypeBeforeStart);
        bs.WriteInt32(BeforeStart.Count);
        foreach (var stageResetData in BeforeStart)
            stageResetData.Serialize(ref bs);

        bs.WriteSByte((sbyte)LayoutTypeCountdown);
        bs.WriteInt32(Countdown.Count);
        foreach (var stageResetData in Countdown)
            stageResetData.Serialize(ref bs);

        bs.WriteSByte((sbyte)LayoutTypeRaceEnd);
        bs.WriteInt32(RaceEnd.Count);
        foreach (var stageResetData in RaceEnd)
            stageResetData.Serialize(ref bs);
    }
}
