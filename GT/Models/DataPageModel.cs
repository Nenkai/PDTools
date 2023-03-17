using System.ComponentModel;
using System.Runtime.CompilerServices;
using PDTools.SimulatorInterface;

namespace GT.Models;

public sealed class DataPageModel : INotifyPropertyChanged
{
    private SimulatorPacket _dataPacket;
    private bool _canStart;
    private bool _canStop;
    private string _ip;

    public SimulatorPacket DataPacket
    {
        get => _dataPacket;
        set
        {
            _dataPacket = value;
            OnPropertyChanged();
        }
    }

    public bool CanStart
    {
        get => _canStart;
        set
        {
            _canStart = value;
            OnPropertyChanged();
        }
    }

    public bool CanStop
    {
        get => _canStop;
        set
        {
            _canStop = value;
            OnPropertyChanged();
        }
    }

    public string Ip
    {
        get => _ip;
        set
        {
            _ip = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public DataPageModel()
    {
        CanStart = true;
        CanStop = false;
        DataPacket = new SimulatorPacket();
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}