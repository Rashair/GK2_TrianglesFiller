using GK2_TrianglesFiller.GeometryRes;
using GK2_TrianglesFiller.Resources;
using GK2_TrianglesFiller.VertexRes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static GK2_TrianglesFiller.Resources.Configuration;

namespace GK2_TrianglesFiller.DrawingRes
{
    static class TriangleDrawing
    {
        public static DrawingVisual GetDrawingVisual(Rect rect)
        {
            var drawingVisual = new DrawingVisual();
            DrawingContext context = drawingVisual.RenderOpen();
            return drawingVisual;
        }

        public static List<List<Vertex>> GetGrid(Rect rect, double sideLength = Configuration.TriangleSideLength)
        {
            int rows = (int)Math.Floor(rect.Height / sideLength);
            int cols = (int)Math.Floor(rect.Width / sideLength);

            var grid = new List<List<Vertex>>(rows);
            for (double heightIt = rect.Top; heightIt < rect.Bottom; heightIt += sideLength)
            {
                grid.Add(new List<Vertex>(cols));
                for (double widthIt = rect.Left; widthIt < rect.Right; widthIt += sideLength)
                {
                    Vertex v1 = new Vertex(widthIt, heightIt);
                    v1.Locked = v1.X == rect.Left || v1.Y == rect.Top || heightIt + sideLength >= rect.Bottom;
                    grid.Last().Add(v1);
                }
                grid.Last().Last().Lock();
            }

            return grid;
        }

        public static void DrawGrid(DrawingGroup drawingGroup, List<List<Vertex>> grid)
        {
            for (int i = 0; i < grid.Count; ++i)
            {
                for (int j = 0; j < grid[i].Count; ++j)
                {
                    Vertex v1 = grid[i][j];
                    if (DrawVertices)
                    {
                        GeometryDrawing gd = new GeometryDrawing(VertexBrush, null, v1.Ellipse);
                        drawingGroup.Children.Add(gd);
                    }

                    bool spaceOnBottom = i < grid.Count - 1;
                    bool spaceOnRight = j < grid[i].Count - 1;
                    if (spaceOnBottom)
                    {
                        var v2 = grid[i + 1][j];
                        var line = v1.AddEdge(v2);
                        GeometryDrawing gd1 = new GeometryDrawing(null, VertexPen, line);
                        drawingGroup.Children.Add(gd1);
                    }
                    if (spaceOnBottom && spaceOnRight)
                    {
                        var v2 = grid[i + 1][j + 1];
                        var line = v1.AddEdge(v2);
                        GeometryDrawing gd1 = new GeometryDrawing(null, VertexPen, line);
                        drawingGroup.Children.Add(gd1);
                    }
                    if (spaceOnRight)
                    {
                        var v2 = grid[i][j + 1];
                        var line = v1.AddEdge(v2);
                        GeometryDrawing gd1 = new GeometryDrawing(null, VertexPen, line);
                        drawingGroup.Children.Add(gd1);
                    }
                }
            }
        }

        public static void FillGrid(this WriteableBitmap bitmap, List<List<Vertex>> grid)
        {
            for (int i = 0; i < grid.Count - 1; ++i)
            {
                for (int j = 0; j < grid[i].Count - 1; ++j)
                {
                    var lowerTriangle = new List<Vertex> { grid[i][j], grid[i + 1][j], grid[i + 1][j + 1] };
                    bitmap.FillTriangle(lowerTriangle);

                    var upperTriangle = new List<Vertex> { grid[i][j], grid[i][j + 1], grid[i + 1][j + 1] };
                    bitmap.FillTriangle(upperTriangle);
                }
            }
        }

        public static void FillTriangle(this WriteableBitmap bitmap, List<Vertex> triangle)
        {
            var scanLine = new ScanLine(triangle);

            foreach (var (xList, y) in scanLine.GetIntersectionPoints())
            {
                for (int i = 0; i < xList.Count - 1; i+= 2)
                {
                    bitmap.DrawLine(xList[i], y, xList[i + 1], y, Colors.Yellow);
                }
            }
        }

        public static void DrawPixel(this WriteableBitmap writeableBitmap, int x, int y)
        {
            int column = x;
            int row = y;

            try
            {
                // Reserve the back buffer for updates.
                writeableBitmap.Lock();

                unsafe
                {
                    // Get a pointer to the back buffer.
                    IntPtr pBackBuffer = writeableBitmap.BackBuffer;

                    // Find the address of the pixel to draw.
                    int positionShift = row * writeableBitmap.BackBufferStride + column * 4;
                    pBackBuffer += positionShift;

                    // Compute the pixel's color.
                    uint color_data = (uint)255 << 24; // A
                    color_data |= 255 << 16; // R
                    color_data |= 0 << 8;  // G
                    color_data |= 0 << 0;  // B

                    // Assign the color data to the pixel.
                    *((uint*)pBackBuffer) = color_data;
                }

                // Specify the area of the bitmap that changed.
                writeableBitmap.AddDirtyRect(new Int32Rect(column, row, 1, 1));
            }
            finally
            {
                // Release the back buffer and make it available for display.
                writeableBitmap.Unlock();
            }
        }
    }
}
