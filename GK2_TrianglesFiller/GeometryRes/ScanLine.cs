using GK2_TrianglesFiller.VertexRes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace GK2_TrianglesFiller.GeometryRes
{
    class ScanLine
    {
        private HashSet<AETPointer> AET;
        private readonly int[] sortedInd;
        private List<Point> polygon;

        public ScanLine(List<Point> polygon)
        {
            AET = new HashSet<AETPointer>();
            sortedInd = Enumerable.Range(0, polygon.Count).ToArray();
            this.polygon = polygon;
            Array.Sort(polygon.ToArray(), sortedInd, Comparer<Point>.Create((v1, v2) => v1.Y.CompareTo(v2.Y)));
        }

        public IEnumerable<(List<int> xList, int y)> GetIntersectionPoints()
        {
            var minVertex = polygon[sortedInd[0]];
            int yMin = (int)minVertex.Y;
            int yMax = (int)polygon[sortedInd[polygon.Count - 1]].Y;

            int startInd = 0;
            int prevMaxInd = 0;
            while (prevMaxInd < polygon.Count - 1 && polygon[sortedInd[prevMaxInd + 1]].Y == yMin)
            {
                ++prevMaxInd;
            }

            for (int y = yMin + 1; y <= yMax; ++y)
            {
                if (prevMaxInd < polygon.Count && y - 1 == polygon[sortedInd[prevMaxInd]].Y)
                {
                    for (int k = startInd; k <= prevMaxInd; ++k)
                    {
                        var ind = sortedInd[k];
                        var current = polygon[ind];
                        var prev = polygon[(ind - 1 + polygon.Count) % polygon.Count];
                        if (prev.Y >= current.Y)
                        {
                            AET.Add(new AETPointer(prev.Y, prev.X, PointGeometry.Slope(prev, current)));
                        }
                        else
                        {
                            AET.Remove(new AETPointer(prev.Y, prev.X, 1));
                        }


                        var next = polygon[(ind + 1) % polygon.Count];
                        if (next.Y >= current.Y)
                        {
                            AET.Add(new AETPointer(next.Y, current.X, PointGeometry.Slope(current, next)));
                        }
                        else
                        {
                            AET.Remove(new AETPointer(next.Y, current.X, 1));
                        }
                    }

                    ++prevMaxInd;
                    startInd = prevMaxInd;
                    while (prevMaxInd < polygon.Count - 1 && polygon[sortedInd[prevMaxInd + 1]].Y == y)
                    {
                        ++prevMaxInd;
                    }
                }

                yield return (AET.Select(ptr => ptr.X).OrderBy(x => x).ToList(), y);

                foreach (var ptr in AET)
                {
                    ptr.UpdateX();
                }
                AET.RemoveWhere((ptr) => ptr.yMax == y);
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
            this.x = (int)x;
            _m = 1.0 / m;
        }

        public int X { get => (int)x; }

        public void UpdateX()
        {
            x += _m;
        }

        public int CompareTo(AETPointer other)
        {
            var xCmp = x.CompareTo(other.x);
            return xCmp == 0 ? yMax.CompareTo(other.yMax) : xCmp;
        }

        public override bool Equals(object obj)
        {
            return obj is AETPointer pointer &&
                   yMax == pointer.yMax &&
                   X == pointer.X &&
                   (int)_m == (int)pointer._m;
        }

        public override int GetHashCode()
        {
            var hashCode = 1999727083;
            hashCode = hashCode * -1521134295 + yMax.GetHashCode();
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + _m.GetHashCode();
            return hashCode;
        }
    }
}
