using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Buffers.Binary;
using System.Collections.ObjectModel;

using SpecDBOld.Core.Formats;
using SpecDBOld.Mapping;
using SpecDBOld.Mapping.Tables;
using SpecDBOld.Mapping.Types;

using Syroot.BinaryData.Memory;
using Syroot.BinaryData;
using Syroot.BinaryData.Core;

namespace SpecDBOld.Core
{
    public class SpecDBTable
    {
        public DatabaseTable DBT { get; set; }
        public LabelInformation IDI { get; set; }

        public string TableName { get; set; }

        // Non original properties
        public bool IsLoaded { get; set; }
        public int LastID { get; set; }
        public List<RowData> Keys { get; set; } // Could be a dictionary, but sometimes tables such as VARIATION or WHEEL have multiple rows for one key
        public TableMetadata TableMetadata { get; set; }
        public ObservableCollection<SpecDBRowData> Rows { get; set; }
        public int TableID { get; set; }

        public bool IsTableProperlyMapped { get; private set; } = true;
        public SpecDBTable(string tableName)
        {
            TableName = tableName;
        }

        public bool IDExists(int id)
            => DBT.SearchNumber(id) != -1;

        /// <summary>
        /// Gets the row data for a row code.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public int GetRow(int code, out Span<byte> rowData)
        {
            int entryIndex = DBT.SearchNumber(code);
            return GetRowN(entryIndex, out rowData);
        }

        /// <summary>
        /// Gets the data for a row number.
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public int GetRowN(int entryNumber, out Span<byte> rowData)
        {
            rowData = default;

            int dataLength = DBT.RowDataLength;

            //if (uVar1 < *(uint *)(file + 8)) {
            if (entryNumber < DBT.EntryCount)
            {
                SpanReader sr = new SpanReader(DBT.Buffer, DBT.Endian);

                // Short version
                sr.Position = DatabaseTable.HeaderSize + (DatabaseTable.EntrySize * entryNumber) + 4;
                int entryOffset = sr.ReadInt32();
                sr.Position = DBT.UnkOffset4 + entryOffset;

                if ((DBT.VersionHigh & 1) == 0)
                    rowData = sr.ReadBytes(dataLength); // memcpy(retSdbIndex,offs,dataLength);
                else
                    rowData = DBT.ExtractRow(ref sr);

                return dataLength;
            }

            return 0;
        }

