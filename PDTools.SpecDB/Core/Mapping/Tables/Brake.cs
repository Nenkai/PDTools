using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables
{
    public class Brake : TableMetadata
    {
        public override string LabelPrefix { get; } = "br_";
        public Brake(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("Price", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("Category", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("BraketorqueF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("BraketorqueR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Sidebraketorque", DBColumnType.Byte));

            if (folderType >= SpecDBFolder.GT5_ACADEMY_09_2900 &&
                folderType != SpecDBFolder.GT5_JP2904 && folderType != SpecDBFolder.GT5_PREVIEWJP2904) // Kiosk exception; doesn't have it
                Columns.Add(new ColumnMetadata("tireMuForBrake", DBColumnType.Byte));
        }
    }
}
