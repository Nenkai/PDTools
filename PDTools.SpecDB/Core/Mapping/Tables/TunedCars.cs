using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables
{
    public class TunedCars : TableMetadata
    {
        public TunedCars(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("Unk", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("CarCode", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("DefParts_Tbl_Index", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("DefParam_Tbl_Index", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));

        }
    }
}
