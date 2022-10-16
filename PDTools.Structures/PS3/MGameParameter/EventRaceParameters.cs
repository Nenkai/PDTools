using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;
using System.Globalization;

using PDTools.Utils;

namespace PDTools.Structures.PS3.MGameParameter
{
    public class EventRaceParameters
    {
        public bool NeedsPopulating { get; set; } = true;

        public bool AcademyEvent { get; set; }
        public bool Accumulation { get; set; }
        public bool AllowCoDriver { get; set; }
        public bool AutostartPitout { get; set; }
        public byte AutoStandingDelay { get; set; }
        public BehaviorDamageType BehaviorDamage { get; set; } = BehaviorDamageType.WEAK;
        public bool BoostFlag { get; set; }
        public bool BoostType { get; set; }
        public byte BoostLevel { get; set; }
        public int BehaviorFallBack { get; set; }
        public CompleteType CompleteType { get; set; } = CompleteType.BYLAPS;
        public byte CourseOutPenaltyMargin { get; set; }
        public DateTime? Date { get; set; } = new DateTime(1970, 6, 1, 12, 00, 00);
        public DecisiveWeatherType DecisiveWeather { get; set; } = DecisiveWeatherType.SUNNY;
        public bool DisableRecordingReplay { get; set; }
        public bool DisableCollision { get; set; }
        public bool EnableDamage { get; set; }
        public bool EnablePit { get; set; }
        public bool Endless { get; set; }
        public int? EventStartV { get; set; }
        public int? EventGoalV { get; set; }
        public sbyte? EventGoalWidth { get; set; }
        public FinishType FinishType { get; set; } = FinishType.TARGET;
        public Flagset Flagset { get; set; } = Flagset.FLAGSET_NORMAL;
        public byte FuelUseMultiplier { get; set; }
        public bool FixedRetention { get; set; }
        public GhostType GhostType { get; set; } = GhostType.NONE;
        public bool GoalTimeUseLapTotal { get; set; }
        public GhostPresenceType GhostPresenceType { get; set; } = GhostPresenceType.NORMAL;
        public GridSortType GridSortType { get; set; } = GridSortType.NONE;
        public bool RollingPlayerGrid { get; set; }
        public byte TireUseMultiplier { get; set; }
        public bool ImmediateFinish { get; set; }
        public short LapCount { get; set; } = 1;
        public LightingMode LightingMode { get; set; } = LightingMode.AUTO;
        public LineGhostRecordType LineGhostRecordType { get; set; }
        public int? LineGhostPlayMax { get; set; }
        public short MinutesCount { get; set; }
        public bool NeedTireChange { get; set; }
        public bool OnlineOn { get; set; }
        public bool PaceNote { get; set; }
        public PenaltyLevel PenaltyLevel { get; set; } = PenaltyLevel.DEFAULT;
        public bool PenaltyNoReset { get; set; }
        public byte PitConstraint { get; set; } // 16 max
        public RaceType RaceType { get; set; } = RaceType.COMPETITION;
        public short RacersMax { get; set; } = 8;
        public byte RaceInitialLaps { get; set; } = 0;

        private int _trackWetness;
        public int TrackWetness
        {
            get => _trackWetness;
            set => _trackWetness = value > 10 ? 10 : value;
        }

        public bool KeepLoadGhost { get; set; }

        private float _timeProgressSpeed;
        public float TimeProgressSpeed
        {
            get => _timeProgressSpeed;
            set => _timeProgressSpeed = value > 300f ? 300f : value;
        }

        public TimeSpan TimeToStart { get; set; } = TimeSpan.FromSeconds(6);
        public TimeSpan TimeToFinish { get; set; }
        public StartType StartType { get; set; } = StartType.GRID;
        public SessionType SessionType { get; set; }
        public SlipstreamBehavior SlipstreamBehavior { get; set; } = SlipstreamBehavior.GAME;
        public StartSignalType StartSignalType { get; set; } = StartSignalType.NORMAL;
        public byte StartTimeOffset { get; set; }
        public bool VehicleFreezeMode { get; set; } // stage_data->at_quick seems to also enable this
        public bool WithGhost { get; set; }
        public bool ReplaceAtCourseOut { get; set; }
        public short WeatherAccel { get; set; }
        public short WeatherAccelWaterRetention { get; set; }

        public sbyte _weatherBaseCelsius = 63;
        public sbyte WeatherBaseCelsius
        {
            get => _weatherBaseCelsius;
            set
            {
                // 7 bits
                if (value < -64)
                    value = -64;
                else if (value > 63)
                    value = 63;
                _weatherBaseCelsius = value;
            }
        }

        public sbyte _weatherMaxCelsius = 3;
        public sbyte WeatherMaxCelsius
        {
            get => _weatherMaxCelsius;
            set
            {
                // 4 bits
                if (value < -8)
                    value = -8;
                else if (value > 7)
                    value = 7;
                _weatherMaxCelsius = value;
            }
        }

        public sbyte _weatherMinCelsius = 3;
        public sbyte WeatherMinCelsius
        {
            get => _weatherMinCelsius;
            set
            {
                // 4 bits
                if (value < -8)
                    value = -8;
                else if (value > 7)
                    value = 3;
                _weatherMinCelsius = value;
            }
        }
        public bool WeatherNoPrecipitation { get; set; } = true;
        public bool WeatherNoSchedule { get; set; }
        public bool WeatherNoWind { get; set; }

        public const byte MaxWeatherPoints = 16;
        public byte _weatherPointNum;
        public byte WeatherPointNum
        {
            get => _weatherPointNum;
            set
            {
                if (value > MaxWeatherPoints)
                    value = MaxWeatherPoints;
                else if (value < 0)
                    value = 0;
                _weatherPointNum = value;
            }
        }

        public bool WeatherPrecRainOnly { get; set; }
        public bool WeatherPrecSnowOnly { get; set; }
        public ushort WeatherTotalSec { get; set; } = 90;
        public bool WeatherRandom { get; set; }
        public int WeatherRandomSeed { get; set; }
        public List<WeatherData> NewWeatherData { get; set; } = new List<WeatherData>();

        public sbyte[] GridList { get; set; } = new sbyte[32];
        public byte[] DelayStartList { get; set; } = new byte[32];

        public BoostTable[] BoostTables { get; set; } = new BoostTable[2] { new BoostTable(), new BoostTable() };

        private byte[] LaunchSpeedList { get; set; } = new byte[32];
        private short[] LaunchPositionList { get; set; } = new short[32];
        private short[] StartTypeSlotList { get; set; } = new short[32];

        public short[] EventVList { get; set; } = new short[30];
        public PenaltyParameter PenaltyParameter { get; set; } = new PenaltyParameter();

        public EventRaceParameters()
        {
            for (int i = 0; i < 32; i++)
            {
                GridList[i] = -1;
                StartTypeSlotList[i] = -1;
            }

            for (int i = 0; i < 30; i++)
                EventVList[i] = -1;
        }

