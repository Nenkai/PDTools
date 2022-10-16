using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PDTools.Utils;

namespace PDTools.Structures.PS3.MGameParameter
{
    public class EventStageData
    {
        public StageLayoutType LayoutTypeAtQuick { get; set; }
        public List<StageResetData> AtQuick { get; set; } = new List<StageResetData>();

        public StageLayoutType LayoutTypeBeforeStart { get; set; }
        public List<StageResetData> BeforeStart { get; set; } = new List<StageResetData>();

        public StageLayoutType LayoutTypeCountdown { get; set; }
        public List<StageResetData> Countdown { get; set; } = new List<StageResetData>();

        public StageLayoutType LayoutTypeRaceEnd { get; set; }
        public List<StageResetData> RaceEnd { get; set; } = new List<StageResetData>();

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_04_DD);
            bs.WriteUInt32(1_02); // Version

            bs.WriteSByte((sbyte)LayoutTypeAtQuick);
            bs.WriteInt32(AtQuick.Count);
            foreach (var stageResetData in AtQuick)
                stageResetData.WriteToCache(ref bs);

            bs.WriteSByte((sbyte)LayoutTypeBeforeStart);
            bs.WriteInt32(BeforeStart.Count);
            foreach (var stageResetData in BeforeStart)
                stageResetData.WriteToCache(ref bs);

            bs.WriteSByte((sbyte)LayoutTypeCountdown);
            bs.WriteInt32(Countdown.Count);
            foreach (var stageResetData in Countdown)
                stageResetData.WriteToCache(ref bs);

            bs.WriteSByte((sbyte)LayoutTypeRaceEnd);
            bs.WriteInt32(RaceEnd.Count);
            foreach (var stageResetData in RaceEnd)
                stageResetData.WriteToCache(ref bs);
        }

        public void ParseStageData(XmlNode node)
        {
            foreach (XmlNode pNode in node.ChildNodes)
            {
                switch (pNode.Name)
                {
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


                    case "at_quick_back":
                        AtQuick = ParseStageDataResetList(pNode);
                        break;
                    case "before_start":
                        BeforeStart = ParseStageDataResetList(pNode);
                        break;
                    case "countdown":
                        Countdown = ParseStageDataResetList(pNode);
                        break;
                    case "race_end":
                        RaceEnd = ParseStageDataResetList(pNode);
                        break;
                }
            }
        }

        public void WriteToXml(XmlWriter xml)
        {
            if (AtQuick.Count == 0 && BeforeStart.Count == 0 && RaceEnd.Count == 0 && Countdown.Count == 0)
                return;

            xml.WriteStartElement("stage_data");

            xml.WriteElementValue("layout_type_at_quick", LayoutTypeAtQuick.ToString());
            xml.WriteElementValue("layout_type_before_start", LayoutTypeBeforeStart.ToString());
            xml.WriteElementValue("layout_type_countdown", LayoutTypeCountdown.ToString());
            xml.WriteElementValue("layout_type_race_end", LayoutTypeRaceEnd.ToString());

            if (AtQuick.Count != 0)
            {
                xml.WriteStartElement("at_quick_back");
                foreach (var stage in AtQuick)
                    stage.WriteToXml(xml);
                xml.WriteEndElement();
            }

            if (BeforeStart.Count != 0)
            {
                xml.WriteStartElement("before_start");
                foreach (var stage in BeforeStart)
                    stage.WriteToXml(xml);
                xml.WriteEndElement();
            }

            if (Countdown.Count != 0)
            {
                xml.WriteStartElement("race_start");
                foreach (var stage in Countdown)
                    stage.WriteToXml(xml);
                xml.WriteEndElement();
            }

            if (RaceEnd.Count != 0)
            {
                xml.WriteStartElement("race_end");
                foreach (var stage in RaceEnd)
                    stage.WriteToXml(xml);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();
        }

        public List<StageResetData> ParseStageDataResetList(XmlNode node)
        {
            var list = new List<StageResetData>();
            foreach (XmlNode pNode in node.SelectNodes("stage_reset_data"))
            {
                var data = new StageResetData();
                switch (pNode.Name)
                {
                    case "code":
                        data.Code = pNode.ReadValueString(); break;
                    case "coord":
                        data.Coord = pNode.ReadValueEnum<StageCoordType>(); break;
                    case "x":
                        data.X = pNode.ReadValueSingle(); break;
                    case "y":
                        data.Y = pNode.ReadValueSingle(); break;
                    case "z":
                        data.Z = pNode.ReadValueSingle(); break;
                    case "rotydeg":
                        data.Z = pNode.ReadValueSingle(); break;
                    case "vcoord":
                        data.Z = pNode.ReadValueSingle(); break;
                }

                list.Add(data);
            }

            return list;
        }
    }

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

        public void WriteToCache(ref BitStream bs)
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
    }

    public enum StageLayoutType
    {
        DEFAULT,
        RANK,
        SLOT,
        FRONT_2GRID,
    }

    public enum StageCoordType
    {
        WORLD,
        GRID,
        PITSTOP,
        VCOORD,
        START,
        GRID_ALL,
        PITSTOP_ALL,
        VCOORD_CENTER
    }
}
