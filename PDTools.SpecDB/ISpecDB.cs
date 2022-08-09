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
        Task InitializeAsync(string path, CancellationTokenSource cancellationTokenSrc);

        Task GetCarRowByCodeAsync(int code, CancellationTokenSource cancellationTokenSrc);
        Task GetCarRowByLabelAsync(string label, CancellationTokenSource cancellationTokenSrc);

        Task GetRowAsync(string table, int code, CancellationTokenSource cancellationTokenSrc);
    }

    public enum SpecDBKind
    {
        Old,
        SQLite
    }
}
