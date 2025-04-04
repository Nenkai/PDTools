﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Net;
using System.Text.Json;
using System.Threading;
using PDTools.LiveTimingApi.Entities;

namespace PDTools.LiveTimingApi;

public class LiveTimingClient : IDisposable
{
    private string _host;
    private ClientWebSocket _client;

    public const int BufferSize = 0x2000;

    public LiveTimingClient(string host)
    {
        _host = host;
    }

    public delegate void LiveTimingRaceStateDelegate(LiveTimingRaceState raceState);

    /// <summary>
    /// Fired when a race state was updated.
    /// </summary>
    public event LiveTimingRaceStateDelegate OnRaceStateUpdate;

    public delegate void LiveTimingRaceInfoDelegate(LiveTimingRaceInfo raceInfo);

    /// <summary>
    /// Fired when a race information was updated.
    /// </summary>
    public event LiveTimingRaceInfoDelegate OnRaceInfoUpdate;

    public delegate void LiveTimingEntryDelegate(LiveTimingEntry entry);

    /// <summary>
    /// Fired when an entry was updated.
    /// </summary>
    public event LiveTimingEntryDelegate OnEntryUpdate;

    public delegate void LiveTimingConditionDelegate(LiveTimingCondition condition);

    /// <summary>
    /// Fired when course conditions were updated.
    /// </summary>
    public event LiveTimingConditionDelegate OnConditionUpdate;

    public delegate void LiveTimingConsumeStateDelegate(LiveTimingConsumeState condition);

    /// <summary>
    /// Fired when an entry consumption state was updated.
    /// </summary>
    public event LiveTimingConsumeStateDelegate OnConsumeStateUpdate;

    /// <summary>
    /// Fired when an entry's best lap was updated.
    /// </summary>
    public event LiveTimingEntryDelegate OnBestLapEntryUpdate;

    public delegate void LiveTimingJsonPayloadDelegate(JsonDocument payload);

    /// <summary>
    /// Fired when any Json payload is received for more manual control.
    /// </summary>
    public event LiveTimingJsonPayloadDelegate OnJsonPayloadReceived;

    public async Task Start(CancellationToken token = default)
    {
        _client = new ClientWebSocket();
        await _client.ConnectAsync(new Uri($"ws://{_host}/livetimingapi/"), token);

        Memory<byte> buffer = new byte[BufferSize];
        await _client.SendAsync(Array.Empty<byte>(), WebSocketMessageType.Text, false, token);

        while (true)
        {
            ValueWebSocketReceiveResult r = await _client.ReceiveAsync(buffer, token);
            if (_client.CloseStatus.HasValue)
            {
                Console.WriteLine($"Client closed ({_client.CloseStatus}).");
                break;
            }

            JsonDocument document = JsonDocument.Parse(buffer.Slice(0, r.Count));
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            if (document.RootElement.TryGetProperty("RaceInfo", out JsonElement raceInfoProp))
            {
                var raceInfo = raceInfoProp.Deserialize<LiveTimingRaceInfo>();
                this.OnRaceInfoUpdate?.Invoke(raceInfo);
            }
            else if (document.RootElement.TryGetProperty("RaceState", out JsonElement raceStateProp))
            {
                var raceState = raceStateProp.Deserialize<LiveTimingRaceState>();
                this.OnRaceStateUpdate?.Invoke(raceState);
            }
            else if (document.RootElement.TryGetProperty("Entry", out JsonElement entryProp))
            {
                var entry = entryProp.Deserialize<LiveTimingEntry>();
                this.OnEntryUpdate?.Invoke(entry);
            }
            else if (document.RootElement.TryGetProperty("Condition", out JsonElement conditionProp))
            {
                var condition = conditionProp.Deserialize<LiveTimingCondition>();
                this.OnConditionUpdate?.Invoke(condition);
            }
            else if (document.RootElement.TryGetProperty("ConsumeState", out JsonElement consumeStateProp))
            {
                var consumeState = consumeStateProp.Deserialize<LiveTimingConsumeState>();
                this.OnConsumeStateUpdate?.Invoke(consumeState);
            }
            else if (document.RootElement.TryGetProperty("BestlapEntry", out JsonElement bestLapEntryProp))
            {
                var bestLapEntry = bestLapEntryProp.Deserialize<LiveTimingEntry>();
                this.OnBestLapEntryUpdate?.Invoke(bestLapEntry);
            }
            else
            {
                throw new Exception("Unsupported property");
            }

            this.OnJsonPayloadReceived?.Invoke(document);
        }
    }

    public void Dispose()
    {
        _client.Dispose();
        _client = null;

        GC.SuppressFinalize(this);
    }
}
