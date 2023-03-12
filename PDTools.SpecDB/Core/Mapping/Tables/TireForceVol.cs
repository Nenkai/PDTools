using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables
{
    public class TireForceVol : TableMetadata
    {
        public TireForceVol(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("forceVolTA", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolGU", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolGR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolSA", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolGV", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolDT", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolWT", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolSF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolWD", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolHT", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolWR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolWH", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolWG", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolG1", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolG2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolG3", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolPB", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("forceVolBE", DBColumnType.Byte));
        }
    }
}
