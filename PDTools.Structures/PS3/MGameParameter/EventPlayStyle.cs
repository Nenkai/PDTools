using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;

using PDTools.Utils;

namespace PDTools.Structures.PS3.MGameParameter
{
    public class EventPlayStyle
    {
        public SpecType BSpecType { get; set; }
        public PlayType PlayType { get; set; }
        public bool NoQuickMenu { get; set; }
        public bool ReplayRecordEnable { get; set; } = true;
        public bool NoInstantReplay { get; set; }
        public bool RentCarSettingEnable { get; set; }

        /// <summary>
        /// Window count in the screen, for split 
        /// </summary>
        public int WindowNum { get; set; } = 1;

        /// <summary>
        /// For demo
        /// </summary>
        public int TimeLimit { get; set; }

        /// <summary>
        /// For demo
        /// </summary>
        public int LeaveLimit { get; set; }

        //rentcar_setting_enable
        //no_instant_replay

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("play_style");
            {
                xml.WriteElementValue("bspec_type", BSpecType.ToString());

                if (PlayType != PlayType.RACE)
                    xml.WriteElementValue("play_type", PlayType.ToString());

                xml.WriteElementBoolIfTrue("no_quickmenu", NoQuickMenu);

                if (!ReplayRecordEnable)
                    xml.WriteElementBool("replay_record_enable", ReplayRecordEnable);

                if (!RentCarSettingEnable)
                    xml.WriteElementBool("rentcar_setting_enable", RentCarSettingEnable);

                if (WindowNum > 1)
                    xml.WriteElementInt("window_num", WindowNum);

                if (TimeLimit != 0)
                    xml.WriteElementInt("time_limit", TimeLimit);

                if (TimeLimit != 0)
                    xml.WriteElementInt("leave_limit", LeaveLimit);

            }
            xml.WriteEndElement();
        }

        public void ReadFromCache(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5E516A7 && magic != 0xE6E616A7)
                ;

            uint playstyleVersion = reader.ReadUInt32();
            if (playstyleVersion < 102)
            {
                BSpecType = (SpecType)reader.ReadInt32();
                PlayType = (PlayType)reader.ReadInt32();
                NoQuickMenu = reader.ReadBool();
                NoInstantReplay = reader.ReadBool();
                ReplayRecordEnable = reader.ReadBool();
                if (playstyleVersion == 101)
                    RentCarSettingEnable = reader.ReadBool();
                WindowNum = reader.ReadInt32();
                TimeLimit = reader.ReadInt32();
                LeaveLimit = reader.ReadInt32();
            }
        }

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_16_A7);
            bs.WriteUInt32(1_01);
            bs.WriteInt32((int)BSpecType);
            bs.WriteInt32((int)PlayType);
            bs.WriteBool(NoQuickMenu);
            bs.WriteBool(NoInstantReplay);
            bs.WriteBool(ReplayRecordEnable);
            bs.WriteBool(RentCarSettingEnable);
            bs.WriteInt32(WindowNum);
            bs.WriteInt32(TimeLimit);
            bs.WriteInt32(LeaveLimit);
        }

        public void ParsePlayStyle(XmlNode node)
        {
            foreach (XmlNode pNode in node.ChildNodes)
            {
                switch (pNode.Name)
                {
                    case "bspec_type":
                        BSpecType = pNode.ReadValueEnum<SpecType>();
                        break;
                    case "play_type":
                        PlayType = pNode.ReadValueEnum<PlayType>();
                        break;

                    case "no_quickmenu":
                        NoQuickMenu = pNode.ReadValueBool();
                        break;
                    case "replay_record_enable":
                        ReplayRecordEnable = pNode.ReadValueBool();
                        break;
                    case "rentcar_setting_enable":
                        RentCarSettingEnable = pNode.ReadValueBool();
                        break;

                    case "window_num":
                        WindowNum = pNode.ReadValueInt();
                        break;

                    case "time_limit":
                        TimeLimit = pNode.ReadValueInt();
                        break;

                    case "leave_limit":
                        LeaveLimit = pNode.ReadValueInt();
                        break;
                }
            }
        }
    }

    public enum SpecType
    {
        [Description("Both A And B")]
        BOTH_A_AND_B,

        [Description("A-Spec Only")]
        ONLY_A,

        [Description("B-Spec Only")]
        ONLY_B,
    }

    public enum PlayType
    {
        [Description("Race")]
        RACE,

        [Description("Demo Mode")]
        DEMO,

        [Description("Gamble (unused/implemented?)")]
        GAMBLE,
    }
}
