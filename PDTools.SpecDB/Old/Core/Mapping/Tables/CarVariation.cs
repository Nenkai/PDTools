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
    public class CarVariation : TableMetadata
    {
        public override string LabelPrefix { get; } = "";

        public CarVariation(SpecDBFolder folderType, string localeName)
        {
            Columns.Add(new ColumnMetadata("VariationID", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Var_Tbl_Index", DBColumnType.Int));
        }
    }
}
