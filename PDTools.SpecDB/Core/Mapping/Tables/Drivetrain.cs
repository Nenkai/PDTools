using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables
{
    public class Drivetrain : TableMetadata
    {
        public override string LabelPrefix { get; } = "dt_";

        public Drivetrain(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("iflywheel", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("enginebrake", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("iwheelF", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("iwheelR", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("ipropF", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("ipropR", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("Price", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("drivetypeflag", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("drivetype", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("type4WD", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("type4WDMIN", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("type4WDMAX", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("type4WDDF", DBColumnType.Byte));

        }
    }
}
