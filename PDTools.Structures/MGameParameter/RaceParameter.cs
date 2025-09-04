using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;
using System.Globalization;

using PDTools.Utils;
using PDTools.Enums;

namespace PDTools.Structures.MGameParameter;

public class RaceParameter
{
    /// <summary>
    /// Defaults to <see cref="RaceType.COMPETITION"/>.
    /// </summary>
    public RaceType RaceType { get; set; } = RaceType.COMPETITION;

    /// <summary>
    /// How entries are starting the race. Defaults to <see cref="StartType.GRID"/>.
    /// </summary>
    public StartType StartType { get; set; } = StartType.GRID;

    /// <summary>
    /// Defaults to <see cref="CompleteType.BYLAPS"/>.
    /// </summary>
    public CompleteType CompleteType { get; set; } = CompleteType.BYLAPS;

    /// <summary>
    /// Defaults to <see cref="FinishType.TARGET"/>.
    /// </summary>
    public FinishType FinishType { get; set; } = FinishType.TARGET;

    /// <summary>
    /// Number of laps to finish. Defaults to 1.
    /// </summary>
    public short RaceLimitLaps { get; set; } = 1;

    /// <summary>
    /// Number of minutes to finish (mostly for endurances). Defaults to 0 (none).
    /// </summary>
    public short RaceLimitMinute { get; set; }

    /// <summary>
    /// Time before the race starts. Defaults to 6 seconds.
    /// </summary>
    public TimeSpan TimeToStart { get; set; } = TimeSpan.FromSeconds(6);

    /// <summary>
    /// Time to give other entries to finish once first has finished. Defaults to 3 seconds.
    /// </summary>
    public TimeSpan TimeToFinish { get; set; } = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Defaults to 16 (max).
    /// </summary>
    public short EntryMax { get; set; } = 16;

    /// <summary>
    /// Defaults to 16 (max).
    /// </summary>
    public short RacersMax { get; set; } = 16;

    /// <summary>
    /// Defaults to <see cref="false"/>.
    /// </summary>
    public bool KeepLoadGhost { get; set; } = false;

    /// <summary>
    /// Whether the race immediately finishes once the timer ends (for use with <see cref="TimeToFinish"/>). Defaults to <see cref="true"/>.
    /// </summary>
    public bool ImmediateFinish { get; set; } = true;

    /// <summary>
    /// Defaults to NONE.
    /// </summary>
    public GridSortType GridSortType { get; set; } = GridSortType.NONE;

    /// <summary>
    /// Defaults to <see cref="true"/>.
    /// </summary>
    public bool AutostartPitout { get; set; } = true;

    /// <summary>
    /// For drift mode. Defaults to <see cref="false"/>.
    /// </summary>
    public bool Endless { get; set; } = false;

    /// <summary>
    /// Ghosts. Defaults to none.
    /// </summary>
    public GhostType GhostType { get; set; } = GhostType.NONE;

    /// <summary>
    /// Whether to disable collision with other cars. Defaults to <see cref="false"/>.
    /// </summary>
    public bool DisableCollision { get; set; } = false;

    /// <summary>
    /// Penalties. Default to off
    /// </summary>
    public PenaltyLevelTypes PenaltyLevel { get; set; } = PenaltyLevelTypes.OFF;

    /// <summary>
    /// Whether oil/engine/body accumulation/degradation/build-up is enabled. Defaults to <see cref="false"/>.
    /// </summary>
    public bool Accumulation { get; set; } = false;

    /// <summary>
    /// Defaults to 10.
    /// </summary>
    public int BSpecVitality10 { get; set; } = 10;

    /// <summary>
    /// Rate of which the fuel will be consumed - defaults to 0 (no fuel consumption).
    /// </summary>
    public byte ConsumeFuelRate { get; set; }

    /// <summary>
    /// Rate of which the tires will be degrade - defaults to 0 (no tire degradation).
    /// </summary>
    public byte ConsumeTireRate { get; set; }

    /// <summary>
    /// GT6 Only. Defaults to 0.
    /// </summary>
    public byte TemperatureTire { get; set; } = 0;

    /// <summary>
    /// GT6 Only. Defaults to 0.
    /// </summary>
    public byte TemperatureBrake { get; set; } = 0;

    /// <summary>
    /// GT6 Only. Defaults to 0.
    /// </summary>
    public byte TemperatureEngine { get; set; } = 0;

    /// <summary>
    /// GT6 Only. Defaults to <see cref="false"/>.
    /// </summary>
    public bool GoalTimeUseLapTotal { get; set; } = false;

    /// <summary>
    /// GT6 Only. Defaults to 0.
    /// </summary>
    public byte StartTimeOffset { get; set; } = 0;

    /// <summary>
    /// GT6 Only. Defaults to <see cref="StartSignalType.NORMAL"/>.
    /// </summary>
    public StartSignalType StartSignalType { get; set; } = StartSignalType.NORMAL;

    /// <summary>
    /// GT6 Only. 0 = Never, 1 = Regular Penalty (pit-through time addition), 2 = Immediate penalty (reset) after judgment - Defaults to 0.
    /// </summary>
    public byte ConsiderationType { get; set; } = 0;

    /// <summary>
    /// Defaults to <see cref="false"/>.
    /// </summary>
    public bool AcademyEvent { get; set; } = false;

    /// <summary>
    /// Defaults to <see cref="LightingMode.AUTO"/>.
    /// </summary>
    public LightingMode LightingMode { get; set; } = LightingMode.AUTO;

    /// <summary>
    /// GT6 Only. Defaults to 0.
    /// </summary>
    public byte BoostLevel { get; set; }

    /// <summary>
    /// Whether to use reference rank possibly. Defaults to 0.
    /// </summary>
    public bool BoostType { get; set; }

    /// <summary>
    /// GT6 Only. Whether to use only one table for boost - it'll be used as the general boost. Defaults to 0.
    /// </summary>
    public bool BoostFlag { get; set; }

    public DateTime? Date { get; set; } = new DateTime(2010, 6, 1, 12, 00, 00);

    /// <summary>
    /// Time of day progression. Defaults to -1.0.
    /// </summary>
    public sbyte TimeProgressSpeed { get; set; } = -1;

    // lucky_player

    /// <summary>
    /// Whether pitstops are enabled - if false, cars will just pass through the pit lane only. Defaults to <see cref="false"/>.
    /// </summary>
    public bool EnablePit { get; set; } = false;

    /// <summary>
    /// Defaults to 0.
    /// </summary>
    public byte PitConstraint { get; set; } // 16 max

    /// <summary>
    /// GT6 Only. Defaults to 0.
    /// </summary>
    public byte CourseOutPenaltyMargin { get; set; }

    /// <summary>
    /// GT6 Only. Defaults to 0.
    /// </summary>
    public int BehaviorFallBack { get; set; }

    /// <summary>
    /// GT6 Only. Defaults to <seealso cref="false"/>.
    /// </summary>
    public bool NeedTireChange { get; set; } = false;

