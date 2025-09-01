using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables;

public class AllowEntry : TableMetadata
{
    public override string LabelPrefix { get; } = "reg_";

    public AllowEntry(SpecDBFolder folderType)
    {
        // is actually uint + table index, but to make it simpler we make it a long instead
        Columns.Add(new ColumnMetadata("Entry0", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry1", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry2", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry3", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry4", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry5", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry6", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry7", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry8", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry9", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry10", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry11", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry12", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry13", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry14", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry15", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry16", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry17", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry18", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry19", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry20", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry21", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry22", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry23", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry24", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry25", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry26", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry27", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry28", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry29", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry30", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry31", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry32", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry33", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry34", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry35", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry36", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry37", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry38", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry39", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry40", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry41", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry42", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry43", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry44", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry45", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry46", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry47", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry48", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry49", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry50", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry51", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry52", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry53", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry54", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry55", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry56", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry57", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry58", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Entry59", DBColumnType.Key));
    }
}
