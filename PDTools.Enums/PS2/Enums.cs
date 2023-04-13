using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace PDTools.Enums.PS2
{
    public enum PartsTypeGT4
    {
        GENERIC_CAR,
        BRAKE,
        BRAKECONTROLLER,
        SUSPENSION,
        ASCC,
        TCSC,
        CHASSIS,
        RACINGMODIFY,
        LIGHTWEIGHT,
        STEER,
        DRIVETRAIN,
        GEAR,
        ENGINE,
        NATUNE,
        TURBINEKIT,
        PORTPOLISH,
        ENGINEBALANCE,
        DISPLACEMENT,
        COMPUTER,
        INTERCOOLER,
        MUFFLER,
        CLUTCH,
        FLYWHEEL,
        PROPELLERSHAFT,
        LSD,
        FRONTTIRE,
        REARTIRE,
        NOS,
        SUPERCHARGER,
        WHEEL,
        WING,
        TIRESIZE,
        TIRECOMPOUND,
        TIREFORCEVOL,
        COURSE,
        RACE,
        DEFAULT_PARTS,
        DEFAULT_PARAM,
        ENEMY_CARS,
        CAR_NAME,
        COURSE_NAME,
        CAR_VARIATION,
        VARIATION,
    }

    public enum Locale
    {
        JP,
        US,
        GB,
        FR,
        DE,
        IT,
        ES,
        PT,
        KR,
        CN,
        TW, 
        DB
    }

    public enum ViewType
    {
        SUBJECTIVE, 
        BONNET, 
        OVERLOOK
    }

    public enum ReplayMode
    {
        NORMAL, 
        DIVE
    }

    public enum MachineRole
    {
        normal, 
        scoreboard, 
        submonitor
    }

    public enum PhotoAspect
    {
        _4x3, 
        _16x9,
        _3x2, 
        _1x1
    }

    public enum ShutterType
    {
        TYPE_A, TYPE_B, TYPE_C, TYPE_D, TYPE_E, TYPE_F, TYPE_G, TYPE_H, TYPE_I, TYPE_J,
        TYPE_K, TYPE_L, TYPE_M, TYPE_N, TYPE_O, TYPE_P, TYPE_Q, TYPE_R, TYPE_S
    }

    public enum PhotoQuality
    {
        STANDARD,
        FINE,
        SUPER_FINE
    }

    public enum PenaltyType
    {
        NONE, 
        SPEED_LIMITATION, 
        PASSING_PITLANE
    }

    public enum ProfessionalControlType
    {
        control_NORMAL,
        PRO,
        SEMIPRO
    }

    public enum DTVType
    {
        normal, 
        _480p, _1080i, 
        _480i_frame
    }

    public enum SoundType
    {
        MONOPHONIC, 
        STEREO, 
        PROLOGIC2
    }

    public enum Difficulty
    {
        EASY, 
        NORMAL, 
        HARD, 
        SUPER
    }

    public enum AutomaticGear
    {
        MT,
        AT
    }

    public enum UnitTorqueType 
    { 
        KGFM, 
        FTLB, 
        NM 
    }

    public enum UnitVelocityType
    {
        KMPH, 
        MPH
    }

    public enum UnitPowerType
    {
        PS, 
        HP, 
        BHP, 
        CH, 
        KW, 
        CV, 
        PF
    }

    public enum RunCourseMode
    {
        NONE,
        FREERUN,
        EVENT,
        PRACTICE,
        PHOTO,
        TRAVEL,
        MACHINE_TEST,
        TIME_TRIAL
    }

    public enum GetCarReason
    {
        NONE,
        BUY_NEW,
        BUY_USED,
        BUY_TRADE,
        GET_PRESENT
    }

    public enum Result
    {
        /// <summary>
        /// Events and Missions
        /// </summary>
        [Description("1st (Events/Missions)")]
        _1,

        [Description("2nd (Events/Missions)")]
        _2,

        [Description("3rd (Events/Missions)")]
        _3,

        [Description("4th (Events/Missions)")]
        _4,

        [Description("5th (Events/Missions)")]
        _5,

        [Description("6th (Events/Missions)")]
        _6,

        [Description("7th (Events/Missions)")]
        _7,

        [Description("8th (Events/Missions)")]
        _8,

        [Description("Abandon")]
        abandon,

        /// <summary>
        /// Licenses
        /// </summary>
        [Description("Gold (License)")]
        gold,

        /// <summary>
        /// Licenses
        /// </summary>
        [Description("Silver (License)")]
        silver,

        /// <summary>
        /// Licenses
        /// </summary>
        [Description("Bronze (License)")]
        bronze,

        [Description("Red")]
        red,

        [Description("Failed")]
        failed,

        [Description("View")]
        view,

        [Description("No Result")]
        none,
    }

    public enum DayEventType
    {
        NO_EVENT,
        GET_CAR,
        SELL_CAR,
        RUN_RACE,
        RUN_LICENSE,
        RUN_MISSION,
        BUY_WHEEL,
        BUY_WING,
        RUN_COURSE
    }

    public enum EventType
    {
        None,
        License,
        Event,
        Mission
    }

    public enum WheelCategoryType
    {
        NONE,
        NORMAL,
        RACING,
        DIRT,
        SNOW
    }

    public enum GameZoneType
    {
        UNDEF,
        JP,
        US,
        UK,
        EU,
        KR,
        CN
    }
}
