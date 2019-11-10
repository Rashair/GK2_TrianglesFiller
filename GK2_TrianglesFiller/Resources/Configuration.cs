using GK2_TrianglesFiller.DrawingRes;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GK2_TrianglesFiller.Resources
{
    public static class Configuration
    {
        public static bool DrawVertices = false;
        public static bool DrawGrid = false;

        public static bool ObjectColorFromTexture = true;
        public static BitmapImage DefaultImage = new BitmapImage(new Uri(@"pack://application:,,,/Images/wallpaper.png"));

        private static Color lightColor;
        public static Color LightColor
        {
            get => lightColor;
            set
            {
                lightColor = value;
                ColorGenerator.LightColor = (value.R / 255.0, value.G / 255.0, value.B / 255.0);
            }
        }

        public static bool UseConstantVector = false;
        public static BitmapImage DefaultNormalMap = new BitmapImage(new Uri(@"pack://application:,,,/Images/my_normal_map.jpg"));

        public const double SideLength = 120;
        public const double DPI = 96;
        public static PixelFormat MyPixelFormat = PixelFormats.Bgra32;
        public static readonly int BytesPerPixel = (MyPixelFormat.BitsPerPixel + 7) / 8;

        public const double VertexRadius = 5.0;
        public static Brush VertexBrush = Brushes.Black;
        public static Pen VertexPen = new Pen(VertexBrush, 1.0);


        static Configuration()
        {
            if (VertexBrush.CanFreeze)
            {
                VertexBrush.Freeze();
            }
            if (VertexPen.CanFreeze)
            {
                VertexPen.Freeze();
            }
            if (DefaultImage.CanFreeze)
            {
                DefaultImage.Freeze();
            }
        }
    }
}
