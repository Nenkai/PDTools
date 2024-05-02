using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Windows;

using Syroot.BinaryData;
using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

using System.Threading.Tasks;
using PDTools.SpecDB.Core.Mapping;
using PDTools.SpecDB.Core.Mapping.Types;
using PDTools.SpecDB.Core.Formats;
using PDTools.Utils;

namespace PDTools.SpecDB.Core
{
    public class SpecDB : IDisposable
    {
        public string FolderName { get; set; }
        public SpecDBFolder SpecDBFolderType { get; set; }
        public string SpecDBName { get; set; }
        public int Version { get; set; }

        /// <summary>
        /// Tables for this spec db.
        /// </summary>
        public Dictionary<string, Table> Tables { get; set; } = new Dictionary<string, Table>();

        /// <summary>
        /// Switch as whether we are loading tables like the game does.
        /// If not specialized such as the purpose of this program.
        /// </summary>
        public bool LoadingAsOriginalImplementation { get; }

        public const int SPEC_DB_TABLE_COUNT = 44;
        /// <summary>
        /// All tables that should be loaded as per original implementation.
        /// </summary>
        public Table[] Fixed_Tables { get; }
        public StringDatabase UniversalStringDatabase { get; set; }
        public StringDatabase LocaleStringDatabase { get; set; }
        public string LocaleName { get; set; } = "british"; // Change this accordingly.
        public Dictionary<string, StringDatabase> StringDatabases = new Dictionary<string, StringDatabase>();

        /// <summary>
        /// List of all tables used by the game
        /// </summary>
        private readonly string[] TABLE_NAMES = Enum.GetNames(typeof(SpecDBTables));

        /* Code: ID of the column.
         * Label: name of the row i.e: _117_coupe_68. 
         */

        public SpecDB()
        {

        }

        public SpecDB(string folderName, SpecDBFolder type, bool loadAsOriginalImplementation)
        {
            FolderName = folderName;

            if (folderName.Length > 4 && int.TryParse(folderName.Substring(folderName.Length - 4, 4), out int vers))
                Version = vers;
            else
            {
                string typeName = type.ToString();
                Version = int.Parse(typeName.Substring(typeName.Length - 4, 4));
            }


            SpecDBFolderType = type;

            SpecDBName = Path.GetFileNameWithoutExtension(folderName);
            LoadingAsOriginalImplementation = loadAsOriginalImplementation;

            if (LoadingAsOriginalImplementation)
            {
                Fixed_Tables = new Table[SPEC_DB_TABLE_COUNT];
                for (int i = 0; i < Fixed_Tables.Length; i++)
                    Fixed_Tables[i] = new Table(TABLE_NAMES[i]);
            }

#if DEBUG
            // Will be used for debug printer
            Directory.CreateDirectory(Path.Combine(folderName, "debug"));
#endif
        }

        public static SpecDBFolder? DetectSpecDBType(string folderName)
        {
            if (Enum.TryParse(folderName, out SpecDBFolder folderType))
                return folderType;

            return null;
        }

        public static SpecDB LoadFromSpecDBFolder(string folderName, SpecDBFolder specDbType, bool loadAsOriginalImplementation)
        {
            if (!Directory.Exists(folderName))
                throw new DirectoryNotFoundException("SpecDB directory is not found.");

            SpecDB db = new SpecDB(folderName, specDbType, loadAsOriginalImplementation);
            if (db.LoadingAsOriginalImplementation)
            {
                db.ReadAllTables();
                db.ReadStringDatabases();
            }
            else
            {
                db.PreLoadAllTablesFromCurrentFolder();
            }

            return db;
        }

        private void ReadStringDatabases()
        {
            UniversalStringDatabase = ReadStringDatabase("UnistrDB.sdb");
            LocaleStringDatabase = ReadStringDatabase($"{LocaleName}_StrDB.sdb");
        }

        private StringDatabase ReadStringDatabase(string name)
        {
            byte[] sdbFile = File.ReadAllBytes(Path.Combine(FolderName, name));
            SpanReader sr = new SpanReader(sdbFile);
            sr.Position = 0x08;
            Endian endian = sr.ReadInt16() == 1 ? Endian.Little : Endian.Big;
            StringDatabase sdb = new StringDatabase(endian);

            return sdb;
        }

