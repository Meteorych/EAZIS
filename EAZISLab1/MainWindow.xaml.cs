using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using EAZISLab1.Services;
using Microsoft.Extensions.Configuration;

namespace EAZISLab1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    public Response? ResponseData
    {
        get => _responseData;
        set
        {
            _responseData = value;
            OnPropertyChanged();
        }
    }

    public int TextLength
    {
        get => _textLength;
        set
        {
            _textLength = value;
            OnPropertyChanged();
        }
    }

    private readonly HttpClient _httpClient = new();
    private readonly HttpClientService _httpClientService;

    private IConfiguration _configuration;

    private Response? _responseData;
    private int _textLength;

    public MainWindow()
    {
        InitializeConfiguration();
        _httpClientService = new HttpClientService(_httpClient, _configuration!);
        _textLength = 5;
        InitializeComponent();
        DataContext = this;
    }

    private void InitializeConfiguration()
    {
        _configuration = new ConfigurationBuilder()
            .AddKeyPerFile(Path.Combine(Directory.GetCurrentDirectory(),"configFiles"))
            .Build();
    }

    private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        await _httpClientService.SendPathToDocuments();
    }

    private async void SendQueryButton_OnClick(object sender, RoutedEventArgs e)
    {
        ResponseData = await _httpClientService.SendQuery(InputText.Text, int.Parse(InputLength.Text));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void InputLength_OnTextChanged(object sender, TextChangedEventArgs e)
    {
            var input = InputLength.Text;
            if (SendQueryButton is null) return;
            if (int.TryParse(input, out var inputLength) && inputLength is >= 1 and <= 50)
            {
                SendQueryButton.IsEnabled = true;
            }
            else
            {
                SendQueryButton.IsEnabled = false;
            }
    }
}

