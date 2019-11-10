﻿using GK2_TrianglesFiller.DrawingRes;
using GK2_TrianglesFiller.VertexRes;
using System;
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
            realHeight = (triangleGrid.Rows - 1) * SideLength;
            realWidth = (triangleGrid.Cols - 1) * SideLength;
            this.Rect = new Rect(rect.Left, rect.Top, realWidth, realHeight);

            WriteableBitmap bitmap = new WriteableBitmap((int)realWidth, (int)realHeight, DPI, DPI, GK2_TrianglesFiller.Resources.Configuration.MyPixelFormat, null);
            background = new Background(bitmap, triangleGrid.Grid, Rect);

            currentColor = Colors.Yellow;
            currentColorBitmap = GetScaledImage(DefaultImage);

            currentNormalMap = GetBytesFromBitmap(GetRepeatedImage(DefaultNormalMap));

            background.FillGrid(currentColorBitmap, currentNormalMap);
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
                if (currPos.X < realWidth && currPos.Y < realHeight && currPos.X > Rect.Left && currPos.Y > Rect.Top)
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
                if (currPos.X < realWidth && currPos.Y < realHeight && currPos.X > Rect.Left && currPos.Y > Rect.Top)
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
            if (ObjectColorFromTexture)
            {
                if (UseConstantVector)
                {
                    background.FillGrid(currentColorBitmap);
                }
                else
                {
                    background.FillGrid(currentColorBitmap, currentNormalMap);
                }
            }
            else
            {
                if (UseConstantVector)
                {
                    background.FillGrid(currentColor);
                }
                else
                {
                    background.FillGrid(currentColor, currentNormalMap);
                }
            }
            Render();
        }

        public void SetBackground(Color color)
        {
            ObjectColorFromTexture = false;
            currentColor = color;
            UpdateBackground();
        }

        public void SetBackground(BitmapImage image)
        {
            ObjectColorFromTexture = true;
            currentColorBitmap = GetScaledImage(image);
            UpdateBackground();
        }


        private BitmapSource GetScaledImage(BitmapImage image)
        {
            BitmapSource source = image;
            if (source.Format != MyPixelFormat)
            {
                source = new FormatConvertedBitmap(source, MyPixelFormat, null, 0);
            }
            return FreezeImage(new TransformedBitmap(source, new ScaleTransform(realWidth / source.PixelWidth, realHeight / source.PixelHeight)));
        }

        private BitmapSource GetRepeatedImage(BitmapImage image)
        {
            BitmapSource source = image;
            if (source.Format != MyPixelFormat)
            {
                source = new FormatConvertedBitmap(source, MyPixelFormat, null, 0);
            }

            int stride = source.PixelWidth * (BytesPerPixel);
            byte[] data = new byte[stride * source.PixelHeight];
            source.CopyPixels(data, stride, 0);

            WriteableBitmap target = new WriteableBitmap(
                (int)realWidth,
                (int)realHeight,
                DPI, DPI,
                MyPixelFormat, null
            );

            for (int x = 0; x <= target.PixelWidth; x += source.PixelWidth)
            {
                for (int y = 0; y <= target.PixelHeight; y += source.PixelHeight)
                {
                    int width = Math.Min(source.PixelWidth, target.PixelWidth - x);
                    int height = Math.Min(source.PixelHeight, target.PixelHeight - y);
                    target.WritePixels(
                        new Int32Rect(x, y, width, height),
                        data, stride, 0
                    );
                }
            }

            return FreezeImage(target);
        }

        private byte[] GetBytesFromBitmap(BitmapSource source)
        {
            int stride = source.PixelWidth * BytesPerPixel;
            byte[] result = new byte[stride * source.PixelHeight];
            source.CopyPixels(result, stride, 0);
            return result;
        }

        private BitmapSource FreezeImage(BitmapSource source)
        {
            if (source.CanFreeze)
            {
                source.Freeze();
            }

            return source;
        }
    }
}
