using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PDTools.Enums
{
    public enum AttackSeparateType : byte
    {
        DISABLE = 0,
        ALL_MEMBERS = 0,
        ALONE = 1,
        ALONE_SEQUENCE = 2,
        TWIN = 3,
        TWIN_SEQUENCE = 4,
    }

    public enum LowMuType : byte
    {
        MODERATE = 0,
        STRONG = 1,
        REAL = 2
    }

    public enum BehaviorDamageType : byte
    {
        [Description("None")]
        WEAK,

        [Description("Light")]
        MIDDLE,

        [Description("Heavy")]
        STRONG
    }

    public enum PenaltyLevelTypes : sbyte
    {
        NO_TIME5 = -5,

        NO_TIME4 = -4,

        NO_TIME3 = -3,

        NO_TIME2 = -2,

        NO_TIME1 = 1,

        [Description("Off")]
        OFF = -5,
    }

    public enum StartType : sbyte
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
    public enum GridSortType : byte
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

    public enum GhostType : byte
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

    public enum LineGhostRecordType : byte
    {
        OFF,
        ONE,
        TRACKDAY
    }

    public enum GhostPresenceType : byte
    {
        [Description("Normal - Transparent?")]
        NORMAL,

        [Description("None")]
        NONE,

        [Description("Real - Shows an actual car?")]
        REAL,
    }


    public enum BehaviorSlipStreamType : byte
    {
        GAME,
        SIMULATION,
        REAL,
    }

    public enum RaceType : byte
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

    public enum LightingMode : byte
    {
        AUTO,
        OFF,
        POSITION,
        LOW_BEAM,
        HIGH_BEAM
    }

    public enum Flagset : byte
    {
        FLAGSET_NONE,
        FLAGSET_NORMAL,
        FLAGSET_F1,
        FLAGSET_NASCAR,
        FLAGSET_LOW,
        FLAGSET_RALLY,
    }

    public enum DecisiveWeatherType : byte
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

    public enum FinishType : byte
    {
        [Description("None")]
        NONE,

        [Description("Target")]
        TARGET,

        [Description("Fastest Car")]
        FASTEST,
    }

    public enum CompleteType : byte
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

    public enum SessionType : byte
    {
        FINAL,
        QUALIFY,
        PRACTICE,
    }

    public enum StartSignalType : byte
    {
        NORMAL,
        BLACK_OUT,
        GREEN_LIGHT,
    }

    public enum BSpecType : byte
    {
        [Description("Both A And B")]
        BOTH_A_AND_B,

        [Description("A-Spec Only")]
        ONLY_A,

        [Description("B-Spec Only")]
        ONLY_B,
    }

    public enum PlayType : byte
    {
        [Description("Race")]
        RACE,

        [Description("Demo Mode")]
        DEMO,

        [Description("Gamble (unused/implemented?)")]
        GAMBLE,
    }


    public enum TireType : sbyte
    {
        [Description("No restrictions")]
        NONE_SPECIFIED = -1,

        [Description("Comfort - Hard")]
        COMFORT_HARD,

        [Description("Comfort - Medium")]
        COMFORT_MEDIUM,

        [Description("Comfort - Soft")]
        COMFORT_SOFT,

        [Description("Sports - Hard")]
        SPORTS_HARD,

        [Description("Sports - Medium")]
        SPORTS_MEDIUM,

        [Description("Sports - Soft")]
        SPORTS_SOFT,

        [Description("Sports - Super Soft")]
        SPORTS_SUPER_SOFT,

        [Description("Racing - Hard")]
        RACING_HARD,

        [Description("Racing - Medium")]
        RACING_MEDIUM,

        [Description("Racing - Soft")]
        RACING_SOFT,

        [Description("Racing - Super Soft")]
        RACING_SUPER_SOFT,

        [Description("Racing - Rain Intermediate")]
        RAIN_INTERMEDIATE,

        [Description("Racing - Heavy Wet")]
        RAIN_HEAVY_WET,

        [Description("Dirt Tyres")]
        DIRT,

        [Description("Spiked Snow Tyres")]
        SPIKED_SNOW,
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

    public enum EventType : int
    {
        RACE,
        RACE_WITH_QUALIFY,
        TRACKDAY,
    }

    public enum RankingType : short
    {
        [Description("No Rankings")]
        NONE,

        [Description("By Best Time")]
        TIME,

        [Description("By Drift Score")]
        DRIFT,
    }

    public enum EvalConditionType : int
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

    public enum StageLayoutType : sbyte
    {
        DEFAULT,
        RANK,
        SLOT,
        FRONT_2GRID,
    }

    public enum StageCoordType
    {
        WORLD,
        GRID,
        PITSTOP,
        VCOORD,
        START,
        GRID_ALL,
        PITSTOP_ALL,
        VCOORD_CENTER
    }

    public enum FinishResult
    {
        NONE = -1,
        RANK_1,
        RANK_2,
        RANK_3,
        RANK_4,
        RANK_5,
        RANK_6,
        RANK_7,
        RANK_8,
        RANK_9,
        RANK_10,
        RANK_11,
        RANK_12,
        RANK_13,
        RANK_14,
        RANK_15,
        RANK_16,
        RANK_17,
        RANK_18,
        RANK_19,
        RANK_20,
        RANK_21,
        RANK_22,
        RANK_23,
        RANK_24,
        RANK_25,
        RANK_26,
        RANK_27,
        RANK_28,
        RANK_29,
        RANK_30,
        RANK_31,
        RANK_32,
        WIN,
        LOSE,
        DSQ,
        GOLD,
        SILVER,
        BRONZE,
        COMPLETE,
    }

    public enum RewardPresentType
    {
        [Description("By Placement Order (1st/2nd/3rd)")]
        ORDER,

        [Description("Randomly regardless of Placement")]
        RANDOM,
    }

    public enum RewardEntryPresentType
    {
        [Description("Finishing Event")]
        FINISH,

        [Description("All (?)")]
        ALL,

        [Description("Completing One Lap")]
        LAP,
    }


    public enum DriftModeType
    {
        NONE,
        FREELAP,
        FREESECTION,
        ONELAP,
        SECTION,
        USER_V,
    }

    public enum CourseGeneratorKind
    {
        GENERATOR_CIRCUIT = 0,
        GENERATOR_RALLY = 1
    }

    public enum CourseGeneratorLengthType
    {
        LENGTH = 0,
        EVAL_TIME = 1
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

    public enum EnemySortType : sbyte
    {
        [Description("No sorting")]
        NONE,

        [Description("Sort selected race entries by Ascending PP")]
        PP_ASCEND,

        [Description("Sort selected race entries by Descending PP")]
        PP_DESCEND,
    }

    public enum EnemyListType : byte
    {
        SAME,
        MIX,
        ONLY_PREMIUM,
        ONLY_STANDARD,
    }

    public enum AchieveType
    {
        NONE,
        STOP,
        GOAL_V,
        TIME,
        ORDER,
        PYLON,
        SLIP_ANGLE,
        MORE_SPEED,
        MAX_GFORCE,
        OVERTAKE_NUM,
    }

    public enum ReplayRecordingQuality : sbyte
    {
        EXTRA_HIGH = 0,
        HIGH = 1,
        LOW = 2,
        EXTRA_LOW = 3,
        FULL = 4
    }

    public enum RegistrationType : sbyte
    {
        NORMAL,
        ENTRY,
        RANKING,
        HIDDEN
    }
}