        public bool KeyExistsAtTable(int keyCode, int tableID)
        {
            if (tableID > Fixed_Tables.Length)
                return false;

            var table = Fixed_Tables[tableID];
            return table.IDExists(keyCode);
        }

        private void ReadAllTables()
        {
            for (int i = 0; i < TABLE_NAMES.Length; i++)
            {
                Fixed_Tables[i].AddressInitialize(this);
                Fixed_Tables[i].ReadIDIMapOffsets(this);
                Fixed_Tables[i].CreateDebugPrinter(Path.Combine(FolderName, "debug", $"{Fixed_Tables[i].TableName}.txt"));
            }
        }

        /// <summary>
        /// Gets the actual full name of a car by its code.
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public string GetCarName(string label)
        {
            int rowID = GetIDOfCarLabel(label);
            if (TryGetCarNameStringIDOfCarID(rowID, out int stringIndex))
                return LocaleStringDatabase.GetStringByID(stringIndex);

            return null;
        }

        /// <summary>
        /// Gets the shortened car name of a car by its label.
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public string GetCarShortName(string label)
        {
            int rowID = GetIDOfCarLabel(label);
            if (TryGetCarShortNameStringIDOfCarID(rowID, out int stringIndex))
                return LocaleStringDatabase.GetStringByID(stringIndex);

            return null;
        }

        public List<string> GetCarLabelList()
        {
            List<string> labels = new List<string>();
            int labelCount = GetCarLabelCount();
            for (int i = 0; i < labelCount; i++)
            {
                string label = GetCarLabelByIndex(i);
                labels.Add(label);
            }

            return labels;
        }

        /// <summary>
        /// Gets the ID of a car by its label.
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public int GetIDOfCarLabel(string label)
            => GetIDOfLabelFromTable(SpecDBTables.GENERIC_CAR, label);


        /// <summary>
        /// Gets the row data from a table by ID.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="keyCode"></param>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public int GetRowFromTable(SpecDBTables table, int keyCode, out Span<byte> rowData)
        {
            rowData = default;
            if (Fixed_Tables[(int)table] != null)
                return Fixed_Tables[(int)table].GetRowN(keyCode, out rowData);

            return 0;
        }

        /// <summary>
        /// Gets the ID of a row by its label.
        /// </summary>
        /// <param name="table">Table to look at.</param>
        /// <param name="label">Label name.</param>
        /// <returns></returns>
        public int GetIDOfLabelFromTable(SpecDBTables table, string label)
        {
            if ((int)table >= 0 && (int)table < Fixed_Tables.Length)
                return Fixed_Tables[(int)table].GetIDOfLabel(label);

            return -1;
        }

        public int GetCarLabelCount()
        {
            IDI_LabelInformation idTable = Fixed_Tables[(int)SpecDBTables.GENERIC_CAR].LabelInformation;
            return idTable.IDCount;
        }

        public int GetLabelCountForTable(SpecDBTables table)
        {
            IDI_LabelInformation idTable = Fixed_Tables[(int)table].LabelInformation;
            return idTable.IDCount;
        }

        /// <summary>
        /// Gets a car code by raw index (not ID).
        /// </summary>
        /// <param name="index">Index within the table.</param>
        /// <returns></returns>
        public string GetCarLabelByIndex(int index)
        {
            int carLabelCount = GetLabelCountForTable(SpecDBTables.GENERIC_CAR);
            if (carLabelCount > -1 && index < carLabelCount)
            {
                //idi = MSpecDB::GetIDITableByIndex(pMVar2, 0);
                IDI_LabelInformation idTable = Fixed_Tables[(int)SpecDBTables.GENERIC_CAR].LabelInformation;
                SpanReader sr = new SpanReader(idTable.Buffer);

                // buf = buf + *(int*)(buf + param_1 * 8 + 0x10) + *(int*)(buf + 4) * 8 + 0x12;
                sr.Position = IDI_LabelInformation.HeaderSize + index * 8;
                int strOffset = sr.ReadInt32();

                sr.Position = 4;
                int entryCount = sr.ReadInt32();

                sr.Position = IDI_LabelInformation.HeaderSize + entryCount * 8 + strOffset; // str map offset + strOffset
                sr.Position += 2; // Ignore string size as per original implementation

                /* Original Returns the length of the string (pointer) and the buffer
                strncpy(strOut, buf, strBufLen);
                strOut[strBufLen + -1] = '\0';
                iVar1 = strlen(strOut, (char*)buf);
                * return iVar1; */

                return sr.ReadString0(); // Null-Terminated
            }

            return null;
        }