    /// <summary>
    /// Whether to disable replay recording. Defaults to <see cref="false"/> (replay recording is enabled).
    /// </summary>
    public bool DisableRecordingReplay { get; set; } = false;

    /// <summary>
    /// Defaults to <see cref="Flagset.FLAGSET_NORMAL"/>.
    /// </summary>
    public Flagset Flagset { get; set; } = Flagset.FLAGSET_NORMAL;

    /// <summary>
    /// Defaults to <see cref="false"/>.
    /// </summary>
    public bool OnlineOn { get; set; } = false;

    /// <summary>
    /// Defaults to 0.
    /// </summary>
    public byte AutoStandingDelay { get; set; }

    /// <summary>
    /// Defaults to <see cref="false"/>.
    /// </summary>
    public bool AllowCoDriver { get; set; } = false;

    /// <summary>
    /// Defaults to <see cref="false"/>.
    /// </summary>
    public bool PaceNote { get; set; } = false;

    /// <summary>
    /// Whether car damage is enabled. Defaults to <see cref="true"/>.
    /// </summary>
    public bool EnableDamage { get; set; } = true;

    /// <summary>
    /// GT6 Only. Whether to reset the car when going off-course. Defaults to 0.
    /// </summary>
    public bool ReplaceAtCourseOut { get; set; } = false;

    /// <summary>
    /// Defaults to 100.
    /// </summary>
    public byte MuRatio100 { get; set; } = 100;

    /// <summary>
    /// Defaults to <see cref="false"/>.
    /// </summary>
    public bool PenaltyNoReset { get; set; } = false;

    /// <summary>
    /// Handling behavior when under low grip ("mu") conditions. Defaults to <see cref="LowMuType.MODERATE"/>.
    /// </summary>
    public LowMuType LowMuType { get; set; } = LowMuType.MODERATE;

    /// <summary>
    /// Behavior when car damage occurs. Defaults to <see cref="BehaviorDamageType.WEAK"/>.
    /// </summary>
    public BehaviorDamageType BehaviorDamage { get; set; } = BehaviorDamageType.WEAK;

    /// <summary>
    /// Slipstream behavior. Defaults to <see cref="BehaviorSlipStreamType.GAME"/>.
    /// </summary>
    public BehaviorSlipStreamType SlipstreamBehavior { get; set; } = BehaviorSlipStreamType.GAME;

    /// <summary>
    /// Ghost presence type. Defaults to <see cref="GhostPresenceType.NORMAL"/>.
    /// </summary>
    public GhostPresenceType GhostPresenceType { get; set; } = GhostPresenceType.NORMAL;

    /// <summary>
    /// GT6 Only. Maximum amount of visual ghost lines. Defaults to 0 (none).
    /// </summary>
    public byte LineGhostPlayMax { get; set; }

    /// <summary>
    /// GT6 Only. Line ghost record type. Defaults to <see cref="LineGhostRecordType.OFF"/>.
    /// </summary>
    public LineGhostRecordType LineGhostRecordType { get; set; } = LineGhostRecordType.OFF;

    /// <summary>
    /// GT6 Only. Attack separate type. Defaults to <see cref="AttackSeparateType.DISABLE"/>.
    /// </summary>
    public AttackSeparateType AttackSeparateType { get; set; } = AttackSeparateType.DISABLE;

    public sbyte[] GridList { get; set; } = new sbyte[32];

    public List<short> EventVList { get; set; } = new List<short>();

    /// <summary>
    /// GT6 Only. Location of the start in VCoord. Defaults to -1 (not specified).
    /// </summary>
    public int EventStartV { get; set; } = -1;

    /// <summary>
    /// Location of the goal in VCoord. Defaults to -1 (not specified).
    /// </summary>
    public int EventGoalV { get; set; } = -1;

    /// <summary>
    /// Goal width. Defaults to -1 (not specified).
    /// </summary>
    public sbyte EventGoalWidth { get; set; } = -1;

    /// <summary>
    /// Defaults to false.
    /// </summary>
    public bool FixedRetention { get; set; } = false;

    private int _initialRetention10_trackWetness = -1;

    /// <summary>
    /// Defaults to -1. Max 10.
    /// </summary>
    public int InitialRetention10_TrackWetness
    {
        get => _initialRetention10_trackWetness;
        set => _initialRetention10_trackWetness = value > 10 ? 10 : value;
    }

    /// <summary>
    /// Defaults to <see cref="DecisiveWeatherType.NONE"/>
    /// </summary>
    public DecisiveWeatherType DecisiveWeather { get; set; } = DecisiveWeatherType.NONE;

    public const byte MaxWeatherPoints = 16;
    public byte _weatherPointNum = 4;

    /// <summary>
    /// Defaults to 4. <see cref="MaxWeatherPoints"/> for the maximum (for GT6), otherwise 4 for GT5.
    /// </summary>
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

    /// <summary>
    /// Total length of the weather progression in seconds. In GT5 this will be the last step based on <see cref="WeatherPointNum"/>. Defaults to 180.0 (3 minutes).
    /// </summary>
    public ushort WeatherTotalSec { get; set; } = 180;

    /// <summary>
    /// For GT5 mostly (still works in GT6 but <see cref="NewWeatherData"/> should be used instead). At which second the 1st step will occur, which is after the start of the race. 
    /// Should be within <see cref="WeatherTotalSec"/>. Defaults to -1.
    /// </summary>
    public float WeatherRateSec1 { get; set; } = -1;

    /// <summary>
    /// For GT5 mostly (still works in GT6 but <see cref="NewWeatherData"/> should be used instead). At which second the 2nd step will occur. Should be within <see cref="WeatherTotalSec"/>.
    /// Defaults to -1.
    /// </summary>
    public float WeatherRateSec2 { get; set; } = -1;

    /// <summary>
    /// For GT5 mostly (still works in GT6 but <see cref="NewWeatherData"/> should be used instead). Weather value for the start of the race. (which is at the start of the race, 0s).
    /// Defaults to -1.
    /// </summary>
    public float WeatherValue0 { get; set; } = -1;

    /// <summary>
    /// For GT5 mostly (still works in GT6 but <see cref="NewWeatherData"/> should be used instead). Weather value for the start of the race to 1st step, specified by <see cref="WeatherRateSec1"/>).
    /// Defaults to -1.
    /// </summary>
    public float WeatherValue1 { get; set; } = -1;

    /// <summary>
    /// For GT5 mostly (still works in GT6 but <see cref="NewWeatherData"/> should be used instead). Weather value for the 1st to 2nd step, specified by <see cref="WeatherRateSec2"/>).
    /// Defaults to -1.
    /// </summary>
    public float WeatherValue2 { get; set; } = -1;

    /// <summary>
    /// For GT5 mostly (still works in GT6 but <see cref="NewWeatherData"/> should be used instead). Weather value for the 2nd to last step/end of progression specified by <see cref="WeatherTotalSec"/>.
    /// Defaults to -1.
    /// </summary>
    public float WeatherValue3 { get; set; } = -1;

    /// <summary>
    /// Defaults to 0.
    /// </summary>
    public int WeatherRandomSeed { get; set; }

