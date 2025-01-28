using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables;

public class AirCleaner : TableMetadata
{
    public override string LabelPrefix { get; } = "ac_";

    public AirCleaner(SpecDBFolder folderType)
    {
        Columns.Add(new ColumnMetadata("torquemodifier", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("torquemodifier2", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("torquemodifier3", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
    }
}
