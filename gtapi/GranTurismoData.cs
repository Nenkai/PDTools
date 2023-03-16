namespace gtapi;

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
    
    
}

public class TireSurfaceData {
    public float FR_Surface_Temperature  { get; set; }
    public float FL_Surface_Temperature  { get; set; }
    public float RR_Surface_Temperature  { get; set; }
    public float RL_Surface_Temperature  { get; set; }
}