        public void WriteToXml(Event parent, XmlWriter xml)
        {
            xml.WriteStartElement("race");
            xml.WriteElementBool("academy_event", AcademyEvent);
            xml.WriteElementBool("accumulation", Accumulation);
            xml.WriteElementBool("allow_codriver", AllowCoDriver);
            xml.WriteElementInt("auto_standing_delay", 0);
            xml.WriteElementBool("autostart_pitout", AutostartPitout);
            if (BehaviorFallBack != 0)
                xml.WriteElementInt("behavior_fallback", BehaviorFallBack);
            xml.WriteElementValue("behavior_damage_type", BehaviorDamage.ToString());
            xml.WriteElementValue("behavior_slip_stream_type", SlipstreamBehavior.ToString());
            xml.WriteElementBool("boost_type", BoostType);
            if (BoostLevel != 0)
                xml.WriteElementInt("boost_level", BoostLevel);

            xml.WriteElementInt("bspec_vitality10", 10);
            xml.WriteElementValue("complete_type", CompleteType.ToString());
            xml.WriteElementInt("consume_fuel", FuelUseMultiplier);
            xml.WriteElementInt("consume_tire", TireUseMultiplier);
            if (CourseOutPenaltyMargin != 0)
                xml.WriteElementInt("course_out_penalty_margin", CourseOutPenaltyMargin);
            xml.WriteStartElement("datetime");
            if (Date is null)
                xml.WriteAttributeString("datetime", "1970/00/00 00:00:00");
            else
                xml.WriteAttributeString("datetime", Date.Value.ToString("yyyy/MM/dd HH:mm:ss"));
            xml.WriteEndElement();

            xml.WriteElementValue("decisive_weather", DecisiveWeather.ToString());
            xml.WriteElementBool("disable_collision", DisableCollision);
            xml.WriteElementBool("disable_recording_replay", DisableRecordingReplay);
            xml.WriteElementBool("enable_damage", EnableDamage);
            xml.WriteElementBool("enable_pit", EnablePit);
            xml.WriteElementBool("endless", Endless);
            xml.WriteElementIntIfSet("event_start_v", EventStartV);
            xml.WriteElementFloatOrNull("event_goal_v", EventGoalV);
            xml.WriteElementFloatOrNull("event_goal_width", EventGoalWidth);
            xml.WriteElementValue("finish_type", FinishType.ToString());
            xml.WriteElementBool("fixed_retention", FixedRetention);
            xml.WriteElementValue("flagset", Flagset.ToString());
            xml.WriteElementValue("ghost_presence_type", GhostPresenceType.ToString());
            xml.WriteElementValue("ghost_type", GhostType.ToString());
            xml.WriteElementValue("grid_sort_type", GridSortType.ToString());
            xml.WriteElementBool("immediate_finish", ImmediateFinish);
            xml.WriteElementInt("initial_retention10", TrackWetness);
            xml.WriteElementBool("keep_load_ghost", KeepLoadGhost);
            xml.WriteElementValue("lighting_mode", LightingMode.ToString());
            xml.WriteElementValue("line_ghost_record_type", LineGhostRecordType.ToString());
            xml.WriteElementIntIfSet("line_ghost_play_max", LineGhostPlayMax);
            xml.WriteElementValue("low_mu_type", "MODERATE");
            xml.WriteElementInt("mu_ratio100", 100);
            if (NeedTireChange)
                xml.WriteElementBool("need_tire_change", NeedTireChange);
            xml.WriteElementBool("online_on", OnlineOn);
            xml.WriteElementBool("pace_note", PaceNote);
            xml.WriteElementInt("penalty_level", (int)PenaltyLevel);
            xml.WriteElementBool("penalty_no_reset", PenaltyNoReset);
            if (PitConstraint != 0)
                xml.WriteElementInt("pit_constraint", PitConstraint);
            xml.WriteElementInt("race_limit_laps", LapCount);
            xml.WriteElementInt("race_limit_minute", MinutesCount);
            if (RaceInitialLaps != 0)
                xml.WriteElementInt("race_initial_laps", RaceInitialLaps);

            xml.WriteElementBoolIfTrue("rolling_player_grid", RollingPlayerGrid);
            xml.WriteElementValue("race_type", RaceType.ToString());
            xml.WriteElementValue("start_type", StartType.ToString());
            if (StartSignalType != StartSignalType.NORMAL)
                xml.WriteElementValue("start_signal_type", StartSignalType.ToString());
            xml.WriteElementFloat("time_progress_speed", TimeProgressSpeed);
            xml.WriteElementInt("time_to_finish", (int)TimeToFinish.TotalMilliseconds);
            xml.WriteElementInt("time_to_start", (int)TimeToStart.TotalMilliseconds);
            if (VehicleFreezeMode)
                xml.WriteElementBool("vehicle_freeze_mode", VehicleFreezeMode);
            xml.WriteElementInt("weather_base_celsius", WeatherBaseCelsius);
            xml.WriteElementInt("weather_max_celsius", WeatherMaxCelsius);
            xml.WriteElementInt("weather_min_celsius", WeatherMinCelsius);
            xml.WriteElementBool("weather_no_precipitation", WeatherNoPrecipitation);
            xml.WriteElementBool("weather_no_schedule", WeatherNoSchedule);
            xml.WriteElementBool("weather_no_wind", WeatherNoWind);
            xml.WriteElementInt("weather_point_num", WeatherPointNum);
            xml.WriteElementBool("weather_prec_rain_only", WeatherPrecRainOnly);
            xml.WriteElementBool("weather_prec_snow_only", WeatherPrecSnowOnly);
            xml.WriteElementBool("weather_random", WeatherRandom);
            xml.WriteElementInt("weather_random_seed", WeatherRandomSeed);
            xml.WriteElementFloat("weather_total_sec", WeatherTotalSec);
            xml.WriteElementInt("over_entry_max", 0);
            xml.WriteElementBool("with_ghost", WithGhost);
            xml.WriteElementBool("replace_at_courseout", ReplaceAtCourseOut);
            xml.WriteElementInt("weather_accel10", WeatherAccel);
            xml.WriteElementInt("weather_accel_water_retention10", WeatherAccelWaterRetention);
            xml.WriteElementBool("boost_flag", BoostFlag);

            xml.WriteElementInt("entry_max", RacersMax);
            xml.WriteElementInt("racers_max", RacersMax);

            if (BoostTables.Any(t => !t.IsDefault()))
            {
                xml.WriteStartElement("boost_table_array");
                for (int i = 0; i < 2; i++)
                {
                    xml.WriteStartElement("boost_table");
                    {
                        xml.WriteElementInt("param", i);

                        BoostTable table = BoostTables[i];
                        xml.WriteElementInt("param", table.FrontLimit);
                        xml.WriteElementInt("param", table.FrontMaximumRate);
                        xml.WriteElementInt("param", table.FrontStart);
                        xml.WriteElementInt("param", table.FrontInitialRate);

                        xml.WriteElementInt("param", table.RearLimit);
                        xml.WriteElementInt("param", table.RearMaximumRate);
                        xml.WriteElementInt("param", table.RearStart);
                        xml.WriteElementInt("param", table.RearInitialRate);

                        xml.WriteElementInt("param", table.ReferenceRank);
                        xml.WriteElementInt("param", table.Unk);
                    }
                    xml.WriteEndElement();

                }
                xml.WriteEndElement();
            }

            if (NewWeatherData.Count != 0)
            {
                xml.WriteStartElement("new_weather_data");
                foreach (var data in NewWeatherData)
                {
                    xml.WriteStartElement("point");
                    xml.WriteElementFloat("time_rate", data.TimeRate);
                    xml.WriteElementFloat("low", data.Low);
                    xml.WriteElementFloat("high", data.High);
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
        }

        public void ParseRaceData(XmlNode node)
        {
            foreach (XmlNode raceNode in node.ChildNodes)
            {
                switch (raceNode.Name)
                {
                    case "academy_event":
                        AcademyEvent = raceNode.ReadValueBool();
                        break;
                    case "accumulation":
                        Accumulation = raceNode.ReadValueBool(); break;
                    case "autostart_pitout":
                        AutostartPitout = raceNode.ReadValueBool(); break;
                    case "allow_codriver":
                        AllowCoDriver = raceNode.ReadValueBool(); break;
                    //case "auto_standing_delay":
                    //   Stand = raceNode.ReadValueBool();
                    // break;
                    case "behavior_damage_type":
                        BehaviorDamage = raceNode.ReadValueEnum<BehaviorDamageType>(); break;
                    case "behavior_slip_stream_type":
                        SlipstreamBehavior = raceNode.ReadValueEnum<SlipstreamBehavior>(); break;
                    case "behavior_fallback":
                        BehaviorFallBack = raceNode.ReadValueInt(); break;
                    case "boost_table_array":
                        ParseBoostTables(raceNode); break;
                    case "boost_type":
                        BoostType = raceNode.ReadValueBool(); break;
                    case "boost_level":
                        BoostLevel = raceNode.ReadValueByte(); break;
                    case "complete_type":
                        CompleteType = raceNode.ReadValueEnum<CompleteType>(); break;
                    case "consume_fuel":
                        FuelUseMultiplier = raceNode.ReadValueByte(); break;
                    case "consume_tire":
                        TireUseMultiplier = raceNode.ReadValueByte(); break;
                    case "course_out_penalty_margin":
                        CourseOutPenaltyMargin = raceNode.ReadValueByte(); break;
                    case "datetime":
                        var dateStr = raceNode.Attributes["datetime"].Value;
                        if (dateStr.Equals("1970/00/00 00:00:00"))
                            Date = null;
                        else
                        {
                            string date = dateStr.Replace("/00", "/01");
                            if (DateTime.TryParseExact(date, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time))
                                Date = time;
                            else
                                Date = null;
                        }
                        break;
                    case "decisive_weather":
                        DecisiveWeather = raceNode.ReadValueEnum<DecisiveWeatherType>();
                        break;
                    case "disable_collision":
                        DisableCollision = raceNode.ReadValueBool(); break;
                    case "disable_recording_replay":
                        DisableRecordingReplay = raceNode.ReadValueBool(); break;
                    case "enable_damage":
                        EnableDamage = raceNode.ReadValueBool(); break;
                    case "enable_pit":
                        EnablePit = raceNode.ReadValueBool(); break;
                    case "endless":
                        Endless = raceNode.ReadValueBool(); break;
                    //case "entry_max":
                    //    EntriesCount = raceNode.ReadValueInt(); break;
                    case "event_goal_v":
                        EventGoalV = raceNode.ReadValueInt(); break;
                    case "event_goal_width":
                        EventGoalWidth = (sbyte)raceNode.ReadValueInt(); break;
                    case "finish_type":
                        FinishType = raceNode.ReadValueEnum<FinishType>(); break;
                    case "fixed_retention":
                        FixedRetention = raceNode.ReadValueBool(); break;
                    case "flagset":
                        Flagset = raceNode.ReadValueEnum<Flagset>(); break;
                    case "goal_time_use_lap_total":
                        GoalTimeUseLapTotal = raceNode.ReadValueBool(); break;

                    case "ghost_presence_type":
                        GhostPresenceType = raceNode.ReadValueEnum<GhostPresenceType>(); break;
                    case "ghost_type":
                        GhostType = raceNode.ReadValueEnum<GhostType>(); break;
                    case "grid_sort_type":
                        GridSortType = raceNode.ReadValueEnum<GridSortType>(); break;
                    case "immediate_finish":
                        ImmediateFinish = raceNode.ReadValueBool(); break;
                    case "initial_retention10":
                        TrackWetness = raceNode.ReadValueInt(); break;
                    case "keep_load_ghost":
                        KeepLoadGhost = raceNode.ReadValueBool(); break;
                    case "lighting_mode":
                        LightingMode = raceNode.ReadValueEnum<LightingMode>(); break;
                    case "line_ghost_record_type":
                        LineGhostRecordType = raceNode.ReadValueEnum<LineGhostRecordType>(); break;
                    case "line_ghost_play_max":
                        LineGhostPlayMax = raceNode.ReadValueInt(); break;
                    case "need_tire_change":
                        NeedTireChange = raceNode.ReadValueBool(); break;
                    case "online_on":
                        OnlineOn = raceNode.ReadValueBool(); break;
                    case "pace_note":
                        PaceNote = raceNode.ReadValueBool(); break;
                    case "penalty_level":
                        PenaltyLevel = raceNode.ReadValueEnum<PenaltyLevel>(); break;
                    case "penalty_no_level":
                        PenaltyNoReset = raceNode.ReadValueBool(); break;
                    case "pit_constraint":
                        byte val = raceNode.ReadValueByte();
                        PitConstraint = (byte)(val > 16 ? 16 : val); break;
                    case "race_limit_laps":
                        LapCount = raceNode.ReadValueShort(); break;
                    case "race_limit_minute":
                        MinutesCount = raceNode.ReadValueShort(); break;
                    case "race_initial_laps":
                        RaceInitialLaps = raceNode.ReadValueByte(); break;
                    case "race_type":
                        RaceType = raceNode.ReadValueEnum<RaceType>(); break;
                    case "racers_max":
                        RacersMax = raceNode.ReadValueShort(); break;

                    case "replace_at_courseout":
                        ReplaceAtCourseOut = raceNode.ReadValueBool(); break;
                    case "rolling_player_grid":
                        RollingPlayerGrid = raceNode.ReadValueBool(); break;
                    case "start_type":
                        StartType = raceNode.ReadValueEnum<StartType>(); break;
                    case "start_signal_type":
                        StartSignalType = raceNode.ReadValueEnum<StartSignalType>(); break;
                    case "time_progress_speed":
                        TimeProgressSpeed = float.Parse(raceNode.ReadValueString()); break;
                    case "time_to_finish":
                        TimeToFinish = TimeSpan.FromMilliseconds(raceNode.ReadValueInt()); break;
                    case "time_to_start":
                        TimeToStart = TimeSpan.FromMilliseconds(raceNode.ReadValueInt()); break;
                    case "vehicle_freeze_mode":
                        VehicleFreezeMode = raceNode.ReadValueBool(); break;
                    case "weather_base_celsius":
                        WeatherBaseCelsius = raceNode.ReadValueSByte(); break;
                    case "weather_max_celsius":
                        WeatherMaxCelsius = raceNode.ReadValueSByte(); break;
                    case "weather_min_celsius":
                        WeatherMinCelsius = raceNode.ReadValueSByte(); break;
                    case "weather_no_precipitation":
                        WeatherNoPrecipitation = raceNode.ReadValueBool(); break;
                    case "weather_no_schedule":
                        WeatherNoSchedule = raceNode.ReadValueBool(); break;
                    case "weather_no_wind":
                        WeatherNoSchedule = raceNode.ReadValueBool(); break;

                    /* Do not parse it from here, get ir from the list rather, this will be set from FixInvalidNodesIfNeeded
                case "weather_point_num":
                    WeatherPointNum = raceNode.ReadValueByte(); break; */
                    case "weather_prec_rain_only":
                        WeatherPrecRainOnly = raceNode.ReadValueBool(); break;
                    case "weather_prec_snow_only":
                        WeatherPrecSnowOnly = raceNode.ReadValueBool(); break;
                    case "weather_random":
                        WeatherRandom = raceNode.ReadValueBool(); break;
                    case "weather_random_seed":
                        WeatherRandomSeed = raceNode.ReadValueInt(); break;
                    case "weather_accel_water_retention10":
                        WeatherAccelWaterRetention = raceNode.ReadValueShort(); break;
                    case "weather_total_sec":
                        WeatherTotalSec = raceNode.ReadValueUShort(); break;
                    case "with_ghost":
                        WithGhost = raceNode.ReadValueBool(); break;
                    case "weather_accel10":
                        WeatherAccel = raceNode.ReadValueShort(); break;
                    case "boost_flag":
                        BoostFlag = raceNode.ReadValueBool(); break;

                    case "new_weather_data":
                        {
                            foreach (XmlNode point in raceNode.SelectNodes("point"))
                            {
                                var data = new WeatherData();
                                foreach (XmlNode pointNode in point.ChildNodes)
                                {
                                    switch (pointNode.Name)
                                    {
                                        case "time_rate":
                                            data.TimeRate = pointNode.ReadValueSingle(); break;
                                        case "low":
                                            data.Low = float.Parse(pointNode.ReadValueString(), CultureInfo.InvariantCulture.NumberFormat); break;
                                        case "high":
                                            data.High = float.Parse(pointNode.ReadValueString(), CultureInfo.InvariantCulture.NumberFormat); break;
                                    }
                                }
                                NewWeatherData.Add(data);
                            }
                        }
                        break;
                }
            }
        }

        public void ParseBoostTables(XmlNode boostTableArrayNode)
        {
            foreach (XmlNode boostTableNode in boostTableArrayNode.SelectNodes("boost_table"))
            {
                var paramList = boostTableNode.SelectNodes("param");
                int currentIndex = -1;
                for (int i = 0; i < paramList.Count; i++)
                {
                    XmlNode currentNode = paramList[i];

                    if (i == 0)
                        currentIndex = currentNode.ReadValueInt();
                    else
                    {
                        if (currentIndex == -1 || currentIndex >= 2)
                            break; // No valid table here

                        switch (i)
                        {
                            case 1:
                                BoostTables[currentIndex].FrontLimit = currentNode.ReadValueByte(); break;
                            case 2:
                                BoostTables[currentIndex].FrontMaximumRate = currentNode.ReadValueSByte(); break;
                            case 3:
                                BoostTables[currentIndex].FrontStart = currentNode.ReadValueByte(); break;
                            case 4:
                                BoostTables[currentIndex].FrontInitialRate = currentNode.ReadValueSByte(); break;

                            case 5:
                                BoostTables[currentIndex].RearLimit = currentNode.ReadValueByte(); break;
                            case 6:
                                BoostTables[currentIndex].RearMaximumRate = currentNode.ReadValueSByte(); break;
                            case 7:
                                BoostTables[currentIndex].RearStart = currentNode.ReadValueByte(); break;
                            case 8:
                                BoostTables[currentIndex].RearInitialRate = currentNode.ReadValueSByte(); break;

                            case 9:
                                BoostTables[currentIndex].ReferenceRank = currentNode.ReadValueByte(); break;
                            case 10:
                                BoostTables[currentIndex].Unk = currentNode.ReadValueByte(); break;
                        }
                    }

                }
            }
        }

        public void ReadFromCache(ref BitStream reader)
        {
            int currentPos = reader.Position;
            short bufferSize = reader.ReadInt16();
            short rpVersion = reader.ReadInt16();

            reader.BufferByteSize = bufferSize;
            if (rpVersion < 122)
                ReadFromCacheOld(ref reader, rpVersion); // Read Old
            else
                ReadFromCacheNew(ref reader);

            ReadBoostPenaltyParametersAndNewer(ref reader, rpVersion);

            reader.SeekToByte(currentPos + bufferSize); // pRVar2->BufferSize + param_2;
        }

        private void ReadFromCacheNew(ref BitStream reader)
        {
            SessionType = (SessionType)reader.ReadBits(3);
            RaceType = (RaceType)reader.ReadByte();
            StartType = (StartType)reader.ReadByte();
            CompleteType = (CompleteType)reader.ReadByte();
            FinishType = (FinishType)reader.ReadByte();
            LapCount = reader.ReadInt16();
            MinutesCount = reader.ReadInt16();
            TimeToStart = TimeSpan.FromMilliseconds(reader.ReadInt32());
            TimeToFinish = TimeSpan.FromMilliseconds(reader.ReadInt32());
            reader.ReadInt16(); // entry_max
            reader.ReadInt16(); // ? field_0x16 - Related to racers max
            reader.ReadBits(7); // Unk
            reader.ReadInt32(); // unk
            reader.ReadByte(); // course_layout_no
            reader.ReadByte(); // Race Initial Laps
            KeepLoadGhost = reader.ReadBoolBit();
            reader.ReadInt32(); // course_code
            LineGhostPlayMax = reader.ReadByte();
            GoalTimeUseLapTotal = reader.ReadBoolBit();
            reader.ReadBits(2); // Intentional
            reader.ReadBoolBit(); // Force Pitcrew Off
            reader.ReadInt32(); // scenery_code
            reader.ReadInt32(); // Unk field_0x3c
            reader.ReadInt64(); // unk field_0x40
            reader.ReadInt16(); // packet_timeout_interval
            reader.ReadInt16(); // packet_timeout_latency
            reader.ReadInt16(); // packet_timeout_lag
            RacersMax = reader.ReadInt16();
            reader.ReadInt32(); // unk field 0x2c
            AutostartPitout = reader.ReadBoolBit();
            StartTimeOffset = (byte)reader.ReadBits(5);
            reader.ReadBits(3); // unk - Check this, got 3 on from game and 0 from built
            AutoStandingDelay = reader.ReadByte();
            reader.ReadBits(3); // unk
            StartSignalType = (StartSignalType)reader.ReadBits(2); // start signal type
            reader.ReadBoolBit(); // unk
            reader.ReadBoolBit(); // bench_test
            reader.ReadByte(); // mu_ratio100
            reader.ReadBoolBit(); // unk
            EnableDamage = reader.ReadBoolBit();
            reader.ReadBits(2); // low_mu_type
            BehaviorDamage = (BehaviorDamageType)reader.ReadBits(2);
            reader.ReadBoolBit(); // gps
            PenaltyNoReset = reader.ReadBoolBit();
            SlipstreamBehavior = (SlipstreamBehavior)reader.ReadBits(2);
            PitConstraint = (byte)reader.ReadBits(4);
            reader.ReadBoolBit(); // need_tire_change
            reader.ReadBits(4); // after_race_penalty_sec_5
            reader.ReadBoolBit(); // is_speedtest_milemode
            LineGhostRecordType = (LineGhostRecordType)reader.ReadBits(2);
            reader.ReadBits(2); // attack_seperate_type
            PenaltyLevel = (PenaltyLevel)reader.ReadSByte();
            reader.ReadBoolBit(); // auto_start_with_session
            reader.ReadBoolBit(); // auto_end_with_session
            ImmediateFinish = reader.ReadBoolBit();
            OnlineOn = reader.ReadBoolBit();
            Endless = reader.ReadBoolBit();
            reader.ReadBits(2); // use grid list
            GhostType = (GhostType)reader.ReadByte();
            GridSortType = (GridSortType)reader.ReadByte();
            Accumulation = reader.ReadBool();
            EnablePit = reader.ReadBoolBit();
            Flagset = (Flagset)reader.ReadByte();
            reader.ReadBoolBit(); // Unk
            DisableCollision = reader.ReadBoolBit();
            reader.ReadByte(); // Penalty Condition
            AcademyEvent = reader.ReadBool();
            FuelUseMultiplier = reader.ReadByte();
            reader.ReadByte(); // bspec_vitality_10
            reader.ReadByte(); // consideration_type
            TireUseMultiplier = reader.ReadByte();
            reader.ReadByte(); // temperature_tire
            reader.ReadByte(); // temperature_engine
            reader.ReadByte(); // unk field_0x5a
            LightingMode = (LightingMode)reader.ReadByte();
            Date = new PDIDATETIME32(reader.ReadUInt32()).GetDateTime(); // datetime
            TimeProgressSpeed = reader.ReadByte();
            AllowCoDriver = reader.ReadBool();
            PaceNote = reader.ReadBool();
            reader.ReadByte(); // team_count
            reader.ReadIntoByteArray(32, GridList, BitStream.Byte_Bits);
            reader.ReadIntoByteArray(32, DelayStartList, BitStream.Byte_Bits);
            EventStartV = reader.ReadInt32();
            EventGoalV = reader.ReadInt32();
            EventGoalWidth = reader.ReadSByte();
            FixedRetention = reader.ReadBoolBit();
            TrackWetness = (int)reader.ReadBits(4);
            DecisiveWeather = (DecisiveWeatherType)reader.ReadBits(3);
            WeatherTotalSec = reader.ReadUInt16();
            WeatherPointNum = (byte)reader.ReadBits(4);
            WeatherPointNum = (byte)reader.ReadBits(4);

            for (int i = 1; i < 15; i++)
                reader.ReadBits(0x0c); // Do thing weather related with it

            for (int i = 0; i < 16; i++)
                reader.ReadBits(6); // low
            for (int i = 0; i < 16; i++)
                reader.ReadBits(6); // high

            WeatherRandomSeed = reader.ReadInt32();
            WeatherNoPrecipitation = reader.ReadBoolBit(); //   weather_no_precipitation
            WeatherNoWind = reader.ReadBoolBit(); //   weather_no_wind
            WeatherPrecRainOnly = reader.ReadBoolBit(); //   weather_prec_rain_only
            WeatherPrecSnowOnly = reader.ReadBoolBit(); //   weather_prec_snow_only
            WeatherNoSchedule = reader.ReadBoolBit(); //   weather_no_schedule
            WeatherRandom = reader.ReadBoolBit(); //   weather_random
            reader.ReadBoolBit(); //   param_1->field_0xe0 = (uVar3 & 0x1) << 0x39 | param_1->field_0xe0 & 0xfdffffffffffffff;
            WeatherBaseCelsius = (sbyte)reader.ReadBits(7);
            WeatherMinCelsius = (sbyte)reader.ReadBits(4);
            WeatherMaxCelsius = (sbyte)reader.ReadBits(4);
            WeatherAccel = (short)reader.ReadBits(10);
            WeatherAccelWaterRetention = (short)reader.ReadBits(10);
            reader.ReadBits(6); //   param_1->field_0xe0 = (uVar3 & 0x3f) << 0x10 | param_1->field_0xe0 & 0xffffffffffc0ffff;
            reader.ReadBoolBit(); // Unk
        }

        private void ReadFromCacheOld(ref BitStream reader, int rpVersion)
        {
            SessionType = (SessionType)reader.ReadBits(3); // session_type
            RaceType = (RaceType)reader.ReadBits(13); // race_type
            StartType = (StartType)reader.ReadInt16();
            CompleteType = (CompleteType)reader.ReadInt16();
            FinishType = (FinishType)reader.ReadInt16();
            LapCount = reader.ReadInt16();
            MinutesCount = reader.ReadInt16();
            TimeToStart = TimeSpan.FromMilliseconds(reader.ReadInt32());
            TimeToFinish = TimeSpan.FromMilliseconds(reader.ReadInt32());
            reader.ReadInt16(); // entry_max
            reader.ReadInt16(); // unk_entry
            reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadByte(); // course_layout_no
            reader.ReadByte();
            RaceInitialLaps = reader.ReadByte(); // race_initial_laps
            KeepLoadGhost = reader.ReadBool(); // keep_load_ghost
            reader.ReadInt32(); // course_code
            LineGhostPlayMax = reader.ReadByte();
            GoalTimeUseLapTotal = reader.ReadBoolBit();

            ulong unk;
            if (rpVersion < 117)
            {
                unk = reader.ReadBits(23); // 17 bits
            }
            else
            {
                reader.ReadBits(2); // 2bit skip
                if (rpVersion < 119)
                {
                    unk = reader.ReadBits(21); // 15 bits
                }
                else
                {
                    /* ForcePitcrewOff = */
                    reader.ReadBoolBit();
                    unk = reader.ReadBits(20);
                }
            }

            // do something with unk

            if (rpVersion >= 121)
            {
                reader.ReadInt32(); // scenery_code
                reader.ReadInt32(); // unk
                reader.ReadInt64(); // unk
            }

            reader.ReadInt16(); // packet_timeout_interval
            reader.ReadInt16(); // packet_timeout_latency
            reader.ReadInt16(); // packet_timeout_lag
            RacersMax = reader.ReadInt16();
            reader.ReadInt32(); // unk

            reader.ReadBits(7); // skipped
            AutostartPitout = reader.ReadBoolBit();
            StartTimeOffset = (byte)reader.ReadBits(5);
            reader.ReadBits(3); // unk (((uVar2 & 0x7) << 0x3c | param_1->BoolBits)
            AutoStandingDelay = reader.ReadByte();
            reader.ReadBits(3); // unk (uVar2 & 0x7) << 0x39
            StartSignalType = (StartSignalType)reader.ReadBits(2);
            reader.ReadBits(2); // skipped
            reader.ReadBoolBit(); // unk (Bits & 0x01 << 0x38)
            reader.ReadBoolBit(); // bench_test
            reader.ReadByte(); // mu_ratio_100
            reader.ReadBoolBit(); // unk (Bits & 0x1 << 0x17)
            EnableDamage = reader.ReadBoolBit();
            reader.ReadBits(2); // low_mu_type?
            reader.ReadBits(2); // unk (Bits & 0x3 << 0x12)
            reader.ReadBoolBit(); // gps
            PenaltyNoReset = reader.ReadBoolBit();
            SlipstreamBehavior = (SlipstreamBehavior)reader.ReadBits(2);
            reader.ReadBits(4); // unk (Bits & 0xf << 0xa)
            NeedTireChange = reader.ReadBoolBit(); // need_tire_change
            reader.ReadBits(4); // after_race_penalty_sec5? (Bits & 0xf << 5)
            reader.ReadBoolBit(); // is_speedtest_milemode
            LineGhostRecordType = (LineGhostRecordType)reader.ReadBits(2);
            reader.ReadBits(2); // attack_seperate_type? (Bits & 0x03)
            PenaltyLevel = (PenaltyLevel)reader.ReadByte();
            reader.ReadByte(); // auto_start_with_session
            reader.ReadByte(); // auto_end_session_with_finish
            ImmediateFinish = reader.ReadBool();
            OnlineOn = reader.ReadBool();
            Endless = reader.ReadBool();
            reader.ReadByte(); // use grid list
            GhostType = (GhostType)reader.ReadByte();
            GridSortType = (GridSortType)reader.ReadByte();
            Accumulation = reader.ReadBool();
            EnablePit = reader.ReadBool();
            Flagset = (Flagset)reader.ReadByte();
            reader.ReadByte(); // unk (Bits & 0x1 << 0x26)
            DisableCollision = reader.ReadBool();
            reader.ReadByte(); // penalty_condition
            AcademyEvent = reader.ReadBool();
            reader.ReadByte(); // consume_fuel
            reader.ReadByte(); // bspec_vitality_10
            reader.ReadByte(); // consideration_type
            TireUseMultiplier = reader.ReadByte();
            reader.ReadByte(); // temperature_tire
            reader.ReadByte(); // temperature_engine
            reader.ReadByte(); // unk field_5a
            LightingMode = (LightingMode)reader.ReadByte();
            Date = new PDIDATETIME32(reader.ReadUInt32()).GetDateTime();
            reader.ReadByte(); // time_progress_speed
            AllowCoDriver = reader.ReadBool();
            reader.ReadByte(); // pace_note
            reader.ReadByte(); // team_count
            reader.ReadIntoByteArray(32, new byte[32], BitStream.Byte_Bits); // grid_list
            reader.ReadIntoByteArray(32, new byte[32], BitStream.Byte_Bits); // delay_start_sec_list
            if (rpVersion >= 113)
                EventStartV = reader.ReadInt32();
            EventGoalV = reader.ReadInt32();
            EventGoalWidth = reader.ReadSByte();
            FixedRetention = reader.ReadBoolBit();
            TrackWetness = (int)reader.ReadBits(4); // initial_retention10

            // entering weather zone
            DecisiveWeather = (DecisiveWeatherType)reader.ReadBits(3); // decisive_weather
            WeatherTotalSec = reader.ReadUInt16(); // weather points? confirm this
            reader.ReadBits(4); // unk (field_0xae & 0x3ff)
            reader.ReadBits(4); // unk (field_0xae & 0xfc00)

            for (int i = 1; i < 15; i++)
                NewWeatherData[i].TimeRate = reader.ReadBits(12);

            for (int i = 0; i < 16; i++)
                NewWeatherData[i].High = reader.ReadBits(6);
            for (int i = 0; i < 16; i++)
                NewWeatherData[i].Low = reader.ReadBits(6);

            WeatherRandomSeed = reader.ReadInt32();
            WeatherNoPrecipitation = reader.ReadBoolBit(); //   weather_no_precipitation
            WeatherNoWind = reader.ReadBoolBit(); //   weather_no_wind
            WeatherPrecRainOnly = reader.ReadBoolBit(); //   weather_prec_rain_only
            WeatherPrecSnowOnly = reader.ReadBoolBit(); //   weather_prec_snow_only
            WeatherNoSchedule = reader.ReadBoolBit(); //   weather_no_schedule
            WeatherRandom = reader.ReadBoolBit(); //   weather_random
            reader.ReadBoolBit(); //   param_1->field_0xe0 = (uVar3 & 0x1) << 0x39 | param_1->field_0xe0 & 0xfdffffffffffffff;
            WeatherBaseCelsius = (sbyte)reader.ReadBits(7);
            WeatherMinCelsius = (sbyte)reader.ReadBits(4);
            WeatherMaxCelsius = (sbyte)reader.ReadBits(4);
            WeatherAccel = (short)reader.ReadBits(10);
            WeatherAccelWaterRetention = (short)reader.ReadBits(10);
            reader.ReadBits(6); //   param_1->field_0xe0 = (uVar3 & 0x3f) << 0x10 | param_1->field_0xe0 & 0xffffffffffc0ffff;

            if (rpVersion >= 123)
                reader.ReadBoolBit(); // unk

        }

        private void ReadBoostPenaltyParametersAndNewer(ref BitStream reader, int rpVersion)
        {
            reader.ReadByte(); // useLaunchData
            reader.ReadIntoByteArray(32, LaunchSpeedList, BitStream.Byte_Bits); // launch speed list
            reader.ReadIntoShortArray(32, LaunchPositionList, BitStream.Short_Bits); // launch_position_list
            reader.ReadIntoShortArray(32, StartTypeSlotList, BitStream.Short_Bits); // start_type_slot_list

            // boost_table
            for (int i = 0; i < 2; i++)
            {
                BoostTables[i].FrontLimit = reader.ReadByte();
                BoostTables[i].FrontMaximumRate = reader.ReadSByte();
                BoostTables[i].FrontStart = reader.ReadByte();
                BoostTables[i].FrontInitialRate = reader.ReadSByte();

                BoostTables[i].RearLimit = reader.ReadByte();
                BoostTables[i].RearMaximumRate = reader.ReadSByte();
                BoostTables[i].RearStart = reader.ReadByte();
                BoostTables[i].RearInitialRate = reader.ReadSByte();

                BoostTables[i].ReferenceRank = reader.ReadByte();
                reader.ReadByte();
            }

            // boost_params? Maybe?
            for (int i = 0; i < 32; i++)
            {
                reader.ReadIntoByteArray(6, new byte[6], BitStream.Byte_Bits); // Front
                reader.ReadIntoByteArray(6, new byte[6], BitStream.Byte_Bits); // Rear
            }

            BoostLevel = reader.ReadByte();
            RollingPlayerGrid = reader.ReadBool();
            reader.ReadByte(); // field_0x323
            BoostFlag = reader.ReadBool();
            BoostType = reader.ReadBool();
            DisableRecordingReplay = reader.ReadBool(); // disable_recording_replay
            GhostPresenceType = (GhostPresenceType)reader.ReadByte(); // ghost_presence_type
            reader.ReadIntoShortArray(30, EventVList, BitStream.Short_Bits); // event_v_list

            PenaltyParameter.ReadPenaltyParameter(ref reader);

            var arr2 = new short[1];
            // Unk
            reader.ReadIntoShortArray(1, arr2, BitStream.Short_Bits); // field_0x3f0

            var arr3 = new byte[4];
            reader.ReadIntoByteArray(4, arr3, BitStream.Byte_Bits); // field_0x3f2
            reader.ReadByte(); // field_0x3f6
            reader.ReadByte(); // large_entry_max

            if (rpVersion >= 114)
                reader.ReadByte(); // pitstage_revision

            if (rpVersion >= 115)
                VehicleFreezeMode = reader.ReadBool(); // vehicle_freeze_mode

            if (rpVersion >= 116)
                CourseOutPenaltyMargin = reader.ReadByte(); // course_out_penalty_margine

            if (rpVersion >= 118)
                BehaviorFallBack = reader.ReadInt32(); // behavior_fallback

            if (rpVersion >= 120)
            {
                // pilot stuff
                reader.ReadBits(1); //  param_1->pilot_commands = uVar2 << 0x3f | param_1->pilot_commands & 0x7fffffffffffffff;
                reader.ReadBits(7); //  param_1->pilot_commands = (uVar2 & 0x7f) << 0x38 | param_1->pilot_commands & 0x80ffffffffffffff;
            }
        }

        public void WriteToCache(ref BitStream bs, Event parent)
        {
            int cPos = bs.Position;
            bs.WriteInt16(0x410); // Buffer Size. Writen size does not match, but is intentionally larger for later versions
            bs.WriteInt16(1_23); // Version

            WriteGeneralSettings(ref bs, parent);
            WriteOtherSettings(ref bs);
            bs.SeekToByte(cPos + 0x410);
        }

        private void WriteGeneralSettings(ref BitStream bs, Event parent)
        {
            bs.WriteBits((ulong)SessionType, 3);
            bs.WriteByte((byte)RaceType);
            bs.WriteByte((byte)StartType);
            bs.WriteByte((byte)CompleteType);
            bs.WriteByte((byte)FinishType);
            bs.WriteInt16(LapCount);
            bs.WriteInt16(MinutesCount);
            bs.WriteInt32((int)TimeToStart.TotalMilliseconds);
            bs.WriteInt32((int)TimeToFinish.TotalMilliseconds);
            bs.WriteInt16(RacersMax); // EntryMax
            bs.WriteInt16(16); // ? field_0x16 - Related to racers max
            bs.WriteBits(0, 7); // Unk
            bs.WriteInt32(-1); // Unk
            bs.WriteByte(0); // CourseLayoutNo
            bs.WriteByte(RaceInitialLaps);
            bs.WriteBoolBit(KeepLoadGhost);
            bs.WriteInt32(parent.Course.CourseCode); // course_code
            bs.WriteByte((byte)(LineGhostPlayMax ?? 0));
            bs.WriteBoolBit(GoalTimeUseLapTotal);
            bs.WriteBits(0, 2); // Intentional
            bs.WriteBoolBit(false); // force_pitcrew_off
            bs.WriteInt32(-1); // scenery_code
            bs.WriteInt32(0); // Unk field_0x3c
            bs.WriteInt64(0); // unk field_0x40
            bs.WriteInt16(4000); // packet_timeout_interval
            bs.WriteInt16(4000); // packet_timeout_latency
            bs.WriteInt16(4000); // packet_timeout_lag
            bs.WriteInt16(RacersMax);
            bs.WriteInt32(0); // unk field 0x2c
            bs.WriteBoolBit(AutostartPitout);
            bs.WriteBits(StartTimeOffset, 5);
            bs.WriteBits(0, 3); // unk
            bs.WriteByte(AutoStandingDelay);
            bs.WriteBits(0, 3); // Unk
            bs.WriteBits((ulong)StartSignalType, 2);
            bs.WriteBoolBit(false); // Unk
            bs.WriteBoolBit(false); // bench_test
            bs.WriteByte(100); // mu_ratio100
            bs.WriteBoolBit(true); // unk
            bs.WriteBoolBit(EnableDamage);
            bs.WriteBits(0, 2); // low_mu_type
            bs.WriteBits((ulong)BehaviorDamage, 2);
            bs.WriteBoolBit(false); // gps
            bs.WriteBoolBit(PenaltyNoReset);
            bs.WriteBits((ulong)SlipstreamBehavior, 2);
            bs.WriteBits(PitConstraint, 4);
            bs.WriteBoolBit(NeedTireChange);
            bs.WriteBits(0, 4); // after_race_penalty_sec5
            bs.WriteBoolBit(false); // is_speedtest_milemode
            bs.WriteBits((ulong)LineGhostRecordType, 2);
            bs.WriteBits(0, 2); // attack_seperate_type
            bs.WriteByte((byte)PenaltyLevel);
            bs.WriteBoolBit(true); // auto_start_with_session, no xml
            bs.WriteBoolBit(false); // auto_end_with_finish, no xml
            bs.WriteBoolBit(ImmediateFinish);
            bs.WriteBoolBit(OnlineOn);
            bs.WriteBoolBit(Endless);
            bs.WriteBits(0, 2); // use grid list
            bs.WriteByte((byte)GhostType);
            bs.WriteByte((byte)GridSortType);
            bs.WriteBool(Accumulation);
            bs.WriteBoolBit(EnablePit);
            bs.WriteByte((byte)Flagset);
            bs.WriteBoolBit(false); // Unk
            bs.WriteBoolBit(DisableCollision);
            bs.WriteByte(0); // penalty_condition
            bs.WriteBool(AcademyEvent);
            bs.WriteByte(FuelUseMultiplier);
            bs.WriteByte(10); // bspec_vitality_10
            bs.WriteByte(0); // consideration_type
            bs.WriteByte(TireUseMultiplier);
            bs.WriteByte(0); // temperature_tire
            bs.WriteByte(0); // temperature_engine
            bs.WriteByte(0); // unk field_0x5a
            bs.WriteByte((byte)LightingMode);

            // datetime parts's bits are written seperately (i.e 5 5 5 6 6 6)
            var pdtime = new PDIDATETIME32();
            pdtime.SetDateTime(Date ?? new DateTime(1970, 6, 1, 12, 00, 00));
            bs.WriteUInt32(pdtime.GetRawData());

            bs.WriteByte((byte)TimeProgressSpeed);
            bs.WriteBool(AllowCoDriver);
            bs.WriteBool(PaceNote);
            bs.WriteByte(0); // team_count, not in xmls

            for (int i = 0; i < 32; i++)
                bs.WriteSByte(GridList[i]);
            for (int i = 0; i < 32; i++)
                bs.WriteByte(DelayStartList[i]);

            bs.WriteInt32(EventStartV ?? -1);
            bs.WriteInt32(EventGoalV ?? -1);
            bs.WriteSByte(EventGoalWidth ?? -1);

            bs.WriteBoolBit(FixedRetention);
            bs.WriteBits((ulong)TrackWetness + 1, 4); // initial_retention10 - 10% each
            bs.WriteBits((ulong)DecisiveWeather, 3);

            bs.WriteUInt16((ushort)(WeatherTotalSec / 2));
            bs.WriteBits(WeatherPointNum, 4);
            bs.WriteBits(0, 4); // Again Maybe?

            WriteWeatherData(ref bs);

            bs.WriteInt32(WeatherRandomSeed);
            bs.WriteBoolBit(WeatherNoPrecipitation);
            bs.WriteBoolBit(WeatherNoWind);
            bs.WriteBoolBit(WeatherPrecRainOnly);
            bs.WriteBoolBit(WeatherPrecSnowOnly);
            bs.WriteBoolBit(WeatherNoSchedule);
            bs.WriteBoolBit(WeatherRandom);
            bs.WriteBoolBit(false); // Unk
            bs.WriteBits((ulong)WeatherBaseCelsius, 7);
            bs.WriteBits((ulong)WeatherMinCelsius, 4);
            bs.WriteBits((ulong)WeatherMaxCelsius, 4);
            bs.WriteBits((ulong)WeatherAccel, 10);
            bs.WriteBits((ulong)WeatherAccelWaterRetention, 10); // weather_accel_water_retention10
            bs.WriteBits(0, 6); //   param_1->field_0xe0 = (uVar3 & 0x3f) << 0x10 | param_1->field_0xe0 & 0xffffffffffc0ffff; - Unk
            bs.WriteBoolBit(true); // Unk
        }

        private void WriteOtherSettings(ref BitStream bs)
        {
            bs.WriteByte(0); // Use launch data
            for (int i = 0; i < 32; i++)
                bs.WriteByte(LaunchSpeedList[i]);
            for (int i = 0; i < 32; i++)
                bs.WriteInt16(LaunchPositionList[i]);
            for (int i = 0; i < 32; i++)
                bs.WriteInt16(StartTypeSlotList[i]);

            for (int i = 0; i < 2; i++)
            {
                bs.WriteByte(BoostTables[i].FrontLimit);
                bs.WriteSByte(BoostTables[i].FrontMaximumRate);
                bs.WriteByte(BoostTables[i].FrontStart);
                bs.WriteSByte(BoostTables[i].FrontInitialRate);

                bs.WriteByte(BoostTables[i].RearLimit);
                bs.WriteSByte(BoostTables[i].RearMaximumRate);
                bs.WriteByte(BoostTables[i].RearStart);
                bs.WriteSByte(BoostTables[i].RearInitialRate);

                bs.WriteByte(BoostTables[i].ReferenceRank);
                bs.WriteByte(0);
            }

            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 6; j++)
                    bs.WriteByte(0); // Front

                for (int j = 0; j < 6; j++)
                    bs.WriteByte(0); // Rear 
            }

            bs.WriteByte(BoostLevel);
            bs.WriteSByte((sbyte)(RollingPlayerGrid ? 1 : -1));
            bs.WriteBool(false); // Unk field_0x323
            bs.WriteBool(BoostFlag);
            bs.WriteBool(BoostType);
            bs.WriteBool(DisableRecordingReplay);
            bs.WriteByte((byte)GhostPresenceType);

            for (int i = 0; i < 30; i++)
                bs.WriteInt16(EventVList[i]);

            // Write Penalty Parameter
            PenaltyParameter.WritePenaltyParameter(ref bs);

            bs.WriteInt16(0); // field_0x3f0 - Array of 1
            for (int i = 0; i < 4; i++)
                bs.WriteByte(0); // field_0x3f2 - Array of 4

            bs.WriteByte(0); // field_0x3f6
            bs.WriteByte(0); // large_entry_max
            bs.WriteByte(0); // Pitstage_revision
            bs.WriteBool(VehicleFreezeMode); // vehicle_freeze_mode
            bs.WriteByte(CourseOutPenaltyMargin); // course_out_penalty_margine
            bs.WriteInt32(BehaviorFallBack); // behavior_fallback

            bs.WriteBits(0, 1);
            bs.WriteBits(0, 7);

        }

