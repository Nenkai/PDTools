using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Xml;

using PDTools.Utils;
using PDTools.Enums;

namespace PDTools.Structures.MGameParameter
{
    public class Event
    {
        /// <summary>
        /// Unique identifier for this game event.
        /// </summary>
        public ulong EventID { get; set; } = 0;

        /// <summary>
        /// Game Mode for this event. Defaults to <see cref="GameMode.SINGLE_RACE"/>.
        /// </summary>
        public GameMode GameMode { get; set; } = GameMode.SINGLE_RACE;

        /// <summary>
        /// A-BSpec settings among other demo settings.
        /// </summary>
        public PlayStyle PlayStyle { get; set; } = new PlayStyle();

        public EventType EventType { get; set; } = EventType.RACE;
        public bool Inheritance { get; set; }
        public bool IsSeasonalEvent { get; set; }

        /// <summary>
        /// Regulations - conditions to enter the event.
        /// </summary>
        public Regulation Regulation { get; set; } = new Regulation();

        /// <summary>
        /// Constraints - Limitations upon entries while playing the event.
        /// </summary>
        public Constraint Constraint { get; set; } = new Constraint();

        /// <summary>
        /// All race parameters.
        /// </summary>
        public RaceParameter RaceParameter { get; set; } = new RaceParameter();

        /// <summary>
        /// Track/Course the event occurs on.
        /// </summary>
        public Track Track { get; set; } = new Track();

        /// <summary>
        /// Entry Set - All defined entries for the event.
        /// </summary>
        public EntrySet EntrySet { get; set; } = new EntrySet();

        /// <summary>
        /// Reward evaluation conditions.
        /// </summary>
        public EvalCondition EvalCondition { get; set; } = new EvalCondition();

        public AchieveCondition AchieveCondition { get; set; } = new AchieveCondition();

        public LicenseCondition LicenseCondition { get; set; } = new LicenseCondition();

        public FailureCondition FailureCondition { get; set; } = new FailureCondition();

        /// <summary>
        /// Rewards for this event.
        /// </summary>
        public Reward Reward { get; set; } = new Reward();

        /// <summary>
        /// Ranking settings for this event, for seasonals and other competitions.
        /// </summary>
        public Ranking Ranking { get; set; } = new Ranking();

        public Replay Replay { get; set; } = new Replay();

        public Information Information { get; set; } = new Information();

        /// <summary>
        /// When this event is available (seasonals).
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// When this event ends (seasonals).
        /// </summary>
        public DateTime? EndDate { get; set; }

        public StageData StageData { get; set; } = new StageData();

        public DriftCondition DriftCondition { get; set; } = new DriftCondition();

        /// <summary>
        /// Arcade style settings when <see cref="GameMode"/> is set to <see cref="GameMode.ARCADE_STYLE_RACE"/>.
        /// </summary>
        public ArcadeStyleSetting ArcadeSetting { get; set; } = new ArcadeStyleSetting();