    /// <summary>
    /// Defaults to true.
    /// </summary>
    public bool WeatherNoPrecipitation { get; set; } = true;

    /// <summary>
    /// Defaults to false.
    /// </summary>
    public bool WeatherNoWind { get; set; } = false;

    /// <summary>
    /// Defaults to false.
    /// </summary>
    public bool WeatherPrecRainOnly { get; set; } = false;

    /// <summary>
    /// Defaults to false.
    /// </summary>
    public bool WeatherPrecSnowOnly { get; set; } = false;

    /// <summary>
    /// Defaults to true.
    /// </summary>
    public bool WeatherNoSchedule { get; set; } = true;

    /// <summary>
    /// Defaults to true.
    /// </summary>
    public bool WeatherRandom { get; set; } = true;

    /// <summary>
    /// Defaults to 10.
    /// </summary>
    public short WeatherAccel10 { get; set; } = 10;

    /// <summary>
    /// Defaults to 10.
    /// </summary>
    public short WeatherAccelWaterRetention10 { get; set; } = 10;

    public sbyte _weatherBaseCelsius = 63;

    /// <summary>
    /// Defaults to 63 (use defaults).
    /// </summary>
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

    /// <summary>
    /// Defaults to 3.
    /// </summary>
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

    /// <summary>
    /// Defaults to 3.
    /// </summary>
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


    public bool RollingPlayerGrid { get; set; }


    public SessionType SessionType { get; set; }
    public bool VehicleFreezeMode { get; set; } // stage_data->at_quick seems to also enable this
    public bool WithGhost { get; set; }

    /// <summary>
    /// GT6 Only.
    /// </summary>
    public List<WeatherData> NewWeatherData { get; set; } = new List<WeatherData>();

    public byte[] DelayStartList { get; set; } = new byte[32];

    /// <summary>
    /// GT5 Only.
    /// </summary>
    public BoostParams? BoostParams { get; set; }

    /// <summary>
    /// GT6 Only.
    /// </summary>
    public BoostTable[] BoostTables { get; set; } = new BoostTable[2] { new BoostTable(), new BoostTable() };

    private byte[] LaunchSpeedList { get; set; } = new byte[32];
    private short[] LaunchPositionList { get; set; } = new short[32];
    private short[] StartTypeSlotList { get; set; } = new short[32];

    public PenaltyParameter PenaltyParameter { get; set; } = new PenaltyParameter();

    /// <summary>
    /// Not part of XML 
    /// </summary>
    public byte RaceInitialLaps { get; set; } = 0;

    /// <summary>
    /// Not part of XML
    /// </summary>
    public int CourseCode { get; set; }
    
    public RaceParameter()
    {
        for (int i = 0; i < 32; i++)
        {
            GridList[i] = (sbyte)i;
            StartTypeSlotList[i] = -1;
        }
    }

    public void CopyTo(RaceParameter other)
    {
        other.RaceType = RaceType;
        other.StartType = StartType;
        other.CompleteType = CompleteType;
        other.FinishType = FinishType;
        other.RaceLimitLaps = RaceLimitLaps;
        other.RaceLimitMinute = RaceLimitMinute;
        other.TimeToStart = TimeToStart;
        other.TimeToFinish = TimeToFinish;
        other.EntryMax = EntryMax;
        other.RacersMax = RacersMax;
        other.KeepLoadGhost = KeepLoadGhost;
        other.ImmediateFinish = ImmediateFinish;
        other.GridSortType = GridSortType;
        other.AutostartPitout = AutostartPitout;
        other.Endless = Endless;
        other.GhostType = GhostType;
        other.DisableCollision = DisableCollision;
        other.PenaltyLevel = PenaltyLevel;
        other.Accumulation = Accumulation;
        other.BSpecVitality10 = BSpecVitality10;
        other.ConsumeFuelRate = ConsumeFuelRate;
        other.ConsumeTireRate = ConsumeTireRate;
        other.TemperatureTire = TemperatureTire;
        other.TemperatureBrake = TemperatureBrake;
        other.TemperatureEngine = TemperatureEngine;
        other.GoalTimeUseLapTotal = GoalTimeUseLapTotal;
        other.StartTimeOffset = StartTimeOffset;
        other.StartSignalType = StartSignalType;
        other.ConsiderationType = ConsiderationType;
        other.AcademyEvent = AcademyEvent;
        other.LightingMode = LightingMode;
        other.BoostLevel = BoostLevel;
        other.BoostType = BoostType;
        other.BoostFlag = BoostFlag;
        other.Date = Date;
        other.TimeProgressSpeed = TimeProgressSpeed;
        other.EnablePit = EnablePit;
        other.PitConstraint = PitConstraint;
        other.CourseOutPenaltyMargin = CourseOutPenaltyMargin;
        other.BehaviorFallBack = BehaviorFallBack;
        other.NeedTireChange = NeedTireChange;
        other.DisableRecordingReplay = DisableRecordingReplay;
        other.Flagset = Flagset;
        other.OnlineOn = OnlineOn;
        other.AutoStandingDelay = AutoStandingDelay;
        other.AllowCoDriver = AllowCoDriver;
        other.PaceNote = PaceNote;
        other.EnableDamage = EnableDamage;
        other.ReplaceAtCourseOut = ReplaceAtCourseOut;
        other.MuRatio100 = MuRatio100;
        other.PenaltyNoReset = PenaltyNoReset;
        other.LowMuType = LowMuType;
        other.BehaviorDamage = BehaviorDamage;
        other.SlipstreamBehavior = SlipstreamBehavior;
        other.GhostPresenceType = GhostPresenceType;
        other.LineGhostPlayMax = LineGhostPlayMax;
        other.LineGhostRecordType = LineGhostRecordType;
        other.AttackSeparateType = AttackSeparateType;
        GridList.AsSpan().CopyTo(other.GridList);

        foreach (short v in EventVList)
            other.EventVList.Add(v);

        other.EventStartV = EventStartV;
        other.EventGoalV = EventGoalV;
        other.EventGoalWidth = EventGoalWidth;
        other.FixedRetention = FixedRetention;
        other.InitialRetention10_TrackWetness = InitialRetention10_TrackWetness;
        other.DecisiveWeather = DecisiveWeather;
        other.WeatherPointNum = WeatherPointNum;
        other.WeatherTotalSec = WeatherTotalSec;
        other.WeatherRateSec1 = WeatherRateSec1;
        other.WeatherRateSec2 = WeatherRateSec2;
        other.WeatherValue0 = WeatherValue0;
        other.WeatherValue1 = WeatherValue1;
        other.WeatherValue2 = WeatherValue2;
        other.WeatherValue3 = WeatherValue3;
        other.WeatherRandomSeed = WeatherRandomSeed;
        other.WeatherNoPrecipitation = WeatherNoPrecipitation;
        other.WeatherNoWind = WeatherNoWind;
        other.WeatherPrecRainOnly = WeatherPrecRainOnly;
        other.WeatherPrecSnowOnly = WeatherPrecSnowOnly;
        other.WeatherNoSchedule = WeatherNoSchedule;
        other.WeatherRandom = WeatherRandom;
        other.WeatherAccel10 = WeatherAccel10;
        other.WeatherAccelWaterRetention10 = WeatherAccelWaterRetention10;
        other.WeatherBaseCelsius = WeatherBaseCelsius;
        other.WeatherMaxCelsius = WeatherMaxCelsius;
        other.WeatherMinCelsius = WeatherMinCelsius;
        other.RollingPlayerGrid = RollingPlayerGrid;
        other.SessionType = SessionType;
        other.VehicleFreezeMode = VehicleFreezeMode;
        other.WithGhost = WithGhost;

        foreach (var weatherData in NewWeatherData)
        {
            var data = new WeatherData();
            weatherData.CopyTo(data);
            other.NewWeatherData.Add(weatherData);
        }

        DelayStartList.AsSpan().CopyTo(DelayStartList);

        if (BoostParams != null && other.BoostParams != null)
            BoostParams.CopyTo(other.BoostParams);

        for (int i = 0; i < 2; i++)
            BoostTables[i].CopyTo(other.BoostTables[i]);

        LaunchSpeedList.AsSpan().CopyTo(LaunchSpeedList);
        LaunchPositionList.AsSpan().CopyTo(LaunchPositionList);
        StartTypeSlotList.AsSpan().CopyTo(StartTypeSlotList);

        other.RaceInitialLaps = RaceInitialLaps;
        other.CourseCode = CourseCode;
    }

