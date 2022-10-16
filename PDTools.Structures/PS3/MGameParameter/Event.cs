using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;

using PDTools.Utils;

namespace PDTools.Structures.PS3.MGameParameter
{
    public class Event
    {
        public EventConstraints Constraints { get; set; }
        public EventInformation Information { get; set; }
        public EventRewards Rewards { get; set; }
        public EventRaceParameters RaceParameters { get; set; }
        public EventRegulations Regulations { get; set; }
        public EventEntries Entries { get; set; }
        public EventCourse Course { get; set; }
        public EventEvalConditions EvalConditions { get; set; }
        public EventPlayStyle PlayStyle { get; set; }
        public EventFailureConditions FailConditions { get; set; }
        public EventAchieveCondition AchieveCondition { get; set; }
        public EventLicenseCondition LicenseCondition { get; set; }
        public EventDriftCondition DriftCondition { get; set; }
        public EventReplay Replay { get; set; }
        public EventStageData StageData { get; set; }
        public EventArcadeStyleSetting ArcadeStyleSettings { get; set; }

        public string AIScript { get; set; }
        public string PenaltyScript { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public RankingDisplayType RankingDisplayType { get; set; } = RankingDisplayType.NONE;
        public DateTime RankingStartDate { get; set; }
        public DateTime RankingEndDate { get; set; }

        public ulong EventID { get; set; } = GameParameter.BaseEventID;
        public int Index { get; set; }

        private string _name { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
            }
        }

        public GameMode GameMode { get; set; } = GameMode.EVENT_RACE;
        public EventType EventType { get; set; } = EventType.RACE;
        public bool Inheritance { get; set; }

        public int PlayerPos { get; set; }

        public int[] MoneyPrizes { get; set; } = new int[16];

        public bool IsSeasonalEvent { get; set; }
        public string PenaltyScriptName { get; set; }
        public string AIScriptName { get; set; }

        public Event()
        {
            Constraints = new EventConstraints();
            Information = new EventInformation();
            Rewards = new EventRewards();
            RaceParameters = new EventRaceParameters();
            Regulations = new EventRegulations();
            Entries = new EventEntries();
            Course = new EventCourse();
            EvalConditions = new EventEvalConditions();
            PlayStyle = new EventPlayStyle();
            FailConditions = new EventFailureConditions();
            AchieveCondition = new EventAchieveCondition();
            LicenseCondition = new EventLicenseCondition();
            DriftCondition = new EventDriftCondition();
            Replay = new EventReplay();
            StageData = new EventStageData();
            ArcadeStyleSettings = new EventArcadeStyleSetting();

            MoneyPrizes[0] = 25_000;
            MoneyPrizes[1] = 12_750;
            MoneyPrizes[2] = 7_500;
            MoneyPrizes[3] = 5_000;
            MoneyPrizes[4] = 2_500;
            MoneyPrizes[5] = 1_000;
            for (int i = 6; i < 16; i++)
                MoneyPrizes[i] = -1;
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteComment($"{EventID} - {Name}");
            xml.WriteStartElement("event");
            {
                AchieveCondition.WriteToXml(xml);
                Constraints.WriteToXml(xml);
                Entries.WriteToXml(xml);
                FailConditions.WriteToXml(xml);
                xml.WriteElementULong("event_id", EventID);
                xml.WriteElementValue("event_type", EventType.ToString());
                xml.WriteElementValue("game_mode", GameMode.ToString());
                Information.WriteToXml(xml);
                xml.WriteElementBool("inheritance", false);
                RaceParameters.WriteToXml(this, xml);
                Rewards.WriteToXml(xml);
                Course.WriteToXml(xml);
                PlayStyle.WriteToXml(xml);
                Regulations.WriteToXml(xml);
                xml.WriteElementValue("penalty_script", PenaltyScriptName);
                xml.WriteElementValue("ai_script", AIScriptName);
                EvalConditions.WriteToXml(xml);

                xml.WriteElementBool("inheritance", Inheritance);
                xml.WriteElementBool("is_seasonal_event", IsSeasonalEvent);
                xml.WriteStartElement("begin_date"); xml.WriteString(StartDate.ToString("yyyy/MM/dd HH:mm:ss")); xml.WriteEndElement();
                xml.WriteStartElement("end_date"); xml.WriteString(EndDate.ToString("yyyy/MM/dd HH:mm:ss")); xml.WriteEndElement();

                if (GameMode == GameMode.ARCADE_STYLE_RACE)
                    ArcadeStyleSettings.WriteToXml(xml);

                if (IsSeasonalEvent && RankingDisplayType != RankingDisplayType.NONE)
                {
                    xml.WriteStartElement("ranking");
                    xml.WriteStartElement("begin_date"); xml.WriteString(RankingStartDate.ToString("yyyy/MM/dd HH:mm:ss")); xml.WriteEndElement();
                    xml.WriteStartElement("end_date"); xml.WriteString(RankingEndDate.ToString("yyyy/MM/dd HH:mm:ss")); xml.WriteEndElement();
                    xml.WriteElementValue("type", RankingDisplayType.ToString());
                    xml.WriteElementULong("board_id", EventID);
                    xml.WriteElementInt("display_rank_limit", -1);
                    xml.WriteElementBool("is_local", false);
                    xml.WriteElementInt("replay_rank_limit", 0);
                    xml.WriteElementInt("registration", 0);
                    xml.WriteEndElement();
                }
            }
            xml.WriteEndElement();
        }

