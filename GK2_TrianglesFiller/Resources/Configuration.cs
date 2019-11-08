using System;
using System.Windows.Media;

namespace GK2_TrianglesFiller.Resources
{
    public static class Configuration
    {
        public static bool DrawVertices = false;
        public static bool DrawGrid = true;

        public static bool ObjectColorFromTexture = true;
        public static Uri DeafultImagePath = new Uri(@"pack://application:,,,/Resources/wallpaper.png");

        public const double SideLength = 120;

        public const double DPI = 96;
        public static PixelFormat PixelFormat = PixelFormats.Bgra32;
        public readonly static int BytesPerPixel = (PixelFormat.BitsPerPixel) / 8;

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
        }
    }
}