        public void WriteWeatherData(ref BitStream bs)
        {
            for (int i = 0; i < 15; i++)
            {
                if (i == 0)
                    continue;

                // Time rate is a int that goes from 0 to 4095
                // 16 points, 14 middle points we need to write
                if (i < NewWeatherData.Count)
                {
                    WeatherData weatherPoint = NewWeatherData[i];

                    ulong tRate = (ulong)(weatherPoint.TimeRate * (Math.Pow(2, 12) - 1)) / 100; // Convert float from 0 to 100 into an int value between 0 and 4095
                    bs.WriteBits(tRate, 12);
                }
                else
                    bs.WriteBits(0, 12);
            }

            // Low & high goes from 0 to 63
            for (int i = 0; i < 16; i++)
            {
                if (i < NewWeatherData.Count)
                {
                    WeatherData weatherPoint = NewWeatherData[i];

                    float rawVal = weatherPoint.Low + 1f; // Ensure to care about -1 to 0
                    ulong lowBits = (ulong)(rawVal * (Math.Pow(2, 6) - 1)); // Convert float from 0 to 2 into an int value between 0 and 63
                    bs.WriteBits(lowBits, 6);
                }
                else
                    bs.WriteBits(0, 6);
            }

            for (int i = 0; i < 16; i++)
            {
                if (i < NewWeatherData.Count)
                {
                    WeatherData weatherPoint = NewWeatherData[i];

                    float rawVal = weatherPoint.Low + 1f; // Ensure to care about -1 to 0
                    ulong highBits = (ulong)(rawVal * (Math.Pow(2, 6) - 1)); // Convert float from 0 to 2 into an int value between 0 and 63
                    bs.WriteBits(highBits, 6);
                }
                else
                    bs.WriteBits(0, 6);
            }
        }
    }

