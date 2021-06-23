using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using SpecDBOld;
using SpecDBOld.Core;

namespace GTOnlineDiscordBot.Services.SpecDB
{
    public class SpecDBOldWrapper : ISpecDB
    {
        public SpecDBOld.SpecDB Database { get; set; }
        public SpecDBFolder FolderType { get; set; }

        public SpecDBOldWrapper(SpecDBFolder folderType)
        {
            FolderType = folderType;
        }

        public Task InitializeAsync(string fileName, CancellationTokenSource cancellationTokenSrc)
        {
            Database = SpecDBOld.SpecDB.LoadFromSpecDBFolder(fileName, FolderType, true);
            return Task.CompletedTask;
        }

        public Task GetCarRowByCodeAsync(int code, CancellationTokenSource cancellationTokenSrc)
        {
            Database.GetRowFromTable(SpecDBOld.SpecDB.SpecDBTables.GENERIC_CAR, code, out Span<byte> rowData);

            return Task.CompletedTask;
        }

        public Task GetCarRowByLabelAsync(string label, CancellationTokenSource cancellationTokenSrc)
        {
            int code = Database.Tables["GENERIC_CAR"].GetIDOfLabel(label);
            Database.Tables["GENERIC_CAR"].GetRowN(code, out Span<byte> rowData);

            return Task.CompletedTask;
        }

        public Task GetRowAsync(string table, int code, CancellationTokenSource cancellationTokenSrc)
        {
            if (!Database.Tables.TryGetValue(table, out SpecDBTable specTable))
                return Task.CompletedTask;

            specTable.GetRowN(code, out Span<byte> rowData);

            return Task.CompletedTask;
        }
    }
}
