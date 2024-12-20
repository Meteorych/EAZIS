﻿using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using EAZIS2.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
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

    private const string OpenFileFilterSettings = "HTML files (*.html)|*.html|Text files (*.txt)|*.txt|All files (*.*)|*.*";
    private const string SaveFileFilterSettings = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";

    private readonly HttpClient _httpClient = new();
    private readonly HttpClientService _httpClientService;
    private readonly Dictionary<string, byte[]> _listOfFiles = new();

    private readonly OpenFileDialog _openFileDialog = new()
    {
        InitialDirectory = "d:\\",
        FilterIndex = 1,
        RestoreDirectory = true,
        Filter = OpenFileFilterSettings,
        Multiselect = true
    };

    private readonly SaveFileDialog _saveFileDialog = new()
    {
        InitialDirectory = "d:\\",
        FilterIndex = 1,
        RestoreDirectory = true,
        Filter = SaveFileFilterSettings
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
        _listOfFiles.Clear();
        ListOfFilePaths.Clear();
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

    private void DownloadReportButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (_saveFileDialog.ShowDialog() != true) return;

        using var fs = new FileStream(_saveFileDialog.FileName, FileMode.Create);
        using var document = new Document(PageSize.A4, 25, 25, 30, 30);
        using var writer = PdfWriter.GetInstance(document, fs);
        document.Open();

        System.Text.EncodingProvider ppp = System.Text.CodePagesEncodingProvider.Instance;
        Encoding.RegisterProvider(ppp);

        var fontPath = "C:\\Users\\Ivan.Tsitlou\\Downloads\\arial.ttf"; // Update font path as needed
        var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        var font = new Font(baseFont, Font.DEFAULTSIZE, Font.NORMAL);

        foreach (var line in _responseData?.ToString().Split("\\n")!)
        {
            document.Add(new Paragraph(line.Replace("\\t", " ----> ")));
        }

        document.Close();
        writer.Close();
        fs.Close();
    }
}

public class PrecisionVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        var selectedValue = (value as ComboBoxItem)?.Content.ToString().ToLower();
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