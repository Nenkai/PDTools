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
    public class NOS : TableMetadata
    {
        public override string LabelPrefix { get; } = "no_";

        public NOS(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("Unk", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Capacity", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Price", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("TorqueVolume", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("TorqueVolumeMin", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("TorqueVolumeMax", DBColumnType.Byte));
        }
    }
}