    public enum BehaviorDamageType
    {
        [Description("None")]
        WEAK,

        [Description("Light")]
        MIDDLE,

        [Description("Heavy")]
        STRONG
    }

    public enum PenaltyLevel
    {
        [Description("Default (Game Mode dependant)")]
        DEFAULT = -1,

        [Description("No Penalties")]
        NONE,

        [Description("Weak")]
        WEAK,

        [Description("Strong")]
        STRONG,
    }

    public enum StartType
    {
        [Description("None (No control of the car")]
        NONE = -1,

        [Description("Grid")]
        GRID = 0,

        [Description("Rolling Start (Start Line)")]
        ROLLING,

        [Description("Pit Start")]
        PIT,

        [Description("For Time Trial")]
        ATTACK,

        [Description("Dispersed")]
        DISPERSED,

        [Description("Drift Position (Standing)")]
        COURSEINFO,

        [Description("Rolling (Same Accel. as Own Car)")]
        ROLLING2,

        [Description("Same Grid (collisions OFF)")]
        SAME_GRID,

        [Description("Rolling (Define Start Time)")]
        ROLLING3,

        [Description("Drift Position (Rolling)")]
        COURSEINFO_ROLLING,

        [Description("Standing (Set Coordinates)")]
        STANDING,

        [Description("Rolling (Define Start & Accel)")]
        ROLLING_NOLIMIT,

