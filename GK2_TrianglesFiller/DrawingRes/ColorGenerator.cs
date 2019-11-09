using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GK2_TrianglesFiller.DrawingRes
{
    public static class ColorGenerator
    {
        public static (double R, double G, double B) LightColor { get; set; }
        public static Vector3D NormalVector { get; set; } = new Vector3D(0, 0, 1);
        public static Vector3D LightVersor { get; set; } = new Vector3D(0, 0, 1);

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

        public static (byte R, byte G, byte B) ComputeColor(byte R, byte G, byte B)
        {
            double rVal = 1 * R * LightColor.R;
            double gVal = 1 * G * LightColor.G;
            double bVal = 1 * B * LightColor.B;


            return ((byte)rVal, (byte)gVal, (byte)bVal);
        }
    }
}
