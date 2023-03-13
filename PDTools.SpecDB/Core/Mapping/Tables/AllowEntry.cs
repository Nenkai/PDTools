using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables
{
    public class AllowEntry : TableMetadata
    {
        public override string LabelPrefix { get; } = "reg_";

        public AllowEntry(SpecDBFolder folderType)
        {
            // is actually uint + table index, but to make it simpler we make it a long instead
            Columns.Add(new ColumnMetadata("Entry0", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry1", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry2", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry3", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry4", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry5", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry6", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry7", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry8", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry9", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry10", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry11", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry12", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry13", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry14", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry15", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry16", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry17", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry18", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry19", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry20", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry21", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry22", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry23", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry24", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry25", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry26", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry27", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry28", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry29", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry30", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry31", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry32", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry33", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry34", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry35", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry36", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry37", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry38", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry39", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry40", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry41", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry42", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry43", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry44", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry45", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry46", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry47", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry48", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry49", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry50", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry51", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry52", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry53", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry54", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry55", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry56", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry57", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry58", DBColumnType.Long));
            Columns.Add(new ColumnMetadata("Entry59", DBColumnType.Long));
        }
    }
}
