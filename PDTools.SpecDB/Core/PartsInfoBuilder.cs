using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;
using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

using PDTools.Utils;

using PDTools.SpecDB.Core.Mapping.Types;
using PDTools.SpecDB.Core.Formats;
using PDTools.SpecDB.Core.Mapping;

namespace PDTools.SpecDB.Core
{
    /// <summary>
    /// Parts Info links all the equippable parts's rows that a car can use.
    /// </summary>
    public class PartsInfoBuilder
    {
        /// <summary>
        /// Write file information for games that uses PartsInfo.tbd/tbi.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="folder"></param>
        public static void WritePartsInformationNew(SpecDB db, IProgress<(int, string)> progress, string folder)
        {
            var carTable = db.Tables["GENERIC_CAR"];
            if (!carTable.IsLoaded)
                carTable.LoadAllRows(db);

            var defaultParts = db.Tables["DEFAULT_PARTS"];
            if (!defaultParts.IsLoaded)
                defaultParts.LoadAllRows(db);

            // TBI and TBD are both linked. We need to save them simultaneously
            using (var tbdWriter = new BinaryStream(new FileStream(Path.Combine(folder, "PartsInfo.tbd"), FileMode.Create)))
            using (var tbiWriter = new BinaryStream(new FileStream(Path.Combine(folder, "PartsInfo.tbi"), FileMode.Create)))
            {
                tbdWriter.ByteConverter = carTable.BigEndian ? ByteConverter.Big : ByteConverter.Little;
                tbiWriter.ByteConverter = tbdWriter.ByteConverter;

                int totalEntriesWriten = 0;

                // We need to iterate through all the cars to save all of their linked parts
                for (int i = 0; i < carTable.Rows.Count; i++)
                {
                    // Begin to write the stride index
                    int fieldCountOffset = (int)tbdWriter.Position;
                    int fieldsWritten = 0;
                    tbdWriter.Position += 4;
                    tbdWriter.Align(0x08, true);

                    RowData car = carTable.Rows[i];
                    progress.Report((i, car.Label));
                    int defaultPartsID = (car.ColumnData[1] as DBInt).Value;
                    RowData df = defaultParts.Rows.FirstOrDefault(e => e.ID == defaultPartsID);

                    // Iterate through each part table
                    int lastTableID = 0;
                    for (int j = 0; j < df.ColumnData.Count; j += 2)
                    {
                        int tableID = (df.ColumnData[j] as DBInt).Value;
                        if (tableID > 32)
                            continue;

                        // When the row is -1 for its table id, use a dirty trick and use the last table ID
                        if (tableID == -1)
                            tableID = lastTableID + 1;
                        lastTableID = tableID;

                        if (lastTableID >= 32)
                            break; // We are done pretty much

                        int partRowID = (df.ColumnData[j + 1] as DBInt).Value;

                        // Get our table by said ID
                        Table partTable = db.Tables.Values.FirstOrDefault(table => table.TableID == tableID);
                        if (partTable is null)
                            throw new Exception($"Table ID {tableID} is missing from the SpecDB. Ensure that your SpecDB is complete.");

                        if (!partTable.IsLoaded)
                            partTable.LoadAllRows(db);

                        // Ignored tables, these may contain data but they are not kept in mind
                        if (partTable.TableName == "NOS")
                            continue;

                        // First or default - avoid linq here for perf
                        RowData mainRow = null;
                        for (int k = 0; k < partTable.Rows.Count; k++)
                        {
                            if (partTable.Rows[k].ID == partRowID)
                            {
                                mainRow = partTable.Rows[k];
                                break;
                            }
                        }

                        // Sometimes the main ID points to a generic part. We have to include it if its not a regular car label.
                        if (partRowID != -1 && mainRow != null && !mainRow.Label.Contains(car.Label))
                        {
                            var colMeta = partTable.TableMetadata.GetCategoryColumn();
                            int cat = (mainRow.ColumnData[colMeta.ColumnIndex] as DBByte).Value;

                            WriteTBDCarInfoPart(tbdWriter, mainRow, tableID, cat);
                            fieldsWritten++;
                        }

                        // Register all alt parts for it.
                        string rowFilter = $"{partTable.TableMetadata.LabelPrefix}{car.Label}";
                        foreach (RowData partRow in partTable.Rows)
                        {
                            // PD's messups
                            if (car.Label.Equals("mazda6_tmv_01") && partRow.Label.Contains("_azda6_tmv_01")
                                    || car.Label.Equals("mazda6_sport_23z_03") && partRow.Label.StartsWith("fw-")
                                    || car.Label.Equals("gs300_vertex_us_00") && partRow.Label.Contains("_s300_vertex_us")
                                    || car.Label.Equals("_350z_gt4_05") && partRow.Label.StartsWith("sc_350z_gt4_05")
                                    || car.Label.Equals("car_color_sample") && partRow.Label.StartsWith("ge_x1_v_10")
                                    || partRow.Label.StartsWith(rowFilter))
                            {
                                if (partRow.Label.CountCharOccurenceFromIndex(rowFilter.Length, '_') > 1) // Assume its a different car which starts with the same label
                                    continue;

                                var colMeta = partTable.TableMetadata.GetCategoryColumn();
                                int cat = (partRow.ColumnData[colMeta.ColumnIndex] as DBByte).Value;

                                // Apparently NATUNE Stage 0 is ignored
                                if (partTable.TableName == "NATUNE" && cat == 0)
                                    continue;

                                WriteTBDCarInfoPart(tbdWriter, partRow, tableID, cat);
                                fieldsWritten++;
                            }
                        }
                    }

                    if (fieldsWritten > 0)
                    {
                        // We are done writing the fields (if any), write the TBI metadata now
                        tbiWriter.Position = TBI_PartsInfo.HeaderSize + totalEntriesWriten * 0x10;
                        tbiWriter.WriteInt32(car.ID);
                        tbiWriter.WriteInt32(fieldCountOffset); // Data Start
                        tbiWriter.WriteInt32((int)tbdWriter.Position - fieldCountOffset); // Data Length
                        tbiWriter.Align(0x08, grow: true);

                        // Write the TBD entry field count
                        using (Seek seek = tbdWriter.TemporarySeek(fieldCountOffset, SeekOrigin.Begin))
                            tbdWriter.WriteInt32(fieldsWritten);

                        totalEntriesWriten++;
                    }
                }

                // Finish up the TBI header
                tbiWriter.Position = 0;
                tbiWriter.WriteString("GTST", StringCoding.Raw);
                tbiWriter.WriteBoolean(carTable.BigEndian);
                tbiWriter.Position = 8;
                tbiWriter.WriteInt32(2); // Version
                tbiWriter.Position += 4;
                tbiWriter.WriteInt32(-1);
                tbiWriter.WriteInt32(totalEntriesWriten);
                tbiWriter.WriteUInt32(3064);
                tbiWriter.WriteInt32(db.Version);
            }
        }