    public void WriteToXml(XmlWriter xml)
    {
        xml.WriteElementValue("race_type", RaceType.ToString());
        xml.WriteElementValue("start_type", StartType.ToString());
        xml.WriteElementValue("complete_type", CompleteType.ToString());
        xml.WriteElementValue("finish_type", FinishType.ToString());
        xml.WriteElementInt("race_limit_laps", RaceLimitLaps);
        xml.WriteElementInt("race_limit_minute", RaceLimitMinute);
        xml.WriteElementInt("time_to_start", (int)TimeToStart.TotalMilliseconds);
        xml.WriteElementInt("time_to_finish", (int)TimeToFinish.TotalMilliseconds);
        xml.WriteElementInt("entry_max", EntryMax);
        xml.WriteElementInt("racers_max", EntryMax);
        xml.WriteElementBool("keep_load_ghost", KeepLoadGhost);
        xml.WriteElementBool("immediate_finish", ImmediateFinish);
        xml.WriteElementValue("grid_sort_type", GridSortType.ToString());
        xml.WriteElementBool("autostart_pitout", AutostartPitout);
        xml.WriteElementBool("endless", Endless);
        xml.WriteElementValue("ghost_type", GhostType.ToString());
        xml.WriteElementBool("disable_collision", DisableCollision);
        xml.WriteElementInt("penalty_level", (int)PenaltyLevel);
        xml.WriteElementBool("accumulation", Accumulation);
        xml.WriteElementInt("bspec_vitality10", BSpecVitality10);
        xml.WriteElementInt("consume_fuel", ConsumeFuelRate);
        xml.WriteElementInt("consume_tire", ConsumeTireRate);
        xml.WriteElementInt("temperature_tire", TemperatureTire);
        xml.WriteElementInt("temperature_brake", TemperatureBrake);
        xml.WriteElementInt("temperature_engine", TemperatureEngine);
        xml.WriteElementBool("goal_time_use_lap_total", GoalTimeUseLapTotal);
        xml.WriteElementInt("start_time_offset", StartTimeOffset);
        xml.WriteElementValue("start_signal_type", StartSignalType.ToString());
        xml.WriteElementInt("consideration_type", ConsiderationType);

        xml.WriteElementBool("academy_event", AcademyEvent);
        xml.WriteElementValue("lighting_mode", LightingMode.ToString());

        if (BoostParams != null)
        {
            xml.WriteStartElement("boost_params");
            xml.WriteElementInt("param", BoostParams.BoostFront);
            xml.WriteElementInt("param", BoostParams.BoostRear);
            xml.WriteElementInt("param", BoostParams.BoostFrontMax);
            xml.WriteElementInt("param", BoostParams.BoostRearMax);
            xml.WriteElementInt("param", BoostParams.BoostFrontMin);
            xml.WriteElementInt("param", BoostParams.BoostRearMin);
            xml.WriteEndElement();
        }

        if (BoostTables.Any(t => !t.IsDefault()))
        {
            xml.WriteStartElement("boost_table_array");
            for (int i = 0; i < 2; i++)
            {
                xml.WriteStartElement("boost_table");
                {
                    xml.WriteElementInt("param", i);

                    BoostTable table = BoostTables[i];
                    xml.WriteElementInt("param", table.FrontDistance1);
                    xml.WriteElementInt("param", table.FrontRate1);
                    xml.WriteElementInt("param", table.FrontDistance2);
                    xml.WriteElementInt("param", table.FrontRate2);

                    xml.WriteElementInt("param", table.RearDistance1);
                    xml.WriteElementInt("param", table.RearRate1);
                    xml.WriteElementInt("param", table.RearDistance2);
                    xml.WriteElementInt("param", table.RearRate2);

                    xml.WriteElementInt("param", table.TargetPosition);
                    xml.WriteElementInt("param", table.RaceProgress);
                }
                xml.WriteEndElement();

            }
            xml.WriteEndElement();
        }

        xml.WriteElementInt("boost_level", BoostLevel);
        xml.WriteElementBool("boost_type", BoostType);
        xml.WriteElementBool("boost_flag", BoostFlag);

        xml.WriteStartElement("datetime");
        if (Date is null)
            xml.WriteAttributeString("datetime", "1970/00/00 00:00:00");
        else
            xml.WriteAttributeString("datetime", Date.Value.ToString("yyyy/MM/dd HH:mm:ss"));
        xml.WriteEndElement();

        xml.WriteElementInt("time_progress_speed", TimeProgressSpeed);
        // lucky_player

        xml.WriteElementBool("enable_pit", EnablePit);
        xml.WriteElementInt("pit_constraint", PitConstraint);
        xml.WriteElementInt("course_out_penalty_margin", CourseOutPenaltyMargin);
        xml.WriteElementInt("behavior_fallback", BehaviorFallBack);
        xml.WriteElementBool("need_tire_change", NeedTireChange);
        xml.WriteElementBool("disable_recording_replay", DisableRecordingReplay);
        xml.WriteElementValue("flagset", Flagset.ToString());
        xml.WriteElementBool("online_on", OnlineOn);

        var out_of_order = false;
        for (var i = 0; i < GridList.Length; i++)
        {
            if (GridList[i] != i)
            {
                out_of_order = true;
                break;
            }
        }

        if (out_of_order)
        {
            xml.WriteStartElement("grid_list");
            foreach (var v in GridList)
                xml.WriteElementInt("grid", v);
            xml.WriteEndElement();
        }

        out_of_order = false;
        for (var i = 0; i < EventVList.Count; i++)
        {
            if (EventVList[i] != i)
            {
                out_of_order = true;
                break;
            }
        }

        if (out_of_order)
        {
            xml.WriteStartElement("event_v_list");
            foreach (var v in EventVList)
                xml.WriteElementInt("v", v);
            xml.WriteEndElement();
        }

        xml.WriteElementInt("auto_standing_delay", AutoStandingDelay);
        xml.WriteElementBool("allow_codriver", AllowCoDriver);
        xml.WriteElementBool("pace_note", PaceNote);
        xml.WriteElementBool("replace_at_courseout", ReplaceAtCourseOut);
        xml.WriteElementInt("mu_ratio100", MuRatio100);
        xml.WriteElementBool("enable_damage", EnableDamage);
        xml.WriteElementBool("penalty_no_reset", PenaltyNoReset);
        xml.WriteElementValue("low_mu_type", LowMuType.ToString());
        xml.WriteElementValue("behavior_damage_type", BehaviorDamage.ToString());
        xml.WriteElementValue("behavior_slip_stream_type", SlipstreamBehavior.ToString());
        xml.WriteElementValue("ghost_presence_type", GhostPresenceType.ToString());
        xml.WriteElementInt("line_ghost_play_max", LineGhostPlayMax);
        xml.WriteElementValue("line_ghost_record_type", LineGhostRecordType.ToString());
        xml.WriteElementValue("attack_separate_type", AttackSeparateType.ToString());
        xml.WriteElementInt("event_start_v", EventStartV);
        xml.WriteElementFloat("event_goal_v", EventGoalV);
        xml.WriteElementFloat("event_goal_width", EventGoalWidth);

        xml.WriteElementBool("fixed_retention", FixedRetention);
        xml.WriteElementValue("decisive_weather", DecisiveWeather.ToString());
        xml.WriteElementInt("weather_random_seed", WeatherRandomSeed);
        xml.WriteElementBool("weather_random", WeatherRandom);

        xml.WriteElementInt("weather_accel100", WeatherAccel10); // FIXME
        xml.WriteElementInt("weather_base_celsius", WeatherBaseCelsius);
        xml.WriteElementInt("weather_max_celsius", WeatherMaxCelsius);
        xml.WriteElementInt("weather_min_celsius", WeatherMinCelsius);

        xml.WriteElementFloat("weather_rate_sec1", WeatherRateSec1);
        xml.WriteElementFloat("weather_rate_sec2", WeatherRateSec2);
        xml.WriteElementFloat("weather_value0", WeatherValue0);
        xml.WriteElementFloat("weather_value1", WeatherValue1);
        xml.WriteElementFloat("weather_value2", WeatherValue2);
        xml.WriteElementFloat("weather_value3", WeatherValue3);

        xml.WriteElementBool("weather_no_schedule", WeatherNoSchedule);
        xml.WriteElementBool("weather_no_precipitation", WeatherNoPrecipitation);
        xml.WriteElementBool("weather_no_wind", WeatherNoWind);
        xml.WriteElementBool("weather_prec_rain_only", WeatherPrecRainOnly);
        xml.WriteElementBool("weather_prec_snow_only", WeatherPrecSnowOnly);
        xml.WriteElementInt("weather_point_num", WeatherPointNum);
        xml.WriteElementFloat("weather_total_sec", WeatherTotalSec);
        xml.WriteElementInt("initial_retention10", InitialRetention10_TrackWetness);
        xml.WriteElementInt("weather_accel10", WeatherAccel10);
        xml.WriteElementInt("weather_accel_water_retention10", WeatherAccelWaterRetention10);

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
    }

