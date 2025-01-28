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
        Columns.Add(new ColumnMetadata("GenericCar", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("Gen_Tbl_Index", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("DefaultParts", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("DefPrs_Tbl_Index", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("DefaultParam", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("DefPrm_Tbl_Index", DBColumnType.Int));
    }
}
