using GK2_TrianglesFiller.GeometryRes;
using GK2_TrianglesFiller.Resources;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GK2_TrianglesFiller.DrawingRes
{
    public class ColorGenerator
    {
        public static (double R, double G, double B) LightColor { get; set; }
        public static Vector3D DefaultNormalVector { get; } = new Vector3D(0, 0, 255);
        public static Vector3D DefaultLightVersor { get; } = new Vector3D(0, 0, 255);
        public static Vector3D V { get; } = new Vector3D(0, 0, 1);

        public static int GetValue(Color c)
        {
            unchecked
            {
                return (c.A << 24) | (c.R << 16) | (c.G << 8) | (c.B);
            }
        }

        public static byte[] GetByteValue(Color c)
        {
            var arr = new byte[4];
            arr[0] = c.B;
            arr[1] = c.G;
            arr[2] = c.R;
            arr[3] = c.A;
            return arr;
        }

        private static Vector3D GetRelativeVector(Vector3D v)
        {
            return Vector3D.Divide(v, 255.0);
        }


        private double Kd;
        private double Ks;
        private int M;

        public ColorGenerator(double Kd, double Ks, int M)
        {
            this.Kd = Kd;
            this.Ks = Ks;
            this.M = M;
        }

        public (byte R, byte G, byte B) ComputeColor(byte R, byte G, byte B, Vector3D? normalVectorNullable = null, Vector3D? lightVersorNullable = null)
        {
            var N = normalVectorNullable ?? DefaultNormalVector;
            var L = lightVersorNullable ?? DefaultLightVersor;

            N = GetRelativeVector(N);
            N.ComputeNormalVector();
            L = GetRelativeVector(L);
            Vector3D RVector = 2 * Vector3D.DotProduct(N, L) * N - L;

            double rVal = R * LightColor.R;
            double gVal = G * LightColor.G;
            double bVal = B * LightColor.B;

            var cos1 = Vector3D.DotProduct(N, L);
            var cos2 = Math.Pow(Vector3D.DotProduct(V, RVector), M);

            rVal = Kd * rVal * cos1 + Ks * rVal * cos2;
            gVal = Kd * gVal * cos1 + Ks * gVal * cos2;
            bVal = Kd * bVal * cos1 + Ks * bVal * cos2;

            return ((byte)rVal, (byte)gVal, (byte)bVal);
        }


        /// <summary>
        ///  Interpolation section
        /// </summary>
        private byte[] interpolationColors = null;
        private byte[] computedColors = null;
        private Vector3D[] interVectors = null;
        private List<Point> triangle = null;
        public void SetColorsForInterpolation(List<Point> triangle, byte[] colors, Vector3D[] normalVectors)
        {
            this.triangle = triangle;
            // ARGB
            computedColors = new byte[colors.Length];
            for (int i = 0; i < 9; i += 3)
            {
                var (R, G, B) = ComputeColor(colors[i], colors[i + 1], colors[i + 2], normalVectors[i / 3]);
                computedColors[i] = R;
                computedColors[i + 1] = G;
                computedColors[i + 2] = B;
            }

            interpolationColors = colors;
            interVectors = normalVectors;
        }


        public (byte R, byte G, byte B) ComputeInterpolatedColor(Point currPoint)
        {
            Vector3D distanceFromVertices = GetDistanceFromVertices(currPoint);
            byte R = ComputeSingleColor(distanceFromVertices, 0);
            byte G = ComputeSingleColor(distanceFromVertices, 1);
            byte B = ComputeSingleColor(distanceFromVertices, 2);

            return (R, G, B);
        }

        private Vector3D GetDistanceFromVertices(Point currPoint)
        {
            return new Vector3D(PointGeometry.Distance(currPoint, triangle[0]),
                    PointGeometry.Distance(currPoint, triangle[1]),
                    PointGeometry.Distance(currPoint, triangle[2])
            );
        }

        private byte ComputeSingleColor(Vector3D d, int i)
        {
            if (Configuration.FillColor == 2)
            {
                return (byte)((d.X * computedColors[i] +
                               d.Y * computedColors[i + 3] +
                               d.Z * computedColors[i + 6])
                               / (d.X + d.Y + d.Z));
            }
            else
            {
                var L = DefaultLightVersor;
                Vector3D N = GetInterpolatedVector(d);
                N = GetRelativeVector(N);
                N.ComputeNormalVector();
                L = GetRelativeVector(L);
                Vector3D RVector = 2 * Vector3D.DotProduct(N, L) * N - L;

                double val = (d.X * interpolationColors[i] + d.Y * interpolationColors[i + 3] + d.Z * interpolationColors[i + 6]) / (d.X + d.Y + d.Z) 
                    * GetLightColor(i);

                var cos1 = Vector3D.DotProduct(N, L);
                var cos2 = Math.Pow(Vector3D.DotProduct(V, RVector), M);

                return (byte)(Kd * val * cos1 + Ks * val * cos2);
            }
        }


        private double GetLightColor(int i)
        {
            return i % 3 == 0 ? LightColor.R :
                   i % 3 == 1 ? LightColor.G :
                   LightColor.B;
        }

        private Vector3D GetInterpolatedVector(Vector3D dist)
        {
            return ((dist.X * interVectors[0]) + (dist.Y * interVectors[1]) + (dist.Z * interVectors[2])) / (dist.X + dist.Y + dist.Z);
        }
    }
}
