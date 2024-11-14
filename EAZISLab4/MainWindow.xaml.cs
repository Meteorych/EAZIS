using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using EAZIS4;
using EAZIS4.Services;
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

    private const string SaveDictionaryFilterSetttings = "JSON files (*.json)|*.json|All files (*.*)|*.*";
    private const string SaveReportFilterSettings = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";

    private readonly SaveFileDialog _saveFileDialog = new()
    {
        InitialDirectory = "d:\\",
        FilterIndex = 1
    };

    private readonly HttpClient _httpClient = new();
    private readonly HttpClientService _httpClientService;

    private Response? _responseData;

    public MainWindow()
    {
        _httpClientService = new HttpClientService(_httpClient);
        InitializeComponent();
        DataContext = this;
    }

    private async void SendTextAnalysisQueryButton_OnClick(object sender, RoutedEventArgs e)
    {
        ResponseData = await _httpClientService.SendTextAnalysisQuery(BeginningText.Text);
        TreePanel.Visibility = Visibility.Visible;
        DownloadReportButton.Visibility = Visibility.Visible;
    }

    private async void SendTranslateQueryButton_OnClick(object sender, RoutedEventArgs e)
    {
        ResponseData = await _httpClientService.TranslateText(BeginningText.Text);
        BeginningText.Text = ResponseData?.Translation;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void DownloadDictionary_Click(object sender, RoutedEventArgs e)
    {
        _saveFileDialog.Filter = SaveDictionaryFilterSetttings;
        if (_saveFileDialog.ShowDialog() != true) return;

        try
        {
            var fileBytes = await _httpClientService.GetDocument(ApiPathConstants.GetDictionary);
            await File.WriteAllBytesAsync(_saveFileDialog.FileName, fileBytes);
        }
        catch (HttpRequestException exception)
        {
            MessageBox.Show(exception.Message);
        }
    }

    private async void DownloadReportButton_OnClick(object sender, RoutedEventArgs e)
    {
        _saveFileDialog.Filter = SaveReportFilterSettings;
        if (_saveFileDialog.ShowDialog() != true) return;

        try
        {
            var fileBytes = await _httpClientService.GetDocument(ApiPathConstants.GetReport);
            await File.WriteAllBytesAsync(_saveFileDialog.FileName, fileBytes);
        } 
        catch (HttpRequestException exception)
        {
            MessageBox.Show(exception.Message);
        }
    }

    private async void GetTreeImageButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var fileBytes = await _httpClientService.GetTree(int.Parse(NumOfSentense.Text.Trim()));
            var imageWindow = new ImageWindow(fileBytes);
            imageWindow.Show();
        }
        catch (HttpRequestException exception)
        {
            MessageBox.Show(exception.Message);
        }
        catch (FormatException)
        {
            MessageBox.Show("Неверное число в строке!");
        }
    }
}

public class PrecisionVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        var selectedValue = (value as ComboBoxItem)?.Content?.ToString()?.ToLower();
        if (selectedValue == "ngramm" || selectedValue == "neuro" || selectedValue == "alphabet")
        {
            return Visibility.Visible;
        }
    
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}