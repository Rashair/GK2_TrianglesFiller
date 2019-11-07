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

        public Background(WriteableBitmap bitmap, List<List<Vertex>> grid, Rect rect)
        {
            drawingGroup = new DrawingGroup();
            Rect = rect;
            this.grid = grid;
            this.bitmap = bitmap;

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

        public void FillGrid(BitmapImage img)
        {
            //IntPtr[] arr = new IntPtr[img.PixelWidth * img.PixelHeight];
            //img.CopyPixels(arr, 0, 0);
            //bitmap.WritePixels(img.SourceRect, arr, 0, 0);
        }

        public void FillGrid(Color color)
        {
            try
            {
                bitmap.Lock();
                for (int i = 0; i < grid.Count - 1; ++i)
                {
                    for (int j = 0; j < grid[i].Count - 1; ++j)
                    {
                        var lowerTriangle = new List<Point> { grid[i][j], grid[i + 1][j], grid[i + 1][j + 1] };
                        FillTriangle(lowerTriangle, color);

                        var upperTriangle = new List<Point> { grid[i][j], grid[i][j + 1], grid[i + 1][j + 1] };
                        FillTriangle(upperTriangle, color);
                    }
                }
                bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            }
            finally
            {
                bitmap.Unlock();
            }
        }

        public void FillTriangle(List<Point> triangle, Color color)
        {
            var scanLine = new ScanLine(triangle);
            uint colorVal = ((uint)color.A << 24) | ((uint)color.R << 16) | ((uint)color.G << 8) | (color.B);
            foreach (var (xList, y) in scanLine.GetIntersectionPoints())
            {
                if (y >= bitmap.PixelHeight)
                {
                    continue;
                }

                IntPtr pBackBuffer = bitmap.BackBuffer + y * bitmap.BackBufferStride;
                for (int i = 0; i < xList.Count - 1; i += 2)
                {
                    pBackBuffer += xList[i] * bytesPerPixel;
                    int endCol = Math.Min(xList[i + 1], bitmap.PixelWidth);
                    for (int x = xList[i]; x < endCol; ++x)
                    {
                        unsafe
                        {
                            *((uint*)pBackBuffer) = colorVal;
                        }
                        pBackBuffer += bytesPerPixel;
                    }
                }
            }
        }

        public void Clear()
        {
            bitmap.Clear(Colors.White);
        }
    }
}