    public void ParseFromXml(XmlNode node)
    {
        foreach (XmlNode raceNode in node.ChildNodes)
        {
            switch (raceNode.Name)
            {
                case "race_type":
                    RaceType = raceNode.ReadValueEnum<RaceType>(); break;
                case "start_type":
                    StartType = raceNode.ReadValueEnum<StartType>(); break;
                case "complete_type":
                    CompleteType = raceNode.ReadValueEnum<CompleteType>(); break;
                case "finish_type":
                    FinishType = raceNode.ReadValueEnum<FinishType>(); break;
                case "race_limit_laps":
                    RaceLimitLaps = raceNode.ReadValueShort(); break;
                case "race_limit_minute":
                    RaceLimitMinute = raceNode.ReadValueShort(); break;
                case "time_to_start":
                    TimeToStart = TimeSpan.FromMilliseconds(raceNode.ReadValueInt()); break;
                case "time_to_finish":
                    TimeToFinish = TimeSpan.FromMilliseconds(raceNode.ReadValueInt()); break;
                case "entry_max":
                    EntryMax = raceNode.ReadValueShort(); break;
                case "racers_max":
                    EntryMax = raceNode.ReadValueShort(); break;
                case "keep_load_ghost":
                    KeepLoadGhost = raceNode.ReadValueBool(); break;
                case "immediate_finish":
                    ImmediateFinish = raceNode.ReadValueBool(); break;
                case "grid_sort_type":
                    GridSortType = raceNode.ReadValueEnum<GridSortType>(); break;
                case "autostart_pitout":
                    AutostartPitout = raceNode.ReadValueBool(); break;
                case "endless":
                    Endless = raceNode.ReadValueBool(); break;
                case "ghost_type":
                    GhostType = raceNode.ReadValueEnum<GhostType>(); break;
                case "disable_collision":
                    DisableCollision = raceNode.ReadValueBool(); break;
                case "penalty_level":
                    PenaltyLevel = (PenaltyLevelTypes)raceNode.ReadValueInt(); break;
                case "accumulation":
                    Accumulation = raceNode.ReadValueBool(); break;
                case "bspec_vitality10":
                    BSpecVitality10 = raceNode.ReadValueInt(); break;
                case "consume_fuel":
                    ConsumeFuelRate = raceNode.ReadValueByte(); break;
                case "consume_tire":
                    ConsumeTireRate = raceNode.ReadValueByte(); break;
                case "temperature_tire":
                    TemperatureTire = raceNode.ReadValueByte(); break;
                case "temperature_brake":
                    TemperatureBrake = raceNode.ReadValueByte(); break;
                case "temperature_engine":
                    TemperatureEngine = raceNode.ReadValueByte(); break;
                case "goal_time_use_lap_total":
                    GoalTimeUseLapTotal = raceNode.ReadValueBool(); break;
                case "start_time_offset":
                    StartTimeOffset = raceNode.ReadValueByte(); break;
                case "start_signal_type":
                    StartSignalType = raceNode.ReadValueEnum<StartSignalType>(); break;
                case "consideration_type":
                    ConsiderationType = raceNode.ReadValueByte(); break;
                case "academy_event":
                    AcademyEvent = raceNode.ReadValueBool(); break;
                case "lighting_mode":
                    LightingMode = raceNode.ReadValueEnum<LightingMode>(); break;
                case "boost_params":
                    ParseBoostParams(raceNode); break;
                case "boost_table_array":
                    ParseBoostTables(raceNode); break;
                case "boost_level":
                    BoostLevel = raceNode.ReadValueByte(); break;
                case "boost_type":
                    BoostType = raceNode.ReadValueBool(); break;
                case "boost_flag":
                    BoostFlag = raceNode.ReadValueBool(); break;
                case "datetime":
                    string? dateStr = raceNode.Attributes?["datetime"]?.Value;
                    if (dateStr is not null)
                    {
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
                    }
                    break;
                case "time_progress_speed":
                    TimeProgressSpeed = raceNode.ReadValueSByte(); break;
                // TODO: lucky_player
                case "enable_pit":
                    EnablePit = raceNode.ReadValueBool(); break;
                case "pit_constraint":
                    PitConstraint = raceNode.ReadValueByte(); break;
                case "course_out_penalty_margin":
                    CourseOutPenaltyMargin = raceNode.ReadValueByte(); break;
                case "behavior_fallback":
                    BehaviorFallBack = raceNode.ReadValueInt(); break;
                case "need_tire_change":
                    NeedTireChange = raceNode.ReadValueBool(); break;
                case "disable_recording_replay":
                    DisableRecordingReplay = raceNode.ReadValueBool(); break;
                case "flagset":
                    Flagset = raceNode.ReadValueEnum<Flagset>(); break;
                case "online_on":
                    OnlineOn = raceNode.ReadValueBool(); break;
                case "auto_standing_delay":
                   AutoStandingDelay = raceNode.ReadValueByte(); break;
                case "allow_codriver":
                    AllowCoDriver = raceNode.ReadValueBool(); break;
                case "pace_note":
                    PaceNote = raceNode.ReadValueBool(); break;
                case "enable_damage":
                    EnableDamage = raceNode.ReadValueBool(); break;
                case "replace_at_courseout":
                    ReplaceAtCourseOut = raceNode.ReadValueBool(); break;
                case "mu_ratio100":
                    MuRatio100 = raceNode.ReadValueByte(); break;
                case "penalty_no_reset":
                    PenaltyNoReset = raceNode.ReadValueBool(); break;
                case "low_mu_type":
                    LowMuType = raceNode.ReadValueEnum<LowMuType>(); break;
                case "behavior_damage_type":
                    BehaviorDamage = raceNode.ReadValueEnum<BehaviorDamageType>(); break;
                case "behavior_slip_stream_type":
                    SlipstreamBehavior = raceNode.ReadValueEnum<BehaviorSlipStreamType>(); break;
                case "ghost_presence_type":
                    GhostPresenceType = raceNode.ReadValueEnum<GhostPresenceType>(); break;
                case "line_ghost_play_max":
                    LineGhostPlayMax = raceNode.ReadValueByte(); break;
                case "line_ghost_record_type":
                    LineGhostRecordType = raceNode.ReadValueEnum<LineGhostRecordType>(); break;
                case "attack_separate_type":
                    AttackSeparateType = raceNode.ReadValueEnum<AttackSeparateType>(); break;
                case "grid_list":
                    {
                        int i = 0;
                        var nodes = raceNode.SelectNodes("v");
                        if (nodes is not null)
                        {
                            foreach (XmlNode? n in nodes)
                            {
                                if (i >= 32)
                                    break;

                                if (n is null)
                                {
                                    throw new ArgumentException("grid_list has a null node");
                                }


                                GridList[i++] = n.ReadValueSByte();
                            }
                        }
                    }
                    break;
                case "event_v_list":
                    {
                        var nodes = raceNode.SelectNodes("v");
                        int i = 0;
                        if (nodes is not null)
                        {
                            foreach (XmlNode n in nodes)
                            {
                                if (i >= 30)
                                    break;

                                if (n is null)
                                {
                                    throw new ArgumentException("event_v_list has a null node");
                                }

                                EventVList.Add(n.ReadValueShort());
                                i++;
                            }
                        }
                    }
                    break;

                case "event_start_v":
                    EventStartV = raceNode.ReadValueInt(); break;
                case "event_goal_v":
                    EventGoalV = raceNode.ReadValueInt(); break;
                case "event_goal_width":
                    EventGoalWidth = (sbyte)raceNode.ReadValueInt(); break;
                case "fixed_retention":
                    FixedRetention = raceNode.ReadValueBool(); break;
                case "initial_retention10":
                    InitialRetention10_TrackWetness = raceNode.ReadValueInt(); break;
                case "decisive_weather":
                    DecisiveWeather = raceNode.ReadValueEnum<DecisiveWeatherType>(); break;
                case "weeather_point_num":
                    WeatherPointNum = raceNode.ReadValueByte(); break;
                case "weather_total_sec":
                    WeatherTotalSec = (ushort)raceNode.ReadValueSingle(); break;
                case "weather_random_seed":
                    WeatherRandomSeed = raceNode.ReadValueInt(); break;
                case "weather_random":
                    WeatherRandom = raceNode.ReadValueBool(); break;
                case "weather_no_precipitation":
                    WeatherNoPrecipitation = raceNode.ReadValueBool(); break;
                case "weather_no_wind":
                    WeatherNoSchedule = raceNode.ReadValueBool(); break;
                case "weather_prec_rain_only":
                    WeatherPrecRainOnly = raceNode.ReadValueBool(); break;
                case "weather_prec_snow_only":
                    WeatherPrecSnowOnly = raceNode.ReadValueBool(); break;
                case "weather_no_schedule":
                    WeatherNoSchedule = raceNode.ReadValueBool(); break;
                case "weather_accel10":
                    WeatherAccel10 = raceNode.ReadValueShort(); break;
                case "weather_accel_water_retention10":
                    WeatherAccelWaterRetention10 = raceNode.ReadValueShort(); break;

                case "weather_base_celsius":
                    WeatherBaseCelsius = raceNode.ReadValueSByte(); break;
                case "weather_max_celsius":
                    WeatherMaxCelsius = raceNode.ReadValueSByte(); break;
                case "weather_min_celsius":
                    WeatherMinCelsius = raceNode.ReadValueSByte(); break;

                case "weather_rate_sec1":
                    WeatherRateSec1 = raceNode.ReadValueSingle(); break;
                case "weather_rate_sec2":
                    WeatherRateSec2 = raceNode.ReadValueSingle(); break;
                case "weather_value0":
                    WeatherValue0 = raceNode.ReadValueSingle(); break;
                case "weather_value1":
                    WeatherValue1 = raceNode.ReadValueSingle(); break;
                case "weather_value2":
                    WeatherValue2 = raceNode.ReadValueSingle(); break;
                case "weather_value3":
                    WeatherValue3 = raceNode.ReadValueSingle(); break;

                /* Not part of XML reading
                case "race_initial_laps":
                    RaceInitialLaps = raceNode.ReadValueByte(); break;
                case "rolling_player_grid":
                    RollingPlayerGrid = raceNode.ReadValueBool(); break;
                case "vehicle_freeze_mode":
                    VehicleFreezeMode = raceNode.ReadValueBool(); break;
                case "with_ghost":
                    WithGhost = raceNode.ReadValueBool(); break;
                */
                case "new_weather_data":
                    {
                        var nodes = raceNode.SelectNodes("point");
                        if (nodes is not null)
                        {
                            foreach (XmlNode point in nodes)
                            {
                                var data = new WeatherData();
                                foreach (XmlNode pointNode in point.ChildNodes)
                                {
                                    switch (pointNode.Name)
                                    {
                                        case "time_rate":
                                            data.TimeRate = pointNode.ReadValueSingle(); break;
                                        case "low":
                                            {
                                                string? value = pointNode.ReadValueString();
                                                if (value is not null && float.TryParse(value, CultureInfo.InvariantCulture.NumberFormat, out float low))
                                                    data.Low = low;
                                            }
                                            break;
                                        case "high":
                                            {
                                                string? value = pointNode.ReadValueString();
                                                if (value is not null && float.TryParse(value, CultureInfo.InvariantCulture.NumberFormat, out float high))
                                                    data.High = high;
                                            }
                                            break;
                                    }
                                }
                                NewWeatherData.Add(data);
                            }
                        }
                    }
                    break;
            }
        }
    }