        /// <summary>
        /// Gets the offset of the string key for a row code in the IDI.
        /// </summary>
        /// <param name="table">Table to look at.</param>
        /// <param name="code">Code of the row.</param>
        /// <returns></returns>
        public int GetLabelOffsetByIDFromTable(SpecDBTables table, int code)
        {
            Table sTable = Fixed_Tables[(int)table];
            IDI_LabelInformation idi = sTable.LabelInformation;

            SpanReader sr = new SpanReader(idi.Buffer);
            sr.Position = 4;
            int entryCount = sr.ReadInt32();

            // "original" implementation had one while and one do loop, probably decompiler that just failed
            for (int i = 0; i < entryCount; i++)
            {
                sr.Position = IDI_LabelInformation.HeaderSize + i * 8 + 4;
                int entryCode = sr.ReadInt32();
                if (entryCode == code)
                {
                    // Original: return (char*)(idiFile + index * 8 + *(int*)(iVar3 * 8 + idiFile + 0x10) + 0x12);

                    // *(int*)(iVar3 * 8 + idiFile + 0x10)
                    int entryPos = IDI_LabelInformation.HeaderSize + i * 8;
                    sr.Position = entryPos;
                    int stringOffset = sr.ReadInt32();

                    // idiFile + index * 8 (go to the beginning of the second table)
                    sr.Position = IDI_LabelInformation.HeaderSize + entryCount * 8; // Header is added due to below

                    // Add the two
                    sr.Position += stringOffset;

                    //0x12 is just the base header + the string length as short, optimized
                    return sr.Position + 2;
                }
            }

            return -1; // NULL
        }

