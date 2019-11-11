using GK2_TrianglesFiller.DrawingRes;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace GK2_TrianglesFiller.Resources
{
    public static class Configuration
    {
        public static bool DrawVertices = false;
        public static bool DrawGrid = false;

        public static bool UseConstantColor = false;
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

        private static Vector3D lightVersor;
        public static int currentAngle;
        public static Vector3D LightVersor
        {
            get => lightVersor;
            set
            {
                lightVersor = value;
                ColorGenerator.LightVersor = value;
            }
        }

        public static bool UseConstantVector { get; set; } = false;
        public static BitmapImage DefaultNormalMap = new BitmapImage(new Uri(@"pack://application:,,,/Images/brick_normal_map2.png"));

        public static int FillColor { get; set; } = 1;

        public static bool SameForAll { get; set; } = true;
        public static double Kd { get; set; } = 0.5;
        public static double Ks { get; set; } = 0.5;
        public static int M { get; set; } = 10;

        public static double SideLength = 120;

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
            if (DefaultNormalMap.CanFreeze)
            {
                DefaultNormalMap.Freeze();
            }

            LightVersor = new Vector3D(0, 0, 255);
        }
    }
}
