using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace GK2_TrianglesFiller.VertexRes
{
    class Vertex : IComparable<Vertex>
    {
        public bool Locked { get; set; }

        public List<LineGeometry> Lines { get; }
        public List<LineGeometry> ReverseLines { get; }

        private Point point;
        public Point Point
        {
            get => point;
            set
            {
                if (!Locked)
                {
                    point = value;
                    Lines.ForEach((line) => line.StartPoint = value);
                    ReverseLines.ForEach((line) => line.EndPoint = value);
                }
            }
        }
        public double X { get => point.X; }
        public double Y { get => point.Y; }

        public Vertex(Point point)
        {
            this.point = point;
            Lines = new List<LineGeometry>();
            ReverseLines = new List<LineGeometry>();
        }

        public Vertex(double x, double y) : this(new Point(x, y))
        {
        }


        public static implicit operator Point(Vertex v)
        {
            return v.Point;
        }

        public void Offset(double x, double y)
        {
            if (!Locked)
            {
                point.Offset(x, y);
                Lines.ForEach((line) => line.StartPoint.Offset(x, y));
                ReverseLines.ForEach((line) => line.EndPoint.Offset(x, y));
            }
        }

        public void Offset(Point p)
        {
            Offset(p.X, p.Y);
        }

        public LineGeometry AddEdge(Vertex v2)
        {
            var line = new LineGeometry(this, v2);
            this.Lines.Add(line);
            v2.ReverseLines.Add(line);

            return line;
        }

        public void Lock()
        {
            Locked = true;
        }

        public int CompareTo(Vertex other)
        {
            return point.X - other.point.X < GeometryRes.Geometry.Eps ?
               point.Y.CompareTo(other.point.Y) :
                point.X.CompareTo(other.point.X);
        }

        public override bool Equals(object obj)
        {
            return obj is Vertex vertex &&
                   point.Equals(vertex.point);
        }

        public override int GetHashCode()
        {
            var hashCode = -1667306863;
            hashCode = hashCode * -1521134295 + point.GetHashCode();
            return hashCode;
        }
    };

}
