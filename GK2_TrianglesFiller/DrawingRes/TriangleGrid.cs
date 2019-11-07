using GK2_TrianglesFiller.GeometryRes;
using GK2_TrianglesFiller.VertexRes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using static GK2_TrianglesFiller.Resources.Configuration;

namespace GK2_TrianglesFiller.DrawingRes
{
    class TriangleGrid
    {
        private List<List<Vertex>> grid;

        public TriangleGrid(Rect rect)
        {
            Drawing = new DrawingGroup();
            InitializeGrid(rect);
        }

        public int Rows { get => Grid.Count; }
        public int Cols { get => Grid.First().Count; }
        public List<List<Vertex>> Grid { get => grid; }
        public DrawingGroup Drawing { get; }

        private void InitializeGrid(Rect rect)
        {
            int rows = (int)Math.Floor(rect.Height / SideLength);
            int cols = (int)Math.Floor(rect.Width / SideLength);

            grid = new List<List<Vertex>>(rows);
            for (double heightIt = rect.Top; heightIt < rect.Bottom; heightIt += SideLength)
            {
                Grid.Add(new List<Vertex>(cols));
                for (double widthIt = rect.Left; widthIt < rect.Right; widthIt += SideLength)
                {
                    Vertex v1 = new Vertex(widthIt, heightIt);
                    v1.Locked = v1.X == rect.Left || v1.Y == rect.Top || heightIt + SideLength >= rect.Bottom;
                    Grid.Last().Add(v1);
                }
                Grid.Last().Last().Lock();
            }

            for (int i = 0; i < Grid.Count; ++i)
            {
                for (int j = 0; j < Grid[i].Count; ++j)
                {
                    Vertex v1 = Grid[i][j];
                    bool spaceOnBottom = i < Grid.Count - 1;
                    bool spaceOnRight = j < Grid[i].Count - 1;
                    if (spaceOnBottom)
                    {
                        var v2 = Grid[i + 1][j];
                        var line = v1.AddEdge(v2);
                        GeometryDrawing gd1 = new GeometryDrawing(null, VertexPen, line);
                        Drawing.Children.Add(gd1);
                    }
                    if (spaceOnBottom && spaceOnRight)
                    {
                        var v2 = Grid[i + 1][j + 1];
                        var line = v1.AddEdge(v2);
                        GeometryDrawing gd1 = new GeometryDrawing(null, VertexPen, line);
                        Drawing.Children.Add(gd1);
                    }
                    if (spaceOnRight)
                    {
                        var v2 = Grid[i][j + 1];
                        var line = v1.AddEdge(v2);
                        GeometryDrawing gd1 = new GeometryDrawing(null, VertexPen, line);
                        Drawing.Children.Add(gd1);
                    }
                }
            }
        }

        public Vertex FindVertex(Point pos)
        {
            int row = (int)Math.Floor(pos.Y / SideLength);
            int col = (int)Math.Floor(pos.X / SideLength);
            if (row >= Rows || col >= Cols || row < 0 || col < 0)
            {
                return null;
            }

            return pos.ArePointsIntersecting(Grid[row][col]) ? Grid[row][col] :
                   row < Rows - 1 && pos.ArePointsIntersecting(Grid[row + 1][col]) ? Grid[row + 1][col] :
                   col < Cols - 1 && pos.ArePointsIntersecting(Grid[row][col + 1]) ? Grid[row][col + 1] :
                   row < Rows - 1 && col < Cols - 1 && pos.ArePointsIntersecting(Grid[row + 1][col + 1]) ? Grid[row + 1][col + 1] :
                   SearchAll(pos);
        }

        private Vertex SearchAll(Point pos)
        {
            for (int i = 0; i < Grid.Count; ++i)
            {
                var closest = Grid[i].FirstOrDefault(v => pos.ArePointsIntersecting(v));
                if (closest != null)
                {
                    return closest;
                }
            }

            return null;
        }

    }
}
