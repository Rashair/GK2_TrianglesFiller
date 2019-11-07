using GK2_TrianglesFiller.Resources;
using System;
using System.Windows;

namespace GK2_TrianglesFiller.GeometryRes
{
    public static class PointGeometry
    {
        public static bool ArePointsIntersecting(this Point p1, Point p2)
        {
            return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)
                <= Configuration.VertexRadius * Configuration.VertexRadius;
        }

        public static Point Multiply(this Point p, double a)
        {
            return new Point((int)(p.X * a), (int)(p.Y * a));
        }

        public static double Magnitude(this Point p)
        {
            return Math.Sqrt(p.X * p.X + p.Y * p.Y);
        }

        public static Point Difference(this Point p, Point d)
        {
            return new Point(p.X - d.X, p.Y - d.Y);
        }

        public static double Distance(this Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        public static double Angle(Point p1, Point p2)
        {
            double xDiff = p2.X - p1.X;
            double yDiff = p2.Y - p1.Y;

            return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
        }

        public static double CrossProduct(Point p1, Point p2)
        {
            return p1.X * p2.Y - p1.Y * p2.X;
        }

        public static double DotProduct(Point p1, Point p2)
        {
            return p1.X * p2.X + p1.Y * p2.Y;
        }

        // https://math.stackexchange.com/questions/175896/finding-a-point-along-a-line-https://stackoverflow.com/questions/13302396/given-two-points-find-a-third-point-on-the-line?rq=1a-certain-distance-away-from-another-point/175906
        public static Point SameLinePoint(Point p1, double distanceRatio, Point p2)
        {
            return new Point(
               (1 - distanceRatio) * p1.X + distanceRatio * p2.X,
               (1 - distanceRatio) * p1.Y + distanceRatio * p2.Y
            );
        }

        public static (int x, int y) GetIntCoordinates(this Point p)
        {
            return ((int)p.X, (int)p.Y);
        }

        public static double Slope(Point p1, Point p2)
        {
            return (p1.X - p2.X) < Geometry.Eps ? double.MaxValue : (p2.Y - p1.Y) / (p2.X - p1.X);
        }
    }
}
