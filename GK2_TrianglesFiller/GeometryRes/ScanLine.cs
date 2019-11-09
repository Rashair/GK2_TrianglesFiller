using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace GK2_TrianglesFiller.GeometryRes
{
    class ScanLine
    {
        private readonly List<AETPointer> AET;
        private readonly Stack<int> sortedInd;
        private readonly List<Point> polygon;

        public ScanLine(List<Point> polygon)
        {
            AET = new List<AETPointer>();
            this.polygon = polygon;


            sortedInd = new Stack<int>(polygon.Select((p, i) => new KeyValuePair<Point, int>(p, i)).
                OrderByDescending(pair => pair.Key.Y).
                Select(pair => pair.Value));
        }

        public IEnumerable<(List<int> xList, int y)> GetIntersectionPoints()
        {
            int yMin = (int)polygon[sortedInd.Peek()].Y;
            int yMax = (int)polygon[sortedInd.Last()].Y;
            var mockPointer = new AETPointer(0, 0, 0);
            for (int y = yMin + 1; y <= yMax; ++y)
            {
                while (sortedInd.Count > 0 && polygon[sortedInd.Peek()].Y == y - 1)
                {
                    var ind = sortedInd.Pop();
                    var current = polygon[ind];
                    var prev = polygon[(ind - 1 + polygon.Count) % polygon.Count];
                    if (prev.Y > current.Y)
                    {
                        AET.Add(new AETPointer(prev.Y, current.X, PointGeometry.Slope(current, prev)));
                    }
                    else if (prev.Y < current.Y)
                    {
                        mockPointer.x = prev.X;
                        mockPointer.yMax = (int)prev.Y;
                        AET.Remove(mockPointer);
                    }

                    var next = polygon[(ind + 1) % polygon.Count];
                    if (next.Y > current.Y)
                    {
                        AET.Add(new AETPointer(next.Y, current.X, PointGeometry.Slope(current, next)));
                    }
                    else if (next.Y < current.Y)
                    {
                        mockPointer.x = next.X;
                        mockPointer.yMax = (int)next.Y;
                        AET.Remove(mockPointer);
                    }
                }

                yield return (AET.Select(ptr => ptr.X).OrderBy(x => x).ToList(), y);

                AET.RemoveAll((ptr) => ptr.yMax <= y);
                foreach (var ptr in AET)
                {
                    ptr.UpdateX();
                }
            }
        }
    }


    class AETPointer : IComparable<AETPointer>
    {
        public int yMax;
        public double x;
        public double _m;

        public AETPointer(double yMax, double x, double m)
        {
            this.yMax = (int)yMax;
            this.x = x;
            _m = 1.0 / m;
        }

        public int X { get => (int)Math.Round(x); }

        public void UpdateX()
        {
            x = _m == Geometry.Infinity ? Geometry.Infinity : x + _m;
        }

        public int CompareTo(AETPointer other)
        {
            var xCmp = x.CompareTo(other.x);
            return xCmp == 0 ? yMax.CompareTo(other.yMax) : xCmp;
        }
    }
}
