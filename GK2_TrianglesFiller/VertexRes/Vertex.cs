using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using GK2_TrianglesFiller.GeometryRes;
using GK2_TrianglesFiller.Resources;

namespace GK2_TrianglesFiller.VertexRes
{
    class Vertex : IComparable<Vertex>
    {
        public bool Locked { get; set; }
        public EllipseGeometry Ellipse { get; }
        public List<LineGeometry> Lines { get; }
        public List<LineGeometry> ReverseLines { get; }
        public Point Point
        {
            get => Ellipse.Center;
            set
            {
                if (!Locked)
                {
                    Ellipse.Center = value;
                    Lines.ForEach((line) => line.StartPoint = value);
                    ReverseLines.ForEach((line) => line.EndPoint = value);
                }
            }
        }
        public double X { get => Point.X; }
        public double Y { get => Point.Y; }

        public Vertex(Point point)
        {
            Ellipse = new EllipseGeometry(point, Configuration.VertexRadius, Configuration.VertexRadius);
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
                Ellipse.Center.Offset(x, y);
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
            return Point.X - other.Point.X < GeometryRes.Geometry.Eps ?
               Point.Y.CompareTo(other.Point.Y) :
                Point.X.CompareTo(other.Point.X);
        }

        public override bool Equals(object obj)
        {
            return obj is Vertex vertex &&
                   Point.Equals(vertex.Point);
        }

        public override int GetHashCode()
        {
            var hashCode = -1667306863;
            hashCode = hashCode * -1521134295 + Point.GetHashCode();
            return hashCode;
        }
    };

}
