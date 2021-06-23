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
    public class Chassis : TableMetadata
    {
        public override string LabelPrefix { get; } = "ch_";

        public Chassis(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("length", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("height", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("wheelbase", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("mass", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("dlength", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("dheight", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("dmass", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("performanceF", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("performanceR", DBColumnType.Short));

            if (folderType >= SpecDBFolder.GT5_JP3009 || folderType <= SpecDBFolder.GT4_PREMIUM_JP2560)
            {
                Columns.Add(new ColumnMetadata("treadF", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("treadR", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("width", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("dwidth", DBColumnType.Short));
                if (folderType >= SpecDBFolder.GT5_JP3009)
                {
                    Columns.Add(new ColumnMetadata("original_rideHeight_F", DBColumnType.Short));
                    Columns.Add(new ColumnMetadata("original_rideHeight_R", DBColumnType.Short));
                
                    Columns.Add(new ColumnMetadata("electricMotor", DBColumnType.Byte));
                    Columns.Add(new ColumnMetadata("unk", DBColumnType.Byte));
                }
            }

            Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("cartype", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("percentageF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("yaw", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("cheight", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("WheelLayout", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("GrowInch", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("TireWidthR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("FrontStiffness", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("RearStiffness", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("GasCapacity", DBColumnType.Byte));

            if (folderType >= SpecDBFolder.GT5_JP3009)
            {
                Columns.Add(new ColumnMetadata("EngineMount", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("cd", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("clMINF", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("clMAXF", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("clDFF", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("clMINR", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("clMAXR", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("clDFR", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("wheeloffsetF", DBColumnType.SByte));
                Columns.Add(new ColumnMetadata("wheeloffsetR", DBColumnType.SByte));
                Columns.Add(new ColumnMetadata("susarmF", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("susarmR", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("ActiveWingType", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("ActiveWingVelocity1", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("ActiveWingVelocity2", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("ActiveWingMovingSpeed", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("ActiveWingDownForce", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("rollCenterPolicy", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("downForcePointOfLoadF", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("downForcePointOfLoadR", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("activeWingVelocity3", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("activeWingDownForce2", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("originalClF", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("originalClR", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("batteryCapacity", DBColumnType.Byte));
            }

        }
    }
}
