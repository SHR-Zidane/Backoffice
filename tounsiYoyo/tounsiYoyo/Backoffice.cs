using System.Buffers;
using System.IO;
using System.Text;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using MQTTnet;
using tounsiYoyo.Model;

namespace tounsiYoyo;

public class Backoffice : Window
{
    private const string MqttBrokerHost = "blue.section-inf.ch";
    private const int MqttBrokerPort = 1883;
    private const string MqttUsername = "ict";
    private const string MqttPassword = "321";
    private const string MqttTimeTopic = "umddcmid3b/time";

    private readonly TextBlock BusinessName = new();
    private readonly Image BusinessLogo = new();
    private readonly TextBlock BusinessID = new();
    private readonly TextBlock SpecialName = new();
    private readonly Image SpecialLogo = new();
    private readonly TextBlock SpecialID = new();
    private readonly TextBlock SpecialPriceLabel = new();
    private readonly NumericUpDown SpecialPrice = new();
    private readonly Border StatusBanner = new();
    private readonly TextBlock StatusMessage = new();
    private readonly TextBlock TimeDisplay = new();
    private readonly Button MqttConnectButton = new();
    private readonly Button MqttDisconnectButton = new();
    private readonly Border MqttStatusBanner = new();
    private readonly TextBlock MqttStatusMessage = new();
    private IMqttClient? _mqttClient;
    private Business? _business;
    private string? _resolvedJsonPath;

    public Backoffice()
    {
        InitializeComponent();
        LoadData();
    }

    private void InitializeComponent()
    {
        Title = "BackOffice";
        Width = 827;
        Height = 486;
        MinWidth = 500;
        MinHeight = 400;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Background = new SolidColorBrush(Color.Parse("#F0F0F0"));

        Grid layout = new Grid
        {
            Margin = new Thickness(23),
            ColumnDefinitions = new ColumnDefinitions("*,36,*"),
            RowDefinitions = new RowDefinitions("Auto,8,164,8,Auto,8,Auto,4,Auto,*,Auto")
        };

        AddText(layout, BusinessName, "Business Name", 0, 0, 26);
        AddImage(layout, BusinessLogo, 0, 2);
        AddText(layout, BusinessID, "Business ID", 0, 4, 13);

        AddText(layout, SpecialName, "Special Name", 2, 0, 26);
        AddImage(layout, SpecialLogo, 2, 2);
        AddText(layout, SpecialID, "Special ID", 2, 4, 13);
        AddText(layout, SpecialPriceLabel, "Prix", 2, 6, 13);

        SpecialPrice.Minimum = 0;
        SpecialPrice.Maximum = 100000;
        SpecialPrice.Increment = 0.1m;
        SpecialPrice.FormatString = "0.00";
        SpecialPrice.Width = 120;
        SpecialPrice.Height = 28;
        Grid.SetColumn(SpecialPrice, 2);
        Grid.SetRow(SpecialPrice, 8);
        layout.Children.Add(SpecialPrice);

        TimeDisplay.Text = "Heure non chargée";
        TimeDisplay.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
        Button localTimeButton = new Button
        {
            Content = "Lire l'heure depuis le fichier",
            Height = 36,
            FontSize = 14,
            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
        };
        localTimeButton.Click += ReadTime_button_click;

        StackPanel timePanel = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            Spacing = 12,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Children = { localTimeButton, TimeDisplay }
        };

        MqttConnectButton.Content = "Se connecter et s'abonner";
        MqttConnectButton.Height = 36;
        MqttConnectButton.Click += ConnectMqtt_button_click;
        MqttDisconnectButton.Content = "Déconnecter";
        MqttDisconnectButton.Height = 36;
        MqttDisconnectButton.IsEnabled = false;
        MqttDisconnectButton.Click += DisconnectMqtt_button_click;

        MqttStatusMessage.Text = "Non connecté";
        MqttStatusBanner.Padding = new Thickness(10, 6);
        MqttStatusBanner.CornerRadius = new CornerRadius(4);
        MqttStatusBanner.BorderThickness = new Thickness(1);
        MqttStatusBanner.Child = MqttStatusMessage;
        ShowMqttStatus("Non connecté", "#E8E8E8", "#CCCCCC", "#333333");

