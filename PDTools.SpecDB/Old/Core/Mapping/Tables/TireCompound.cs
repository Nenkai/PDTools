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
    public class TireCompound : TableMetadata
    {
        public TireCompound(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("tirewear", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("Mu", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("weightgripx1", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("weightgripx2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("weightgripx3", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("weightgripx4", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("weightgripy1", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("weightgripy2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("weightgripy3", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("weightgripy4", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sideforceprecision", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("sideforcex1", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sideforcex2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sideforcex3", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sideforcex4", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sideforcex5", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sideforcex6", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sideforcex7", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sideforcex8", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("sideforcey1", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sideforcey2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sideforcey3", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sideforcey4", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sideforcey5", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sideforcey6", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sideforcey7", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sideforcey8", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("corneringdragx1", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("corneringdragx2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("corneringdragx3", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("corneringdragx4", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("corneringdragx5", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("corneringdragx6", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("corneringdragy1", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("corneringdragy2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("corneringdragy3", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("corneringdragy4", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("corneringdragy5", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("corneringdragy6", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("slipmuAx1", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuAx2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuAx3", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuAx4", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuAx5", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuAx6", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("slipmuAy1", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuAy2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuAy3", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuAy4", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuAy5", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuAy6", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("sidemuAx1", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuAx2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuAx3", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuAx4", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuAx5", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuAx6", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("sidemuAy1", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuAy2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuAy3", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuAy4", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuAy5", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuAy6", DBColumnType.Byte));


            Columns.Add(new ColumnMetadata("slipmuBx1", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuBx2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuBx3", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuBx4", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuBx5", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuBx6", DBColumnType.Byte));
                                                  
            Columns.Add(new ColumnMetadata("slipmuBy1", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuBy2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuBy3", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuBy4", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuBy5", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("slipmuBy6", DBColumnType.Byte));
                                                
            Columns.Add(new ColumnMetadata("sidemuBx1", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuBx2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuBx3", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuBx4", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuBx5", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuBx6", DBColumnType.Byte));
                                                  
            Columns.Add(new ColumnMetadata("sidemuBy1", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuBy2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuBy3", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuBy4", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuBy5", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sidemuBy6", DBColumnType.Byte));

            
            Columns.Add(new ColumnMetadata("lslide", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("cslide", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("sideforce", DBColumnType.Byte));


            if (folderType >= SpecDBFolder.GT5_PROLOGUE2813)
            {
                Columns.Add(new ColumnMetadata("sidedir", DBColumnType.Byte));

                Columns.Add(new ColumnMetadata("sslideAx1", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideAx2", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideAx3", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideAx4", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideAx5", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideAx6", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideAx7", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideAx8", DBColumnType.Byte));

                Columns.Add(new ColumnMetadata("sslideAy1", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideAy2", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideAy3", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideAy4", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideAy5", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideAy6", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideAy7", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideAy8", DBColumnType.Byte));

                Columns.Add(new ColumnMetadata("sslideBx1", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideBx2", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideBx3", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideBx4", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideBx5", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideBx6", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideBx7", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideBx8", DBColumnType.Byte));


                Columns.Add(new ColumnMetadata("sslideBy1", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideBy2", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideBy3", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideBy4", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideBy5", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideBy6", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideBy7", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("sslideBy8", DBColumnType.Byte));
            }

            if (folderType >= SpecDBFolder.GT5_JP3009)
            {
                Columns.Add(new ColumnMetadata("FrictionOvalSide", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("FrictionOvalDir", DBColumnType.Byte));
            }
        }
    }
}
