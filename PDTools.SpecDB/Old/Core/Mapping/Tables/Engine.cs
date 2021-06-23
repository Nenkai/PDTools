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
    public class Engine : TableMetadata
    {
        public override string LabelPrefix { get; } = "en_";

        public Engine(SpecDBFolder folderType)
        {
            // soundnum for newer games goes here since they ran out of space on the short one (see below)
            if (folderType >= SpecDBFolder.GT5_TRIAL_EU2704)
                Columns.Add(new ColumnMetadata("soundNum", DBColumnType.Int));

            Columns.Add(new ColumnMetadata("discplacement", DBColumnType.String, "UnistrDB.sdb"));
            Columns.Add(new ColumnMetadata("enginetype", DBColumnType.String, "UnistrDB.sdb"));
            Columns.Add(new ColumnMetadata("cam", DBColumnType.String, "UnistrDB.sdb"));
            Columns.Add(new ColumnMetadata("aspiration", DBColumnType.String, "UnistrDB.sdb"));
            Columns.Add(new ColumnMetadata("psrpm", DBColumnType.String, "UnistrDB.sdb"));
            Columns.Add(new ColumnMetadata("torquerpm", DBColumnType.String, "UnistrDB.sdb"));

            if (folderType < SpecDBFolder.GT5_JP3009 // Kiosk demo, 5 prologue has it twice..
                && (folderType < SpecDBFolder.GT5_TRIAL_EU2704 || folderType > SpecDBFolder.GT5_TRIAL_JP2704)) // Except gthd which already had the int but not the short what the fuck PD?
                Columns.Add(new ColumnMetadata("soundNum", DBColumnType.UShort)); // GT4 and older has it as short

            Columns.Add(new ColumnMetadata("psvalue", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torquevalue", DBColumnType.Short));

            if (folderType >= SpecDBFolder.GT5_PROLOGUE2813)
                Columns.Add(new ColumnMetadata("idlerpm", DBColumnType.Short));

            Columns.Add(new ColumnMetadata("torqueA", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueB", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueC", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueD", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueE", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueF", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueG", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueH", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueI", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueJ", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueK", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueL", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueM", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueN", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueO", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueP", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueQ", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueR", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueS", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueT", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueU", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueV", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueW", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torqueX", DBColumnType.Short));

            Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("dpsflag", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("shiftlimit", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("revlimit", DBColumnType.Byte));

            if (folderType < SpecDBFolder.GT5_PROLOGUE2813)
                Columns.Add(new ColumnMetadata("Unk", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("clutchmeetrpm", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("torquepoint", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmA", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmB", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmC", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmD", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmE", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmG", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmH", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmI", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmJ", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmK", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmL", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmM", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmN", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmO", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmP", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmQ", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmS", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmT", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmU", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmV", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmW", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rpmX", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("RedLine", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("MeterScale", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("torquevol", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("GasConsumptionRate", DBColumnType.Byte));
        }
    }
}
