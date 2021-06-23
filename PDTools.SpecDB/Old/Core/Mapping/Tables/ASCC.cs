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
    public class ASCC : TableMetadata
    {
        public override string LabelPrefix { get; } = "";
        public ASCC(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("Price", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VSCbrakeoff", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VSCparam1level", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VSCparam1MIN", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VSCparam1MAX", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VSCparam1DF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VSCparam2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VSCbrakeon", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VSCparamBlevel", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VSCparamBMIN", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VSCparamBMAX", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VSCparamBDF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VUCbrakeoff", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VUCparam1level", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VUCparam1MIN", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VUCparam1MAX", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VUCparam1DF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VUCparam2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VUCbrakeon", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VUCparamBlevel", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VUCparamBMIN", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VUCparamBMAX", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VUCparamBDF", DBColumnType.Byte));
        }
    }
}
