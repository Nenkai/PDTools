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
    public class TireSize : TableMetadata
    {
        public TireSize(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("flatness", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("unk", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("diameter", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("width", DBColumnType.Byte));
        }
    }
}
