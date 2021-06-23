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
    public class PortPolish : TableMetadata
    {
        public override string LabelPrefix { get; } = "pp_";

        public PortPolish(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("torquemodifier", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torquemodifier2", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("Price", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
        }
    }
}
