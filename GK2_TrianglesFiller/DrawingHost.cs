using GK2_TrianglesFiller.DrawingRes;
using GK2_TrianglesFiller.VertexRes;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static GK2_TrianglesFiller.Resources.Configuration;

namespace GK2_TrianglesFiller
{
    public class DrawingHost : FrameworkElement
    {
        private readonly DrawingGroup backingStore;

        private readonly Background background;
        private readonly TriangleGrid triangleGrid;

        public Rect Rect { get; }
        private double realHeight;
        private double realWidth;

        private Vertex currentlyHold;
        private Color currentColor;


        public DrawingHost(Rect rect, Color color)
        {
            this.MouseDown += Host_MouseDown;
            this.MouseMove += Host_MouseMove;
            this.MouseUp += Host_MouseUp;
            backingStore = new DrawingGroup();

            triangleGrid = new TriangleGrid(rect);
            realHeight = (triangleGrid.Rows - 1) * SideLength;
            realWidth = (triangleGrid.Cols - 1) * SideLength;
            this.Rect = new Rect(rect.Left, rect.Top, realWidth, realHeight);

            WriteableBitmap bitmap = new WriteableBitmap((int)realWidth, (int)realHeight, DPI, DPI, pixelFormat, null);
            background = new Background(bitmap, triangleGrid.Grid, Rect);

            currentColor = color;
            background.FillGrid(currentColor);
        }

        private void Host_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this);
            currentlyHold = triangleGrid.FindVertex(pos);
            if (currentlyHold != null)
            {
                Mouse.Capture(this);
                background.Clear();
            }
        }

        private void Host_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentlyHold != null)
            {
                var currPos = e.GetPosition(this);
                if (currPos.X < realWidth && currPos.Y < realHeight && currPos.X > Rect.Left && currPos.Y > Rect.Top)
                {
                    currentlyHold.Point = currPos;
                }
            }
        }

        private void Host_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentlyHold != null)
            {
                var currPos = e.GetPosition(this);
                Mouse.Capture(null);
                if (currPos.X < realWidth && currPos.Y < realHeight && currPos.X > Rect.Left && currPos.Y > Rect.Top)
                {
                    currentlyHold.Point = currPos;
                }
                currentlyHold = null;
                UpdateBackground(currentColor);
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            Render();
            drawingContext.DrawDrawing(backingStore);
        }

        private void Render()
        {
            var drawingContext = backingStore.Open();
            Render(drawingContext);
            drawingContext.Close();
        }

        private void Render(DrawingContext drawingContext)
        {
            drawingContext.DrawDrawing(background.Drawing);
            if (DrawGrid)
            {
                drawingContext.DrawDrawing(triangleGrid.Drawing);
            }
        }


        public void UpdateBackground(Color color)
        {
            currentColor = color;
            background.FillGrid(color);
            Render();
        }
    }
}
