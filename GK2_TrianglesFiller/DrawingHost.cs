using GK2_TrianglesFiller.DrawingRes;
using GK2_TrianglesFiller.VertexRes;
using System;
using System.Threading.Tasks;
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
        public int PixelHeight;
        public int PixelWidth;
        private Vertex currentlyHeld;

        private Color currentColor;
        private BitmapSource currentColorBitmap;

        private byte[] currentNormalMap;

        public DrawingHost(Rect rect)
        {
            this.MouseDown += Host_MouseDown;
            this.MouseMove += Host_MouseMove;
            this.MouseUp += Host_MouseUp;
            backingStore = new DrawingGroup();

            triangleGrid = new TriangleGrid(rect);
            PixelHeight = (int)((triangleGrid.Rows - 1) * SideLength) + 1;
            PixelWidth = (int)((triangleGrid.Cols - 1) * SideLength) + 1;
            this.Rect = new Rect(rect.Left, rect.Top, PixelWidth, PixelHeight);

            WriteableBitmap bitmap = new WriteableBitmap(PixelWidth, PixelHeight, DPI, DPI, MyPixelFormat, null);
            currentNormalMap = new byte[bitmap.PixelHeight * bitmap.BackBufferStride];
            background = new Background(bitmap, triangleGrid.Grid, Rect, currentNormalMap);

            currentColor = Colors.White;
            currentColorBitmap = GetScaledImage(DefaultImage);

            InitializeNormapBitmap(UseConstantVector ? null : DefaultNormalMap);
            if (UseConstantColor)
            {
                background.FillGrid(currentColor);
            }
            else
            {
                background.FillGrid(currentColorBitmap);
            }
        }

        private void Host_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this);
            currentlyHeld = triangleGrid.FindVertex(pos);
            if (currentlyHeld != null)
            {
                Mouse.Capture(this);
                background.Clear();
            }
        }

        private void Host_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentlyHeld != null)
            {
                var currPos = e.GetPosition(this);
                if (currPos.X < PixelWidth && currPos.Y < PixelHeight && currPos.X > Rect.Left && currPos.Y > Rect.Top)
                {
                    currentlyHeld.Point = currPos;
                }
            }
        }

        private void Host_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentlyHeld != null)
            {
                var currPos = e.GetPosition(this);
                Mouse.Capture(null);
                if (currPos.X < PixelWidth && currPos.Y < PixelHeight && currPos.X > Rect.Left && currPos.Y > Rect.Top)
                {
                    currentlyHeld.Point = currPos;
                }
                currentlyHeld = null;
                UpdateBackground();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            Render();
            drawingContext.DrawDrawing(backingStore);
        }

        public void Render()
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

        public void UpdateBackground()
        {
            if (UseConstantColor)
            {
                background.FillGrid(currentColor);
            }
            else
            {
                background.FillGrid(currentColorBitmap);
            }

            Render();

        }

        public void SetBackground(Color color)
        {
            currentColor = color;
            UpdateBackground();
        }

        public void SetBackground(BitmapImage image)
        {
            currentColorBitmap = GetScaledImage(image);
            UpdateBackground();
        }

        public void SetMap(BitmapImage img)
        {
            InitializeNormapBitmap(img);
            UpdateBackground();
        }

        private BitmapSource GetScaledImage(BitmapImage image)
        {
            var source = ConvertToTargetFormat(image);
            return FreezeImage(new TransformedBitmap(source, new ScaleTransform(PixelWidth / (double)source.PixelWidth, PixelHeight / (double)source.PixelHeight)));
        }

        private void InitializeNormapBitmap(BitmapImage image)
        {
            if (image == null)
            {
                FillBitmapWithConstantVector();
            }
            else
            {
                FillBitmapWithImage(image);
            }
        }

        private void FillBitmapWithConstantVector()
        {
            for (int i = 0; i < currentNormalMap.Length; i += 4)
            {
                currentNormalMap[i] = 255;
                currentNormalMap[i + 1] = 0;
                currentNormalMap[i + 2] = 0;
            }
        }

        private void FillBitmapWithImage(BitmapImage image)
        {
            BitmapSource source = ConvertToTargetFormat(image);
            int stride = source.PixelWidth * (BytesPerPixel);
            byte[] data = new byte[stride * source.PixelHeight];
            source.CopyPixels(data, stride, 0);

            CopyBytesFromSourceToBitmap(data, source.PixelWidth, source.PixelHeight);
        }

        private void CopyBytesFromSourceToBitmap(byte[] data, int sWidth, int sHeight)
        {
            int stride = sWidth * BytesPerPixel;
            for (int x = 0; x < PixelWidth; x += sWidth)
            {
                for (int y = 0; y < PixelHeight; y += sHeight)
                {
                    int currentStride = Math.Min(sWidth, PixelWidth - x) * BytesPerPixel;
                    int height = Math.Min(sHeight, PixelHeight - y) * stride;
                    int startPos = y * background.Stride + x * BytesPerPixel;
                    for (int rowIt = 0; rowIt < height; rowIt += stride)
                    {
                        Array.Copy(data, rowIt, currentNormalMap, startPos, currentStride);
                        startPos += background.Stride;
                    }
                }
            }
        }

        private BitmapSource FreezeImage(BitmapSource source)
        {
            if (source.CanFreeze)
            {
                source.Freeze();
            }

            return source;
        }

        private BitmapSource ConvertToTargetFormat(BitmapSource source)
        {
            if (source.Format != MyPixelFormat)
            {
                return new FormatConvertedBitmap(source, MyPixelFormat, null, 0);
            }

            return source;
        }
    }
}
