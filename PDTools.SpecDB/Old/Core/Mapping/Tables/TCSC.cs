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
    public class TCSC : TableMetadata
    {
        public override string LabelPrefix { get; } = "tcsc_";

        public TCSC(SpecDBFolder folderType)
        {
            if (folderType <= SpecDBFolder.TT_EU2630)
            {
                Columns.Add(new ColumnMetadata("Price", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("unk", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TCSUnk1", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TCSUnk2", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TCSUnk3", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TCSUnkValue1", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TCSUnkValue2", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TCSUnkValue3", DBColumnType.Byte));
            }
            else
            {
                Columns.Add(new ColumnMetadata("Price", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TCSparamA", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TCSparamB", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TCSgrad", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TCStarget", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TCSUserValueDF", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TCSUserValueLevel", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TCSUserValueMin", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("TCSUserValueMax", DBColumnType.Byte));
            }

        }
    }
}
