using PDTools.SimulatorInterface;

namespace GT;

public class GranTurismoData
{
    public float GasLevel { get; set; }

    public byte Throttle { get; set; }

    public byte Brake { get; set; }

    public float BodyHeight { get; set; }

    public int CarCode { get; set; }

    public float[] GearRatios { get; set; }

    public float GasCapacity { get; set; }

    public float OilPressure { get; set; }

    public byte CurrentGear { get; set; }

    public float OilTemperature { get; set; }

    public TimeSpan BestLapTime { get; set; }

    public TireSurfaceData TireSurfaceTemperature { get; set; }

    public GranTurismoData()
    {
    }

    public GranTurismoData(SimulatorPacket packet)
    {
        OilTemperature = packet.OilTemperature;
        OilPressure = packet.OilPressure;
        CurrentGear = packet.CurrentGear;
        GasCapacity = packet.GasCapacity;
        GearRatios = packet.GearRatios;
        CarCode = packet.CarCode;
        BodyHeight = packet.BodyHeight;
        Brake = packet.Brake;
        Throttle = packet.Throttle;
        GasLevel = packet.GasLevel;
        BestLapTime = packet.BestLapTime;
        TireSurfaceTemperature = new TireSurfaceData()
        {
            FlSurfaceTemperature = packet.TireFL_SurfaceTemperature,
            FrSurfaceTemperature = packet.TireFR_SurfaceTemperature,
            RlSurfaceTemperature = packet.TireRL_SurfaceTemperature,
            RrSurfaceTemperature = packet.TireRR_SurfaceTemperature
        };
    }
}

public class TireSurfaceData
{
    public float FrSurfaceTemperature { get; set; }
    public float FlSurfaceTemperature { get; set; }
    public float RrSurfaceTemperature { get; set; }
    public float RlSurfaceTemperature { get; set; }
}