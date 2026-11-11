using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using tounsiYoyo.Model;

namespace tounsiYoyo;

public class Backoffice : Window
{
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

        Button readTimeButton = new Button
        {
            Content = "Lire l'heure",
            Width = 120,
            Height = 36,
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
        };
        readTimeButton.Click += ReadTime_button_click;

        TimeDisplay.Text = "Heure non chargée";
        TimeDisplay.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
        StackPanel timePanel = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            Spacing = 12,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Children = { readTimeButton, TimeDisplay }
        };
        Grid.SetColumn(timePanel, 0);
        Grid.SetRow(timePanel, 9);
        Grid.SetColumnSpan(timePanel, 3);
        layout.Children.Add(timePanel);

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
            string path = JsonPathResolver.ResolvePath("timestamp.json");
            if (!File.Exists(path))
            {
                ShowError($"Fichier d'heure introuvable : '{path}'.");
                return;
            }

            Time time = Time.LoadFromJson(path);
            TimeDisplay.Text = time.ToDateTimeOffset().ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
        }
        catch (Exception ex)
        {
            ShowError($"Erreur lors de la lecture de l'heure : {ex.Message}");
        }
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
}
