using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables;

public class GenericItems : TableMetadata
{
    public GenericItems(SpecDBFolder folderType)
    {
        Columns.Add(new ColumnMetadata("FT_TableID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("FrontTireLabel", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("RT_TableID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("RearTireLabel", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("CarCodeTblIndex", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("Generic_TableID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ItemCategory", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ItemType", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("EngineRpmOffset", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("EffectiveTime", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID_Number", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("Strings", DBColumnType.String, "UnistrDB.sdb"));
        Columns.Add(new ColumnMetadata("WeightPercentage", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("TorquePercentage", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("CompRatioPercentage", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("torquemodifier", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("torquemodifier2", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("torquemodifier3", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("Capacity", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("TorqueVolume", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("TorqueVolumeMin", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("TorqueVolumeMax", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("WeightChange", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("cIMINR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("cIMAXR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("cIDFR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("cgheightchange", DBColumnType.Byte));
    }
}
