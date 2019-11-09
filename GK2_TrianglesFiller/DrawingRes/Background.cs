using GK2_TrianglesFiller.GeometryRes;
using GK2_TrianglesFiller.VertexRes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static GK2_TrianglesFiller.Resources.Configuration;

namespace GK2_TrianglesFiller.DrawingRes
{

    class Background
    {
        private readonly List<List<Vertex>> grid;
        private readonly DrawingGroup drawingGroup;
        private readonly WriteableBitmap bitmap;

        private readonly byte[] buffer;
        private readonly Int32Rect dirtyRect;

        public Background(WriteableBitmap bitmap, List<List<Vertex>> grid, Rect rect)
        {
            drawingGroup = new DrawingGroup();
            Rect = rect;
            this.grid = grid;
            this.bitmap = bitmap;

            buffer = new byte[bitmap.PixelHeight * bitmap.BackBufferStride];
            dirtyRect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);

            Clear();
        }

        public Rect Rect { get; }

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
            for (int i = 0; i < buffer.Length; i += 4)
            {
                var (R, G, B) = ColorGenerator.ComputeColor(buffer[i + 2], buffer[i + 1], buffer[i]);
                buffer[i] = B;
                buffer[i + 1] = G;
                buffer[i + 2] = R;
            }
            FillGridFromBuffer();
        }

        public void FillGrid(Color color)
        {
            byte[] cValues = color.GetByteValue();
            for (int i = 0; i < buffer.Length; i += 4)
            {
                buffer[i] = cValues[0];
                buffer[i + 1] = cValues[1];
                buffer[i + 2] = cValues[2];
                buffer[i + 3] = cValues[3];
            }

            FillGridFromBuffer();
        }

        private void FillGridFromBuffer()
        {
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

        public void FillTriangle(List<Point> triangle)
        {
            var scanLine = new ScanLine(triangle);
            foreach (var (xList, y) in scanLine.GetIntersectionPoints())
            {
                if (y == bitmap.PixelHeight)
                {
                    continue;
                }

                for (int i = 0; i < xList.Count - 1; i += 2)
                {
                    int width = Math.Min(xList[i + 1], bitmap.PixelWidth) - xList[i];
                    bitmap.WritePixels(new Int32Rect(xList[i], y, width, 1), buffer, bitmap.BackBufferStride, y * bitmap.BackBufferStride + xList[i] * BytesPerPixel);

                }
            }
        }

        public void Clear()
        {
            bitmap.Clear(Colors.White);
        }
    }
}