        FREE,
        STANDING_L,
        STANDING_R,
        PITWORK,

        [Description("Rolling Start - Dbl. File, Left Ahead")]
        ROLLING_DL,

        [Description("Rolling Start - Dbl. File, Right Ahead")]
        ROLLING_DR,

        GRID_FLYING,
        PITIN,
        RALLY,
        STANDING_CENTER,

        /*
        [Description("Double-File Rolling (Left)")]
        ROLLING_L,

        [Description("Double-File Rolling (Right)")]
        ROLLING_R,
        */

    }
    public enum GridSortType
    {
        [Description("None")]
        NONE,

        [Description("Random")]
        RANDOM,

        [Description("By Points")]
        POINT_UP,

        [Description("Reverse Points Grid")]
        POINT_DOWN,

        [Description("Fastest First")]
        FASTEST_UP,

        [Description("Fastest Last")]
        FASTEST_DOWN,

        [Description("Based on ranks")]
        PREV_RANK,

        [Description("Reverse Ranks")]
        PREV_RANK_REVESE,
    }

    public enum GhostType
    {
        [Description("No Ghost")]
        NONE,

        [Description("Full (GT5?)")]
        FULL,

        [Description("One Lap")]
        ONELAP,

        RECORD,
        SECTOR_ATTACK,
        NORMAL,
        TRGRANK_ALL,


    }

