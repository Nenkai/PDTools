using System.Net;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace GT;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnClicked(object sender, EventArgs e)
    {
        var ip = tbIp.Text;
        if (!ValidateIPv4(ip))
        {
            var toast = Toast.Make("Invalid IP Address", ToastDuration.Long);
            await toast.Show(new CancellationTokenSource().Token);
        }

        await Navigation.PushAsync(new DataPage(ip));
    }

    private static bool ValidateIPv4(string ipString)
    {
        return ipString.Count(c => c == '.') == 3 && IPAddress.TryParse(ipString, out _);
    }
}