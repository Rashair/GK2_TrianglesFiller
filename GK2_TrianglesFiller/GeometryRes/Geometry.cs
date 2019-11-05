﻿using System;
using System.Collections.Generic;
using System.Windows;
using static System.Math;

namespace GK2_TrianglesFiller.GeometryRes
{
    static class Geometry
    {
        public const double Eps = 1e-6;

        public static double Atan2_2PI(int y, int x)
        {
            double value = Atan2(y, x) * 180 / PI;
            return value < 0 ? value + 360 : value;
        }

        // https://stackoverflow.com/a/14998816/6841224
        // The function counts the number of sides of the polygon that:
        //  - intersect the Y coordinate of the point (the first if() condition) 
        //  - are to the left of it (the second if() condition).
        // If the number of such sides is odd, then the point is inside the polygon
        public static bool IsPointInsidePolygon(Point p, List<Point> polygon)
        {
            bool result = false;
            int j = polygon.Count - 1;
            for (int i = 0; i < polygon.Count; ++i)
            {
                if ((polygon[j].Y < p.Y && polygon[i].Y >= p.Y) ||
                    (polygon[i].Y < p.Y && polygon[j].Y >= p.Y))
                {
                    if (polygon[i].X + (p.Y - polygon[i].Y) * (polygon[j].X - polygon[i].X)
                        / (double)(polygon[j].Y - polygon[i].Y) < p.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }

            return result;
        }
    }
}