    public enum LineGhostRecordType
    {
        OFF,
        ONE,
        TRACKDAY
    }

    public enum GhostPresenceType
    {
        [Description("Normal - Transparent?")]
        NORMAL,

        [Description("None")]
        NONE,

        [Description("Real - Shows an actual car?")]
        REAL,
    }


    public enum SlipstreamBehavior
    {
        GAME,
        SIMULATION,
        REAL,
    }

    public enum RaceType
    {
        COMPETITION,
        TIMEATTACK,
        DRIFTATTACK,
        DEMO,
        OVERTAKE,
        SPEEDTEST,
        DARALOGGER,
        NONE,
    }

    public enum LightingMode
    {
        AUTO,
        OFF,
        POSITION,
        LOW_BEAM,
        HIGH_BEAM
    }

    public enum Flagset
    {
        FLAGSET_NONE,
        FLAGSET_NORMAL,
        FLAGSET_F1,
        FLAGSET_NASCAR,
        FLAGSET_LOW,
        FLAGSET_RALLY,
    }

    public enum DecisiveWeatherType
    {
        [Description("None")]
        NONE,

        [Description("Sunny")]
        SUNNY,

        [Description("Rainy")]
        RAINY,

        [Description("Snowy")]
        SNOWY,
    }

    public enum FinishType
    {
        [Description("None")]
        NONE,

        [Description("Target")]
        TARGET,

        [Description("Fastest Car")]
        FASTEST,
    }

    public enum CompleteType
    {
        [Description("Finish After a Number of Laps")]
        BYLAPS = 0,

        [Description("Finish After Time (Endurance)")]
        BYTIME = 1,

        [Description("By Section")]
        BYSECTION = 2,

        [Description("None")]
        NONE = 3,

        [Description("Other (?)")]
        OTHER = 4,

        [Description("By Stop (licenses)")]
        BYSTOP = 5,
    }

    public enum SessionType
    {
        FINAL,
        QUALIFY,
        PRACTICE,
    }

    public enum StartSignalType
    {
        NORMAL,
        BLACK_OUT,
        GREEN_LIGHT,
    }
}
