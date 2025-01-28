using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables;

public class Suspension : TableMetadata
{
    public override string LabelPrefix { get; } = "su_";

    public Suspension(SpecDBFolder folderType)
    {
        Columns.Add(new ColumnMetadata("rideheightMINF", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("rideheightMAXF", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("rideheightDFF", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("rideheightMINR", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("rideheightMAXR", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("rideheightMDFR", DBColumnType.Short));

        if (folderType > SpecDBFolder.GT5_TRIAL_JP2704)
        {
            Columns.Add(new ColumnMetadata("targetFrequencyFMin", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("targetFrequencyFMax", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("targetFrequencyFDF", DBColumnType.Short));

            Columns.Add(new ColumnMetadata("targetFrequencyRMin", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("targetFrequencyRMax", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("targetFrequencyRDF", DBColumnType.Short));
        }

        Columns.Add(new ColumnMetadata("Price", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("camberMINF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("camberMAXF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("camberDFF", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("camberMINR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("camberMAXR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("camberDFR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("strokecamberF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("strokecamberR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("cmbgripFX1", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("cmbgripFx2", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("cmbgripFx3", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("cmbgripFx4", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("cmbgripFy1", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("cmbgripFy2", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("cmbgripFy3", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("cmbgripFy4", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("cmbgripRx1", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("cmbgripRx2", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("cmbgripRx3", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("cmbgripRx4", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("cmbgripRy1", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("cmbgripRy2", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("cmbgripRy3", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("cmbgripRy4", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("toeMINF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("toeMAXF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("toeDFF", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("toeMINR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("toeMAXR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("toeDFR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("brmarginF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("brmarginR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("brtouchF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("brtouchR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("limrF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("limrR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("springratevol", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("springrateMINF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("springrateMAXF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("springrateDFF", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("springrateMINR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("springrateMAXR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("springrateDFR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("leverratioDFF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("leverratioDFR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("bumprubberF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("bumprubberR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("bumprubberDMF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("bumprubberR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("dampV1BF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dampV1BR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("dampV2BF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dampV2BR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("dampV1RF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dampV1RR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("dampV2RF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dampV2RR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("damplevelBF", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("dampF1BMINF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dampF1BMAXF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dampF1BDFF", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("dampF2BMINF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dampF2BMAXF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dampF2BDFF", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("damplevelRF", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("damplevelF1RMINF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("damplevelF1RMAXF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("damplevelF1RDFF", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("damplevelF2RMINF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("damplevelF2RMAXF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("damplevelF2RDFF", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("damplevelBR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("damplevelF1BMINR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("damplevelF1BMAXR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("damplevelF1BDFF", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("damplevelF2BMINR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("damplevelF2BMAXR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("damplevelF2BDFR", DBColumnType.Byte));


        Columns.Add(new ColumnMetadata("damplevelRR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("damplevelF1RMINR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("damplevelF1RMAXR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("damplevelF1RDFF", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("damplevelF2RMINR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("damplevelF2RMAXR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("damplevelF2BRFR", DBColumnType.Byte));


        Columns.Add(new ColumnMetadata("unsprungmassF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("unsprungmassR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("stabilizerFlevel", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("stabilizerMINF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("stabilizerMAXF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("stabilizerDFF", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("stabilizerRlevel", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("stabilizerMINR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("stabilizerMAXR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("stabilizerDFR", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("ActiveSuspensionType", DBColumnType.Byte));

        if (folderType > SpecDBFolder.GT5_TRIAL_JP2704)
        {
            Columns.Add(new ColumnMetadata("springratelevelF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("springratelevelR", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("rideheightlevelF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("rideheightlevelR", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("AutoDampingForce", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("DampingRatioFBLevel", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("DampingRatioFBDF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("DampingRatioFBMin", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("DampingRatioFBMax", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("DampingRatioFRLevel", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("DampingRatioFRDF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("DampingRatioFRMin", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("DampingRatioFRMax", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("DampingRatioRBLevel", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("DampingRatioRBDF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("DampingRatioRBMin", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("DampingRatioRRMax", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("DampingRatioRRLevel", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("DampingRatioRRDF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("DampingRatioRRMin", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("DampingRatioRRMax", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("caster", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("targetFrequencyFLv", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("targetFrequencyRLv", DBColumnType.Byte));
        }
    }
}
