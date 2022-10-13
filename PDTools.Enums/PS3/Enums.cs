using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Enums.PS3
{
    public enum AutomobileControllerType
    {
        UNKNOWN,
        AI,
        PAD_BUTTON,
        PAD_STICK,
        WHEEL200,
        WHEEL900S,
        WHEEL900G,
        WHEEL900GC,
        PAD_MIN,
        PAD_MAX,
        WHEEL_MIN,
        WHEEL_MAX,
    }

    public enum CarPartsType
    {
        BRAKE = 2,
        BRAKE_CONTROLLER,
        SUSPENSION,
        ASCC,
        TCSC,
        RACING_MODIFY,
        CHASSIS,
        LIGHT_WEIGHT,
        STEER,
        DRIVETRAIN,
        GEAR,
        ENGINE,
        NATUNE,
        TURBINEKIT,
        DISPLACEMENT,
        COMPUTER,
        INTERCOOLER,
        MUFFLER,
        CLUTCH,
        FLYWHEEL,
        PROPELLERSHAFT,
        LSD,
        FRONT_TIRE,
        REAR_TIRE,
        NOS,
        SUPERCHARGER,
        INTAKE_MANIFOLD,
        EXHAUST_MANIFOLD,
        CATALYST,
        AIR_CLEANER,
        BOOST_CONTROLLER,
        INDEP_THROTTLE,
        LIGHT_WEIGHT_WINDOW,
        BONNET,
        AERO,
        FLAT_FLOOR,
        FREEDOM,
        WING,
        STIFFNESS,
        SPECIAL_GAS
    }

    public enum PARTS_BRAKE
    {
        NORMAL,
        _4PISTON,
        _6PISTON,
        _8PISTON,
        CARBON,
    }

    public enum PARTS_BRAKE_CONTROLLER
    {
        NO,
        ONE,
    }

    public enum PARTS_SUSPENSION
    {
        NORMAL,
        SPORTS1,
        SPORTS2,
        SPORTS3,
        RACING,
        FULL_ACTIVE,
    }

    public enum PARTS_ASCC
    {
        NO,
        ONE,
    }

    public enum PARTS_TCSC
    {
        NO,
        ONE,
    }

    public enum PARTS_LIGHT_WEIGHT
    {
        NONE = -1,
        STAGE1 = 1,
        STAGE2,
        STAGE3,
        STAGE4,
        STAGE5,
        STAGE6,
        STAGE7,
        STAGE8,
    }

    public enum PARTS_DRIVETRAIN
    {
        NORMAL,
        VARIABLE_CENTER_DIFF,
        ACTIVE_CENTER_DIFF,
    }

    public enum PARTS_GEAR
    {
        NORMAL,
        CLOSE,
        SUPER_CLOSE,
        VARIABLE,
    }

    public enum PARTS_NATUNE
    {
        NONE = -1,
        LEVEL1 = 1,
        LEVEL2,
        LEVEL3,
        LEVEL4,
        LEVEL5,
    }

    public enum PARTS_TURBINEKIT
    {
        NONE = -1,
        NO,
        LEVEL1,
        LEVEL2,
        LEVEL3,
        LEVEL4,
        LEVEL5,
    }

    public enum PARTS_DISPLACEMENT
    {
        NONE = -1,
        LEVEL1 = 1,
        LEVEL2,
        LEVEL3,
    }

    public enum PARTS_COMPUTER
    {
        NONE = -1,
        LEVEL1 = 1,
        LEVEL2,
    }

    public enum PARTS_INTERCOOLER
    {
        NONE = -1,
        S,
        M,
        L,
        LL,
    }

    public enum PARTS_MUFFLER
    {
        NONE,
        SPORTS,
        SEMI_RACING,
        RACING,
    }

    public enum PARTS_CLUTCH
    {
        NONE = -1,
        NORMAL,
        HIGH_CAPACITY,
        TWIN,
        TRIPLE,
    }

    public enum PARTS_FLYWHEEL
    {
        NONE = -1,
        LIGHT = 1,
        Cr_Mo,
        LIGHT_Cr_Mo,
    }

    public enum PARTS_PROPELLERSHAFT
    {
        NONE = -1,
        ONE = 1,
    }

    public enum PARTS_LSD
    {
        NORMAL,
        VARIABLE,
        AYCC,
    }

    public enum PARTS_TIRE
    {
        COMFORT_HARD,
        COMFORT_MEDIUM,
        COMFORT_SOFT,
        SPORTS_HARD,
        SPORTS_MEDIUM,
        SPORTS_SOFT,
        SPORTS_SUPER_SOFT,
        RACING_HARD,
        RACING_MEDIUM,
        RACING_SOFT,
        RACING_SUPER_SOFT,
        RAIN_INTERMEDIATE,
        RAIN_HEAVY_WET,
        DIRT,
        SNOW,
        TIRE_DRY_MIN = 0,
        TIRE_DRY_MAX = 11,
        TIRE_TARMAC_MIN = 0,
        TIRE_TARMAC_MAX = 13,
    }

    public enum PARTS_NOS
    {
        NONE = -1,
        ONE = 1,
    }

    public enum PARTS_SUPERCHARGER
    {
        NONE = -1,
        ONE = 1,
    }

    public enum PARTS_INTAKE_MANIFOLD
    {
        NONE = -1,
        ONE = 1,
    }

    public enum PARTS_EXHAUST_MANIFOLD
    {
        NONE = -1,
        ONE = 1,
    }

    public enum PARTS_CATALYST
    {
        NONE = -1,
        SPORTS = 1,
        RACING = 2
    }

    public enum PARTS_AIR_CLEANER
    {
        NONE = -1,
        SPORTS = 1,
        RACING = 2
    }

    public enum PARTS_BOOST_CONTROLLER
    {
        ONE = 1,
    }

    public enum PARTS_INDEP_THROTTLE
    {
        ONE = 1,
    }

    public enum PARTS_LIGHT_WEIGHT_WINDOW
    {
        NONE,
        ONE,
    }

    public enum PARTS_BONNET
    {
        NONE,
        CARBON,
        PAINT_CARBON,
    }

    public enum PARTS_AERO
    {
        A = 1,
        B = 2,
        C = 3
    }

    public enum PARTS_FLAT_FLOOR
    {
        A = 1,
    }

    public enum PARTS_FREEDOM
    {
        F1 = 1,
        F2 = 2,
        F3 = 3
    }

    public enum PARTS_WING
    {
        WINGLESS = 1,
        CUSTOM,
        W1,
        W2,
        W3,
    }

    public enum PARTS_STIFFNESS
    {
        ONE = 1,
    }
}
