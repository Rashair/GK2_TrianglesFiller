using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GK2_TrianglesFiller.Resources
{
    static class Configuration
    {
        public const double VertexRadius = 5.0;
        public const double TriangleSideLength = 60.0;
        public const double CanvasMargin = 0;
        public const double DPI = 96;
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
