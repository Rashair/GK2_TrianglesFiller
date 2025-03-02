﻿using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using static GK2_TrianglesFiller.Resources.Configuration;

namespace GK2_TrianglesFiller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private DrawingHost host;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
        }

        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {

            var response = Microsoft.VisualBasic.Interaction.InputBox(
                "Number of triangles will be roughly equal to " + MyCanvas.RenderSize.Width + "*" + MyCanvas.RenderSize.Height + " / sideLenght",
                                            "Triangle side lenght", "120");
            if (int.TryParse(response, out int side) && side > 0 && side <= 600)
            {
                SideLength = side;
            }

            host = new DrawingHost(new Rect(MyCanvas.RenderSize));
            MyCanvas.Children.Add(host);
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
            if (!this.IsLoaded)
                return;

            if (RadioFillColor1.IsChecked.Value && FillColor != 1)
            {
                FillColor = 1;
                host?.UpdateBackground();
            }
            else if (RadioFillColor2.IsChecked.Value && FillColor != 2)
            {
                FillColor = 2;
                host?.UpdateBackground();
            }
            else if (RadioFillColor3.IsChecked.Value && FillColor != 3)
            {
                FillColor = 3;
                host?.UpdateBackground();
            }
        }

        private void RadioLightVersor2_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded && RadioLightVersor2.IsChecked.Value)
            {
                // currentAngle = 0;
                LightVersor = new Vector3D(0, 0, 255);
                dispatcherTimer.Start();
            }
        }


        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {

            // currentAngle = (currentAngle + 5) % 90;
            LightVersor = GetNewLightVector();
            host.UpdateBackground();
        }

        private Vector3D GetNewLightVector()
        {
            return new Vector3D((LightVersor.X + 1) % 125, 0, LightVersor.Z);
        }


        private void RadioLightVersor2_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded && RadioLightVersor1.IsChecked.Value)
            {
                dispatcherTimer.Stop();
                LightVersor = new Vector3D(0, 0, 255);
                host.UpdateBackground();
            }
        }
    }
}
