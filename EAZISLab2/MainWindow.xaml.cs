using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using EAZIS2.Services;
using Microsoft.Win32;

namespace EAZIS2;

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

    private const string FilterSettings = "HTML files (*.html)|*.html|All files (*.*)|*.*";

    private readonly HttpClient _httpClient = new();
    private readonly HttpClientService _httpClientService;
    private readonly Dictionary<string, byte[]> _listOfFiles = new();

    private readonly OpenFileDialog _openFileDialog = new()
    {
        InitialDirectory = "d:\\",
        FilterIndex = 1,
        RestoreDirectory = true,
        Filter = FilterSettings,
        Multiselect = true
    };

    private string _recognitionMethod;
    private Response? _responseData;

    public MainWindow()
    {
        _httpClientService = new HttpClientService(_httpClient);
        InitializeComponent();
        DataContext = this;
        _recognitionMethod = "ngramm";
    }

    private async void SendQueryButton_OnClick(object sender, RoutedEventArgs e)
    {
        ResponseData = await _httpClientService.SendQuery(_listOfFiles, _recognitionMethod);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void DownloadFile_OnClick(object sender, RoutedEventArgs e)
    {
        if (_openFileDialog.ShowDialog() != true) return;

        foreach (var filePath in _openFileDialog.FileNames)
        {
            var fileContent = await File.ReadAllBytesAsync(filePath);
            ListOfFilePaths.Text += $"{filePath}\n";
            _listOfFiles.Add(filePath, fileContent);
        }
    }

    private void RecognitionMethodBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _recognitionMethod = (RecognitionMethodBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "ngramm";
    }
}