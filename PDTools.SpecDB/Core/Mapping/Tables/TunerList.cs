
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables;

public class TunerList : TableMetadata
{
    public TunerList(SpecDBFolder folderType)
    {
        Columns.Add(new ColumnMetadata("Maker1", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("Maker2", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("Maker3", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("Maker4", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("Maker5", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("Maker6", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("Maker7", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("Maker8", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("Maker9", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("Maker10", DBColumnType.Byte));
    }
}
