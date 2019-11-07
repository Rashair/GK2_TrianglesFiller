using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Media;

namespace GK2_TrianglesFiller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public string ObjectColorGroup { get; } = Guid.NewGuid().ToString();
        public bool[] ObjectColor { get; } = { true, false };

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
            host = new DrawingHost(new Rect(MyCanvas.RenderSize), ObjectColorPicker.SelectedColor.Value);
            MyCanvas.Children.Add(host);
        }

        private void ObjectColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (host != null)
            {
                host.UpdateBackground(e.NewValue.Value);
            }
        }
    }
}
