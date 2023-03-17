using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT;

public partial class DataPage : ContentPage
{
    private readonly SimulatorGt7 _simulatorGt;

    public DataPage(string ip)
    {
        InitializeComponent();
        _simulatorGt = new SimulatorGt7(ip);

        Run();
    }

    private void Run()
    {
        Task.Run(async () =>
        {
            var data = await _simulatorGt.GetData();

            Dispatcher.Dispatch(() =>
            {
                lbBrake.Text = data.Brake.ToString();
                lbThrottle.Text = data.Throttle.ToString();
                lbBodyHeight.Text = data.BodyHeight.ToString("F2");
                lbCarCode.Text = data.CarCode.ToString();
                lbOilTemperature.Text = data.OilTemperature.ToString("F2");
                lbOilPressure.Text = data.OilPressure.ToString("F2");
                lbCurrentGear.Text = data.CurrentGear.ToString("F2");
                lbGasCapacity.Text = data.GasCapacity.ToString("F2");
                lbCarCode.Text = data.CarCode.ToString("F2");
                lbGearRatios.Text = data.GearRatios.ToString();
                lbBestLapTime.Text = data.BestLapTime.ToString();
                lbGasLevel.Text = data.GasLevel.ToString("F2");

                lbFlSurfaceTemperature.Text = data.TireSurfaceTemperature.FlSurfaceTemperature.ToString("F2");
                lbFrSurfaceTemperature.Text = data.TireSurfaceTemperature.FrSurfaceTemperature.ToString("F2");
                lbRlSurfaceTemperature.Text = data.TireSurfaceTemperature.RlSurfaceTemperature.ToString("F2");
                lbRrSurfaceTemperature.Text = data.TireSurfaceTemperature.RrSurfaceTemperature.ToString("F2");
            });

            await Task.Delay(500);
        });
    }
}