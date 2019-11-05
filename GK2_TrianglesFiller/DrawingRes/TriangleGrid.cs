using GK2_TrianglesFiller.GeometryRes;
using GK2_TrianglesFiller.VertexRes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using static GK2_TrianglesFiller.Resources.Configuration;

namespace GK2_TrianglesFiller.DrawingRes
{
    class TriangleGrid : FrameworkElement
    {
        private readonly DrawingGroup drawingGroup;
        private List<List<Vertex>> grid;
        private Vertex currentlyHold;

        public TriangleGrid(Rect rect)
        {
            this.MouseDown += TriangleGrid_MouseDown;
            this.MouseMove += TriangleGrid_MouseMove;
            this.MouseUp += TriangleGrid_MouseUp;
            this.SizeChanged += TriangleGrid_SizeChanged;

            drawingGroup = new DrawingGroup();
            grid = TriangleDrawing.GetGrid(new Rect(20, 20, 1024, 1024));

            TriangleDrawing.DrawGrid(drawingGroup, grid);
        }



        private void TriangleGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void TriangleGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Console.WriteLine("Capture: " + e.GetPosition(this));
            var pos = e.GetPosition(this);
            for (int i = 0; i < grid.Count; ++i)
            {
                var closest = grid[i].FirstOrDefault(v => PointGeometry.ArePointsIntersecting(v, pos, VertexRadius));
                if (closest != null)
                {
                    Mouse.Capture(this);
                    currentlyHold = closest;
                    break;
                }
            }

        }

        private void TriangleGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentlyHold != null)
            {
                currentlyHold.Point = e.GetPosition(this);
            }
        }

        private void TriangleGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentlyHold != null)
            {
                Mouse.Capture(null);
                Console.WriteLine("here");
                currentlyHold.Point = e.GetPosition(this);
                currentlyHold = null;
            }

            Console.WriteLine("Leave: " + e.GetPosition(this));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawDrawing(drawingGroup);
        }
    }
}
