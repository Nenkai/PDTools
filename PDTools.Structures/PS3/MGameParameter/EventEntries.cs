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
    public class EventEntries
    {
        /* Major Note:
         * Entry Bases and Entries can have different data. 
         * Most notably, Entries have a fixed car parameter when serialized, a driver parameter list aswell
         * It also has the initial position/delay/velocity etc.
         * Entry Base has the tuning settings.
         * 
         * Entries can refer to a child Base Entry.
         * 
         * We represent both as the same class "EventEntry" for simplicity.
         */
        public List<EventEntry> AI { get; set; } = new List<EventEntry>();
        public List<EventEntry> AIBases { get; set; } = new List<EventEntry>();
        public EventEntry Player { get; set; }

        private int aisToPickFromPool;
        public int AIsToPickFromPool
        {
            get => aisToPickFromPool;
            set
            {
                if (value > 16)
                    value = 16;

                aisToPickFromPool = value;
            }
        }

        public int PlayerPos { get; set; } = 1;
        public short RollingStartV { get; set; }
        public short GapForRollingDistance { get; set; }

        public sbyte AIRoughness { get; set; } = -1;
        public short AIBaseSkillStarting { get; set; } = 80;
        public short AICornerSkillStarting { get; set; } = 80;
        public short AIBrakingSkillStarting { get; set; } = 80;
        public sbyte AIAccelSkillStarting { get; set; } = 80;
        public sbyte AIStartSkillStarting { get; set; } = 80;

        public int EnemyLevel { get; set; }
        public sbyte EnemyBSpecLevel { get; set; }
        public sbyte BSpecLevelOffset { get; set; }

        public EntryGenerateType AIEntryGenerateType { get; set; } = EntryGenerateType.ENTRY_BASE_SHUFFLE;
        public EnemySortType AISortType { get; set; } = EnemySortType.NONE;
        public EnemyListType EnemyListType { get; set; }

        public bool NeedsPopulating { get; set; } = true;

        /// <summary>
        /// Delays used for entry bases, when they cannot be provided by fixed entries
        /// </summary>
        public int[] EntryBaseDelays { get; set; } = new int[32];

        public void WriteToXml(XmlWriter xml)
        {
            if (AI.Count == 0 && AIBases.Count == 0 && Player is null)
                return;

            xml.WriteStartElement("entry_set");
            bool hasGeneratedAI = AIBases.Count != 0 && AIsToPickFromPool != 0;
            if (hasGeneratedAI || AI.Count != 0) // entry_generate is also present if using <entry>
            {
                xml.WriteStartElement("entry_generate");
                {
                    xml.WriteStartElement("delays");
                    if (EntryBaseDelays.Any(e => e != 0))
                    {
                        foreach (var delay in EntryBaseDelays)
                            xml.WriteElementInt("delay", delay);
                    }
                    xml.WriteEndElement();

                    if (AIBases.Count < AIsToPickFromPool)
                        AIsToPickFromPool = AIBases.Count;

                    xml.WriteElementInt("entry_num", AI.Count + AIsToPickFromPool + 1); // + 1 as it includes player
                    xml.WriteElementInt("player_pos", PlayerPos - 1);

                    xml.WriteElementValue("generate_type", AIEntryGenerateType.ToString());
                    xml.WriteElementValue("enemy_sort_type", AISortType.ToString());


                    if (GapForRollingDistance != 0 || RollingStartV != 0)
                    {
                        xml.WriteElementBool("use_rolling_start_param", GapForRollingDistance != 0 || RollingStartV != 0);
                        xml.WriteElementInt("rolling_start_v", RollingStartV);
                        xml.WriteElementInt("gap_for_start_rolling_distance", GapForRollingDistance);
                    }

                    if (hasGeneratedAI)
                    {
                        xml.WriteElementInt("ai_roughness", AIRoughness);
                        xml.WriteElementInt("ai_skill_starting", AIBases.Max(x => x.BaseSkill));
                        xml.WriteElementInt("ai_skill", AIBaseSkillStarting);
                        xml.WriteElementInt("ai_skill_breaking", AIBrakingSkillStarting);
                        xml.WriteElementInt("ai_skill_cornering", AICornerSkillStarting);
                        xml.WriteElementInt("ai_skill_accelerating", AIAccelSkillStarting);
                        xml.WriteElementInt("ai_skill_starting", AIStartSkillStarting);
                    }

                    if (AIBases.Count > 0)
                    {
                        xml.WriteStartElement("entry_base_array");
                        {
                            foreach (var ai in AIBases)
                            {
                                ai.IsAI = true; // Just to make sure
                                ai.WriteToXml(xml, false);
                            }
                        }
                        xml.WriteEndElement();
                    }
                }
                xml.WriteEndElement();

                foreach (var fixedAI in AI)
                    fixedAI.WriteToXml(xml, true);
            }

            if (Player != null)
                Player.WriteToXml(xml, true);

            xml.WriteEndElement();
        }

        public void ParseRaceEntrySet(XmlNode node)
        {
            AIBases = new List<EventEntry>();
            foreach (XmlNode entrySetNode in node.ChildNodes)
            {
                if (entrySetNode.Name == "entry_generate")
                {
                    foreach (XmlNode entryGenerateNode in entrySetNode)
                    {
                        switch (entryGenerateNode.Name)
                        {
                            case "ai_skill":
                                AIBaseSkillStarting = entryGenerateNode.ReadValueShort(); break;
                            case "ai_skill_breaking":
                                AIBrakingSkillStarting = entryGenerateNode.ReadValueShort(); break;
                            case "ai_skill_cornering":
                                AICornerSkillStarting = entryGenerateNode.ReadValueShort(); break;
                            case "ai_skill_accelerating":
                                AIAccelSkillStarting = entryGenerateNode.ReadValueSByte(); break;
                            case "ai_skill_starting":
                                AIStartSkillStarting = entryGenerateNode.ReadValueSByte(); break;
                            case "rolling_start_v":
                                RollingStartV = entryGenerateNode.ReadValueShort(); break;

                            case "entry_base_array":
                                foreach (XmlNode entryBaseNode in entryGenerateNode.SelectNodes("entry_base"))
                                {
                                    var newEntry = ParseEntry(entryBaseNode);
                                    AIBases.Add(newEntry);
                                }
                                break;

                            case "enemy_lv":
                                EnemyLevel = entryGenerateNode.ReadValueInt(); break;
                            case "enemy_bspec_lv":
                                EnemyBSpecLevel = entryGenerateNode.ReadValueSByte(); break;
                            case "bspec_lv_offset":
                                BSpecLevelOffset = entryGenerateNode.ReadValueSByte(); break;

                            case "entry_num":
                                AIsToPickFromPool = entryGenerateNode.ReadValueInt(); break;

                            case "player_pos":
                                PlayerPos = entryGenerateNode.ReadValueInt() + 1; break;

                            case "enemy_sort_type":
                                AISortType = entryGenerateNode.ReadValueEnum<EnemySortType>(); break;

                            case "enemy_list_type":
                                EnemyListType = entryGenerateNode.ReadValueEnum<EnemyListType>(); break;

                            case "delays":
                                int i = 0;
                                foreach (XmlNode delay in entryGenerateNode.SelectNodes("delay"))
                                    EntryBaseDelays[i++] = delay.ReadValueInt();
                                break;

                            case "generate_type":
                                AIEntryGenerateType = entryGenerateNode.ReadValueEnum<EntryGenerateType>(); break;

                            case "gap_for_start_rolling_distance":
                                GapForRollingDistance = entryGenerateNode.ReadValueShort(); break;

                        }
                    }
                }
                else if (entrySetNode.Name == "entry")
                {
                    var newEntry = ParseEntry(entrySetNode);
                    if (!newEntry.IsAI && !string.IsNullOrEmpty(newEntry.CarLabel))
                    {
                        if (!string.IsNullOrEmpty(newEntry.CarLabel))
                        {
                            Player = newEntry;
                            Player?.SetAsPlayerSkills();
                        }
                    }
                    else
                        AI.Add(newEntry);
                }
            }


        }

        public EventEntry ParseEntry(XmlNode entryNode, EventEntry parentEntry = null)
        {
            var newEntry = parentEntry ?? new EventEntry();
            foreach (XmlNode entryDetailNode in entryNode)
            {
                switch (entryDetailNode.Name)
                {
                    case "driver_name":
                        newEntry.DriverName = entryDetailNode.ReadValueString();
                        break;

                    case "driver_region":
                        newEntry.DriverRegion = entryDetailNode.ReadValueString();
                        break;

                    case "car":
                        newEntry.ColorIndex = short.Parse(entryDetailNode.Attributes["color"].Value);
                        newEntry.CarLabel = entryDetailNode.Attributes["label"].Value;
                        break;

                    case "race_class_id":
                        newEntry.RaceClassID = entryDetailNode.ReadValueByte(); break;

                    case "delay":
                        newEntry.Delay = entryDetailNode.ReadValueInt(); break;


                    case "ai_skill":
                        newEntry.BaseSkill = entryDetailNode.ReadValueShort(); break;
                    case "ai_skill_breaking":
                        newEntry.BrakingSkill = entryDetailNode.ReadValueShort(); break;
                    case "ai_skill_cornering":
                        newEntry.CorneringSkill = entryDetailNode.ReadValueShort(); break;
                    case "ai_skill_accelerating":
                        newEntry.AccelSkill = entryDetailNode.ReadValueSByte(); break;
                    case "ai_skill_starting":
                        newEntry.StartingSkill = entryDetailNode.ReadValueSByte(); break;
                    case "ai_roughness":
                        newEntry.Roughness = entryDetailNode.ReadValueSByte(); break;

                    case "entry_base": // For fixed entries with a child entry_base.
                        ParseEntry(entryDetailNode, newEntry); break;

                    case "initial_velocity":
                        newEntry.InitialVelocity = entryDetailNode.ReadValueInt(); break;
                    case "initial_position":
                        newEntry.InitialVCoord = entryDetailNode.ReadValueInt(); break;

                    case "engine_na_tune_stage":
                        newEntry.EngineStage = entryDetailNode.ReadValueEnum<EngineNATuneState>(); break;
                    case "engine_turbo_kit":
                        newEntry.TurboKit = entryDetailNode.ReadValueEnum<EngineTurboKit>(); break;
                    case "engine_computer":
                        newEntry.Computer = entryDetailNode.ReadValueEnum<EngineComputer>(); break;
                    case "muffler":
                        newEntry.Exhaust = entryDetailNode.ReadValueEnum<Muffler>(); break;
                    case "suspension":
                        newEntry.Suspension = entryDetailNode.ReadValueEnum<Suspension>(); break;
                    case "transmission":
                        newEntry.Transmission = entryDetailNode.ReadValueEnum<Transmission>(); break;

                    case "power_limiter":
                        newEntry.PowerLimiter = entryDetailNode.ReadValueUInt(); break;
                    case "wheel":
                        newEntry.WheelID = entryDetailNode.ReadValueInt(); break;
                    case "wheel_color":
                        newEntry.WheelPaintID = entryDetailNode.ReadValueShort(); break;
                    case "wheel_inch_up":
                        newEntry.WheelInchUp = entryDetailNode.ReadValueInt(); break;
                    case "ballast_weight":
                        newEntry.BallastWeight = entryDetailNode.ReadValueByte(); break;
                    case "ballast_position":
                        newEntry.BallastPosition = entryDetailNode.ReadValueSByte(); break;
                    case "downforce_f":
                        newEntry.DownforceFront = entryDetailNode.ReadValueSByte(); break;
                    case "downforce_r":
                        newEntry.DownforceRear = entryDetailNode.ReadValueSByte(); break;
                    case "paint_id":
                        newEntry.BodyPaintID = entryDetailNode.ReadValueShort(); break;
                    case "aero_1":
                        newEntry.AeroKit = entryDetailNode.ReadValueInt(); break;
                    case "aero_2":
                        newEntry.FlatFloor = entryDetailNode.ReadValueInt(); break;
                    case "aero_3":
                        newEntry.AeroOther = entryDetailNode.ReadValueInt(); break;
                    case "gear_max_speed":
                        newEntry.MaxGearSpeed = entryDetailNode.ReadValueInt(); break;

                    case "tire_f":
                        newEntry.TireFront = (TireType)Enum.Parse(typeof(TireType), entryDetailNode.ReadValueString());
                        break;

                    case "tire_r":
                        newEntry.TireRear = (TireType)Enum.Parse(typeof(TireType), entryDetailNode.ReadValueString());
                        break;

                    case "player_no":
                        newEntry.IsAI = entryDetailNode.ReadValueInt() == -1;
                        break;
                }
            }

            return newEntry;
        }

        public void ReadFromCache(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5_E5_00_2F && magic != 0xE6_E6_00_2F)
                throw new System.IO.InvalidDataException($"Course magic did not match - Got {magic.ToString("X8")}, expected 0xE6E6002F");

            int version = reader.ReadInt32();
            ReadEntryGenerateFromCache(ref reader);

            int entries = reader.ReadInt32();
            for (int i = 0; i < entries; i++)
            {
                EventEntry entry = new EventEntry();
                entry.ReadEntryFromCache(ref reader);
                if (entry.IsAI)
                    AI.Add(entry);
                else
                    Player = entry;
            }
        }

        public void ReadEntryGenerateFromCache(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5_E5_41_14 && magic != 0xE6_E6_41_14)
                throw new System.IO.InvalidDataException($"Course magic did not match - Got {magic.ToString("X8")}, expected 0xE6E64114");

            int version = reader.ReadInt32();

            AIsToPickFromPool = reader.ReadInt32() - 1;
            PlayerPos = reader.ReadInt32();
            AIEntryGenerateType = (EntryGenerateType)reader.ReadInt32();
            EnemyListType = (EnemyListType)reader.ReadInt32();
            reader.ReadInt64();
            AIBaseSkillStarting = (short)reader.ReadInt32();
            AIBrakingSkillStarting = (short)reader.ReadInt32();
            AICornerSkillStarting = (short)reader.ReadInt32();
            AIAccelSkillStarting = reader.ReadSByte();
            AIStartSkillStarting = reader.ReadSByte();
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
                EventEntry entry = new EventEntry();
                entry.ReadEntryBaseFromCache(ref reader);
                AIBases.Add(entry);
            }

            int delays = reader.ReadInt32();
            EntryBaseDelays = new int[delays];
            for (int i = 0; i < delays; i++)
                EntryBaseDelays[i] = reader.ReadInt32();

            EnemyBSpecLevel = reader.ReadSByte();
            BSpecLevelOffset = reader.ReadSByte();
            GapForRollingDistance = reader.ReadInt16();
            RollingStartV = reader.ReadInt16();
            reader.ReadBool(); // Rolling start param
            EnemyListType = (EnemyListType)reader.ReadByte();
        }

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_00_2F);
            bs.WriteUInt32(1_00);

            WriteEntryGenerateToBuffer(ref bs);

            bool hasGeneratedAI = AIBases.Count != 0 && AIsToPickFromPool != 0;
            if (!hasGeneratedAI)
            {
                int fixEntryCount = AI.Count + 1;
                bs.WriteInt32(fixEntryCount);
                if (fixEntryCount > 0)
                {
                    foreach (var ai in AI)
                        ai.WriteEntryToBuffer(ref bs, false);

                    var player = Player ?? new EventEntry(true);
                    player.WriteEntryToBuffer(ref bs, true);
                }
            }
            else
                bs.WriteInt32(0);
        }

        private void WriteEntryGenerateToBuffer(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_41_14);
            bs.WriteUInt32(1_03);

            bs.WriteInt32(AIsToPickFromPool + 1); // entry_num - must account for player
            bs.WriteInt32(AIBases.Count > 0 ? PlayerPos - 1 : 0);
            bs.WriteInt32(AIBases.Count > 0 ? (int)AIEntryGenerateType : (int)EntryGenerateType.NONE);
            bs.WriteInt32((int)EnemyListType);
            bs.WriteUInt64(4294967295); // race_code
            bs.WriteInt32(AIBaseSkillStarting);
            bs.WriteInt32(AIBrakingSkillStarting);
            bs.WriteInt32(AICornerSkillStarting);
            bs.WriteSByte(AIAccelSkillStarting);
            bs.WriteSByte(AIStartSkillStarting);
            bs.WriteSByte(AIRoughness);
            bs.WriteInt32(EnemyLevel); // enemy_lv

            // list of cars (as carthin) - dunno how its used, we'll just write an empty list
            bs.WriteInt32(0);

            bs.WriteInt32(AIBases.Count);
            for (int i = 0; i < AIBases.Count; i++)
                AIBases[i].WriteEntryBaseToBuffer(ref bs);

            bs.WriteInt32(EntryBaseDelays.Length);
            for (int i = 0; i < 32; i++)
                bs.WriteInt32(EntryBaseDelays[i]);

            bs.WriteSByte(EnemyBSpecLevel); // enemy_bspec_lv
            bs.WriteSByte(BSpecLevelOffset); // bspec_lv_offset
            bs.WriteInt16(GapForRollingDistance);
            bs.WriteInt16(RollingStartV);
            bs.WriteBool(GapForRollingDistance != 0 || RollingStartV != 0); // Rolling start param
            bs.WriteSByte((sbyte)AISortType);
        }

    }


    public enum EntryGenerateType
    {
        [Description("None (Pool Ignored, use for fixed entries)")]
        NONE = 0,

        [Description("Shuffle - Depends on entry_generate->cars XML node array")]
        SHUFFLE = 1,

        [Description("One Make - Depends on the player's car")]
        ONE_MAKE = 2,

        [Description("Enemy List - Randomly Chosen, Type depends on EnemyListType")]
        ENEMY_LIST = 3,

        [Description("SpecDB (Do not use - unimplemented)")]
        SPEC_DB = 4,

        [Description("Order - Depends on entry_generate->cars XML node array")]
        ORDER = 5,

        [Description("Shuffle and Randomly Pick (Default)")]
        ENTRY_BASE_SHUFFLE = 6,

        [Description("Pick entries by Order")]
        ENTRY_BASE_ORDER = 7,
    }

    public enum EnemySortType
    {
        [Description("No sorting")]
        NONE,

        [Description("Sort selected race entries by Ascending PP")]
        PP_ASCEND,

        [Description("Sort selected race entries by Descending PP")]
        PP_DESCEND,
    }

    public enum EnemyListType
    {
        SAME,
        MIX,
        ONLY_PREMIUM,
        ONLY_STANDARD,
    }
}
