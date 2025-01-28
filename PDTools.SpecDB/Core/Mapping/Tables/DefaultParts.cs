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
        Columns.Add(new ColumnMetadata("Brake", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("BrakeCtrl", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("Susp", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ASCC", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("TCSC", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("Chassis", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("R_Modify", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("LWeight", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("Steer", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("DrvTrain", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("Gear", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("Engine", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("NA", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("Turbo", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("Displ", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("Computer", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("Intercooler", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("Muffler", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));

        Columns.Add(new ColumnMetadata("Clutch", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("Flywheel", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("Propel", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("LSD", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));

        Columns.Add(new ColumnMetadata("F_Tire", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("R_Tire", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("F_Tire_G", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("R_Tire_G", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));

        if (folderType >= SpecDBFolder.GT5_JP3009)
        {
            Columns.Add(new ColumnMetadata("NOS", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));

            Columns.Add(new ColumnMetadata("Wing", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Aero", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("FlatFloor", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Freedom", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("LWeight Window", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Bonnet", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("IntakeManifold", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ExhaustManifold", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Catalyst", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("AirCleaner", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("BoostControl", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("IndepThrottle", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Supercharger", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("ID", DBColumnType.Int));

            Columns.Add(new ColumnMetadata("?", DBColumnType.Short));

        }
    }
}