    public void ParseBoostParams(XmlNode boostParamsNode)
    {
        BoostParams = new BoostParams();

        var nodes = boostParamsNode.SelectNodes("param");
        if (nodes is not null)
        {
            int i = 0;
            foreach (XmlNode node in nodes)
            {
                switch (i)
                {
                    case 0:
                        BoostParams.BoostFront = node.ReadValueByte(); break;
                    case 1:
                        BoostParams.BoostRear = node.ReadValueSByte(); break;
                    case 2:
                        BoostParams.BoostFrontMax = node.ReadValueByte(); break;
                    case 3:
                        BoostParams.BoostRearMax = node.ReadValueSByte(); break;
                    case 4:
                        BoostParams.BoostFrontMin = node.ReadValueByte(); break;
                    case 5:
                        BoostParams.BoostRear = node.ReadValueSByte(); break;
                }

                i++;
            }
        }
    }

    public void ParseBoostTables(XmlNode boostTableArrayNode)
    {
        XmlNodeList? boostTableNodes = boostTableArrayNode.SelectNodes("boost_table");
        if (boostTableNodes is null)
            return;

        foreach (XmlNode? boostTableNode in boostTableNodes)
        {
            if (boostTableNode is null)
                continue;

            var paramList = boostTableNode.SelectNodes("param");
            int currentIndex = -1;

            if (paramList is null)
                continue;

            for (int i = 0; i < paramList.Count; i++)
            {
                XmlNode? currentNode = paramList[i];

                if (currentNode is null)
                    continue;

                if (i == 0)
                    currentIndex = currentNode.ReadValueInt();
                else
                {
                    if (currentIndex == -1 || currentIndex >= 2)
                        break; // No valid table here

                    switch (i)
                    {
                        case 1:
                            BoostTables[currentIndex].FrontDistance1 = currentNode.ReadValueByte(); break;
                        case 2:                       
                            BoostTables[currentIndex].FrontRate1 = currentNode.ReadValueSByte(); break;
                        case 3:                       
                            BoostTables[currentIndex].FrontDistance2 = currentNode.ReadValueByte(); break;
                        case 4:                       
                            BoostTables[currentIndex].FrontRate2 = currentNode.ReadValueSByte(); break;

                        case 5:
                            BoostTables[currentIndex].RearDistance1 = currentNode.ReadValueByte(); break;
                        case 6:                       
                            BoostTables[currentIndex].RearRate1 = currentNode.ReadValueSByte(); break;
                        case 7:                       
                            BoostTables[currentIndex].RearDistance2 = currentNode.ReadValueByte(); break;
                        case 8:                       
                            BoostTables[currentIndex].RearRate2 = currentNode.ReadValueSByte(); break;

                        case 9:
                            BoostTables[currentIndex].TargetPosition = currentNode.ReadValueByte(); break;
                        case 10:
                            BoostTables[currentIndex].RaceProgress = currentNode.ReadValueByte(); break;
                    }
                }

            }
        }
    }

