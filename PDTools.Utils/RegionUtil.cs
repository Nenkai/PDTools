﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTEventMaker
{
    public static class RegionUtil
    {
        public static Dictionary<int, Region> _initialRegionMap = new Dictionary<int, Region>()
        {
            {0, new Region("ABCDEFGHIJKLMNOPRSTVW", "UZ", 2,
                new[] { "US", "US", "US", "GB", "GB", "GB", "CA", "CA", "IT", "BR", "SG"}) },

            {99, new Region("ABCDEFGHIJKLMNOPRSTVW", "UZ", 2,
                new[] { "US", "US", "US", "GB", "GB", "GB", "CA", "CA", "IT", "BR", "SG"}) },

            {1, new Region("AKSJTNHMYRD", "IIUUEEEEOOGGGGCCCCFFFFBBBBZZZZWWWWP", 2,
                new[] { "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "US", "BR"}) },

            {2, new Region("ABCDEFGLMPRSTUV", "HIIJJNNOO", 2,
                new[] { "IT", "IT", "IT", "IT", "IT", "IT", "IT", "IT", "IT", "IT", "IT", "US"}) },

            {3, new Region("ABCDEFGHJKLMNOPRSTUW", "IU", 2,
                new[] { "DE", "DE", "DE", "DE", "DE", "DE", "DE", "DE", "DE", "DE", "DE", "AT", "AT"}) },

            {4, new Region("ABCDEFGHJLMOPRSTV", "INNNUUXYY", 2,
                new[] { "FR", "FR", "FR", "FR", "FR", "FR", "FR", "FR", "FR", "BE"}) },

            {5, new Region("ABCDEFGHIJLMNOPRSTUV", "QWXZ", 2,
                new[] { "ES", "ES", "ES", "ES", "ES", "ES","BR","BR","BR","BR","MX","MX","MX","AR","AR","CO","CO","CO","PE" }) },

            {6, new Region("AEFHIJKLMNOPRSTUV", "DY", 2,
                new[] { "FI", "FI", "FI", "FI", "NO" }) },

            {7, new Region("ABCDEFGHIJKLMNOPRSTUVWY", "UZ", 2,  // Copied from 0
                new[] { "SE", "SE" }) },

            {8, new Region("ABCDEFGHIJKLMNOPRSTVW", "UZ", 2, // Copied from 0
                new[] { "NO" }) },

            {9, new Region("ABCDEFGHIJKLMNOPRSTVW", "UZ", 2, // Copied from 0
                new[] { "NL" }) },

            {10, new Region("ABCDEFGHIJKLMNOPRSTVW", "UZ", 2, // Copied from 0
                new[] { "DK" }) },

            {11, new Region("ACDEGHIJLMNOPRST", "UXZ", 2,
                new[] { "GR", "GR", "GR", "GR", "GR", "TR", "CY" }) },

            {12, new Region("ABCDEFGHIJKLMNOPRSTVW", "UZ", 2, // Copied from 0
                new[] { "BE" }) },

            {13, new Region("AVGDEILMNPRS", "BZKOTFEY", 2,
                new[] { "RU", "RU", "RU", "RU", "RU", "RU", "UA" }) },

            {14, new Region("ABCDEFGHIJKLMNOPRSTVW", "UZ", 2,  // Copied from 0
                new[] { "HU" }) },

            {15, new Region("ABCDEFGHJKLMNOPQRSTWXYZ", "ABCDEFGHJKLMNOPQRSTWXYZ", 2,
                new[] { "HK", "HK", "TW", "TW", "US", "US" }) },

            {16, new Region("ABCDEGHIJKMNOPRSTWY", "ABCDEGHIJKMNOPRSTWY", 2,
                new[] { "KR", "KR", "KR", "US"}) },

            {17, new Region("ABCDEFGHIJKLMNOPRSTVW", "UZ", 2, // Copied from 0
                new[] { "SG" }) },

            {18, new Region("ABCDEFGHJKLMNOPQRSTWXYZ", "ABCDEFGHJKLMNOPQRSTWXYZ", 2,  // Copied from 15
                new[] { "HK", "HK", "TW", "TW", "US", "US" }) },

            {19, new Region("ABCDEFGHIJKLMNOPRSTVW", "UZ", 2, // Copied from 0
                new[] { "IE" }) },

            {20, new Region("ABCDEFGHIJKLMNOPRSTVW", "UZ", 2, // Copied from 0
                new[] { "AT" }) },

            {21, new Region("ABCDEFGHIJKLMNOPRSTVW", "UZ", 2, // Copied from 0
                new[] { "PT" }) },

            {22, new Region("ABCDEFGHIJKLMNOPRSTVW", "UZ", 2, // Copied from 0
                new[] { "PL" }) },

            {23, new Region("ABCDEFGHIJKLMNOPRSTUVYZ", "ABCDEFGHIJKLMNOPRSTUVYZ", 2,
                new[] { "TR", "TR", "US" }) },
        };

        public static (char initial, string country) GetRandomInitial(Random random, int code)
        {
            Region region = _initialRegionMap[code];

            /* VARIABLE_PUSH: rare_ratio, Value=6
             * VARIABLE_EVAL: tbl, ValueEval=4
             * ATTRIBUTE_EVAL: size
             * BINARY_ASSIGN_OPERATOR: * (__mul__) */
            int maxR = region.RareRatio * region.Table.Length;

            /* VARIABLE_EVAL: pdistd,MRandom,GetValue,pdistd::MRandom::GetValue
               INT_CONST: 0 (0x00)
                VARIABLE_EVAL: rare_ratio, ValueEval=6
                CALL: Value=2 */
            var r = random.Next(0, maxR);

            char initial;
            // INT_CONST: 0 (0x00)
            // BINARY_OPERATOR: == (__eq__)
            if (r == 0)
            {
                /* VARIABLE_EVAL: tbl_rare, ValueEval=5
                 * VARIABLE_EVAL: pdistd,MRandom,GetValue,pdistd::MRandom::GetValue
                 * INT_CONST: 0
                 * VARIABLE_EVAL: tbl_rare
                 * ATTRIBUTE_EVAL: size
                 * CALL: Value=2 */
                initial = region.RareTable[random.Next(0, region.RareTable.Length)];
            }
            else
            {
                /* VARIABLE_EVAL: tbl, ValueEval=5
                 * VARIABLE_EVAL: pdistd,MRandom,GetValue,pdistd::MRandom::GetValue
                 * INT_CONST: 0
                 * VARIABLE_EVAL: tbl
                 * ATTRIBUTE_EVAL: size
                 * CALL: Value=2 */
                initial = region.Table[random.Next(0, region.Table.Length)];
            }

            /* VARIABLE_EVAL: pdistd,MRandom,GetValue,pdistd::MRandom::GetValue
             * INT_CONST: 0 (0x00)
             * VARIABLE_EVAL: countries
             * ATTRIBUTE_EVAL: size
             * CALL: Value=2 */
            string country = region.Countries[random.Next(0, region.Countries.Length)];
            return (initial, country);
        }
    }

    public class Region
    {
        public string Table;
        public string RareTable;
        public int RareRatio;
        public string[] Countries;

        public Region(string tbl, string tbl_rare, int ratio, string[] countries)
        {
            Table = tbl;
            RareTable = tbl_rare;
            RareRatio = ratio;
            Countries = countries;
        }
    }
}
