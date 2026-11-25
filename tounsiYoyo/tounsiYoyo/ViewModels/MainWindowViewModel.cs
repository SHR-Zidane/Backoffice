using System.Globalization;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using tounsiYoyo.Model;
using tounsiYoyo.Services;

namespace tounsiYoyo.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IAsyncDisposable
{
    private readonly MqttTimeService _mqttTimeService;

    [ObservableProperty]
    private string formattedDate = "Heure non chargée";

    [ObservableProperty]
    private string shopStatus = string.Empty;

    [ObservableProperty]
    private string connectionStatus = "Non connecté";

    [ObservableProperty]
    private bool isConnected;

    public MainWindowViewModel(MqttTimeService mqttTimeService)
    {
        _mqttTimeService = mqttTimeService;
        _mqttTimeService.TimeReceived += OnTimeReceived;
        _mqttTimeService.StatusChanged += OnStatusChanged;
    }

    [RelayCommand]
    private async Task ConnectAsync()
    {
        try
        {
            await _mqttTimeService.ConnectAsync();
            IsConnected = true;
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Échec MQTT : {ex.Message}";
            IsConnected = false;
        }
    }

    [RelayCommand]
    private async Task DisconnectAsync()
    {
        try
        {
            await _mqttTimeService.DisconnectAsync();
            ConnectionStatus = "Déconnecté du broker.";
            IsConnected = false;
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Échec de la déconnexion MQTT : {ex.Message}";
        }
    }

    public void ShowDate(DateTimeOffset dateTime)
    {
        DateTime localDateTime = dateTime.LocalDateTime;
        FormattedDate = localDateTime.ToString("HH:mm:ss dd/MM/yyyy", CultureInfo.InvariantCulture);
        ShopStatus = OpeningHours.IsOpen(localDateTime) ? "Ouvert" : "Fermé";
    }

    private void OnTimeReceived(DateTimeOffset dateTime)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ShowDate(dateTime);
            ConnectionStatus = "Heure synchronisée depuis le broker.";
        });
    }

    private void OnStatusChanged(string status)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ConnectionStatus = status;
            IsConnected = _mqttTimeService.IsConnected;
        });
    }

    public async ValueTask DisposeAsync()
    {
        _mqttTimeService.TimeReceived -= OnTimeReceived;
        _mqttTimeService.StatusChanged -= OnStatusChanged;
        await _mqttTimeService.DisposeAsync();
    }
}
