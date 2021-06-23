using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using SpecDBOld;

namespace GTOnlineDiscordBot.Services
{
    public interface ISpecDB
    {
        public Task InitializeAsync(string path, CancellationTokenSource cancellationTokenSrc);

        public Task GetCarRowByCodeAsync(int code, CancellationTokenSource cancellationTokenSrc);
        public Task GetCarRowByLabelAsync(string label, CancellationTokenSource cancellationTokenSrc);

        public Task GetRowAsync(string table, int code, CancellationTokenSource cancellationTokenSrc);
    }

    public enum SpecDBKind
    {
        Old,
        SQLite
    }
}
