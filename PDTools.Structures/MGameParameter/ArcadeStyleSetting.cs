using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    public class ArcadeStyleSetting
    {
        /// <summary>
        /// Defaults to 120.
        /// </summary>
        public byte StartSeconds { get; set; } = 120;

        /// <summary>
        /// Defaults to 25.
        /// </summary>
        public byte DefaultExtendSeconds { get; set; } = 25;

        /// <summary>
        /// Defaults to 150.
        /// </summary>
        public byte LimitSeconds { get; set; } = 150;

        /// <summary>
        /// Defaults to 7.
        /// </summary>
        public byte LevelUpStep { get; set; } = 7;

        /// <summary>
        /// Defaults to 5.
        /// </summary>
        public byte OvertakeSeconds { get; set; } = 5;

        /// <summary>
        /// Defaults to 700.
        /// </summary>
        public ushort AppearStepV { get; set; } = 700;

        /// <summary>
        /// Defaults to 250.
        /// </summary>
        public ushort DisappearStepV { get; set; } = 250;

        /// <summary>
        /// Defaults to true.
        /// </summary>
        public bool EnableSpeedTrap { get; set; } = true;

        /// <summary>
        /// Defaults to true.
        /// </summary>
        public bool EnableJumpBonus { get; set; } = true;

        /// <summary>
        /// Defaults to 4000.
        /// </summary>
        public ushort AffordTime { get; set; } = 4000;

        /// <summary>
        /// Defaults to 0.
        /// </summary>
        public ushort OvertakeScore { get; set; } = 0;

        /// <summary>
        /// Defaults to 0.
        /// </summary>
        public ushort SpeedTrapScore { get; set; } = 0;

        /// <summary>
        /// Defaults to 0.
        /// </summary>
        public ushort JumpBonusScore { get; set; } = 0;

        /// <summary>
        /// Defaults to 200.
        /// </summary>
        public ushort StartupStepV { get; set; } = 200;

        /// <summary>
        /// Defaults to 500.
        /// </summary>
        public ushort StartupOffsetV { get; set; } = 500;

        /// <summary>
        /// Defaults to 80.
        /// </summary>
        public ushort InitialVelocityL { get; set; } = 80;

        /// <summary>
        /// Defaults to 150.
        /// </summary>
        public ushort InitialVelocityH { get; set; } = 150;

        /// <summary>
        /// Max 16. Default 16 (all -1).
        /// </summary>
        public sbyte[] SectionExtendSeconds { get; set; } = new sbyte[16];

        /// <summary>
        /// Max 16. Default 16 (all 0).
        /// </summary>
        public uint[] SpeedTraps { get; set; } = new uint[16];

        public ArcadeStyleSetting()
        {
            for (int i = 0; i < 16; i++)
                SectionExtendSeconds[i] = -1;

            for (int i = 0; i < 16; i++)
                SpeedTraps[i] = 0;
        }

        public bool IsDefault()
        {
            var defaultArcadeStyle = new ArcadeStyleSetting();
            return StartSeconds == defaultArcadeStyle.StartSeconds &&
                DefaultExtendSeconds == defaultArcadeStyle.DefaultExtendSeconds &&
                LimitSeconds == defaultArcadeStyle.LimitSeconds &&
                LevelUpStep == defaultArcadeStyle.LevelUpStep &&
                OvertakeSeconds == defaultArcadeStyle.OvertakeSeconds &&
                EnableSpeedTrap == defaultArcadeStyle.EnableSpeedTrap &&
                EnableJumpBonus == defaultArcadeStyle.EnableJumpBonus &&
                AppearStepV == defaultArcadeStyle.AppearStepV &&
                DisappearStepV == defaultArcadeStyle.DisappearStepV &&
                AffordTime == defaultArcadeStyle.AffordTime &&
                OvertakeScore == defaultArcadeStyle.OvertakeScore &&
                SpeedTrapScore == defaultArcadeStyle.SpeedTrapScore &&
                JumpBonusScore == defaultArcadeStyle.JumpBonusScore &&
                StartupStepV == defaultArcadeStyle.StartupStepV &&
                StartupOffsetV == defaultArcadeStyle.StartupOffsetV &&
                InitialVelocityL == defaultArcadeStyle.InitialVelocityL &&
                InitialVelocityH == defaultArcadeStyle.InitialVelocityH &&
                SectionExtendSeconds.Length == defaultArcadeStyle.SectionExtendSeconds.Length && SectionExtendSeconds.SequenceEqual(defaultArcadeStyle.SectionExtendSeconds) &&
                SpeedTraps.Length == defaultArcadeStyle.SectionExtendSeconds.Length && SpeedTraps.SequenceEqual(defaultArcadeStyle.SpeedTraps);
        }

        public void WriteToXml(XmlWriter xml)
        {
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
            for (int i = 0; i < SectionExtendSeconds.Length; i++)
                xml.WriteElementInt("seconds", SectionExtendSeconds[i]);
            xml.WriteEndElement();

            xml.WriteStartElement("speed_trap");
            for (int i = 0; i < SpeedTraps.Length; i++)
                xml.WriteElementUInt("coursev", SpeedTraps[i]);
            xml.WriteEndElement();
        }

        public void ParseFromXml(XmlNode asNode)
        {
            SectionExtendSeconds.AsSpan().Fill(-1);
            SpeedTraps.AsSpan().Fill(0);

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
                            SectionExtendSeconds[i] = sec.ReadValueSByte();
                        }
                        break;

                    case "speed_trap":
                        int j = 0;
                        foreach (XmlNode sec in node.SelectNodes("coursev"))
                        {
                            if (j >= 16)
                                break;

                            SpeedTraps[j] = sec.ReadValueUInt();
                        }
                        break;
                }
            }
        }

        public void Serialize(ref BitStream bs)
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
                if (i < SectionExtendSeconds.Length)
                    bs.WriteSByte(SectionExtendSeconds[i]);
                else
                    bs.WriteSByte(-1);

                if (i < SpeedTraps.Length)
                    bs.WriteSingle(SpeedTraps[i]);
                else
                    bs.WriteSingle(0);
            }
        }
    }
}
