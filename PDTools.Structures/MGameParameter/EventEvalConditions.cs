using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using System.ComponentModel;
using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    public class EventEvalConditions : INotifyPropertyChanged
    {
        public bool NeedsPopulating { get; set; } = true;

        public EvalConditionType ConditionType { get; set; }
        public string GhostDataPath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private int _gold;
        public int Gold
        {
            get => _gold;
            set
            {
                _gold = value;

                if (ConditionType == EvalConditionType.TIME)
                {
                    if (value > _silver) // New Gold is higher than silver? set silver to it
                        Silver = _gold;

                    if (value > _bronze) // New Gold is higher than bronze? set bronze to it
                        Bronze = _gold;
                }
                else
                {
                    if (value < _silver) // New Gold is lower than silver? set silver to it
                        Silver = _gold;

                    if (value < _bronze) // New Gold is higher than bronze? set bronze to it
                        Bronze = _gold;
                }

            }
        }

        private int _silver;
        public int Silver
        {
            get => _silver;
            set
            {
                if (ConditionType == EvalConditionType.TIME)
                {
                    if (value < _gold) // Silver is lower than gold? set silver to gold
                        value = _gold;

                    if (value > _bronze) // Silver is higher than bronze? set bronze to silver
                        Bronze = value;
                }
                else
                {
                    if (value > _gold) // Silver is higher than gold? set silver to gold
                        value = _gold;

                    if (value < _bronze) // Silver is lower than bronze? set bronze to silver
                        Bronze = value;
                }

                _silver = value;
                OnPropertyChanged("Silver");
            }
        }

        private int _bronze;
        public int Bronze
        {
            get => _bronze;
            set
            {
                if (ConditionType == EvalConditionType.TIME)
                {
                    if (value < _gold) // Bronze lower than gold? set it to the same time as gold
                        value = _gold;

                    if (value < _silver) // Bronze lower than silver? set it to the same time as silver
                        value = _silver;
                }
                else
                {
                    if (value > _gold) // Bronze lower than gold? set it to the same time as gold
                        value = _gold;

                    if (value > _silver) // Bronze lower than silver? set it to the same time as silver
                        value = _silver;
                }

                _bronze = value;
                OnPropertyChanged("Bronze");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void WriteToXml(XmlWriter xml)
        {
            if (ConditionType == EvalConditionType.NONE && Gold == 0 && Silver == 0 && Bronze == 0)
                return;

            xml.WriteStartElement("eval_condition");
            {
                if (ConditionType != EvalConditionType.NONE)
                    xml.WriteElementValue("type", ConditionType.ToString());
                xml.WriteElementInt("gold", Gold);
                xml.WriteElementInt("silver", Silver);
                xml.WriteElementInt("bronze", Bronze);

                if (!string.IsNullOrEmpty("ghost_data_path"))
                    xml.WriteElementValue("ghost_data_path", GhostDataPath);
            }
            xml.WriteEndElement();
        }

        public void ReadFromCache(ref BitStream reader)
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

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_A1_00);
            bs.WriteUInt32(1_00);
            bs.WriteInt32((int)ConditionType);
            bs.WriteInt32(Gold);
            bs.WriteInt32(Silver);
            bs.WriteInt32(Bronze);
            bs.WriteNullStringAligned4(GhostDataPath);
        }

        public void ParseEvalConditionData(XmlNode node)
        {
            foreach (XmlNode evalNode in node.ChildNodes)
            {
                switch (evalNode.Name)
                {
                    case "bronze":
                        _bronze = evalNode.ReadValueInt();
                        break;
                    case "silver":
                        _silver = evalNode.ReadValueInt();
                        break;
                    case "gold":
                        _gold = evalNode.ReadValueInt();
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

    public enum EvalConditionType
    {
        [Description("None")]
        NONE,

        [Description("By Time (in MS)")]
        TIME,

        [Description("By Finish Order")]
        ORDER,

        [Description("Cones Hit")]
        PYLON,

        [Description("Drift Score/Arcade Style Score")]
        DRIFT,

        [Description("VS Ghost (to be specified)")]
        VS_GHOST,

        [Description("Distance Travelled")]
        DIST,

        [Description("Fuel Spent")]
        FUEL,

        [Description("By Overtake Count")]
        OVER_TAKE,
    }
}
