using GK2_TrianglesFiller.Resources;
using GK2_TrianglesFiller.VertexRes;
using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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
            int rows = (int)Math.Ceiling(rect.Height / sideLength);
            int cols = (int)Math.Ceiling(rect.Width / sideLength);

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
                    GeometryDrawing gd = new GeometryDrawing(VertexBrush, null, v1.Ellipse);
                    drawingGroup.Children.Add(gd);

                    bool spaceOnBottom = i < grid.Count - 1;
                    bool spaceOnRight = j < grid[i].Count - 1;
                    if (spaceOnBottom)
                    {
                        var v2 = grid[i + 1][j];
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
                    if (spaceOnBottom && spaceOnRight)
                    {
                        var v2 = grid[i + 1][j + 1];
                        var line = v1.AddEdge(v2);
                        GeometryDrawing gd1 = new GeometryDrawing(null, VertexPen, line);
                        drawingGroup.Children.Add(gd1);
                    }
                }
            }
        }
    }
}