        /// <summary>
        /// Gets a label by raw index (not ID).
        /// </summary>
        /// <param name="index">Index within the table.</param>
        /// <returns></returns>
        public string GetLabelByIndex(int index)
        {
            int labelCount = IDI.KeyCount;
            if (labelCount > -1 && index < labelCount)
            {
                //idi = MSpecDB::GetIDITableByIndex(pMVar2, 0);
                SpanReader sr = new SpanReader(IDI.Buffer, IDI.Endian);

                sr.Position = LabelInformation.HeaderSize + (index * LabelInformation.EntrySize);
                int strOffset = sr.ReadInt32();

                sr.Position = 4;
                int entryCount = sr.ReadInt32();

                sr.Position = LabelInformation.HeaderSize + (entryCount * 8) + strOffset; // str map offset + strOffset
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
        /// Gets the ID/Code of a label.
        /// </summary>
        /// <param name="label">Label name.</param>
        /// <returns></returns>
        public int GetIDOfLabel(string label)
            => IDI.SearchLabelID(label);

        #region Initializers

        public void AddressInitialize(SpecDB specDb)
        {
            //if (TableName.Equals("CAR_NAME_"))
            //    TableName += specDb.LocaleName;

            var buffer = File.ReadAllBytes(Path.Combine(specDb.FolderName, TableName) + ".dbt");
            SpanReader sr = new SpanReader(buffer);

            var magic = sr.ReadStringRaw(4);
            if (magic != "GTDB")
                throw new InvalidDataException("DBT Table had invalid magic.");

            ushort versionHigh = sr.ReadUInt16();
            switch (versionHigh)
            {
                case 0x0001:
                case 0x0003:
                case 0x0103:
                case 0x0004:
                case 0x0104:
                    sr.Endian = Endian.Little; break;
                case 0x0500:
                case 0x0700:
                case 0x0800:
                case 0x0801:
                    sr.Endian = Endian.Big; break;
            }
            DBT = new DatabaseTable(buffer, sr.Endian);

            sr.Position = 4;
            versionHigh = sr.ReadUInt16();
            sr.Position += 2;
            uint entryCount = sr.ReadUInt32();

            if (sr.Length <= 32)
                return;

            sr.Position = (int)(DatabaseTable.HeaderSize + (entryCount * 8));
            DBT.EntryInfoMapOffset = sr.Position;
            if ((versionHigh & 1) != 0)
            {
                DBT.RawEntryInfoMapOffset = sr.Position + 0x08;
                DBT.SearchTableOffset = sr.Position + 0x208;

                sr.Position += sr.ReadInt32();
                DBT.DataMapOffset = sr.Position;
                DBT.RawDataMapOffset = sr.Position + 0x08;

                DBT.UnkOffset4 = sr.Position + sr.ReadInt32();
            }
            else
                DBT.UnkOffset4 = sr.Position;
        }

        public void ReadIDIMapOffsets(SpecDB specDb)
        {
            //if (TableName.Equals("CAR_NAME_"))
            //    TableName += specDb.LocaleName;

            var buffer = File.ReadAllBytes(Path.Combine(specDb.FolderName, TableName) + ".idi");
            SpanReader sr = new SpanReader(buffer);

            var magic = sr.ReadStringRaw(4);
            if (magic != "GTID")
                throw new InvalidDataException("IDI Table had invalid magic.");

            Endian endian = sr.ReadByte() != 0 ? Endian.Little : Endian.Big;

            IDI = new LabelInformation(buffer, endian);
            sr.Endian = endian;
            sr.Position = 0x0C;
            TableID = sr.ReadInt32();
        }

        #endregion

        // Non original functions

        public void LoadAllRows(SpecDB db)
        {
            LoadAllRowKeys();
            LoadMetadata(db);
            LoadAllRowData();
            PopulateRowStringsIfNeeded(db);
            IsLoaded = true;
        }

        private void LoadAllRowKeys()
        {

            // Make a list of all the keys the IDI contains. IDI sometimes have keys without data.
            SpanReader idiReader = new SpanReader(IDI.Buffer, IDI.Endian);
            SortedList<int, string> idsToLabels = new SortedList<int, string>();

            int idiKeyCount = IDI.KeyCount;
            for (int i = 0; i < idiKeyCount; i++)
            {
                idiReader.Position = LabelInformation.HeaderSize + (i * 0x08);
                int labelOffset = idiReader.ReadInt32();
                int id = idiReader.ReadInt32();

                idiReader.Position = LabelInformation.HeaderSize + (idiKeyCount * 0x08) + labelOffset;
                idiReader.Position += 2; // Ignore string length
                string label = idiReader.ReadString0();
                idsToLabels.Add(id, label);
            }


            // Register all our keys that actually have data now.
            SpanReader dbtReader = new SpanReader(DBT.Buffer, DBT.Endian);
            Keys = new List<RowData>();

            int keyCount = DBT.EntryCount;
            for (int i = 0; i < keyCount; i++)
            {
                dbtReader.Position = DatabaseTable.HeaderSize + (i * 0x08);
                int id = dbtReader.ReadInt32();
                Keys.Add(new RowData() { Id = id, Label = idsToLabels[id] });
            }

            LastID = Keys.Any() ? Keys.Last().Id : 0;
        }

        public int DumpTable(string path)
        {
            // Make a list of all the keys the IDI contains. IDI sometimes have keys without data.
            SpanReader idiReader = new SpanReader(IDI.Buffer, IDI.Endian);
            SortedList<int, string> idsToLabels = new SortedList<int, string>();
            int idiKeyCount = IDI.KeyCount;
            for (int i = 0; i < idiKeyCount; i++)
            {
                idiReader.Position = LabelInformation.HeaderSize + (i * 0x08);
                int labelOffset = idiReader.ReadInt32();
                int id = idiReader.ReadInt32();

                idiReader.Position = LabelInformation.HeaderSize + (idiKeyCount * 0x08) + labelOffset;
                idiReader.Position += 2; // Ignore string length
                string label = idiReader.ReadString0();
                idsToLabels.Add(id, label);
            }


            // Register all our keys that actually have data now.
            SpanReader dbtReader = new SpanReader(DBT.Buffer, DBT.Endian);
            var keys = new List<RowData>();

            int keyCount = DBT.EntryCount;
            for (int i = 0; i < keyCount; i++)
            {
                dbtReader.Position = DatabaseTable.HeaderSize + (i * 0x08);
                int id = dbtReader.ReadInt32();
                keys.Add(new RowData() { Id = id, Label = idsToLabels[id] });
            }

            using (var sw = new StreamWriter(path))
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    RowData key = keys[i];
                    GetRowN(i, out Span<byte> rowData);
                    sw.WriteLine($"{key.Label} ({key.Id}) | {BitConverter.ToString(rowData.ToArray())}");
                }
            }

            return keys.Count;
        }

