using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static GK2_TrianglesFiller.Resources.Configuration;

namespace GK2_TrianglesFiller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public string ObjectColorGroup { get; } = Guid.NewGuid().ToString();

        public string LightVersorGroup { get; } = Guid.NewGuid().ToString();
        public bool[] LightVersor { get; } = { true, false };

        public string VectorGroup { get; } = Guid.NewGuid().ToString();

        public string FillColorGroup { get; } = Guid.NewGuid().ToString();
        //public bool[] FillColor { get; } = { true, false, false };

        public string FactorsGroup { get; } = Guid.NewGuid().ToString();
        public bool[] Factors { get; } = { true, false };

        private DrawingHost host;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(50);
        }

        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            host = new DrawingHost(new Rect(MyCanvas.RenderSize));
            MyCanvas.Children.Add(host);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            host.UpdateBackground();
        }

        private void ObjectColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (host != null)
            {
                UseConstantColor = true;
                host.SetBackground(e.NewValue.Value);
            }
        }

        private void ObjectColorFileButton_Click(object sender, RoutedEventArgs e)
        {
            var img = GetImageFromFile();
            if (img != null)
            {
                UseConstantColor = false;
                host.SetBackground(img);
            }

        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            UseConstantColor = false;
            host.SetBackground(DefaultImage);
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            DrawGrid = drawGridCheck.IsChecked.Value;
            host?.Render();
        }

        private void LightColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                LightColor = e.NewValue.Value;
                host?.UpdateBackground();
            }
        }


        private void RadioVector_Checked(object sender, RoutedEventArgs e)
        {
            if (host != null)
            {
                if (RadioVector1.IsChecked.Value)
                {
                    host.SetMap(null);
                }
                else
                {
                    var img = GetImageFromFile();
                    if (img != null)
                    {
                        host.SetMap(img);
                    }
                }
            }
        }

        private BitmapImage GetImageFromFile()
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Images (*.jpg, *.jpeg, *.jpe, *.jfif, *.png)|*.jpg;*.jpeg;*.jpe;*.jfif;*.png"
            };

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                string fileName = dlg.FileName;
                BitmapImage img;
                if (fileName.EndsWith("png"))
                {
                    img = new BitmapImage(new Uri(fileName));
                    img.Freeze();
                }
                else
                {
                    using Stream bmpStream = File.Open(fileName, FileMode.Open);
                    {
                        img = new BitmapImage();
                        img.BeginInit();
                        img.StreamSource = bmpStream;
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.EndInit();
                        img.Freeze();
                    }
                }

                return img;
            }

            return null;
        }


        #region Sliders
        private bool dragStarted = false;
        // Kd
        private void KdSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Kd = KdSlider.Value;
            host.UpdateBackground();
            dragStarted = false;
        }

        private void KdSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStarted = true;
        }

        private void KdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted)
            {
                Kd = e.NewValue;
                host?.UpdateBackground();
            }
        }

        // Ks
        private void KsSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStarted = true;
        }

        private void KsSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Ks = KsSlider.Value;
            host.UpdateBackground();
            dragStarted = false;
        }

        private void KsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted)
            {
                Ks = e.NewValue;
                host?.UpdateBackground();
            }
        }

        // M
        private void MSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStarted = true;
        }

        private void MSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            M = (int)MSlider.Value;
            host.UpdateBackground();
            dragStarted = false;
        }

        private void MSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted)
            {
                M = (int)e.NewValue;
                host?.UpdateBackground();
            }
        }

        #endregion Slider

        private void RadioFactors2_Unchecked(object sender, RoutedEventArgs e)
        {
            KdSlider.IsEnabled = true;
            KsSlider.IsEnabled = true;
            MSlider.IsEnabled = true;
            host.UpdateBackground();
        }

        private void RadioFactors2_Checked(object sender, RoutedEventArgs e)
        {
            KdSlider.IsEnabled = false;
            KsSlider.IsEnabled = false;
            MSlider.IsEnabled = false;
            host.UpdateBackground();
        }

        private void RadioFillColor_Checked(object sender, RoutedEventArgs e)
        {
            if (RadioFillColor1.IsChecked.Value)
            {
                FillColor = 1;
            }
            else if (RadioFillColor2.IsChecked.Value)
            {
                FillColor = 2;
            }
            else if (RadioFillColor3.IsChecked.Value)
            {
                FillColor = 3;
            }
        }
    }
}
