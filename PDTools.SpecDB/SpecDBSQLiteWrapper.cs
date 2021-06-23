using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.Data.Sqlite;

namespace GTOnlineDiscordBot.Services.SpecDB
{
    public class SpecDBSQLiteWrapper : ISpecDB
    {
        private SqliteConnection _conn;
        public Task InitializeAsync(string fileName, CancellationTokenSource cancellationTokenSrc)
        {
            _conn = new SqliteConnection($"Data Source={fileName}");
            return _conn.OpenAsync(cancellationTokenSrc.Token);
        }

        public async Task GetCarRowByCodeAsync(int code, CancellationTokenSource cancellationTokenSrc)
        {
            _conn.Open();

            var com = _conn.CreateCommand();
            com.CommandText = "SELECT * FROM GENERIC_CAR WHERE id = $id";
            com.Parameters.AddWithValue("$code", code);

            using var reader = await com.ExecuteReaderAsync(cancellationTokenSrc.Token);
            while (reader.Read())
            {
               
            }
        }

        public async Task GetCarRowByLabelAsync(string label, CancellationTokenSource cancellationTokenSrc)
        {
            _conn.Open();

            var com = _conn.CreateCommand();
            com.CommandText = "SELECT * FROM GENERIC_CAR WHERE label = $label";
            com.Parameters.AddWithValue("$label", label);

            using var reader = await com.ExecuteReaderAsync(cancellationTokenSrc.Token);
            while (reader.Read())
            {
                
            }
        }

        public async Task GetRowAsync(string table, int code, CancellationTokenSource cancellationTokenSrc)
        {
            _conn.Open();

            var com = _conn.CreateCommand();
            com.CommandText = "SELECT * FROM $table WHERE id = $id";
            com.Parameters.AddWithValue("$table", table);
            com.Parameters.AddWithValue("$id", code);

            using var reader = await com.ExecuteReaderAsync(cancellationTokenSrc.Token);
            while (reader.Read())
            {

            }
        }
    }
}
