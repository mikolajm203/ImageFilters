using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;

namespace lab1v2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Filter> activeFilters;
        public System.Drawing.Image filteredImageDrawing;
        public MainWindow()
        {
            activeFilters = new List<Filter>();
            InitializeComponent();
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.DefaultExt = ".png";
            fileDialog.Filter = "PNG Files (*.png)|*.png|EG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            fileDialog.ShowDialog();
            FilteredImage.Source = new BitmapImage(new Uri(fileDialog.FileName));
            OriginalImage.Source = new BitmapImage(new Uri(fileDialog.FileName));

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.DefaultExt = "png";
            fileDialog.Filter = "PNG Files (*.png)|*.png";
            fileDialog.ShowDialog();
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)FilteredImage.Source));
            if (fileDialog.FileName != String.Empty)
            {
                using (var fileStream = new System.IO.FileStream(fileDialog.FileName, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
        }

        private void ContrastCheckBox_Click(object sender, RoutedEventArgs e)
        {
        }

        private void InvertCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)InvertCheckBox.IsChecked)
            {
                activeFilters.Add(new InverseFilter());

            }
            else
            {
                activeFilters.RemoveAll((Filter f) => f.Name == "InverseFilter");
            }
            applyFilters();
        }
        private void BrightenCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)BrightenCheckBox.IsChecked)
            {
                activeFilters.Add(new BrightenFilter());

            }
            else
            {
                activeFilters.RemoveAll((Filter f) => f.Name == "BrightenFilter");
            }
            applyFilters();
        }
        private void ContrastCheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            if ((bool)ContrastCheckBox.IsChecked)
            {
                activeFilters.Add(new ContrastFilter());

            }
            else
            {
                activeFilters.RemoveAll((Filter f) => f.Name == "ContrastFilter");
            }
            applyFilters();
        }
        private void GammaCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)GammaCheckBox.IsChecked)
            {
                activeFilters.Add(new GammaFilter());

            }
            else
            {
                activeFilters.RemoveAll((Filter f) => f.Name == "GammaFilter");
            }
            applyFilters();
        }
        private void BlurCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)BlurCheckBox.IsChecked)
            {
                activeFilters.Add(new BlurFilter());

            }
            else
            {
                activeFilters.RemoveAll((Filter f) => f.Name == "BlurFilter");
            }
            applyFilters();
        }
        private void GaussianBlurCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)GaussianBlurCheckBox.IsChecked)
            {
                activeFilters.Add(new GaussianBlurFilter());

            }
            else
            {
                activeFilters.RemoveAll((Filter f) => f.Name == "GaussianBlurFilter");
            }
            applyFilters();
        }
        private void SharpenCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)SharpenCheckBox.IsChecked)
            {
                activeFilters.Add(new SharpenFilter());

            }
            else
            {
                activeFilters.RemoveAll((Filter f) => f.Name == "SharpenFilter");
            }
            applyFilters();
        }
        private void EdgeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)EdgeCheckBox.IsChecked)
            {
                activeFilters.Add(new EdgeFilter());

            }
            else
            {
                activeFilters.RemoveAll((Filter f) => f.Name == "EdgeFilter");
            }
            applyFilters();
        }
        private void EmbossCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)EmbossCheckBox.IsChecked)
            {
                activeFilters.Add(new EmbossFilter());

            }
            else
            {
                activeFilters.RemoveAll((Filter f) => f.Name == "EmbossFilter");
            }
            applyFilters();
        }
        private void HCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)HCheckBox.IsChecked)
            {
                activeFilters.Add(new HFilter());

            }
            else
            {
                activeFilters.RemoveAll((Filter f) => f.Name == "HFilter");
            }
            applyFilters();
        }
        private void SCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)SCheckBox.IsChecked)
            {
                activeFilters.Add(new SFilter());

            }
            else
            {
                activeFilters.RemoveAll((Filter f) => f.Name == "SFilter");
            }
            applyFilters();
        }
        private void VCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)VCheckBox.IsChecked)
            {
                activeFilters.Add(new VFilter());
            }
            else
            {
                activeFilters.RemoveAll((Filter f) => f.Name == "VFilter");
            }
            applyFilters();
        }
        private void applyFilters()
        {
            FilteredImage.Source = OriginalImage.Source;
            WpfImageToDrawing();
            foreach(var filter in activeFilters)
            {
                filteredImageDrawing = filter.apply(filteredImageDrawing);
            }
            DrawingToWpfImage();
        }

        private void DrawingToWpfImage()
        {
            MemoryStream memoryStream = new MemoryStream();
            filteredImageDrawing.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
            memoryStream.Seek(0, SeekOrigin.Begin);
            BitmapImage wpfImage = new BitmapImage();
            wpfImage.BeginInit();
            wpfImage.CacheOption = BitmapCacheOption.OnLoad;
            wpfImage.StreamSource = memoryStream;
            wpfImage.EndInit();
            FilteredImage.Source = wpfImage;
        }

        private void WpfImageToDrawing()
        {
            System.Windows.Media.Imaging.BmpBitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
            if(FilteredImage.Source != null)
            {
                bitmapEncoder.Frames.Add(BitmapFrame.Create(new Uri(FilteredImage.Source.ToString())));
                MemoryStream memoryStream = new System.IO.MemoryStream();
                bitmapEncoder.Save(memoryStream);
                filteredImageDrawing = System.Drawing.Bitmap.FromStream(memoryStream);
            }
        }

        private void DitheringButton_Click(object sender, RoutedEventArgs e)
        {
            int r, g, b;
            if (AverageDitheringFilter.isApplied)
            {
                AverageDitheringFilter.isApplied = !AverageDitheringFilter.isApplied;
                DitheringButton.Content = "Average Dithering";
                activeFilters.RemoveAll((Filter f) => f.Name == "AverageDitheringFilter");
            }
            else
            {
                AverageDitheringFilter.isApplied = !AverageDitheringFilter.isApplied;
                DitheringButton.Content = "Cancel dithering";
                if ((bool)ShadesCheckBox.IsChecked)
                {
                    int K = Int32.Parse(ShadesTextBox.Text);
                    GrayFilter filter = new GrayFilter();
                    
                }
                else
                {
                    r = Int32.Parse(RTextBox.Text);
                    g = Int32.Parse(GTextBox.Text);
                    b = Int32.Parse(BTextBox.Text);
                    activeFilters.Add(new AverageDitheringFilter(r, g, b));
                }
            }
            applyFilters();
        }

        private void GrayCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)GrayCheckBox.IsChecked)
            {
                activeFilters.Add(new GrayFilter());

            }
            else
            {
                activeFilters.RemoveAll((Filter f) => f.Name == "GrayFilter");
            }
            applyFilters();
        }

        private void MedianButton_Click(object sender, RoutedEventArgs e)
        {
            int colors;
            if (AverageDitheringFilter.isApplied)
            {
                AverageDitheringFilter.isApplied = !AverageDitheringFilter.isApplied;
                MedianButton.Content = "Median Cut";
                activeFilters.RemoveAll((Filter f) => f.Name == "MedianCutFilter");
            }
            else
            {
                AverageDitheringFilter.isApplied = !AverageDitheringFilter.isApplied;
                MedianButton.Content = "Cancel median cut";
                colors = Int32.Parse(ColorsTextBox.Text);
                activeFilters.Add(new MedianCutFilter(colors));
            }
            applyFilters();
        }

        private void ShadesCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)ShadesCheckBox.IsChecked)
            {
                ShadesTextBox.IsEnabled = false;
                RTextBox.IsEnabled = true;
                GTextBox.IsEnabled = true;
                BTextBox.IsEnabled = true;
            }
            else
            {
                RTextBox.IsEnabled = false;
                GTextBox.IsEnabled = false;
                BTextBox.IsEnabled = false;
                ShadesTextBox.IsEnabled = true;
            }
        }

        private void YCbCrButton_Click(object sender, RoutedEventArgs e)
        {
            int r, g, b;
            if (YCbCrDitheringFilter.isApplied)
            {
                YCbCrDitheringFilter.isApplied = !YCbCrDitheringFilter.isApplied;
                YCbCrButton.Content = "YCbCr Dithering";
                activeFilters.RemoveAll((Filter f) => f.Name == "YCbCrDitheringFilter");
            }
            else
            {
                YCbCrDitheringFilter.isApplied = !YCbCrDitheringFilter.isApplied;
                YCbCrButton.Content = "Cancel dithering";
                if ((bool)ShadesCheckBox.IsChecked)
                {
                    int K = Int32.Parse(ShadesTextBox.Text);
                    GrayFilter filter = new GrayFilter();

                }
                else
                {
                    r = Int32.Parse(RTextBox.Text);
                    g = Int32.Parse(GTextBox.Text);
                    b = Int32.Parse(BTextBox.Text);
                    activeFilters.Add(new YCbCrDitheringFilter(r, g, b));
                }
            }
            applyFilters();
        }
    }
}