        public void ReadFromCache(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5E5043D && magic != 0xE6E6043D)
                throw new System.IO.InvalidDataException($"Event magic did not match - Got {magic.ToString("X8")}, expected 0xE6E6043D");

            uint eventVersion = reader.ReadUInt32();
            EventID = reader.ReadUInt64();
            GameMode = (GameMode)reader.ReadInt32();
            PlayStyle.ReadFromCache(ref reader);

            EventType = (EventType)reader.ReadInt32();
            Inheritance = reader.ReadBool();
            IsSeasonalEvent = reader.ReadBool();
            reader.ReadInt16();
            Regulations.ReadFromCache(ref reader);
            Constraints.ReadFromCache(ref reader);
            RaceParameters.ReadFromCache(ref reader);
            Course.ReadFromCache(ref reader);
            Entries.ReadFromCache(ref reader);
            EvalConditions.ReadFromCache(ref reader);
            AchieveCondition.ReadFromCache(ref reader);
            FailConditions.ReadFromCache(ref reader);
        }

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_04_3D);
            bs.WriteUInt32(1_06); // Version
            bs.WriteUInt64(EventID);
            bs.WriteInt32((int)GameMode);
            PlayStyle.WriteToCache(ref bs);
            bs.WriteInt32((int)EventType);
            bs.WriteBool(Inheritance);
            bs.WriteBool(IsSeasonalEvent);
            bs.WriteInt16(0); // field_0x2a - Defaulted to 0
            Regulations.WriteToCache(ref bs);
            Constraints.WriteToCache(ref bs);
            RaceParameters.WriteToCache(ref bs, this);
            Course.WriteToCache(ref bs);
            Entries.WriteToCache(ref bs);
            EvalConditions.WriteToCache(ref bs);
            AchieveCondition.WriteToCache(ref bs);
            FailConditions.WriteToCache(ref bs);
            LicenseCondition.WriteToCache(ref bs);
            DriftCondition.WriteToCache(ref bs);
            Rewards.WriteToCache(ref bs);
            WriteRankingsToBuffer(ref bs);
            Replay.WriteToCache(ref bs);
            Information.WriteToCache(ref bs);
            bs.WriteDouble(PDIDATETIME_Julian.DateTimeToJulian(StartDate));
            bs.WriteDouble(PDIDATETIME_Julian.DateTimeToJulian(EndDate));
            StageData.WriteToCache(ref bs);
            bs.WriteNullStringAligned4(PenaltyScript);
            bs.WriteNullStringAligned4(AIScript);
            ArcadeStyleSettings.WriteToCache(ref bs);
        }

        private void WriteRankingsToBuffer(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_6D_E9);
            bs.WriteUInt32(1_01); // Version

            bs.WriteInt16((short)RankingDisplayType);
            bs.WriteInt16(1); // is_local
            bs.WriteInt16(0); // replay_rank_limit
            bs.WriteInt16(100); // display_rank_limit
            bs.WriteUInt64(EventID); // board_id
            bs.WriteDouble(PDIDATETIME_Julian.DateTimeToJulian(RankingStartDate)); // begin_date todo
            bs.WriteDouble(PDIDATETIME_Julian.DateTimeToJulian(RankingEndDate)); // end_date todo
            bs.WriteInt16(0); // registration
            bs.WriteSByte(0); // registration_type
        }

        public void ParseFromXml(XmlNode eventNode)
        {
            foreach (XmlNode node in eventNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "constraint":
                        Constraints.ParseRaceConstraints(node);
                        break;

                    case "entry_set":
                        Entries.ParseRaceEntrySet(node);
                        foreach (XmlNode entryNode in node.SelectNodes("entry_generate")) // entry_generate
                        {
                            if (entryNode.Name == "player_pos")
                            {
                                PlayerPos = entryNode.ReadValueInt() + 1;
                                break;
                            }
                        }
                        break;

                    case "game_mode":
                        GameMode = node.ReadValueEnum<GameMode>(); break;

                    case "event_type":
                        EventType = node.ReadValueEnum<EventType>(); break;

                    case "event_id":
                        EventID = node.ReadValueULong();
                        break;

                    case "information":
                        Information.ParseRaceInformation(this, node);
                        break;

                    case "race":
                        RaceParameters.ParseRaceData(node);
                        break;

                    case "reward":
                        Rewards.ParseRaceRewards(this, node);
                        break;
                    case "eval_condition":
                        EvalConditions.ParseEvalConditionData(node);
                        break;

                    case "track":
                        Course.ReadEventCourse(node);
                        break;

                    case "regulation":
                        Regulations.ParseRegulations(node);
                        break;
                    case "play_style":
                        PlayStyle.ParsePlayStyle(node);
                        break;

                    case "failure_condition":
                        FailConditions.ParseFailConditions(node);
                        break;

                    case "achieve_condition":
                        AchieveCondition.ParseAchieveCondition(node);
                        break;

                    case "is_seasonal_event":
                        IsSeasonalEvent = node.ReadValueBool();
                        break;

                    case "arcade_style_setting":
                        ArcadeStyleSettings.ParseArcadeStyleSetting(node);
                        break;

                    case "inheritance":
                        Inheritance = node.ReadValueBool();
                        break;

                    case "stage_data":
                        StageData.ParseStageData(node);
                        break;

                    case "begin_date":
                        string date = node.InnerText.Replace("/00", "/01");
                        DateTime.TryParseExact(date, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time);
                        StartDate = time;
                        break;
                    case "end_date":
                        string eDate = node.InnerText.Replace("/00", "/01");
                        DateTime.TryParseExact(eDate, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime eTime);
                        EndDate = eTime;
                        break;

                    case "ranking":
                        ParseRankingData(node);
                        break;
                }
            }
        }

        /// <summary>
        /// Update some invalid nodes that can only be fixed once the event is fully loaded.
        /// </summary>
        public void FixInvalidNodesIfNeeded()
        {
            int freeSlots = GetFreeCarSlotsLeft();
            if (Entries.AIsToPickFromPool > freeSlots)
                Entries.AIsToPickFromPool = freeSlots;

            // Ensure we can set it from the actual list
            RaceParameters.WeatherPointNum = (byte)RaceParameters.NewWeatherData.Count;
        }

        public void ParseRankingData(XmlNode node)
        {
            foreach (XmlNode rNode in node.ChildNodes)
            {
                switch (rNode.Name)
                {
                    case "begin_date":
                        string date = rNode.InnerText.Replace("/00", "/01");
                        DateTime.TryParseExact(date, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time);
                        RankingStartDate = time;
                        break;
                    case "end_date":
                        string eDate = rNode.InnerText.Replace("/00", "/01");
                        DateTime.TryParseExact(eDate, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime eTime);
                        RankingEndDate = eTime;
                        break;
                    case "type":
                        RankingDisplayType = rNode.ReadValueEnum<RankingDisplayType>();
                        break;
                }
            }
        }

        public int GetFreeCarSlotsLeft()
        {
            return RaceParameters.RacersMax - GetTotalEntries();
        }

        public int GetTotalEntries()
        {
            return Entries.AI.Count + 1;
        }

        public void MarkUnpopulated()
        {
            Constraints.NeedsPopulating = true;
            Rewards.NeedsPopulating = true;
            RaceParameters.NeedsPopulating = true;
            Regulations.NeedsPopulating = true;
            Entries.NeedsPopulating = true;
            Course.NeedsPopulating = true;
            EvalConditions.NeedsPopulating = true;
            Information.NeedsPopulating = true;
            ArcadeStyleSettings.NeedsPopulating = true;
        }
    }

    public enum GameMode
    {
        [Description("Arcade Race")]
        SINGLE_RACE = 0,

        [Description("Arcade Time Trial")]
        TIME_ATTACK = 1,

        [Description("Arcade Drift Attack")]
        DRIFT_ATTACK = 2,

        [Description("Free Run")]
        FREE_RUN = 3,

        [Description("GT Mode Race")]
        EVENT_RACE = 4,

        [Description("Rally Event (GT5)")]
        EVENT_RALLY = 5,

        [Description("Split Battle")]
        SPLIT_BATTLE = 6,

        [Description("Split Battle (Online)")]
        SPLIT_ONLINE_BATTLE = 7,

        [Description("Online Room")]
        ONLINE_ROOM = 8,

        [Description("Online Battle")]
        ONLINE_BATTLE = 9,

        [Description("Seasonal Time Trial")]
        ONLINE_TIME_ATTACK = 10,

        [Description("License")]
        LICENSE = 11,

        [Description("Adhoc Battle Pro (PSP)")]
        ADHOC_BATTLE_PRO = 12,

        [Description("Adhoc Battle Ama (PSP)")]
        ADHOC_BATTLE_AMA = 13,

        [Description("Adhoc Battle Shuffle (PSP)")]
        ADHOC_BATTLE_SHUFFLE = 14,

        [Description("Multimonitor Client")]
        MULTIMONITOR_CLIENT = 15,

        [Description("Behavior")]
        BEHAVIOR = 16,

        [Description("Race Edit (GT5)")]
        RACE_EDIT = 17,

        [Description("Ranking View")]
        RANKING_VIEW = 18,

        [Description("Track Test (GT6)")]
        COURSE_EDIT = 19,

        [Description("Special Event (GT5 School)")]
        SCHOOL = 20,

        [Description("Arena")]
        ARENA = 21,

        [Description("Tour (GT5)")]
        TOUR = 22,

        [Description("Speed Test (GT5)")]
        SPEED_TEST = 23,

        [Description("Course Maker (GT5)")]
        COURSE_MAKER = 24,

        [Description("Drag Race (GT6, 2P only)")]
        DRAG_RACE = 25,

        [Description("Tutorial (GT6)")]
        TUTORIAL = 26,

        [Description("Mission")]
        MISSION = 27,

        [Description("Coffee Break (GT6)")]
        COFFEE_BREAK = 28,

        [Description("Seasonal Drift Attack")]
        ONLINE_DRIFT_ATTACK = 29,

        [Description("GPS Replay")]
        GPS_REPLAY = 30,

        [Description("Seasonal Race")]
        ONLINE_SINGLE_RACE = 31,

        [Description("Sierra/Arcade Style/Overtake Mission (GT6)")]
        ARCADE_STYLE_RACE = 32,

        [Description("Practice (GT6)")]
        PRACTICE = 33,

    }

    public enum EventType
    {
        RACE,
        RACE_WITH_QUALIFY,
        TRACKDAY,
    }

    public enum RankingDisplayType
    {
        [Description("No Rankings")]
        NONE,

        [Description("By Best Time")]
        TIME,

        [Description("By Drift Score")]
        DRIFT,
    }

}
