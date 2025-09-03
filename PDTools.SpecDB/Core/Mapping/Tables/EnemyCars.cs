using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables;

public class EnemyCars : TableMetadata
{
    public EnemyCars(SpecDBFolder folderType)
    {
        Columns.Add(new ColumnMetadata("GenericCar", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("DefaultParts", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("DefaultParam", DBColumnType.Key));
    }
}
