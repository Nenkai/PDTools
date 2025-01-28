using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables;

public class BrakeController : TableMetadata
{
    public override string LabelPrefix { get; } = "bc_";
    public BrakeController(SpecDBFolder folderType)
    {
        Columns.Add(new ColumnMetadata("Price", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("ABSLevelF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("ABSMinF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("ABSMaxF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("ABSDefaultF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("ABSLevelR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("ABSMinR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("ABSMaxR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("ABSDefaultR", DBColumnType.Byte));
    }
}
