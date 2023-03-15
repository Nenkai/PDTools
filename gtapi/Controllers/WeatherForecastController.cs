using Microsoft.AspNetCore.Mvc;
using PDTools.SimulatorInterface;

namespace gtapi.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : Controller
{
    public ApiController()
    {
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<JsonResult> Get(string ip)
    {
        /* Mostly a test sample for using the Simulator Interface library */

        var _showUnknown = true;
        bool gtsport = false;
        bool gt6 = false;
    

        SimulatorInterfaceGameType type = SimulatorInterfaceGameType.GT7;
        if (gtsport)
            type = SimulatorInterfaceGameType.GTSport;
        else if (gt6)
            type = SimulatorInterfaceGameType.GT6;
        

        SimulatorInterfaceClient simInterface = new SimulatorInterfaceClient(ip, type);

        var cts = new CancellationTokenSource();

        // Cancel token from outside source to end simulator

        var task = simInterface.Start(cts.Token, true);

        try
        {
            await task;
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine($"Simulator Interface ending..");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Errored during simulation: {e.Message}");
        }
        finally
        {
            // Important to clear up underlaying socket
            simInterface.Dispose();
        }

        return Json(simInterface.Packet);
    }

}