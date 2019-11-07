using GK2_TrianglesFiller.VertexRes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GK2_TrianglesFiller.GeometryRes
{
    class ScanLine
    {
        private List<AETPointer> AET;
        private readonly int[] sortedInd;
        private List<Vertex> polygon;

        public ScanLine(List<Vertex> polygon)
        {
            AET = new List<AETPointer>();
            sortedInd = Enumerable.Range(0, polygon.Count).ToArray();
            this.polygon = polygon;
            Array.Sort(polygon.ToArray(), sortedInd, Comparer<Vertex>.Create((v1, v2) => v1.Y.CompareTo(v2.Y)));
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
                            AET.Add(new AETPointer(next.Y, next.X, PointGeometry.Slope(current, next)));
                        }
                        else
                        {
                            AET.Remove(new AETPointer(next.Y, next.X, 1));
                        }
                    }

                    ++prevMaxInd;
                    startInd = prevMaxInd;
                    while (prevMaxInd < polygon.Count - 1 && polygon[sortedInd[prevMaxInd + 1]].Y == y)
                    {
                        ++prevMaxInd;
                    }
                }

                AET.Sort();
                yield return (AET.Select(ptr => ptr.x).ToList(), y);

                AET.ForEach((ptr) => ptr.UpdateX());
                AET.RemoveAll((ptr) => ptr.yMax == y);
            }
        }
    }

    class AETPointer : IComparable<AETPointer>
    {
        public int yMax;
        public int x;
        public double _m;
        public double accumulation;

        public AETPointer(double yMax, double x, double m)
        {
            this.yMax = (int)yMax;
            this.x = (int)x;
            _m = 1.0 / m;
            accumulation = 0.0d;
        }

        public void UpdateX()
        {
            accumulation += _m;
            int val = (int)accumulation;
            if (val != 0)
            {
                accumulation = 0.0d;
                x += val;
            }
        }

        public int CompareTo(AETPointer other)
        {
            var xCmp = x.CompareTo(other.x);
            return xCmp == 0 ? yMax.CompareTo(other.yMax) : xCmp;
        }
    }
}
