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
using System.Windows.Media.Imaging;
using static GK2_TrianglesFiller.Resources.Configuration;

namespace GK2_TrianglesFiller.DrawingRes
{
    class TriangleGrid : FrameworkElement
    {
        private readonly DrawingGroup gridDrawing;
        private List<List<Vertex>> grid;
        private WriteableBitmap bitmap;
        private Rect rect;
        private int rows;
        private int cols;

        private Vertex currentlyHold;

        public TriangleGrid(Rect rect)
        {
            this.rect = rect;
            this.MouseDown += TriangleGrid_MouseDown;
            this.MouseMove += TriangleGrid_MouseMove;
            this.MouseUp += TriangleGrid_MouseUp;


            gridDrawing = new DrawingGroup();
            grid = TriangleDrawing.GetGrid(rect);
            rows = grid.Count();
            cols = grid.First().Count();
            TriangleDrawing.DrawGrid(gridDrawing, grid);
            bitmap = new WriteableBitmap((int)rect.Width, (int)rect.Height, DPI, DPI, PixelFormats.Bgra32, null);
            bitmap.FillGrid(grid);
        }

        public void TriangleGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // TODO
            Console.WriteLine(e.NewSize);
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
                bitmap.FillTriangle(new List<Vertex> { currentlyHold.Parents[0], currentlyHold.Parents[2], currentlyHold});
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
            drawingContext.DrawImage(bitmap, rect);
            drawingContext.DrawDrawing(gridDrawing);
            //drawingContext.DrawImage(bitmap, rect);
        }
    }
}
