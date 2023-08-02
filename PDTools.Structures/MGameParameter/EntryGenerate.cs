using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using PDTools.Enums;

using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    public class EntryGenerate
    {
        /// <summary>
        /// Number of entries to generate. Defaults to 0 (none).
        /// </summary>
        public int EntryNum { get; set; } = 0;

        /// <summary>
        /// Defaults to 0 (1st).
        /// </summary>
        public int PlayerPos { get; set; } = 0;

        /// <summary>
        /// How entries are generated. Defaults to <see cref="EntryGenerateType.NONE"/>
        /// </summary>
        public EntryGenerateType GenerateType { get; set; } = EntryGenerateType.NONE;

        /// <summary>
        /// Enemy list type. Defaults to <see cref="EnemyListType.SAME"/>
        /// </summary>
        public EnemyListType EnemyListType { get; set; } = EnemyListType.SAME;

        /// <summary>
        /// Race code to use (from SpecDB->RACE). Defaults to -1 (none).
        /// </summary>
        public long RaceCode { get; set; } = -1;

        /// <summary>
        /// AI Skill. Defaults to 100.
        /// </summary>
        public int AISkill { get; set; } = 100;

        /// <summary>
        /// AI Braking Skill. Defaults to -1.
        /// </summary>
        public int AISkillBraking { get; set; } = -1;

        /// <summary>
        /// AI Cornering Skill. Defaults to -1.
        /// </summary>
        public int AISkillCornering { get; set; } = -1;

        /// <summary>
        /// AI Accelerating Skill. Defaults to -1.
        /// </summary>
        public sbyte AISkillAccelerating { get; set; } = -1;

        /// <summary>
        /// GT6 Only. AI Starting Skill. Defaults to -1.
        /// </summary>
        public sbyte AISkillStarting { get; set; } = -1;

        /// <summary>
        /// AI Roughness. Defaults to -1.
        /// </summary>
        public sbyte AIRoughness { get; set; } = -1;

        /// <summary>
        /// Enemy Level. Defaults to 0.
        /// </summary>
        public int EnemyLevel { get; set; } = 0;

        /// <summary>
        /// Enemy B-Spec Level. Defaults to 0.
        /// </summary>
        public sbyte EnemyBSpecLevel { get; set; } = 0;

        /// <summary>
        /// Defaults to 0.
        /// </summary>
        public sbyte BSpecLevelOffset { get; set; } = 0;

        /// <summary>
        /// Distance between cars for a rolling start. Must be a supported start type, or <see cref="UseRollingStartParameter"/> enabled. Defaults to 0.
        /// </summary>
        public short GapForStartRollingDistance { get; set; } = 0;

        /// <summary>
        /// GT6 Only. VCoord where the rolling start starts. Must be a supported start type, or <see cref="UseRollingStartParameter"/> enabled. Defaults to 0.
        /// </summary>
        public short RollingStartV { get; set; } = 0;

        /// <summary>
        /// GT6 Only. Whether to force use rolling start parameters such as <see cref="GapForStartRollingDistance"/> and <see cref="RollingStartV"/> regardless of start type. Defaults to false.
        /// </summary>
        public bool UseRollingStartParameter { get; set; } = false;

        /// <summary>
        /// Car list to use when <see cref="GenerateType"/> is <see cref="EntryGenerateType.SHUFFLE"/> or <see cref="EntryGenerateType.ORDER"/>.
        /// </summary>
        public List<MCarThin> Cars { get; set; } = new List<MCarThin>();

        /// <summary>
        /// Starting delays. Defaults to 32 entries set to 0.
        /// </summary>
        public int[] Delays { get; set; } = new int[32];

        /// <summary>
        /// How to sort generated entries. Defaults to <see cref="EnemySortType.NONE"/>.
        /// </summary>
        public EnemySortType EnemySortType { get; set; } = EnemySortType.NONE;

        /// <summary>
        /// GT6 Only.
        /// </summary>
        public List<EntryBase> EntryBaseArray { get; set; } = new List<EntryBase>();

        public bool IsDefault()
        {
            var defaultEntrySet = new EntryGenerate();
            return EntryNum == defaultEntrySet.EntryNum &&
                PlayerPos == defaultEntrySet.PlayerPos &&
                GenerateType == defaultEntrySet.GenerateType &&
                EnemyListType == defaultEntrySet.EnemyListType &&
                RaceCode == defaultEntrySet.RaceCode &&
                AISkill == defaultEntrySet.AISkill &&
                AISkillBraking == defaultEntrySet.AISkillBraking &&
                AISkillCornering == defaultEntrySet.AISkillCornering &&
                AISkillAccelerating == defaultEntrySet.AISkillAccelerating &&
                AIRoughness == defaultEntrySet.AIRoughness &&
                EnemyLevel == defaultEntrySet.EnemyLevel &&
                Cars.Count == 0 &&
                EnemyBSpecLevel == defaultEntrySet.EnemyBSpecLevel &&
                BSpecLevelOffset == defaultEntrySet.BSpecLevelOffset &&
                GapForStartRollingDistance == defaultEntrySet.GapForStartRollingDistance &&
                RollingStartV == defaultEntrySet.RollingStartV &&
                UseRollingStartParameter == defaultEntrySet.UseRollingStartParameter &&
                Delays.Length == defaultEntrySet.Delays.Length && Delays.AsSpan().SequenceEqual(defaultEntrySet.Delays) &&
                EnemySortType == defaultEntrySet.EnemySortType &&
                EntryBaseArray.Count == 0;
        }

        /// <summary>
        /// Delays used for entry bases, when they cannot be provided by fixed entries
        /// </summary>
        public int[] EntryBaseDelays { get; set; } = new int[32];

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("entry_generate");
            {
                xml.WriteElementInt("entry_num", EntryNum);
                xml.WriteElementInt("player_pos", PlayerPos);
                xml.WriteElementValue("generate_type", GenerateType.ToString());
                xml.WriteElementValue("enemy_list_type", EnemyListType.ToString());
                xml.WriteElementValue("race_code", $"0x{RaceCode:x16}"); // Specifically hex string
                xml.WriteElementInt("ai_skill", AISkill);
                xml.WriteElementInt("ai_skill_breaking", AISkillBraking);
                xml.WriteElementInt("ai_skill_cornering", AISkillCornering);
                xml.WriteElementInt("ai_skill_accelerating", AISkillAccelerating);
                xml.WriteElementInt("ai_skill_starting", AISkillStarting);
                xml.WriteElementInt("ai_roughness", AIRoughness);
                xml.WriteElementInt("enemy_lv", EnemyLevel);
                xml.WriteElementInt("enemy_bspec_lv", EnemyBSpecLevel);
                xml.WriteElementInt("bspec_lv_offset", BSpecLevelOffset);
                xml.WriteElementInt("gap_for_start_rolling_distance", GapForStartRollingDistance);
                xml.WriteElementInt("rolling_start_v", RollingStartV);
                xml.WriteElementBool("use_rolling_start_param", UseRollingStartParameter);

                xml.WriteStartElement("cars");
                foreach (var car in Cars)
                {
                    xml.WriteStartElement("car");
                    xml.WriteAttributeString("label", car.CarLabel);
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();


                xml.WriteStartElement("delays");
                foreach (var delay in Delays)
                    xml.WriteElementInt("delay", delay);
                xml.WriteEndElement();

                xml.WriteElementValue("enemy_sort_type", EnemySortType.ToString());

                if (EntryBaseArray.Count > 0)
                {
                    xml.WriteStartElement("entry_base_array");
                    foreach (var entry in EntryBaseArray)
                        entry.WriteToXml(xml);
                    xml.WriteEndElement();
                }
            }
            xml.WriteEndElement();
        }

        public void ParseFromXml(XmlNode node)
        {
            foreach (XmlNode entryGenerateNode in node.ChildNodes)
            {
                switch (entryGenerateNode.Name)
                {
                    case "entry_num":
                        EntryNum = entryGenerateNode.ReadValueInt(); break;
                    case "player_pos":
                        PlayerPos = entryGenerateNode.ReadValueInt(); break;
                    case "generate_type":
                        GenerateType = entryGenerateNode.ReadValueEnum<EntryGenerateType>(); break;
                    case "enemy_list_type":
                        EnemyListType = entryGenerateNode.ReadValueEnum<EnemyListType>(); break;
                    case "race_code":
                        RaceCode = Convert.ToInt64(entryGenerateNode.ReadValueString(), 16); break; // Hex long
                    case "ai_skill":
                        AISkill = entryGenerateNode.ReadValueShort(); break;
                    case "ai_skill_breaking":
                        AISkillBraking = entryGenerateNode.ReadValueShort(); break;
                    case "ai_skill_cornering":
                        AISkillCornering = entryGenerateNode.ReadValueShort(); break;
                    case "ai_skill_accelerating":
                        AISkillAccelerating = entryGenerateNode.ReadValueSByte(); break;
                    case "ai_skill_starting":
                        AISkillStarting = entryGenerateNode.ReadValueSByte(); break;
                    case "ai_roughness":
                        AIRoughness = entryGenerateNode.ReadValueSByte(); break;
                    case "enemy_lv":
                        EnemyLevel = entryGenerateNode.ReadValueInt(); break;
                    case "enemy_bspec_lv":
                        EnemyBSpecLevel = entryGenerateNode.ReadValueSByte(); break;
                    case "bspec_lv_offset":
                        BSpecLevelOffset = entryGenerateNode.ReadValueSByte(); break;
                    case "gap_for_start_rolling_distance":
                        GapForStartRollingDistance = entryGenerateNode.ReadValueShort(); break;
                    case "rolling_start_v":
                        RollingStartV = entryGenerateNode.ReadValueShort(); break;
                    case "use_rolling_start_param":
                        UseRollingStartParameter = entryGenerateNode.ReadValueBool(); break;

                    case "cars":
                        ParseMCarThinList(entryGenerateNode);
                        break;

                    case "delays":
                        int i = 0;
                        foreach (XmlNode delay in entryGenerateNode.SelectNodes("delay"))
                            EntryBaseDelays[i++] = delay.ReadValueInt();
                        break;

                    case "enemy_sort_type":
                        EnemySortType = entryGenerateNode.ReadValueEnum<EnemySortType>(); break;

                    case "entry_base_array":
                        ParseEntryBaseArray(entryGenerateNode);
                        break;
                }
            }

        }

        private void ParseEntryBaseArray(XmlNode node)
        {
            foreach (XmlNode entryBaseNode in node.SelectNodes("entry_base"))
            {
                var entryBase = new EntryBase();
                entryBase.ReadFromXml(entryBaseNode);
                EntryBaseArray.Add(entryBase);
            }
        }

        private void ParseMCarThinList(XmlNode node)
        {
            Cars = new List<MCarThin>();
            foreach (XmlNode vehicleNode in node.SelectNodes("car"))
            {
                string label = vehicleNode.Attributes["label"].Value;
                Cars.Add(new MCarThin(label));
            }
        }

        public void Deserialize(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5_E5_41_14 && magic != 0xE6_E6_41_14)
                throw new System.IO.InvalidDataException($"Course magic did not match - Got {magic.ToString("X8")}, expected 0xE6E64114");

            int version = reader.ReadInt32();

            EntryNum = reader.ReadInt32();
            PlayerPos = reader.ReadInt32();
            GenerateType = (EntryGenerateType)reader.ReadInt32();
            EnemyListType = (EnemyListType)reader.ReadInt32();
            reader.ReadInt64();
            AISkill = (short)reader.ReadInt32();
            AISkillBraking = (short)reader.ReadInt32();
            AISkillCornering = (short)reader.ReadInt32();
            AISkillAccelerating = reader.ReadSByte();
            AISkillStarting = reader.ReadSByte();
            AIRoughness = reader.ReadSByte();
            EnemyLevel = reader.ReadInt32();

            int carThinCount = reader.ReadInt32();
            for (int i = 0; i < carThinCount; i++)
            {
                var carThin = new MCarThin(0);
                carThin.Read(ref reader);
            }

            int bases = reader.ReadInt32();
            for (int i = 0; i < bases; i++)
            {
                EntryBase entry = new EntryBase();
                entry.Deserialize(ref reader);
                EntryBaseArray.Add(entry);
            }

            int delays = reader.ReadInt32();
            EntryBaseDelays = new int[delays];
            for (int i = 0; i < delays; i++)
                EntryBaseDelays[i] = reader.ReadInt32();

            EnemyBSpecLevel = reader.ReadSByte();
            BSpecLevelOffset = reader.ReadSByte();
            GapForStartRollingDistance = reader.ReadInt16();
            RollingStartV = reader.ReadInt16();
            UseRollingStartParameter = reader.ReadBool();
            EnemyListType = (EnemyListType)reader.ReadByte();
        }

        public void Serialize(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_41_14);
            bs.WriteUInt32(1_03);

            bs.WriteInt32(EntryNum);
            bs.WriteInt32(PlayerPos);
            bs.WriteInt32((int)GenerateType);
            bs.WriteInt32((int)EnemyListType);
            bs.WriteInt64(RaceCode);
            bs.WriteInt32(AISkill);
            bs.WriteInt32(AISkillBraking);
            bs.WriteInt32(AISkillCornering);
            bs.WriteSByte(AISkillAccelerating);
            bs.WriteSByte(AISkillStarting);
            bs.WriteSByte(AIRoughness);
            bs.WriteInt32(EnemyLevel);

            bs.WriteInt32(Cars.Count);
            foreach (var car in Cars)
                car.Serialize(ref bs);

            bs.WriteInt32(EntryBaseArray.Count);
            for (int i = 0; i < EntryBaseArray.Count; i++)
                EntryBaseArray[i].Serialize(ref bs);

            bs.WriteInt32(EntryBaseDelays.Length);
            for (int i = 0; i < 32; i++)
                bs.WriteInt32(EntryBaseDelays[i]);

            bs.WriteSByte(EnemyBSpecLevel);
            bs.WriteSByte(BSpecLevelOffset);
            bs.WriteInt16(GapForStartRollingDistance);
            bs.WriteInt16(RollingStartV);
            bs.WriteBool(UseRollingStartParameter);
            bs.WriteSByte((sbyte)EnemySortType);
        }

    }
}