        public void ExportTableText(string path)
        {
            int[] maxColumnLengths = new int[2 + TableMetadata.Columns.Count];

            int maxIDLen = 0;
            int maxLabelLen = 0;
            foreach (var row in Rows)
            {
                int idlen = row.ID.ToString().Length;
                if (idlen > maxIDLen)
                    maxIDLen = idlen;

                int labelLen = row.Label.ToString().Length;
                if (labelLen > maxLabelLen)
                    maxLabelLen = labelLen;

                for (int i = 0; i < row.ColumnData.Count; i++)
                {
                    string val = row.ColumnData[i].ToString();
                    if (val.Length > maxColumnLengths[i])
                        maxColumnLengths[i] = val.Length;
                }
            }
            maxColumnLengths[0] = maxIDLen;
            maxColumnLengths[1] = maxLabelLen;

            using (var sw = new StreamWriter(path))
            {
                sw.Write($"{"ID".PadRight(maxIDLen)} | {"Label".PadRight(maxLabelLen)} |");
                for (int i = 0; i < maxColumnLengths.Length - 2; i++)
                {
                    int len = Math.Max((int)maxColumnLengths[i+2], TableMetadata.Columns[i].ColumnName.Length);
                    sw.Write($"{TableMetadata.Columns[i].ColumnName.PadRight(len)} |");
                }
                sw.WriteLine();

                foreach (var row in Rows)
                {
                    sw.Write($"{row.ID.ToString().PadRight(maxIDLen)} | {row.Label.PadRight(maxLabelLen)} |");
                    for (int i = 0; i < maxColumnLengths.Length - 2; i++)
                    {
                        var val = row.ColumnData[i].ToString();
                        int len = Math.Max((int)maxColumnLengths[i+2], TableMetadata.Columns[i].ColumnName.Length);
                        sw.Write($"{val.PadRight(len)} |");
                    }
                    sw.WriteLine();
                }
            }
        }

        public void ExportTableCSV(string path)
        {
            using (var sw = new StreamWriter(path))
            {
                sw.Write("ID,Label,");
                for (int i = 0; i < TableMetadata.Columns.Count; i++)
                {
                    sw.Write($"{TableMetadata.Columns[i].ColumnName}");
                    if (i != TableMetadata.Columns.Count - 1)
                        sw.Write(",");
                }
                sw.WriteLine();

                foreach (var row in Rows)
                {
                    sw.Write($"{row.ID},{row.Label},");
                    for (int i = 0; i < row.ColumnData.Count; i++)
                    {
                        sw.Write($"{row.ColumnData[i].ToString()}");
                        if (i != TableMetadata.Columns.Count - 1)
                            sw.Write(",");
                    }
                    sw.WriteLine();
                }
            }
                
        }