        StackPanel mqttButtons = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            Spacing = 8,
            Children = { MqttConnectButton, MqttDisconnectButton, MqttStatusBanner }
        };
        StackPanel timeAndMqttPanel = new StackPanel
        {
            Spacing = 8,
            Children = { timePanel, mqttButtons }
        };
        Grid.SetColumn(timeAndMqttPanel, 0);
        Grid.SetRow(timeAndMqttPanel, 9);
        Grid.SetColumnSpan(timeAndMqttPanel, 3);
        layout.Children.Add(timeAndMqttPanel);
        Closed += async (_, _) => await DisconnectMqttAsync();

        Button exportButton = new Button
        {
            Content = "Export",
            Width = 78,
            Height = 40,
            Padding = new Thickness(15, 10),
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            Background = Brushes.Gray,
            Foreground = Brushes.White,
            BorderThickness = new Thickness(0),
            CornerRadius = new CornerRadius(10),
            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
        };
        exportButton.Click += Export_button_click;
        Grid.SetColumn(exportButton, 1);
        Grid.SetRow(exportButton, 0);
        exportButton.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right;
        exportButton.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
        exportButton.Margin = new Thickness(12, 0, 0, 0);

        StatusBanner.Height = 42;
        StatusBanner.Padding = new Thickness(12, 8);
        StatusBanner.CornerRadius = new CornerRadius(4);
        StatusBanner.BorderThickness = new Thickness(1);
        StatusBanner.IsVisible = false;
        StatusMessage.TextWrapping = TextWrapping.Wrap;
        StatusMessage.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
        StatusMessage.FontSize = 12;
        StatusBanner.Child = StatusMessage;

        Grid footer = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto")
        };
        Grid.SetColumn(StatusBanner, 0);
        footer.Children.Add(StatusBanner);
        footer.Children.Add(exportButton);
        Grid.SetRow(footer, 10);
        Grid.SetColumnSpan(footer, 3);
        layout.Children.Add(footer);

        Content = new ScrollViewer
        {
            Content = layout,
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto
        };
    }

    private static void AddText(Grid grid, TextBlock textBlock, string text, int column, int row, double fontSize)
    {
        textBlock.Text = text;
        textBlock.FontFamily = new FontFamily("Arial");
        textBlock.FontSize = fontSize;
        textBlock.Foreground = Brushes.Black;
        Grid.SetColumn(textBlock, column);
        Grid.SetRow(textBlock, row);
        grid.Children.Add(textBlock);
    }

    private static void AddImage(Grid grid, Image image, int column, int row)
    {
        Border border = new Border
        {
            MaxWidth = 219,
            Height = 164,
            Background = new SolidColorBrush(Color.Parse("#E8E8E8")),
            BorderBrush = new SolidColorBrush(Color.Parse("#CCCCCC")),
            BorderThickness = new Thickness(1),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
            Child = image
        };
        image.Stretch = Stretch.Uniform;
        Grid.SetColumn(border, column);
        Grid.SetRow(border, row);
        grid.Children.Add(border);
    }

    private void LoadData()
    {
        try
        {
            string path = JsonPathResolver.ResolvePath();
            _resolvedJsonPath = path;

            if (!File.Exists(path))
            {
                ShowError($"Fichier de données introuvable : '{path}'.");
                return;
            }

            string jsonString = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                ShowError($"Le fichier '{path}' est vide.");
                return;
            }

            Business data = JsonSerializer.Deserialize<Business>(jsonString);
            if (data == null)
            {
                ShowError($"Impossible de désérialiser les données depuis '{path}'.");
                return;
            }

            _business = data;

            BusinessName.Text = data.Name;
            BusinessID.Text = $"ID : {data.ID}";
            if (!string.IsNullOrWhiteSpace(data.Logo))
            {
                BusinessLogo.Source = Base64ToBitmap(data.Logo);
            }

            if (data.special != null)
            {
                SpecialName.Text = data.special.Name;
                SpecialID.Text = $"ID : {data.special.ID}";
                SpecialPrice.Value = (decimal)data.special.Price;

                if (!string.IsNullOrWhiteSpace(data.special.Picture))
                {
                    SpecialLogo.Source = Base64ToBitmap(data.special.Picture);
                }
            }
        }
        catch (JsonException jex)
        {
            ShowError($"Format JSON invalide dans le fichier : {jex.Message}");
        }
        catch (Exception ex)
        {
            ShowError($"Erreur lors de la lecture des données : {ex.Message}");
        }
    }

    private static Bitmap Base64ToBitmap(string base64String)
    {
        int comma = base64String.IndexOf(',');
        if (comma >= 0)
        {
            base64String = base64String[(comma + 1)..];
        }

        byte[] imageBytes = Convert.FromBase64String(base64String.Trim());
        using MemoryStream stream = new MemoryStream(imageBytes);
        return new Bitmap(stream);
    }

    private void ReadTime_button_click(object? sender, RoutedEventArgs e)
    {
        try
        {
            TimeDisplay.Text = ReadTimeFromFile().ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
        }
        catch (Exception ex)
        {
            ShowError($"Erreur lors de la lecture de l'heure : {ex.Message}");
        }
    }

    private DateTimeOffset ReadTimeFromFile()
    {
        string path = JsonPathResolver.ResolvePath("timestamp.json");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Fichier d'heure introuvable.", path);
        }

        return Time.LoadFromJson(path).ToDateTimeOffset();
    }

    private async void ConnectMqtt_button_click(object? sender, RoutedEventArgs e)
    {

        MqttConnectButton.IsEnabled = false;
        try
        {
            IMqttClient client = new MqttClientFactory().CreateMqttClient();
            client.ApplicationMessageReceivedAsync += HandleMqttMessageAsync;
            _mqttClient = client;

            MqttClientOptionsBuilder optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(MqttBrokerHost.Trim(), MqttBrokerPort);
            if (!string.IsNullOrWhiteSpace(MqttUsername))
            {
                optionsBuilder.WithCredentials(MqttUsername, MqttPassword);
            }

            await client.ConnectAsync(optionsBuilder.Build());
            await client.SubscribeAsync(new MqttTopicFilterBuilder()
                .WithTopic(MqttTimeTopic.Trim())
                .Build());

            MqttDisconnectButton.IsEnabled = true;
            ShowMqttStatus($"Connecté et abonné à « {MqttTimeTopic.Trim()} ».", "#D4EDDA", "#C3E6CB", "#155724");
        }
        catch (Exception ex)
        {
            await DisconnectMqttAsync();
            ShowMqttStatus($"Échec MQTT : {ex.Message}", "#F8D7DA", "#F5C6CB", "#721C24");
        }
        finally
        {
            MqttConnectButton.IsEnabled = _mqttClient?.IsConnected != true;
        }
    }

    private async Task HandleMqttMessageAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        try
        {
            string payload = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload.ToArray());
            Time? receivedTime;
            if (long.TryParse(payload.Trim(), out long unixTimeStamp))
            {
                receivedTime = new Time { UnixTimeStamp = unixTimeStamp };
            }
            else
            {
                receivedTime = JsonSerializer.Deserialize<Time>(payload);
            }

            if (receivedTime == null)
            {
                throw new JsonException("Le message ne contient pas de timestamp Unix valide.");
            }

            DateTimeOffset dateTime = receivedTime.ToDateTimeOffset();
            string path = JsonPathResolver.ResolvePath("timestamp.json");
            File.WriteAllText(path, JsonSerializer.Serialize(receivedTime, new JsonSerializerOptions { WriteIndented = true }));

            Dispatcher.UIThread.Post(() =>
            {
                TimeDisplay.Text = dateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
                ShowMqttStatus("Heure synchronisée depuis le broker.", "#D4EDDA", "#C3E6CB", "#155724");
            });
        }
        catch (Exception ex)
        {
            Dispatcher.UIThread.Post(() =>
                ShowMqttStatus($"Message horaire invalide : {ex.Message}", "#F8D7DA", "#F5C6CB", "#721C24"));
        }
    }

    private async void DisconnectMqtt_button_click(object? sender, RoutedEventArgs e)
    {
        await DisconnectMqttAsync();
        ShowMqttStatus("Déconnecté du broker.", "#E8E8E8", "#CCCCCC", "#333333");
    }

    private async Task DisconnectMqttAsync()
    {
        IMqttClient? client = _mqttClient;
        _mqttClient = null;
        MqttConnectButton.IsEnabled = true;
        MqttDisconnectButton.IsEnabled = false;
        if (client == null)
        {
            return;
        }

        if (client.IsConnected)
        {
            await client.DisconnectAsync();
        }

        client.Dispose();
    }

    public void Export_button_click(object? sender, RoutedEventArgs e)
    {
        try
        {
            Business businessToExport;
            if (_business != null)
            {
                businessToExport = _business;
                if (SpecialPrice.Value.HasValue && businessToExport.special != null)
                {
                    businessToExport.special.Price = (float)SpecialPrice.Value.Value;
                }
            }
            else
            {
                float price = SpecialPrice.Value.HasValue ? (float)SpecialPrice.Value.Value : 19.9f;
                businessToExport = new Business("COM-1510", "Tounsi Yoyo", new Special("PROD-1510", "Yoyo", price));
            }

            string exportPath = _resolvedJsonPath ?? JsonPathResolver.ResolvePath();
            businessToExport.Export(exportPath);

            ShowSuccess($"Business exporté avec succès vers {Path.GetFileName(exportPath)}");
        }
        catch (Exception ex)
        {
            ShowError($"Erreur lors de l'exportation : {ex.Message}");
        }
    }

    private void ShowError(string message)
    {
        ShowStatus(message, "#F8D7DA", "#F5C6CB", "#721C24");
    }

    private void ShowSuccess(string message)
    {
        ShowStatus(message, "#D4EDDA", "#C3E6CB", "#155724");
    }

    private void ShowStatus(string message, string background, string border, string foreground)
    {
        StatusBanner.Background = new SolidColorBrush(Color.Parse(background));
        StatusBanner.BorderBrush = new SolidColorBrush(Color.Parse(border));
        StatusMessage.Foreground = new SolidColorBrush(Color.Parse(foreground));
        StatusMessage.Text = message;
        StatusBanner.IsVisible = true;
    }

    private void ShowMqttStatus(string message, string background, string border, string foreground)
    {
        MqttStatusBanner.Background = new SolidColorBrush(Color.Parse(background));
        MqttStatusBanner.BorderBrush = new SolidColorBrush(Color.Parse(border));
        MqttStatusMessage.Foreground = new SolidColorBrush(Color.Parse(foreground));
        MqttStatusMessage.Text = message;
    }
}
