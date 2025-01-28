using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables;

public class Gear : TableMetadata
{
    public override string LabelPrefix { get; } = "ge_";

    public Gear(SpecDBFolder folderType)
    {
        Columns.Add(new ColumnMetadata("Gear1st", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("Gear2nd", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("Gear3rd", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("Gear4th", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("Gear5th", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("Gear6th", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("Gear7th", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("Gear8th", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("Gear9th", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("Gear10th", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("Gear11th", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("GearReverse", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("finalgearMIN", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("finalgearMAX", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("finalgearDF", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("ExtraFinalGearRatio", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("Price", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("geartype", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("Nshift", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("gearflag", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("maxspeedMIN", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("maxspeedMAX", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("maxspeedDF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("ExtraFinalGearUsage", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LowGearPos", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("ReverseGearPos", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("GearPattern", DBColumnType.Byte));
    }
}
