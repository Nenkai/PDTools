using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables
{
    public class RiderSet : TableMetadata
    {
        public RiderSet(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("Unk", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Helmet", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Suit", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Jacket", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Pants", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Gloves", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Boots", DBColumnType.Int));

        }
    }
}
