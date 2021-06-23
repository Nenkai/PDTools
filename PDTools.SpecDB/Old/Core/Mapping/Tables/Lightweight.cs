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
    public class Lightweight : TableMetadata
    {
        public override string LabelPrefix { get; } = "lw_";

        public Lightweight(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("weighteffect", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("Price", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("yaweffect", DBColumnType.Byte));
        }
    }
}