        private void LoadMetadata(SpecDB db)
        {
            if (TableName.StartsWith("CAR_NAME_"))
            {
                string locale = TableName.Split('_')[2];
                if (locale.Equals("ALPHABET") || locale.Equals("JAPAN"))
                    locale = "UnistrDB.sdb";
                else
                    locale += "_StrDB.sdb";

                TableMetadata = new CarName(db.SpecDBFolderType, locale);
            }
            else if (TableName.StartsWith("CAR_VARIATION_"))
            {
                string locale = TableName.Split('_')[2];
                if (locale.Equals("ALPHABET") || locale.Equals("JAPAN"))
                    locale = "UnistrDB.sdb";
                else
                    locale += "_StrDB.sdb";
                TableMetadata = new CarVariation(db.SpecDBFolderType, locale);
            }
            else if (TableName.StartsWith("COURSE_NAME_"))
            {
                string locale = TableName.Split('_')[2];
                if (locale.Equals("ALPHABET") || locale.Equals("JAPAN"))
                    locale = "UnistrDB.sdb";
                else
                    locale += "_StrDB.sdb";
                TableMetadata = new CourseName(db.SpecDBFolderType, locale);
            }
            else if (TableName.StartsWith("VARIATION"))
            {
                string locale = "UnistrDB.sdb";
                if (TableName.Length > 9)
                {
                    locale = TableName.Substring(9);
                    if (!locale.Equals("ALPHABET") && !locale.Equals("JAPAN"))
                        locale += "_StrDB.sdb";
                }
                TableMetadata = new Variation(db.SpecDBFolderType, locale);
            }
            else if (TableName.StartsWith("RIDER_EQUIPMENT"))
            {
                string locale = "UnistrDB.sdb";
                if (TableName.Length > 15)
                {
                    locale = TableName.Substring(15);
                    if (!locale.Equals("ALPHABET") && !locale.Equals("JAPAN"))
                        locale += "_StrDB.sdb";
                }
                TableMetadata = new RiderEquipment(db.SpecDBFolderType, locale);
            }
            else
            {
                switch (TableName)
                {
                    case "AIR_CLEANER":
                        TableMetadata = new AirCleaner(db.SpecDBFolderType); break;
                    case "ALLOW_ENTRY":
                        TableMetadata = new AllowEntry(db.SpecDBFolderType); break;
                    case "ARCADEINFO_NORMAL":
                    case "ARCADEINFO_TUNED":
                        TableMetadata = new ArcadeInfoNormal(db.SpecDBFolderType); break;
                    case "ASCC":
                        TableMetadata = new ASCC(db.SpecDBFolderType); break;
                    case "BRAKE":
                        TableMetadata = new Brake(db.SpecDBFolderType); break;
                    case "BRAKECONTROLLER":
                        TableMetadata = new BrakeController(db.SpecDBFolderType); break;
                    case "CATALYST":
                        TableMetadata = new Catalyst(db.SpecDBFolderType); break;
                    case "CLUTCH":
                        TableMetadata = new Clutch(db.SpecDBFolderType); break;
                    case "COMPUTER":
                        TableMetadata = new Computer(db.SpecDBFolderType); break;
                    case "COURSE":
                        TableMetadata = new Course(db.SpecDBFolderType); break;
                    case "CAR_CUSTOM_INFO":
                        TableMetadata = new CarCustomInfo(db.SpecDBFolderType); break;
                    case "DEFAULT_PARAM":
                        TableMetadata = new DefaultParam(db.SpecDBFolderType); break;
                    case "DEFAULT_PARTS":
                        TableMetadata = new DefaultParts(db.SpecDBFolderType); break;
                    case "DISPLACEMENT":
                        TableMetadata = new Displacement(db.SpecDBFolderType); break;
                    case "DRIVETRAIN":
                        TableMetadata = new Drivetrain(db.SpecDBFolderType); break;
                    case "ENGINE":
                        TableMetadata = new Engine(db.SpecDBFolderType); break;
                    case "EXHAUST_MANIFOLD":
                        TableMetadata = new ExhaustManifold(db.SpecDBFolderType); break;
                    case "FLYWHEEL":
                        TableMetadata = new Flywheel(db.SpecDBFolderType); break;
                    case "GEAR":
                        TableMetadata = new Gear(db.SpecDBFolderType); break;
                    case "MAKER":
                        TableMetadata = new Maker(db.SpecDBFolderType); break;
                    case "MODEL_INFO":
                        TableMetadata = new ModelInfo(db.SpecDBFolderType); break;
                    case "PAINT_COLOR_INFO":
                        TableMetadata = new PaintColorInfo(db.SpecDBFolderType); break;
                    case "GENERIC_CAR":
                        TableMetadata = new GenericCar(db.SpecDBFolderType); break;
                    case "FRONTTIRE":
                        TableMetadata = new FrontTire(db.SpecDBFolderType); break;
                    case "REARTIRE":
                        TableMetadata = new RearTire(db.SpecDBFolderType); break;
                    case "RACINGMODIFY":
                        TableMetadata = new RacingModify(db.SpecDBFolderType); break;
                    case "CHASSIS":
                        TableMetadata = new Chassis(db.SpecDBFolderType); break;
                    case "INTAKE_MANIFOLD":
                        TableMetadata = new IntakeManifold(db.SpecDBFolderType); break;
                    case "LIGHTWEIGHT":
                        TableMetadata = new Lightweight(db.SpecDBFolderType); break;
                    case "LSD":
                        TableMetadata = new Lsd(db.SpecDBFolderType); break;
                    case "MUFFLER":
                        TableMetadata = new Muffler(db.SpecDBFolderType); break;
                    case "NATUNE":
                        TableMetadata = new Natune(db.SpecDBFolderType); break;
                    case "NOS":
                        TableMetadata = new NOS(db.SpecDBFolderType); break;
                    case "PROPELLERSHAFT":
                        TableMetadata = new PropellerShaft(db.SpecDBFolderType); break;
                    case "RACE":
                        TableMetadata = new Race(db.SpecDBFolderType); break;
                    case "STEER":
                        TableMetadata = new Steer(db.SpecDBFolderType); break;
                    case "SUPERCHARGER":
                        TableMetadata = new Supercharger(db.SpecDBFolderType); break;
                    case "SUSPENSION":
                        TableMetadata = new Suspension(db.SpecDBFolderType); break;
                    case "TIRECOMPOUND":
                        TableMetadata = new TireCompound(db.SpecDBFolderType); break;
                    case "TURBINEKIT":
                        TableMetadata = new TurbineKit(db.SpecDBFolderType); break;
                    case "GENERIC_ITEMS":
                        TableMetadata = new GenericItems(db.SpecDBFolderType); break;
                    case "TUNED_CARS":
                        TableMetadata = new TunedCars(db.SpecDBFolderType); break;
                    case "TUNER":
                        TableMetadata = new Tuner(db.SpecDBFolderType); break;
                    case "WHEEL":
                        TableMetadata = new Wheel(db.SpecDBFolderType); break;
                    // Unmapped, but havent seen having rows
                    case "TCSC":
                        TableMetadata = new TCSC(db.SpecDBFolderType); break;
                    case "TIREFORCEVOL":
                        TableMetadata = new TireForceVol(db.SpecDBFolderType); break;
                    case "GENERIC_CAR_INFO":
                        TableMetadata = new GenericCarInfo(db.SpecDBFolderType); break;
                    case "INDEP_THROTTLE":
                        TableMetadata = new IndepThrottle(db.SpecDBFolderType); break;
                    case "INTERCOOLER":
                        TableMetadata = new Intercooler(db.SpecDBFolderType); break;
                    case "PORTPOLISH":
                        TableMetadata = new PortPolish(db.SpecDBFolderType); break;
                    case "WING":
                        TableMetadata = new Wing(db.SpecDBFolderType); break;
                    case "TUNER_LIST":
                        TableMetadata = new TunerList(db.SpecDBFolderType); break;
                    case "TIRESIZE":
                        TableMetadata = new TireSize(db.SpecDBFolderType); break;
                    case "ENEMY_CARS":
                        TableMetadata = new EnemyCars(db.SpecDBFolderType); break;
                    case "ENGINEBALANCE":
                        TableMetadata = new EngineBalance(db.SpecDBFolderType); break;
                    case "RIDER_SET_ASSIGN":
                        TableMetadata = new RiderSetAssign(db.SpecDBFolderType); break;
                    case "RIDER_SET":
                        TableMetadata = new RiderSet(db.SpecDBFolderType); break;
                    default:
                        throw new NotSupportedException($"This table ({TableName}) is not yet mapped.");
                }

                for (int i = 0; i < TableMetadata.Columns.Count; i++)
                    TableMetadata.Columns[i].ColumnIndex = i;
            }
        }

        private void LoadAllRowData()
        {
            Rows = new ObservableCollection<SpecDBRowData>();
            for (int i = 0; i < Keys.Count; i++)
            {
                RowData key = Keys[i];
                GetRowN(i, out Span<byte> rowData);
                var result = TableMetadata.ReadRow(rowData, DBT.Endian);
                result.Row.ID = key.Id;
                result.Row.Label = key.Label;
                Rows.Add(result.Row);

                IsTableProperlyMapped = result.ReadAll;
            }
        }

        private void PopulateRowStringsIfNeeded(SpecDB db)
        {
            foreach (var row in Rows)
            {
                foreach (var dataType in row.ColumnData)
                {
                    if (dataType is DBString str)
                    {
                        // Lazy load
                        if (!db.StringDatabases.TryGetValue(str.FileName, out StringDatabase strDb))
                        {
                            var newStrDb = StringDatabase.LoadFromFile(Path.Combine(db.FolderName, str.FileName));
                            db.StringDatabases.Add(str.FileName, newStrDb);
                            strDb = newStrDb;
                        }

                        str.Value = strDb.Strings[str.StringIndex];
                    }
                }
            }
        }
    }
}
