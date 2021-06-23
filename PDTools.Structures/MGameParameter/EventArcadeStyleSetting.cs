using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    public class EventArcadeStyleSetting
    {
        public byte StartSeconds { get; set; } = 120;
        public byte DefaultExtendSeconds { get; set; } = 25;
        public byte LimitSeconds { get; set; } = 150;
        public byte LevelUpStep { get; set; } = 7;
        public byte OvertakeSeconds { get; set; } = 5;
        public ushort AppearStepV { get; set; } = 700;
        public ushort DisappearStepV { get; set; } = 250;
        public bool EnableSpeedTrap { get; set; } = true;
        public bool EnableJumpBonus { get; set; } = true;
        public ushort AffordTime { get; set; } = 4000;
        public ushort OvertakeScore { get; set; }
        public ushort SpeedTrapScore { get; set; }
        public ushort JumpBonusScore { get; set; }
        public ushort StartupStepV { get; set; } = 200;
        public ushort StartupOffsetV { get; set; } = 500;
        public ushort InitialVelocityL { get; set; } = 80;
        public ushort InitialVelocityH { get; set; } = 150;

        public bool NeedsPopulating { get; set; } = true;
        public List<ArcadeStyleSettingSection> Sections { get; set; } = new List<ArcadeStyleSettingSection>();

        public EventArcadeStyleSetting()
        {
            for (int i = 0; i < 16; i++)
                Sections.Add(new ArcadeStyleSettingSection());
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("arcade_style_setting");

            xml.WriteElementInt("start_seconds", StartSeconds);
            xml.WriteElementInt("default_extend_seconds", DefaultExtendSeconds);
            xml.WriteElementInt("limit_seconds", LimitSeconds);
            xml.WriteElementInt("level_up_step", LevelUpStep);
            xml.WriteElementInt("overtake_seconds", OvertakeSeconds);
            xml.WriteElementBool("enable_speed_trap", EnableSpeedTrap);
            xml.WriteElementBool("enable_jump_bonus", EnableJumpBonus);
            xml.WriteElementInt("appear_step_v", AppearStepV);
            xml.WriteElementInt("disappear_step_v", DisappearStepV);
            xml.WriteElementInt("afford_time", AffordTime);
            xml.WriteElementInt("overtake_score", OvertakeScore);
            xml.WriteElementInt("speed_trap_score", SpeedTrapScore);
            xml.WriteElementInt("jump_bonus_score", JumpBonusScore);
            xml.WriteElementInt("startup_step_v", StartupStepV);
            xml.WriteElementInt("startup_offset_v", StartupOffsetV);
            xml.WriteElementInt("initial_velocity_l", InitialVelocityL);
            xml.WriteElementInt("initial_velocity_h", InitialVelocityH);

            xml.WriteStartElement("section_extend_seconds");
            for (int i = 0; i < Sections.Count; i++)
                xml.WriteElementInt("seconds", Sections[i].SectionExtendSeconds);
            xml.WriteEndElement();

            xml.WriteStartElement("speed_trap");
            for (int i = 0; i < Sections.Count; i++)
                xml.WriteElementUInt("coursev", Sections[i].CourseV);
            xml.WriteEndElement();

            xml.WriteEndElement();
        }

        public void ParseArcadeStyleSetting(XmlNode asNode)
        {
            foreach (XmlNode node in asNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "start_seconds":
                        StartSeconds = node.ReadValueByte(); break;
                    case "default_extend_seconds":
                        DefaultExtendSeconds = node.ReadValueByte(); break;
                    case "limit_seconds":
                        LimitSeconds = node.ReadValueByte(); break;
                    case "level_up_step":
                        LevelUpStep = node.ReadValueByte(); break;
                    case "overtake_seconds":
                        OvertakeSeconds = node.ReadValueByte(); break;
                    case "enable_speed_trap":
                        EnableSpeedTrap = node.ReadValueBool(); break;
                    case "enable_jump_bonus":
                        EnableJumpBonus = node.ReadValueBool(); break;
                    case "appear_step_v":
                        AppearStepV = node.ReadValueUShort(); break;
                    case "disappear_step_v":
                        DisappearStepV = node.ReadValueUShort(); break;
                    case "afford_time":
                        AffordTime = node.ReadValueUShort(); break;
                    case "overtake_score":
                        OvertakeScore = node.ReadValueUShort(); break;
                    case "speed_trap_score":
                        SpeedTrapScore = node.ReadValueUShort(); break;
                    case "jump_bonus_score":
                        JumpBonusScore = node.ReadValueUShort(); break;
                    case "startup_step_v":
                        StartupStepV = node.ReadValueUShort(); break;
                    case "startup_offset_v":
                        StartupOffsetV = node.ReadValueUShort(); break;
                    case "initial_velocity_l":
                        InitialVelocityL = node.ReadValueUShort(); break;
                    case "initial_velocity_h":
                        InitialVelocityH = node.ReadValueUShort(); break;

                    case "section_extend_seconds":
                        int i = 0;
                        foreach (XmlNode sec in node.SelectNodes("seconds"))
                        {
                            if (i >= 16)
                                break;
                            Sections[i++].SectionExtendSeconds = sec.ReadValueSByte();
                        }
                        break;

                    case "speed_trap":
                        int j = 0;
                        foreach (XmlNode sec in node.SelectNodes("coursev"))
                        {
                            if (j >= 16)
                                break;
                            Sections[j++].CourseV = sec.ReadValueUInt();
                        }
                        break;
                }
            }
        }

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_F9_01);
            bs.WriteUInt32(1_01);

            bs.WriteByte(StartSeconds);
            bs.WriteByte(DefaultExtendSeconds);
            bs.WriteByte(LimitSeconds);
            bs.WriteByte(LevelUpStep);
            bs.WriteByte(OvertakeSeconds);
            bs.WriteUInt16(AppearStepV);
            bs.WriteUInt16(DisappearStepV);
            bs.WriteBool(EnableSpeedTrap);
            bs.WriteBool(EnableJumpBonus);
            bs.WriteUInt16(AffordTime);
            bs.WriteUInt16(OvertakeScore);
            bs.WriteUInt16(SpeedTrapScore);
            bs.WriteUInt16(JumpBonusScore);
            bs.WriteUInt16(StartupStepV);
            bs.WriteUInt16(StartupOffsetV);
            bs.WriteUInt16(InitialVelocityL);
            bs.WriteUInt16(InitialVelocityH);

            for (int i = 0; i < 16; i++)
            {
                bs.WriteSByte(Sections[i].SectionExtendSeconds);
                bs.WriteSingle(Sections[i].CourseV);
            }
        }
    }

    public class ArcadeStyleSettingSection
    {
        public sbyte SectionExtendSeconds { get; set; } = -1;
        public uint CourseV { get; set; } 

        public override string ToString()
        {
            string s = $"Section: ";

            if (SectionExtendSeconds == -1)
                s += $"No Extra Seconds,";
            else
                s += $"+{SectionExtendSeconds}s,";

            if (CourseV != 0)
                s += $"Speed Trap at {CourseV}m";
            else
                s += $"No Speed Trap";

            return s;
        }
    }
}
