using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables
{
    public class Race : TableMetadata
    {
        public Race(SpecDBFolder folderType)
        {
            if (folderType <= SpecDBFolder.GT5_TRIAL_JP2704)
            {
                Columns.Add(new ColumnMetadata("CourseID", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Crs_Tbl_Index", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("AllowEntry", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("AlEnt_Tbl_Index", DBColumnType.Int));

                // 0x10
                Columns.Add(new ColumnMetadata("?", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("PlayerVCoord", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Minutes", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("Gold_MS", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("Silv_MS", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("Bron_MS", DBColumnType.Short));

                // 20 to 2a shorts -> prize
                Columns.Add(new ColumnMetadata("Prize1st", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("P2nd", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("P3rd", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("P4th", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("P5th", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("P6th", DBColumnType.Short));

                Columns.Add(new ColumnMetadata("ChampPrize", DBColumnType.Short));

                // 2e to 36 shorts -> launch pos
                Columns.Add(new ColumnMetadata("StartPos1st", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("S2nd", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("S3rd", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("S4th", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("S5th", DBColumnType.Short));

                // 0x38
                Columns.Add(new ColumnMetadata("YearMin", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("YearMax", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("MaxEntries", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));

                // 0x40
                Columns.Add(new ColumnMetadata("StartType", DBColumnType.Byte));

                Columns.Add(new ColumnMetadata("unkdrag", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("PlBoost", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Laps", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("FailCond", DBColumnType.Byte));

                Columns.Add(new ColumnMetadata("License", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Drivetrain", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Aspiration", DBColumnType.Byte));

                Columns.Add(new ColumnMetadata("CarType", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Tyres", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Boost1st", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("B2", DBColumnType.Byte));

                Columns.Add(new ColumnMetadata("B3", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("B4", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("B5", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("B6", DBColumnType.Byte)); // 0x50
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));

                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));

                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Skill1", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Sk2", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Sk3", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Sk4", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Sk5", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Sk6", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Sk7", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Sk8", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Boost1", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Boost2", DBColumnType.Byte)); // 0x60
                Columns.Add(new ColumnMetadata("Boost3", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Boost4", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Boost5", DBColumnType.Byte));

                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("BoostMaybe", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("BoostMaybe", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("BoostMaybe", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));

                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TiresCold", DBColumnType.Bool));
                Columns.Add(new ColumnMetadata("TireUnk1", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TireUnk2", DBColumnType.Byte)); // 0x70
                Columns.Add(new ColumnMetadata("TireUnk3", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TireUnk4", DBColumnType.Byte));

                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("PylonNum", DBColumnType.Byte));

                Columns.Add(new ColumnMetadata("GoldMin", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("GoldSec", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("SilvMin", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("SilvSec", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("BronMin", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("BronSec", DBColumnType.Byte));

                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("LaunchKph1", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Kph2", DBColumnType.Byte)); // 0x80
                Columns.Add(new ColumnMetadata("Kph3", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Kph4", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Kph5", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Kph6", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("PenaltySec", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("PenaltyFlags", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("PenaltyFlags2", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("Delay1st", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("D2nd", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("D3rd", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("D4th", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("D5th", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("D6th", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("PowerMax", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("PriceMin", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("WeightMax", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("LengthMax", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("CountryReq", DBColumnType.Byte));
            }
            else
            {
                Columns.Add(new ColumnMetadata("CourseLabel", DBColumnType.String, "UnistrDB.sdb"));
                Columns.Add(new ColumnMetadata("AllowEntry", DBColumnType.String, "UnistrDB.sdb"));
                Columns.Add(new ColumnMetadata("goldfrac", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("silverfrac", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("bronzefrac", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Prize1", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Prize2", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Prize3", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Prize4", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Prize5", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Prize6", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Prize7", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Prize8", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Prize9", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Prize10", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Prize11", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Prize12", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Prize13", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Prize14", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Prize15", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("Prize16", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("prizeGC", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("LaunchPosition1", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("LaunchPosition2", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("LaunchPosition3", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("LaunchPosition4", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("LaunchPosition5", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("LaunchPosition6", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("LaunchPosition7", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("LaunchPosition8", DBColumnType.Int));
            }
        }
    }
}
