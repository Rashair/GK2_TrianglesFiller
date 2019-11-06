﻿using GK2_TrianglesFiller.DrawingRes;
using static GK2_TrianglesFiller.Resources.Configuration;
using MahApps.Metro.Controls;
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

namespace GK2_TrianglesFiller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private TriangleGrid grid;

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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (grid != null)
            {
                grid.TriangleGrid_SizeChanged(sender, e);
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            grid = new TriangleGrid(new Rect(CanvasMargin, CanvasMargin,
                MainCanvas.ActualWidth - CanvasMargin, MainCanvas.ActualHeight - CanvasMargin));
            MainCanvas.Children.Add(grid);
        }
    }
}