        public string AIScript { get; set; }
        public string PenaltyScript { get; set; }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("event");
            {
                xml.WriteElementULong("event_id", EventID);
                xml.WriteElementValue("game_mode", GameMode.ToString());

                if (!PlayStyle.IsDefault())
                {
                    xml.WriteStartElement("play_style");
                    PlayStyle.WriteToXml(xml);
                    xml.WriteEndElement();
                }

                xml.WriteElementValue("event_type", EventType.ToString());
                xml.WriteElementBool("inheritance", Inheritance);
                xml.WriteElementBool("is_seasonal_event", IsSeasonalEvent);

                if (!Regulation.IsDefault())
                {
                    xml.WriteStartElement("regulation");
                    Regulation.WriteToXml(xml);
                    xml.WriteEndElement();
                }

                if (!Constraint.IsDefault())
                {
                    xml.WriteStartElement("constraint");
                    Constraint.WriteToXml(xml);
                    xml.WriteEndElement();
                }

                if (!ArcadeSetting.IsDefault())
                {
                    xml.WriteStartElement("arcade_style_setting");
                    ArcadeSetting.WriteToXml(xml);
                    xml.WriteEndElement();
                }

                xml.WriteStartElement("race");
                RaceParameter.WriteToXml(xml);
                xml.WriteEndElement();

                xml.WriteStartElement("track");
                Track.WriteToXml(xml);
                xml.WriteEndElement();

                if (!EntrySet.IsDefault())
                {
                    xml.WriteStartElement("entry_set");
                    EntrySet.WriteToXml(xml);
                    xml.WriteEndElement();
                }

                if (!EvalCondition.IsDefault())
                {
                    xml.WriteStartElement("eval_condition");
                    EvalCondition.WriteToXml(xml);
                    xml.WriteEndElement();
                }

                if (!AchieveCondition.IsDefault())
                {
                    xml.WriteStartElement("achieve_condition");
                    AchieveCondition.WriteToXml(xml);
                    xml.WriteEndElement();
                }

                if (!FailureCondition.IsDefault())
                {
                    xml.WriteStartElement("failure_condition");
                    FailureCondition.WriteToXml(xml);
                    xml.WriteEndElement();
                }

                if (!LicenseCondition.IsDefault())
                {
                    xml.WriteStartElement("license_condition");
                    LicenseCondition.WriteToXml(xml);
                    xml.WriteEndElement();
                }

                if (!DriftCondition.IsDefault())
                {
                    xml.WriteStartElement("drift_condition");
                    DriftCondition.WriteToXml(xml);
                    xml.WriteEndElement();
                }

                if (!Reward.IsDefault())
                {
                    xml.WriteStartElement("reward");
                    Reward.WriteToXml(xml);
                    xml.WriteEndElement();
                }

                if (!Ranking.IsDefault())
                {
                    xml.WriteStartElement("ranking");
                    Ranking.WriteToXml(xml);
                    xml.WriteEndElement();
                }

                if (!Replay.IsDefault())
                {
                    xml.WriteStartElement("replay");
                    Replay.WriteToXml(xml);
                    xml.WriteEndElement();
                }

                if (!Information.IsDefault())
                {
                    xml.WriteStartElement("information");
                    Information.WriteToXml(xml);
                    xml.WriteEndElement();
                }

                if (BeginDate != null)
                {
                    xml.WriteStartElement("begin_date"); 
                    xml.WriteString(BeginDate.Value.ToString("yyyy/MM/dd HH:mm:ss")); 
                    xml.WriteEndElement();
                }

                if (EndDate != null)
                {
                    xml.WriteStartElement("end_date"); 
                    xml.WriteString(EndDate.Value.ToString("yyyy/MM/dd HH:mm:ss")); 
                    xml.WriteEndElement();
                }
                

                if (!StageData.IsDefault())
                {
                    xml.WriteStartElement("stage_data");
                    StageData.WriteToXml(xml);
                    xml.WriteEndElement();
                }

                xml.WriteElementValue("penalty_script", PenaltyScript);
                xml.WriteElementValue("ai_script", AIScript);

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
            Regulation.Deserialize(ref reader);
            Constraint.Deserialize(ref reader);
            RaceParameter.Deserialize(ref reader);
            Track.Deserialize(ref reader);
            EntrySet.Deserialize(ref reader);
            EvalCondition.Deserialize(ref reader);
            AchieveCondition.Deserialize(ref reader);
            FailureCondition.Deserialize(ref reader);
        }

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_04_3D);
            bs.WriteUInt32(1_06); // Version
            bs.WriteUInt64(EventID);
            bs.WriteInt32((int)GameMode);
            PlayStyle.Serialize(ref bs);
            bs.WriteInt32((int)EventType);
            bs.WriteBool(Inheritance);
            bs.WriteBool(IsSeasonalEvent);
            bs.WriteInt16(0); // field_0x2a - Defaulted to 0
            Regulation.Serialize(ref bs);
            Constraint.Serialize(ref bs);
            RaceParameter.Serialize(ref bs);
            Track.Serialize(ref bs);
            EntrySet.Serialize(ref bs);
            EvalCondition.Serialize(ref bs);
            AchieveCondition.Serialize(ref bs);
            FailureCondition.Serialize(ref bs);
            LicenseCondition.Serialize(ref bs);
            DriftCondition.Serialize(ref bs);
            Reward.Serialize(ref bs);
            Ranking.Serialize(ref bs);
            Replay.Serialize(ref bs);
            Information.Serialize(ref bs);
            bs.WriteDouble(PDIDATETIME.DateTimeToJulian_64(BeginDate ?? DateTime.Parse("2010/04/02 12:00")));
            bs.WriteDouble(PDIDATETIME.DateTimeToJulian_64(EndDate ?? DateTime.Parse("2010/04/02 12:00")));
            StageData.Serialize(ref bs);
            bs.WriteNullStringAligned4(PenaltyScript);
            bs.WriteNullStringAligned4(AIScript);
            ArcadeSetting.Serialize(ref bs);
        }


        public void ParseFromXml(XmlNode eventNode)
        {
            foreach (XmlNode node in eventNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "event_id":
                        EventID = node.ReadValueULong(); break;
                    case "game_mode":
                        GameMode = node.ReadValueEnum<GameMode>(); break;

                    case "play_style":
                        PlayStyle.ParseFromXml(node); break;

                    case "event_type":
                        EventType = node.ReadValueEnum<EventType>(); break;
                    case "inheritance":
                        Inheritance = node.ReadValueBool(); break;
                    case "is_seasonal_event":
                        IsSeasonalEvent = node.ReadValueBool(); break;


                    case "regulation":
                        Regulation.ParseFromXml(node); break;
                    case "constraint":
                        Constraint.ParseFromXml(node); break;
                    case "arcade_style_setting":
                        ArcadeSetting.ParseFromXml(node); break;
                    case "race":
                        RaceParameter.ParseFromXml(node); break;
                    case "track":
                        Track.ParseFromXml(node); break;
                    case "entry_set":
                        EntrySet.ParseFromXml(node); break;
                    case "eval_condition":
                        EvalCondition.ParseFromXml(node); break;
                    case "achieve_condition":
                        AchieveCondition.ParseFromXml(node); break;
                    case "failure_condition":
                        FailureCondition.ParseFromXml(node); break;
                    case "license_condition":
                        LicenseCondition.ParseFromXml(node); break;
                    case "reward":
                        Reward.ParseFromXml(node); break;
                    case "ranking":
                        Ranking.ParseFromXml(node); break;
                    case "replay":
                        Replay.ParseFromXml(node); break;
                    case "information":
                        Information.ParseFromXml(node); break;

                    case "begin_date":
                        string date = node.InnerText.Replace("/00", "/01");
                        DateTime.TryParseExact(date, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time);
                        BeginDate = time;
                        break;
                    case "end_date":
                        string eDate = node.InnerText.Replace("/00", "/01");
                        DateTime.TryParseExact(eDate, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime eTime);
                        EndDate = eTime;
                        break;


                    case "stage_data":
                        StageData.ParseFromXml(node); break;
                }
            }
        }

        /// <summary>
        /// Update some invalid nodes that can only be fixed once the event is fully loaded.
        /// </summary>
        public void FixInvalidNodesIfNeeded()
        {
            int freeSlots = GetFreeCarSlotsLeft();
            if (EntrySet.EntryGenerate.EntryNum > freeSlots)
                EntrySet.EntryGenerate.EntryNum = freeSlots;

            // Ensure we can set it from the actual list
            RaceParameter.WeatherPointNum = (byte)RaceParameter.NewWeatherData.Count;
        }

        public int GetFreeCarSlotsLeft()
        {
            return RaceParameter.EntryMax - GetTotalEntries();
        }

        public int GetTotalEntries()
        {
            return EntrySet.Entries.Count + 1;
        }
    }
}
