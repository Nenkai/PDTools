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

        var task = simInterface.Start(cts.Token, false);

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

        GranTurismoData data = new GranTurismoData()
        {
            OilTemperature = simInterface.Packet.OilTemperature,
            OilPressure = simInterface.Packet.OilPressure,
            CurrentGear = simInterface.Packet.CurrentGear,
            GasCapacity = simInterface.Packet.GasCapacity,
            GearRatios = simInterface.Packet.GearRatios,
            CarCode = simInterface.Packet.CarCode,
            BodyHeight = simInterface.Packet.BodyHeight,
            Brake = simInterface.Packet.Brake,
            Throttle = simInterface.Packet.Throttle,
            GasLevel = simInterface.Packet.GasLevel,
            BestLapTime = simInterface.Packet.BestLapTime,
        };

        data.TireSurfaceTemperature = new TireSurfaceData()
        {
            FL_Surface_Temperature = simInterface.Packet.TireFL_SurfaceTemperature,
            FR_Surface_Temperature = simInterface.Packet.TireFR_SurfaceTemperature,
            RL_Surface_Temperature = simInterface.Packet.TireRL_SurfaceTemperature,
            RR_Surface_Temperature = simInterface.Packet.TireRR_SurfaceTemperature,
        };
        
        return Json(data);
    }

}