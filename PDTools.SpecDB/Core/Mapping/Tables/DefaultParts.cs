using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables;

public class DefaultParts : TableMetadata
{
    public override string LabelPrefix { get; } = "df_pt_";

    public DefaultParts(SpecDBFolder folderType)
    {
        /*
        if (folderType < SpecDBFolder.GT5_JP3009)
        {
            Columns.Add(new ColumnMetadata("UnkID", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Category", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("BraketorqueF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("BraketorqueR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Sidebraketorque", DBColumnType.Byte));
        }
        else
        {*/
        Columns.Add(new ColumnMetadata("Brake", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("BrakeCtrl", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Suspension", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("ASCC", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("TCSC", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Chassis", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("RacingModify", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Lightweight", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Steer", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("DriveTrain", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Gear", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Engine", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("NATune", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Turbo", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Displacement", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Computer", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Intercooler", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Muffler", DBColumnType.Key));

        Columns.Add(new ColumnMetadata("Clutch", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("Flywheel", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("PropellerShaft", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("LSD", DBColumnType.Key));

        Columns.Add(new ColumnMetadata("FrontTire", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("RearTire", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("F_Tire_G", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("R_Tire_G", DBColumnType.Key));

        if (folderType >= SpecDBFolder.GT5_JP3009)
        {
            Columns.Add(new ColumnMetadata("NOS", DBColumnType.Key));

            Columns.Add(new ColumnMetadata("Wing", DBColumnType.Key));
            Columns.Add(new ColumnMetadata("Aero", DBColumnType.Key));
            Columns.Add(new ColumnMetadata("FlatFloor", DBColumnType.Key));
            Columns.Add(new ColumnMetadata("Freedom", DBColumnType.Key));
            Columns.Add(new ColumnMetadata("LWeightWindow", DBColumnType.Key));
            Columns.Add(new ColumnMetadata("Bonnet", DBColumnType.Key));
            Columns.Add(new ColumnMetadata("IntakeManifold", DBColumnType.Key));
            Columns.Add(new ColumnMetadata("ExhaustManifold", DBColumnType.Key));
            Columns.Add(new ColumnMetadata("Catalyst", DBColumnType.Key));
            Columns.Add(new ColumnMetadata("AirCleaner", DBColumnType.Key));
            Columns.Add(new ColumnMetadata("BoostControl", DBColumnType.Key));
            Columns.Add(new ColumnMetadata("IndepThrottle", DBColumnType.Key));
            Columns.Add(new ColumnMetadata("Supercharger", DBColumnType.Key));

            Columns.Add(new ColumnMetadata("?", DBColumnType.Short));
        }
    }
}
