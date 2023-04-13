using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables
{
    public class PaintColorInfo : TableMetadata
    {
        public PaintColorInfo(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("LabelEng", DBColumnType.String, "UnistrDB.sdb"));
            Columns.Add(new ColumnMetadata("LabelJpn", DBColumnType.String, "UnistrDB.sdb"));
            Columns.Add(new ColumnMetadata("ColorChip", DBColumnType.UInt));
            Columns.Add(new ColumnMetadata("ColorChip2", DBColumnType.UInt));
            Columns.Add(new ColumnMetadata("CCBinID", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Type", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("TunerID", DBColumnType.Byte));
        }
    }
}
