using System.Windows.Media;

namespace GK2_TrianglesFiller.Resources
{
    static class Configuration
    {
        public const bool DrawVertices = false;

        public const double VertexRadius = 5.0;
        public const double SideLength = 120.0;
        public const double DPI = 96;
        public static Brush VertexBrush = Brushes.Black;
        public static Pen VertexPen = new Pen(VertexBrush, 1.0);
        public static PixelFormat pixelFormat = PixelFormats.Bgr32;
        public static int bytesPerPixel = pixelFormat.BitsPerPixel / 8;

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
