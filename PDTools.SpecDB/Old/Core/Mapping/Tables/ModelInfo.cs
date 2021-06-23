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
    public class ModelInfo : TableMetadata
    {
        public ModelInfo(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("CarLabel", DBColumnType.String, "UnistrDB.sdb"));
            Columns.Add(new ColumnMetadata("ModelWidth", DBColumnType.Float));
            Columns.Add(new ColumnMetadata("ModelHeight", DBColumnType.Float));
            Columns.Add(new ColumnMetadata("ModelFront", DBColumnType.Float));
            Columns.Add(new ColumnMetadata("ModelRear", DBColumnType.Float));
            Columns.Add(new ColumnMetadata("ModelProjection", DBColumnType.Float));

            Columns.Add(new ColumnMetadata("DriverModel", DBColumnType.String, "UnistrDB.sdb"));
            Columns.Add(new ColumnMetadata("HeightModel", DBColumnType.String, "UnistrDB.sdb"));
            Columns.Add(new ColumnMetadata("LeftBeltModel", DBColumnType.String, "UnistrDB.sdb"));
            Columns.Add(new ColumnMetadata("RightBeltModel", DBColumnType.String, "UnistrDB.sdb"));
            Columns.Add(new ColumnMetadata("LeftBeltModelHigh", DBColumnType.String, "UnistrDB.sdb"));
            Columns.Add(new ColumnMetadata("RightbeltModelHigh", DBColumnType.String, "UnistrDB.sdb"));

            Columns.Add(new ColumnMetadata("FrontStrokeMargin", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("RearStrokeMargin", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("BeltTexture", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("DriverType", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Steering", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Crew", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("LeftSeatBeltType", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("RightSeatBeltType", DBColumnType.Byte));
        }
    }
}