        /// <summary>
        /// Gets the string db index (actual name) of a car ID within the string database.
        /// </summary>
        /// <param name="carCode"></param>
        /// <param name="stringIndex"></param>
        /// <returns></returns>
        private bool TryGetCarNameStringIDOfCarID(int carCode, out int stringIndex)
        {
            stringIndex = 0;

            int dataLength = Fixed_Tables[(int)SpecDBTables.CAR_NAME_].GetRowN(carCode, out Span<byte> rowData);
            if (dataLength != 0)
            {
                stringIndex = BinaryPrimitives.ReadInt32LittleEndian(rowData);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the string db index (short name) of a car ID within the string database.
        /// </summary>
        /// <param name="carCode"></param>
        /// <param name="stringIndex"></param>
        /// <returns></returns>
        private bool TryGetCarShortNameStringIDOfCarID(int carCode, out int stringIndex)
        {
            stringIndex = 0;

            int dataLength = Fixed_Tables[(int)SpecDBTables.CAR_NAME_].GetRowN(carCode, out Span<byte> rowData);
            if (dataLength != 0)
            {
                stringIndex = BinaryPrimitives.ReadInt32LittleEndian(rowData.Slice(4));
                return true;
            }

            return false;
        }

        // Non Original Implementations
        public void PreLoadAllTablesFromCurrentFolder()
        {
            var tablePaths = Directory.GetFiles(FolderName, "*.dbt", SearchOption.TopDirectoryOnly);
            foreach (var table in tablePaths)
            {
                string tableName = Path.GetFileNameWithoutExtension(table);
                if (!File.Exists(Path.Combine(FolderName, tableName) + ".idi"))
                    continue;

                var specdbTable = new Table(tableName);
                specdbTable.AddressInitialize(this);
                specdbTable.ReadIDIMapOffsets(this);
                specdbTable.CreateDebugPrinter(Path.Combine(FolderName, "debug", $"{tableName}.txt"));

                Tables.Add(specdbTable.TableName, specdbTable);
            }
        }

        public void SavePartsInfoFile(IProgress<(int, string)> progress, bool tbdFile, string folder)
        {
            if (tbdFile)
                PartsInfoBuilder.WritePartsInformationNew(this, progress, folder);
            else
                PartsInfoBuilder.WritePartsInformationOld(this, progress, folder);
        }

        public void Dispose()
        {
            foreach (var table in Tables)
            {
                table.Value.DisposeDebugPrinter();
            }
        }

        public enum SpecDBTables
        {
            GENERIC_CAR,
            GENERIC_CAR_INFO,
            BRAKE,
            BRAKECONTROLLER,
            SUSPENSION,
            ASCC,
            TCSC,
            CHASSIS,
            RACINGMODIFY,
            LIGHTWEIGHT,
            STEER,
            DRIVETRAIN,
            GEAR,
            ENGINE,
            NATUNE,
            TURBINEKIT,
            DISPLACEMENT,
            COMPUTER,
            INTERCOOLER,
            MUFFLER,
            CLUTCH,
            FLYWHEEL,
            PROPELLERSHAFT,
            LSD,
            FRONTTIRE,
            REARTIRE,
            NOS,
            SUPERCHARGER,
            INTAKE_MANIFOLD,
            EXHAUST_MANIFOLD,
            CATALYST,
            AIR_CLEANER,
            BOOST_CONTROLLER,
            INDEP_THROTTLE,
            TIRESIZE,
            TIRECOMPOUND,
            TIREFORCEVOL,
            DEFAULT_PARTS,
            DEFAULT_PARAM,
            GENERIC_ITEMS,
            TUNED_CARS,
            COURSE,
            RACE,
            VARIATION,
            MODEL_INFO,
            WHEEL,
            CAR_CUSTOM_INFO,
            PAINT_COLOR_INFO,
            MAKER,
            TUNER,
            CAR_NAME_,
            CAR_NAME_ALPHABET,
            CAR_NAME_JAPAN,
        }
    }

    public enum SpecDBFolder
    {
        [Description("Unknown")]
        NONE,

        [Description("Gran Turismo 4 Special Ediiton 2004 Geneva Version (PAL, AMLUX)")]
        GT4_AMLUX1000,

        [Description("Gran Turismo 4 Special Ediiton 2004 Geneva Version (PAL, AUTOSALON)")]
        GT4_AUTOSALON1000,

        [Description("Gran Turismo 4 Special Ediiton 2004 Geneva Version/Toyota Demo (PAL, MTR_PRIUS)")]
        GT4_MTR_PRIUS1000,

        [Description("Gran Turismo 4 Prologue (Pre-Release)")]
        GT4_PROLOGUE_1000,

        [Description("Gran Turismo 4 Prologue (Korean)")]
        GT4_PROLOGUE_KR1000,

        [Description("Gran Turismo 4 Prologue (Taiwan)")]
        GT4_PROLOGUE_TW1000,

        [Description("Gran Turismo 4 Prologue (EU)")]
        GT4_PROLOGUE_EU1110,

        [Description("Gran Turismo 4 (China)")]
        GT4_CN2560,

        [Description("Gran Turismo 4 (Korean)")]
        GT4_KR2560,

        [Description("Gran Turismo 4 (US)")]
        GT4_US2560,

        [Description("Gran Turismo 4 (EU)")]
        GT4_EU2560,

        [Description("Gran Turismo 4 Online Test Version (US)")]
        GT4_PREMIUM_US2560,

        [Description("Gran Turismo 4 Japan Online Test")]
        GT4_PREMIUM_JP2560,

        [Description("Tourist Trophy (US)")]
        TT_EU2630,

        [Description("Gran Turismo HD Concept (EU)")]
        GT5_TRIAL_EU2704,

        [Description("Gran Turismo HD Concept (US)")]
        GT5_TRIAL_US2704,

        [Description("Gran Turismo HD Concept (JP)")]
        GT5_TRIAL_JP2704,

        [Description("Gran Turismo 5 Prologue Demo (JP)")]
        GT5_TRIAL2007_2730,

        [Description("Gran Turismo 5 Prologue")]
        GT5_PROLOGUE2813,

        [Description("Gran Turismo PSP")]
        GT_PSP_JP2817,

        [Description("Gran Turismo 5 Time Trial Challenge")]
        GT5_ACADEMY_09_2900,

        [Description("Gran Turismo 5 Kiosk Demo (Default DB)")]
        GT5_JP2904,

        [Description("Gran Turismo 5 Kiosk Demo (Preview DB)")]
        GT5_PREVIEWJP2904,

        [Description("Gran Turismo 5 QA (16/09/2010)")]
        GT5_JP3003,

        [Description("Gran Turismo 5 (1.00-1.04)")]
        GT5_JP3009,           // GT5 Retail

        [Description("Gran Turismo 5 (1.05+)")]
        GT5_JP3010,           // GT5 - 1.05+
    }
}