        private static void WriteTBDCarInfoPart(BinaryStream tbdWriter, RowData partRow, int tableID, int partCategory)
        {
            tbdWriter.WriteInt32(tableID);
            tbdWriter.WriteInt32(partRow.ID);
            tbdWriter.WriteInt32(partCategory);
            tbdWriter.Align(0x08, true);
        }

        /// <summary>
        /// Write file information for games that uses PartsInfo.tbd/tbi.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="folder"></param>
        public static void WritePartsInformationOld(SpecDB db, IProgress<(int, string)> progress, string folder)
        {
            var carTable = db.Tables["GENERIC_CAR"];
            if (!carTable.IsLoaded)
                carTable.LoadAllRows(db);

            var defaultParts = db.Tables["DEFAULT_PARTS"];
            if (!defaultParts.IsLoaded)
                defaultParts.LoadAllRows(db);

            for (int i = 0; i < carTable.Rows.Count; i++)
            {
                RowData car = carTable.Rows[i];
                // TableIndex - Row 
                // 00000001     00000064
                string fileName = string.Format("{0:X8}{1:X8}", carTable.TableID, car.ID);
                using (var partInfoWriter = new BinaryStream(new FileStream(Path.Combine(folder, fileName), FileMode.Create)))
                {
                    partInfoWriter.Position = 0x08;
                    int fieldsWriten = 0;

                    progress.Report((i, $"{car.Label} - {fileName}"));

                    var carDfID = car.ColumnData[0] as DBInt;
                    var df = defaultParts.Rows.FirstOrDefault(e => e.ID == carDfID.Value);

                    if (df is null)
                        throw new Exception($"Car ({car.Label}/{car.ID}) has a missing default parts row.");

                    int lastTableID = 0;
                    for (int j = 0; j < df.ColumnData.Count + 1; j += 2)
                    {
                        // Skip Rear Tire to NOS
                        int tableID = j / 2 == 26 ? 27 : (df.ColumnData[j + 1] as DBInt).Value;
                        int partRowID = j / 2 == 26 ? -1 : (df.ColumnData[j] as DBInt).Value;

                        if (tableID == -1)
                            tableID = lastTableID + 1;
                        lastTableID = tableID;

                        // Get our table by said ID
                        Table partTable = db.Tables.Values.FirstOrDefault(table => table.TableID == tableID);
                        if (partTable is null)
                            throw new Exception($"Table ID {tableID} is missing from the SpecDB. Ensure that your SpecDB is complete.");

                        if (!partTable.IsLoaded)
                            partTable.LoadAllRows(db);

                        // First or default - avoid linq here for perf
                        RowData mainRow = null;
                        for (int k = 0; k < partTable.Rows.Count; k++)
                        {
                            if (partTable.Rows[k].ID == partRowID)
                            {
                                mainRow = partTable.Rows[k];
                                break;
                            }
                        }

                        // Sometimes the main ID points to a generic part. We have to include it if its not a regular car label.
                        if (partRowID != -1 && mainRow != null && !mainRow.Label.Contains(car.Label))
                        {
                            var colMeta = partTable.TableMetadata.GetCategoryColumn();
                            int cat = (mainRow.ColumnData[colMeta.ColumnIndex] as DBByte).Value;

                            WriteCarInfoPart(partInfoWriter, mainRow, tableID, cat);
                            fieldsWriten++;
                        }

                        // Register all alt parts for it.
                        string rowFilter = $"{partTable.TableMetadata.LabelPrefix}{car.Label}";
                        foreach (RowData partRow in partTable.Rows)
                        {
                            // PD's messups
                            if (IsPDMistakeLabel(car.Label, partRow) || partRow.Label.StartsWith(rowFilter))
                            {
                                if (partRow.Label.CountCharOccurenceFromIndex(rowFilter.Length, '_') > 1) // Assume its a different car which starts with the same label
                                    continue;

                                var colMeta = partTable.TableMetadata.GetCategoryColumn();
                                int cat = (partRow.ColumnData[colMeta.ColumnIndex] as DBByte).Value;

                                // Apparently NATUNE Stage 0 is ignored - or is it?
                                //if (partTable.TableName == "NATUNE" && cat == 0)
                                //    continue;

                                WriteCarInfoPart(partInfoWriter, partRow, tableID, cat);
                                fieldsWriten++;
                            }
                        }

                    }

                    // Do SUPERCHARGER which is NOT in DEFAULT_PARTS in GT4
                    var sc = db.Tables["SUPERCHARGER"];
                    if (!sc.IsLoaded)
                        sc.LoadAllRows(db);

                    string scLabel = $"sc_{car.Label}_b";
                    int id = sc.GetIDOfLabel(scLabel);
                    if (id == -1)
                    {
                        scLabel = $"sc{car.Label}_b"; // Try this against mistakes like 'sc_350z_gt4_05_b' with one underscore instead of two
                        id = sc.GetIDOfLabel(scLabel);
                    }

                    if (id != -1)
                    {
                        var scRow = sc.Rows.FirstOrDefault(e => e.Label == scLabel);
                        if (scRow != null)
                        {
                            var colMeta = sc.TableMetadata.GetCategoryColumn();
                            int cat = (scRow.ColumnData[colMeta.ColumnIndex] as DBByte).Value;

                            WriteCarInfoPart(partInfoWriter, scRow, sc.TableID, cat);
                            fieldsWriten++;
                        }
                    }

                    // Finish header
                    partInfoWriter.Position = 0x00;
                    partInfoWriter.WriteInt32(fieldsWriten);
                }

            }
        }

        private static void WriteCarInfoPart(BinaryStream writer, RowData row, int tableID, int category)
        {
            writer.WriteInt32(row.ID);
            writer.WriteInt32(tableID);
            writer.WriteInt32(category);
            writer.Align(0x08, true);
        }

        private static bool IsPDMistakeLabel(string carLabel, RowData partRow)
        {
            if (carLabel.Equals("mazda6_tmv_01") && partRow.Label.Contains("_azda6_tmv_01")
                 || carLabel.Equals("mazda6_sport_23z_03") && partRow.Label.StartsWith("fw-")
                 || carLabel.Equals("gs300_vertex_us_00") && partRow.Label.Contains("_s300_vertex_us")
                 || carLabel.Equals("_350z_gt4_05") && partRow.Label.StartsWith("sc_350z_gt4_05")
                 || carLabel.Equals("car_color_sample") && partRow.Label.StartsWith("ge_x1_v_10"))
                return true;
            return false;
        }
    }
}
