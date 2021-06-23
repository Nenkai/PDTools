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
    public class Variation : TableMetadata
    {
        public Variation(SpecDBFolder folderType, string locale)
        {
            if (folderType <= SpecDBFolder.GT5_TRIAL_JP2704)
            {
                Columns.Add(new ColumnMetadata("ModelCode", DBColumnType.String, locale));
                Columns.Add(new ColumnMetadata("VarOrder", DBColumnType.UInt));
                Columns.Add(new ColumnMetadata("?", DBColumnType.String, locale));
                Columns.Add(new ColumnMetadata("Name", DBColumnType.String, locale));

                Columns.Add(new ColumnMetadata("?", DBColumnType.Float));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Float));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Float));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Float));
                Columns.Add(new ColumnMetadata("?", DBColumnType.Float));
                Columns.Add(new ColumnMetadata("RGB", DBColumnType.UInt));
                Columns.Add(new ColumnMetadata("RGB2", DBColumnType.UInt));
                Columns.Add(new ColumnMetadata("RGB3", DBColumnType.UInt));
                Columns.Add(new ColumnMetadata("RGB4", DBColumnType.UInt));
                return;
            }

            Columns.Add(new ColumnMetadata("ModelCode", DBColumnType.String, "UnistrDB.sdb"));
            Columns.Add(new ColumnMetadata("VarOrder", DBColumnType.UInt));
            Columns.Add(new ColumnMetadata("NameJpn", DBColumnType.String, "UnistrDB.sdb"));
            Columns.Add(new ColumnMetadata("NameEng", DBColumnType.String, "UnistrDB.sdb"));
            Columns.Add(new ColumnMetadata("Flag", DBColumnType.UInt));
            Columns.Add(new ColumnMetadata("ColorChip0", DBColumnType.UInt));
            Columns.Add(new ColumnMetadata("ColorChip1", DBColumnType.UInt));
            Columns.Add(new ColumnMetadata("ColorChip2", DBColumnType.UInt));
            Columns.Add(new ColumnMetadata("ColorChip3", DBColumnType.UInt));

            if (folderType >= SpecDBFolder.GT5_JP3009)
            {
                Columns.Add(new ColumnMetadata("CarColorID", DBColumnType.UInt));
                Columns.Add(new ColumnMetadata("AllPaintID", DBColumnType.UInt));
            }
        }
    }
}
