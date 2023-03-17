using System.Net;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using GT.Models;
using PDTools.SimulatorInterface;

namespace GT;

public partial class MainPage
{
    private readonly DataPageModel _model;
    private SimulatorGt7 _simulatorGt;
    private CancellationTokenSource _token;

    public MainPage()
    {
        InitializeComponent();
        _token = new CancellationTokenSource();
        _model = new DataPageModel();

        BindingContext = _model;
    }

    private void Run()
    {
        _model.CanStart = false;
        _model.CanStop = true;
        if (_token.IsCancellationRequested)
            _token = new CancellationTokenSource();

        Task.Run(async () =>
        {
            while (!_token.IsCancellationRequested)
            {
                SimulatorPacket data = SimulatorGt7.GetDataTest();
                //_model.DataPacket = await _simulatorGt.GetData(_token.Token);

                _model.DataPacket = data;

                await Task.Delay(10);
            }
        }, _token.Token);
    }

    private static bool ValidateIPv4(string ipString)
    {
        return ipString.Count(c => c == '.') == 3 && IPAddress.TryParse(ipString, out _);
    }

    private async void OnGetDataStart(object sender, EventArgs e)
    {
        if (!ValidateIPv4(_model.Ip ?? string.Empty))
        {
            var toast = Toast.Make("Invalid IP Address", ToastDuration.Long);
            await toast.Show(new CancellationTokenSource().Token);
 
            return;
        }

        _simulatorGt = new SimulatorGt7(_model.Ip);

        Run();
    }

    private void OnGetDataStop(object sender, EventArgs e)
    {
        if (_token.IsCancellationRequested)
            return;

        _token.Cancel();
        _model.CanStart = true;
        _model.CanStop = false;
    }
}