
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
    public class TunerList : TableMetadata
    {
        public TunerList(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("Unk", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Unk2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Unk3", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Unk4", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Unk5", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Unk6", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Unk7", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Unk8", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Unk9", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Unk10", DBColumnType.Byte));
        }
    }
}
