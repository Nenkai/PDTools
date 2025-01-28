using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Net;
using System.Text.Json;

using PDTools.LiveTimingApi;
using PDTools.LiveTimingApi.Entities;

namespace PDTools.LiveTimingApiTestTool;

internal class Program
{
    static async Task Main(string[] args)
    {
        /* Mostly a test sample for using the LiveTiming API 
         * This service is only enabled if
            - The version build is "debug"
            - "stagelink" command line argument is set
            - GTGame.UserData.OptionData.Get().IsLANMode is set (from Option settings, not available to plebs)
            - HasPermissionToHostLiveEvents is set on the online account
            - HasPermissionToSpectateLiveEvents is set on the online account
        */

        Console.WriteLine("LiveTimingApi GT7 - by Nenkai");
        Console.WriteLine();

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: LiveTimingApiTest.exe <host of PS4/PS5 i.e \"192.168.0.23:8888\">");
            Console.WriteLine("NOTE: Requires LiveTimingApi Websocket service active on the console! This is only for those who know what they are doing!");
            return;
        }

        Console.WriteLine("Starting client..");

        LiveTimingClient liveTimingClient = new LiveTimingClient(args[0]);
        liveTimingClient.OnBestLapEntryUpdate += LiveTimingClient_OnBestLapEntryUpdate;
        liveTimingClient.OnConditionUpdate += LiveTimingClient_OnConditionUpdate;
        liveTimingClient.OnConsumeStateUpdate += LiveTimingClient_OnConsumeStateUpdate;
        liveTimingClient.OnEntryUpdate += LiveTimingClient_OnEntryUpdate;
        liveTimingClient.OnRaceInfoUpdate += LiveTimingClient_OnRaceInfoUpdate;
        liveTimingClient.OnRaceStateUpdate += LiveTimingClient_OnRaceStateUpdate;

        liveTimingClient.OnJsonPayloadReceived += LiveTimingClient_OnJsonPayloadReceived;
        var cts = new CancellationTokenSource();

        // Cancel token from outside source to end client

        var task = liveTimingClient.Start(cts.Token);

        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Live Timing API client ending..");
        }
        catch (WebSocketException webSocketException)
        {
            if (webSocketException.Message.Contains("404"))
                Console.WriteLine("Error: 404 from endpoint - might not be in a race");

            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Errored during live timing: {e.Message}");
        }
        finally
        {
            // Important to clear up underlaying socket
            liveTimingClient.Dispose();
        }
    }

    private static void LiveTimingClient_OnJsonPayloadReceived(JsonDocument payload)
    {
        // Will print everything as debug
        Console.WriteLine(payload.RootElement);
    }

    private static void LiveTimingClient_OnRaceStateUpdate(LiveTimingRaceState raceState)
    {

    }

    private static void LiveTimingClient_OnRaceInfoUpdate(LiveTimingRaceInfo raceInfo)
    {

    }

    private static void LiveTimingClient_OnEntryUpdate(LiveTimingEntry entry)
    {

    }

    private static void LiveTimingClient_OnConsumeStateUpdate(LiveTimingConsumeState condition)
    {

    }

    private static void LiveTimingClient_OnConditionUpdate(LiveTimingCondition condition)
    {

    }

    private static void LiveTimingClient_OnBestLapEntryUpdate(LiveTimingEntry entry)
    {

    }
}