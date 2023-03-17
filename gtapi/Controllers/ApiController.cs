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
        var gtsport = false;
        var gt6 = false;


        var type = SimulatorInterfaceGameType.GT7;
        if (gtsport)
            type = SimulatorInterfaceGameType.GTSport;
        else if (gt6)
            type = SimulatorInterfaceGameType.GT6;

        SimulatorInterfaceClient simInterface = new SimulatorInterfaceClient(ip, type);

        var cancellationToken = new CancellationTokenSource();
       
        try
        {
            await simInterface.Start(cancellationToken.Token, false);
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