    public void Deserialize(ref BitStream reader)
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
        RaceLimitLaps = reader.ReadInt16();
        RaceLimitMinute = reader.ReadInt16();
        TimeToStart = TimeSpan.FromMilliseconds(reader.ReadInt32());
        TimeToFinish = TimeSpan.FromMilliseconds(reader.ReadInt32());
        EntryMax = reader.ReadInt16(); // entry_max
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
        EntryMax = reader.ReadInt16();
        reader.ReadInt32(); // unk field 0x2c
        AutostartPitout = reader.ReadBoolBit();
        StartTimeOffset = (byte)reader.ReadBits(5);
        reader.ReadBits(3); // unk - Check this, got 3 on from game and 0 from built
        AutoStandingDelay = reader.ReadByte();
        reader.ReadBits(3); // unk
        StartSignalType = (StartSignalType)reader.ReadBits(2); // start signal type
        reader.ReadBoolBit(); // unk
        reader.ReadBoolBit(); // bench_test
        MuRatio100 = reader.ReadByte();
        reader.ReadBoolBit(); // unk
        EnableDamage = reader.ReadBoolBit();
        LowMuType = (LowMuType)reader.ReadBits(2);
        BehaviorDamage = (BehaviorDamageType)reader.ReadBits(2);
        reader.ReadBoolBit(); // gps
        PenaltyNoReset = reader.ReadBoolBit();
        SlipstreamBehavior = (BehaviorSlipStreamType)reader.ReadBits(2);
        PitConstraint = (byte)reader.ReadBits(4);
        NeedTireChange = reader.ReadBoolBit(); // need_tire_change
        reader.ReadBits(4); // after_race_penalty_sec_5
        reader.ReadBoolBit(); // is_speedtest_milemode
        LineGhostRecordType = (LineGhostRecordType)reader.ReadBits(2);
        AttackSeparateType = (AttackSeparateType)reader.ReadBits(2);
        PenaltyLevel = (PenaltyLevelTypes)reader.ReadSByte();
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
        ConsumeFuelRate = reader.ReadByte();
        reader.ReadByte(); // bspec_vitality_10
        reader.ReadByte(); // consideration_type
        ConsumeTireRate = reader.ReadByte();
        TemperatureTire = reader.ReadByte(); // temperature_tire
        TemperatureEngine = reader.ReadByte(); // temperature_engine
        reader.ReadByte(); // unk field_0x5a
        LightingMode = (LightingMode)reader.ReadByte();
        Date = new PDIDATETIME32(reader.ReadUInt32()).GetDateTime(); // datetime
        TimeProgressSpeed = reader.ReadSByte();
        AllowCoDriver = reader.ReadBool();
        PaceNote = reader.ReadBool();
        reader.ReadByte(); // team_count
        reader.ReadIntoByteArray(32, GridList, BitStream.Byte_Bits);
        reader.ReadIntoByteArray(32, DelayStartList, BitStream.Byte_Bits);
        EventStartV = reader.ReadInt32();
        EventGoalV = reader.ReadInt32();
        EventGoalWidth = reader.ReadSByte();
        FixedRetention = reader.ReadBoolBit();
        InitialRetention10_TrackWetness = (int)reader.ReadBits(4);
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
        WeatherNoPrecipitation = reader.ReadBoolBit();
        WeatherNoWind = reader.ReadBoolBit();
        WeatherPrecRainOnly = reader.ReadBoolBit();
        WeatherPrecSnowOnly = reader.ReadBoolBit();
        WeatherNoSchedule = reader.ReadBoolBit();
        WeatherRandom = reader.ReadBoolBit();
        reader.ReadBoolBit(); //   param_1->field_0xe0 = (uVar3 & 0x1) << 0x39 | param_1->field_0xe0 & 0xfdffffffffffffff;
        WeatherBaseCelsius = (sbyte)reader.ReadBits(7);
        WeatherMinCelsius = (sbyte)reader.ReadBits(4);
        WeatherMaxCelsius = (sbyte)reader.ReadBits(4);
        WeatherAccel10 = (short)reader.ReadBits(10);
        WeatherAccelWaterRetention10 = (short)reader.ReadBits(10);
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
        RaceLimitLaps = reader.ReadInt16();
        RaceLimitMinute = reader.ReadInt16();
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
        EntryMax = reader.ReadInt16();
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
        SlipstreamBehavior = (BehaviorSlipStreamType)reader.ReadBits(2);
        reader.ReadBits(4); // unk (Bits & 0xf << 0xa)
        NeedTireChange = reader.ReadBoolBit(); // need_tire_change
        reader.ReadBits(4); // after_race_penalty_sec5? (Bits & 0xf << 5)
        reader.ReadBoolBit(); // is_speedtest_milemode
        LineGhostRecordType = (LineGhostRecordType)reader.ReadBits(2);
        reader.ReadBits(2); // attack_seperate_type? (Bits & 0x03)
        PenaltyLevel = (PenaltyLevelTypes)reader.ReadByte();
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
        ConsumeTireRate = reader.ReadByte();
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
        InitialRetention10_TrackWetness = (int)reader.ReadBits(4); // initial_retention10

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
        WeatherNoPrecipitation = reader.ReadBoolBit();
        WeatherNoWind = reader.ReadBoolBit();
        WeatherPrecRainOnly = reader.ReadBoolBit();
        WeatherPrecSnowOnly = reader.ReadBoolBit();
        WeatherNoSchedule = reader.ReadBoolBit();
        WeatherRandom = reader.ReadBoolBit();
        reader.ReadBoolBit(); //   param_1->field_0xe0 = (uVar3 & 0x1) << 0x39 | param_1->field_0xe0 & 0xfdffffffffffffff;
        WeatherBaseCelsius = (sbyte)reader.ReadBits(7);
        WeatherMinCelsius = (sbyte)reader.ReadBits(4);
        WeatherMaxCelsius = (sbyte)reader.ReadBits(4);
        WeatherAccel10 = (short)reader.ReadBits(10);
        WeatherAccelWaterRetention10 = (short)reader.ReadBits(10);
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
            BoostTables[i].RearDistance2 = reader.ReadByte();
            BoostTables[i].RearRate2 = reader.ReadSByte();
            BoostTables[i].RearDistance1 = reader.ReadByte();
            BoostTables[i].RearRate1 = reader.ReadSByte();

