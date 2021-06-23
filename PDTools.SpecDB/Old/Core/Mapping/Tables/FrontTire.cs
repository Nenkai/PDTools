using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

using SpecDBOld.Core;
namespace SpecDBOld.Mapping.Tables
{
    public class FrontTire : TableMetadata
    {
        public override string LabelPrefix { get; } = "ft_";

        public FrontTire(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("TS_Tbl_Index", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("tiresize", DBColumnType.Int));

            Columns.Add(new ColumnMetadata("Cmp_Tbl_Index", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("tirecompound0", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Cmp_Tbl_Index", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("tirecompound1", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Cmp_Tbl_Index", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("tirecompound2", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("TF_Tbl_Index", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("tireforcevol0", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("TF_Tbl_Index", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("tireforcevol1", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("TF_Tbl_Index", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("tireforcevol2", DBColumnType.Int));

            Columns.Add(new ColumnMetadata("Price", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));

            if (folderType >= SpecDBFolder.GT5_JP3009)
            {
                Columns.Add(new ColumnMetadata("tireDrainageLevel", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("tireSpring_Auto", DBColumnType.Byte));
            }
        }
    }
}
