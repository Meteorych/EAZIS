using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace EAZIS4
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {

        public ImageWindow(byte[] byteArray)
        {
            var bitmapImage = new BitmapImage();

            using (var memoryStream = new MemoryStream(byteArray))
            {
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            InitializeComponent();

            TreeImage.Source = bitmapImage;
        }
    }
}