            BoostTables[i].FrontDistance2 = reader.ReadByte();
            BoostTables[i].FrontRate2 = reader.ReadSByte();
            BoostTables[i].FrontDistance1 = reader.ReadByte();
            BoostTables[i].FrontRate1 = reader.ReadSByte();

            BoostTables[i].TargetPosition = reader.ReadByte();
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
        DisableRecordingReplay = reader.ReadBool();
        GhostPresenceType = (GhostPresenceType)reader.ReadByte();

        for (var i = 0; i < 30; i++)
            EventVList.Add(reader.ReadInt16());

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

    public void Serialize(ref BitStream bs)
    {
        int cPos = bs.Position;
        bs.WriteInt16(0x410); // Buffer Size. Writen size does not match, but is intentionally larger for later versions
        bs.WriteInt16(1_23); // Version

        WriteGeneralSettings(ref bs);
        WriteOtherSettings(ref bs);
        bs.SeekToByte(cPos + 0x410);
    }

    private void WriteGeneralSettings(ref BitStream bs)
    {
        bs.WriteBits((ulong)SessionType, 3);
        bs.WriteByte((byte)RaceType);
        bs.WriteByte((byte)StartType);
        bs.WriteByte((byte)CompleteType);
        bs.WriteByte((byte)FinishType);
        bs.WriteInt16(RaceLimitLaps);
        bs.WriteInt16(RaceLimitMinute);
        bs.WriteInt32((int)TimeToStart.TotalMilliseconds);
        bs.WriteInt32((int)TimeToFinish.TotalMilliseconds);
        bs.WriteInt16(EntryMax);
        bs.WriteInt16(16); // ? field_0x16 - Related to racers max
        bs.WriteBits(0, 7); // Unk
        bs.WriteInt32(-1); // Unk
        bs.WriteByte(0); // CourseLayoutNo
        bs.WriteByte(RaceInitialLaps);
        bs.WriteBoolBit(KeepLoadGhost);
        bs.WriteInt32(CourseCode);
        bs.WriteByte(LineGhostPlayMax);
        bs.WriteBoolBit(GoalTimeUseLapTotal);
        bs.WriteBits(0, 2); // Intentional
        bs.WriteBoolBit(false); // force_pitcrew_off
        bs.WriteInt32(-1); // scenery_code
        bs.WriteInt32(0); // Unk field_0x3c
        bs.WriteInt64(0); // unk field_0x40
        bs.WriteInt16(4000); // packet_timeout_interval
        bs.WriteInt16(4000); // packet_timeout_latency
        bs.WriteInt16(4000); // packet_timeout_lag
        bs.WriteInt16(EntryMax);
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
        bs.WriteByte(ConsumeFuelRate);
        bs.WriteByte(10); // bspec_vitality_10
        bs.WriteByte(ConsiderationType);
        bs.WriteByte(ConsumeTireRate);
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

        bs.WriteInt32(EventStartV);
        bs.WriteInt32(EventGoalV);
        bs.WriteSByte(EventGoalWidth);

        bs.WriteBoolBit(FixedRetention);
        bs.WriteBits((ulong)InitialRetention10_TrackWetness + 1, 4); // initial_retention10 - 10% each
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
        bs.WriteBits((ulong)WeatherAccel10, 10);
        bs.WriteBits((ulong)WeatherAccelWaterRetention10, 10); // weather_accel_water_retention10
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
            bs.WriteByte(BoostTables[i].RearDistance2);
            bs.WriteSByte(BoostTables[i].RearRate2);
            bs.WriteByte(BoostTables[i].RearDistance1);
            bs.WriteSByte(BoostTables[i].RearRate1);

            bs.WriteByte(BoostTables[i].FrontDistance2);
            bs.WriteSByte(BoostTables[i].FrontRate2);
            bs.WriteByte(BoostTables[i].FrontDistance1);
            bs.WriteSByte(BoostTables[i].FrontRate1);

            bs.WriteByte(BoostTables[i].TargetPosition);
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
