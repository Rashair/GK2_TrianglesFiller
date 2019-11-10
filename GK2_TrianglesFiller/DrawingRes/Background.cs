using GK2_TrianglesFiller.GeometryRes;
using GK2_TrianglesFiller.VertexRes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using static GK2_TrianglesFiller.Resources.Configuration;

namespace GK2_TrianglesFiller.DrawingRes
{

    class Background
    {
        private readonly List<List<Vertex>> grid;
        private readonly DrawingGroup drawingGroup;
        private readonly WriteableBitmap bitmap;
        private readonly byte[] normalMap;

        private readonly byte[] buffer;
        private readonly Int32Rect dirtyRect;
        private ColorGenerator defaultGenerator;
        private Random currentRandom;

        public Background(WriteableBitmap bitmap, List<List<Vertex>> grid, Rect rect, byte[] normalMap)
        {
            drawingGroup = new DrawingGroup();
            Rect = rect;
            this.grid = grid;
            this.bitmap = bitmap;
            this.normalMap = normalMap;

            buffer = new byte[bitmap.PixelHeight * bitmap.BackBufferStride];
            dirtyRect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);

            Clear();
        }

        public Rect Rect { get; }

        public int Stride { get => bitmap.BackBufferStride; }

        public DrawingGroup Drawing
        {
            get
            {
                var drawingContext = drawingGroup.Open();
                drawingContext.DrawImage(bitmap, Rect);
                drawingContext.Close();

                return drawingGroup;
            }
        }

        public void FillGrid(BitmapSource img)
        {
            img.CopyPixels(buffer, bitmap.BackBufferStride, 0);
            FillGridByTriangles();
        }

        public void FillGrid(Color color)
        {
            byte[] cValues = ColorGenerator.GetByteValue(color);
            for (int i = 0; i < buffer.Length; i += 4)
            {
                buffer[i] = cValues[0];
                buffer[i + 1] = cValues[1];
                buffer[i + 2] = cValues[2];
                buffer[i + 3] = cValues[3];
            }
            FillGridByTriangles();
        }


        private void FillGridByTriangles()
        {
            currentRandom = new Random();
            defaultGenerator = new ColorGenerator(Kd, Ks, M);
            try
            {
                bitmap.Lock();
                for (int i = 0; i < grid.Count - 1; ++i)
                {
                    for (int j = 0; j < grid[i].Count - 1; ++j)
                    {
                        var lowerTriangle = new List<Point> { grid[i][j], grid[i + 1][j], grid[i + 1][j + 1] };
                        FillTriangle(lowerTriangle);

                        var upperTriangle = new List<Point> { grid[i][j], grid[i][j + 1], grid[i + 1][j + 1] };
                        FillTriangle(upperTriangle);
                    }
                }
                bitmap.AddDirtyRect(dirtyRect);
            }
            finally
            {
                bitmap.Unlock();
            }
        }

        private ColorGenerator GetGenerator(List<Point> triangle)
        {
            var generator = SameForAll ? defaultGenerator :
                new ColorGenerator(currentRandom.NextDouble(), currentRandom.NextDouble(), currentRandom.Next(1, 101));

            if (FillColor != 1)
            {
                List<byte> vertexColors = new List<byte>(12);
                vertexColors.AddRange(GetColorFromPoint(triangle[0]));
                vertexColors.AddRange(GetColorFromPoint(triangle[1]));
                vertexColors.AddRange(GetColorFromPoint(triangle[2]));

                generator.SetColorsForInterpolation(vertexColors.ToArray());
            }

            return generator;
        }

        private byte[] GetColorFromPoint(Point p)
        {
            var shift = (int)p.Y * bitmap.BackBufferStride + (int)p.X * BytesPerPixel;
            return new byte[] { buffer[shift + 3], buffer[shift + 2], buffer[shift + 1], buffer[shift] };
        }


        public void FillTriangle(List<Point> triangle)
        {
            var scanLine = new ScanLine(triangle);
            var generator = GetGenerator(triangle);
            foreach (var (xList, y) in scanLine.GetIntersectionPoints())
            {
                if (y == bitmap.PixelHeight)
                {
                    continue;
                }

                FillRow(xList, y, generator);
            }
        }

        private void FillRow(List<int> xList, int y, ColorGenerator colorGenerator = null)
        {
            int rowShift = y * bitmap.BackBufferStride;
            for (int i = 0; i < xList.Count - 1; i += 2)
            {
                int currShift = rowShift + xList[i] * BytesPerPixel;
                IntPtr pBackBuffer = bitmap.BackBuffer + currShift;
                int endCol = Math.Min(xList[i + 1], bitmap.PixelWidth);
                for (int x = xList[i]; x < endCol; ++x)
                {
                    var (R, G, B) = colorGenerator.ComputeColor(buffer[currShift + 2], buffer[currShift + 1], buffer[currShift],
                        new Vector3D(normalMap[currShift + 2], normalMap[currShift + 1], normalMap[currShift]));
                    byte A = buffer[currShift + 3];
                    unsafe
                    {
                        *((int*)pBackBuffer) = (A << 24) | (R << 16) | (G << 8) | B;
                        pBackBuffer += BytesPerPixel;
                    }
                    currShift += BytesPerPixel;
                }
            }
        }

        public void Clear()
        {
            bitmap.Clear(Colors.White);
        }
    }
}
