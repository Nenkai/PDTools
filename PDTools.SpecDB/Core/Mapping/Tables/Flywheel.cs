using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables
{
    public class Flywheel : TableMetadata
    {
        public override string LabelPrefix { get; } = "fw_";

        public Flywheel(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("Price", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("iflywheel", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("enginebrake", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("iwheelF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("iwheelR", DBColumnType.Byte));
        }
    }
}
