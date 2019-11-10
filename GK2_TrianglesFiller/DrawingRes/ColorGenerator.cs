using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GK2_TrianglesFiller.DrawingRes
{
    public static class ColorGenerator
    {
        public static (double R, double G, double B) LightColor { get; set; }
        public static Vector3D DefaultNormalVector { get; } = new Vector3D(0, 0, 255);
        public static Vector3D DefaultLightVersor { get; } = new Vector3D(0, 0, 255);

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
            var normalVector = normalVectorNullable ?? DefaultNormalVector;
            var lightVersor = lightVersorNullable ?? DefaultLightVersor;

            normalVector = GetRelativeVector(normalVector);
            normalVector.ComputeNormalVector();

            lightVersor = GetRelativeVector(lightVersor);

            double rVal = 1 * R * LightColor.R * Vector3D.DotProduct(normalVector, lightVersor);
            double gVal = 1 * G * LightColor.G * Vector3D.DotProduct(normalVector, lightVersor);
            double bVal = 1 * B * LightColor.B * Vector3D.DotProduct(normalVector, lightVersor);

            return ((byte)rVal, (byte)gVal, (byte)bVal);
        }

        private static Vector3D GetRelativeVector(Vector3D v)
        {
            return Vector3D.Divide(v, 255.0);
        }

        private static void ComputeNormalVector(this Vector3D v)
        {
            v.X = 2 * v.X - 1;
            v.Y = 2 * v.X - 1;
            v.Z = 2 * v.X - 1;
            v.Normalize();
        }
    }
}
