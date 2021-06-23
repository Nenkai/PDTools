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
    public class RiderEquipment : TableMetadata
    {
        public RiderEquipment(SpecDBFolder folderType, string localeName)
        {
            Columns.Add(new ColumnMetadata("Name", DBColumnType.String, localeName));
            Columns.Add(new ColumnMetadata("ModelCode", DBColumnType.String, localeName));
            Columns.Add(new ColumnMetadata("TypeID", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ManufacturerID", DBColumnType.Int));
        }
    }
}
