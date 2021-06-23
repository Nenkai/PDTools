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
    public class GenericCar : TableMetadata
    {
        // 8 = price
        // 12 = year
        // 14 = regulation displacement
        // 16 = country
        // 17 = maker
        // 18 = tuner
        // 19 = category (% 0x64)
        // 21 = conceptcartype
        // 22 = open model
        // 23 = no change wheel
        // 24 = no change wing
        // 25 = supercharger originally
        // 26 = category
        public GenericCar(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("Df_Tbl_Index", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("DefaultParts", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Price", DBColumnType.Int));

            if (folderType >= SpecDBFolder.GT5_JP3009)
            {
                Columns.Add(new ColumnMetadata("SpecifyFlags1", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("PurchaseLevel", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("HornID", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("NumColor", DBColumnType.Int));
                Columns.Add(new ColumnMetadata("MainColor", DBColumnType.Int));
            }

            Columns.Add(new ColumnMetadata("Year", DBColumnType.Short));

            if (folderType > SpecDBFolder.GT5_TRIAL_JP2704)
            {
                Columns.Add(new ColumnMetadata("PowerMax", DBColumnType.Short));
                Columns.Add(new ColumnMetadata("PowerMin", DBColumnType.Short));
            }
            else
                Columns.Add(new ColumnMetadata("RegulationDisplacementFlags", DBColumnType.Short));

            if (folderType > SpecDBFolder.GT5_TRIAL_JP2704)
                Columns.Add(new ColumnMetadata("Country", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Maker", DBColumnType.Byte));

            if (folderType > SpecDBFolder.GT5_TRIAL_JP2704)
                Columns.Add(new ColumnMetadata("Tuner", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Category", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("GeneralFlags", DBColumnType.Byte));

            if ((folderType >= SpecDBFolder.GT5_TRIAL_EU2704 && folderType <= SpecDBFolder.GT5_TRIAL_JP2704) || folderType >= SpecDBFolder.GT5_JP3009)
            {
                Columns.Add(new ColumnMetadata("GeneralFlags2", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("GeneralFlags3", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("GeneralFlags4", DBColumnType.Byte));
            }

            Columns.Add(new ColumnMetadata("ConceptCarType", DBColumnType.Byte));

            if (folderType >= SpecDBFolder.GT5_PROLOGUE2813 && folderType < SpecDBFolder.GT5_JP3009
                || folderType <= SpecDBFolder.GT5_TRIAL_JP2704)
                Columns.Add(new ColumnMetadata("OpenModel", DBColumnType.Byte));

            if (folderType < SpecDBFolder.GT5_TRIAL_EU2704)
            {
                Columns.Add(new ColumnMetadata("NoChangeWheel", DBColumnType.Bool));
                Columns.Add(new ColumnMetadata("NoChangeWing", DBColumnType.Bool));
                Columns.Add(new ColumnMetadata("SuperchargerOriginally", DBColumnType.Bool));
            }
        }
    }
}
