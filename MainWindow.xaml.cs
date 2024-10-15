﻿using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using EAZIS1.Services;
using EAZIS2.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace EAZIS2
{
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

        private readonly HttpClient _httpClient = new();
        private readonly HttpClientService _httpClientService;
        private readonly OpenFileDialog _openFileDialog = new();
        private List<byte[]> _listOfFiles = new List<byte[]>();

        private IConfiguration _configuration;

        private Response? _responseData;

        public MainWindow()
        {
            InitializeConfiguration();
            _httpClientService = new HttpClientService(_httpClient, _configuration!);
            InitializeComponent();
            DataContext = this;
        }

        private void InitializeConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .AddKeyPerFile(Path.Combine(Directory.GetCurrentDirectory(), "configFiles"))
                .Build();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await _httpClientService.SendPathToDocuments();
        }

        private async void SendQueryButton_OnClick(object sender, RoutedEventArgs e)
        {
            //ResponseData = await _httpClientService.SendQuery(InputText.Text);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DownloadFile_OnClick(object sender, RoutedEventArgs e)
        {
            _openFileDialog.ShowDialog();
        }
    }

}