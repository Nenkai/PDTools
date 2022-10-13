using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.Enums.PS2
{
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
        _1, 
        _2, 
        _3, 
        _4, 
        _5, 
        _6, 
        _7, 
        _8, 
        abandon, 
        gold, 
        silver, 
        bronze, 
        red, 
        failed, 
        view 
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

    public enum WheelCategoryType
    {
        NONE,
        NORMAL,
        RACING,
        DIRT,
        SNOW
    }
}
