using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using static GK2_TrianglesFiller.Resources.Configuration;

namespace GK2_TrianglesFiller.DrawingRes
{
    public static class ColorGenerator
    {
        public static (double R, double G, double B) LightColor { get; set; }
        public static Vector3D DefaultNormalVector { get; } = new Vector3D(0, 0, 255);
        public static Vector3D DefaultLightVersor { get; } = new Vector3D(0, 0, 255);

        public static Vector3D V { get; } = new Vector3D(0, 0, 1);

        public static int GetValue(this Color c)
        {
            unchecked
            {
                return (c.A << 24) | (c.R << 16) | (c.G << 8) | (c.B);
            }
        }

        public static byte[] GetByteValue(this Color c)
        {
            var arr = new byte[4];
            arr[0] = c.B;
            arr[1] = c.G;
            arr[2] = c.R;
            arr[3] = c.A;
            return arr;
        }

        public static (byte R, byte G, byte B) ComputeColor(byte R, byte G, byte B, Vector3D? normalVectorNullable = null, Vector3D? lightVersorNullable = null)
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

        private static Vector3D GetRelativeVector(Vector3D v)
        {
            return Vector3D.Divide(v, 255.0);
        }

        private static void ComputeNormalVector(this Vector3D v)
        {
            v.X = 2 * v.X - 1;
            v.Y = 2 * v.Y - 1;
            v.Z = 2 * v.Z - 1;
            v.Normalize();
        }
    }
}
