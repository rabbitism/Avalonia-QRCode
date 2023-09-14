using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;
using SkiaSharp.QrCode;
using System;
using System.Linq;

namespace Avalonia.QRCode
{
    public static class Extensions
    {
        /// <summary>
        /// Render the specified data into the given area of the target canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="area">The area.</param>
        /// <param name="data">The data.</param>
        /// <param name="qrColor">The color.</param>
        /// <param name="icon">The icon </param>
        /// <param name="iconScale">The icon scale</param>
        public static void Render(this QRCodeData data, SKCanvas canvas, SKRect area,SKColor? qrColor, SKColor? backgroundColor, SKImage? icon = null,int iconScale =10)
        {
            if (data != null)
            {
                using (var lightPaint = new SKPaint() { Color = (backgroundColor.HasValue ? backgroundColor.Value : SKColors.White), Style = SKPaintStyle.StrokeAndFill })
                using (var darkPaint = new SKPaint() { Color = (qrColor.HasValue ? qrColor.Value : SKColors.Black), Style = SKPaintStyle.StrokeAndFill })
                {

                    var rows = data.ModuleMatrix.Count;
                    var columns = data.ModuleMatrix.Select(x => x.Length).Max();
                    var cellHeight = area.Height / rows;
                    var cellWidth = area.Width / columns;

                    for (int y = 0; y < rows; y++)
                    {
                        var row = data.ModuleMatrix.ElementAt(y);
                        for (int x = 0; x < row.Length; x++)
                            canvas.DrawRect(SKRect.Create(area.Left + x * cellWidth, area.Top + y * cellHeight, cellWidth, cellHeight), (row[x] ? darkPaint : lightPaint));
                    }

                    if (icon != null
                        && iconScale > 0
                        && iconScale < 100)
                    {
                        var iconWidth = (area.Width / 100) * iconScale;
                        var iconHeight = (area.Height / 100) * iconScale;

                        var x = (area.Width / 2) - (iconWidth / 2);
                        var y = (area.Height / 2) - (iconHeight / 2);

                        //canvas.DrawBitmap(icon, SKRect.Create(x, y, iconWidth, iconHeight));
                        canvas.DrawImage(icon, SKRect.Create(x, y, iconWidth, iconHeight));
                    }
                }
            }
        }
    }

    internal class FuncCustomDrawOperation : ICustomDrawOperation
    {
        private readonly Action<SKCanvas, SKRect> _draw;

        public FuncCustomDrawOperation(SKRect skRect, Action<SKCanvas, SKRect> draw)
        {
            _draw = draw;
            Bounds = new Rect(skRect.Left, skRect.Top, skRect.Width, skRect.Height);
            SKRect = skRect;
        }

        public void Dispose()
        {
        }

        public Rect Bounds { get; }

        public SKRect SKRect { get; }

        public bool HitTest(Point p) => Bounds.Contains(p);

        public bool Equals(ICustomDrawOperation? other)
        {
            return object.ReferenceEquals(this, other);
        }

        public void Render(ImmediateDrawingContext context)
        {
            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            RenderLeaseFeature(leaseFeature);
        }

        private void RenderLeaseFeature(ISkiaSharpApiLeaseFeature? leaseFeature)
        {
            if (leaseFeature is { })
            {
                using var lease = leaseFeature.Lease();
                var canvas = lease?.SkCanvas;
                if (canvas is not null)
                {
                    _draw(canvas, SKRect);
                }
            }
        }
    }
}
