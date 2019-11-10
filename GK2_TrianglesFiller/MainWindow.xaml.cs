﻿using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        public bool[] Vector { get; } = { true, false };

        public string FillColorGroup { get; } = Guid.NewGuid().ToString();
        public bool[] FillColor { get; } = { true, false, false };

        public string FactorsGroup { get; } = Guid.NewGuid().ToString();
        public bool[] Factors { get; } = { true, false };

        private DrawingHost host;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            host = new DrawingHost(new Rect(MyCanvas.RenderSize));
            MyCanvas.Children.Add(host);
        }

        private void ObjectColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            host?.SetBackground(e.NewValue.Value);
        }

        private void ObjectColorFileButton_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog dlg = new OpenFileDialog
            {
                // TODO: other extensions
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
                host.SetBackground(img);
            }

        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
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

        private void RadioVector2_Checked(object sender, RoutedEventArgs e)
        {
            UseConstantVector = false;
            host?.UpdateBackground();
        }
    }
}
