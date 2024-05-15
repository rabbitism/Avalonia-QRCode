using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;
using SkiaSharp;
using SkiaSharp.QrCode;
using System;

namespace Avalonia.QRCode
{
    public class QRCode : Control
    {
        public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<QRCode, Color>(nameof(Color), Brushes.Black.Color, coerce: (sender, e) =>
        {
            if (sender is not QRCode control)
            {
                return e;
            }

            control.UpdataQrImage(control.Data,e, control.SpaceColor, control.Icon, control.IconScale);
            return e;
        });

        public static readonly StyledProperty<Color> SpaceColorProperty = AvaloniaProperty.Register<QRCode, Color>(nameof(SpaceColor), Brushes.White.Color, coerce: (sender, e) =>
        {
            if (sender is not QRCode control)
            {
                return e;
            }

            control.UpdataQrImage(control.Data, control.Color, e, control.Icon, control.IconScale);
            return e;
        });

        public static readonly StyledProperty<string> DataProperty = AvaloniaProperty.Register<QRCode, string>(nameof(Data), string.Empty, coerce: (sender, e) =>
        {
            if (sender is not QRCode control)
            {
                return e;
            }

            control.UpdataQrImage(e, control.Color, control.SpaceColor, control.Icon, control.IconScale);
            return e;
        });

        public static readonly StyledProperty<SKImage> IconProperty = AvaloniaProperty.Register<QRCode, SKImage>(nameof(Icon), coerce: (sender, e) =>
        {
            if (sender is not QRCode control)
            {
                return e;
            }

            control.UpdataQrImage(control.Data, control.Color, control.SpaceColor, e, control.IconScale);
            return e;
        });

        public static readonly StyledProperty<int> IconScaleProperty = AvaloniaProperty.Register<QRCode, int>(nameof(IconScale), 15, coerce: (sender, e) =>
        {
            if (sender is not QRCode control)
            {
                return e;
            }

            control.UpdataQrImage(control.Data, control.Color, control.SpaceColor, control.Icon, e);
            return e;
        });

        /// <summary>
        /// Value from 1-99. Sets how much % of the QR Code will be covered by the icon
        /// </summary>
        public int IconScale
        {
            get { return GetValue(IconScaleProperty); }
            set
            {
                if(value < 1)
                    value = 1;
                if(value > 99)
                    value = 99;

                SetValue(IconScaleProperty, value);
            }
        }

        /// <summary>
        /// If null, then ignored. If set, the Bitmap is drawn in the middle of the QR Code
        /// </summary>
        public SKImage Icon
        {
            get { return GetValue(IconProperty); }
            set 
            { 
                SetValue(IconProperty, value);
            }
        }

        [Content]
        public string Data
        {
            get { return GetValue(DataProperty); }
            set 
            { 
                SetValue(DataProperty, value);
            }
        }

        /// <summary>
        /// The color of the light/white modules
        /// </summary>
        public Color SpaceColor
        {
            get { return GetValue(SpaceColorProperty); }
            set 
            {
                SetValue(SpaceColorProperty, value);
            }
        }

        /// <summary>
        /// The color of the dark/black modules
        /// </summary>
        public Color Color
        {
            get { return GetValue(ColorProperty); }
            set 
            { 
                SetValue(ColorProperty, value);
            }
        }

        public static readonly AvaloniaProperty<Stretch> StretchProperty =
            AvaloniaProperty.Register<QRCode, Stretch>(
                nameof(Stretch), Stretch.Uniform);

        public static readonly AvaloniaProperty<StretchDirection> StretchDirectionProperty =
            AvaloniaProperty.Register<QRCode, StretchDirection>(
                nameof(StretchDirection), StretchDirection.Both);

        public Stretch Stretch
        {
            get => (Stretch)this.GetValue(StretchProperty)!;
            set => this.SetValue(StretchProperty, value);
        }

        public StretchDirection StretchDirection
        {
            get => (StretchDirection)this.GetValue(StretchDirectionProperty)!;
            set => this.SetValue(StretchDirectionProperty, value);
        }

        public QRCode()
        {
            AffectsRender<QRCode>(DataProperty, ColorProperty, SpaceColorProperty, IconProperty, IconScaleProperty,  StretchProperty, StretchDirectionProperty);
            AffectsMeasure<QRCode>(DataProperty, StretchProperty, StretchDirectionProperty);
            AffectsArrange<QRCode>(DataProperty, StretchProperty, StretchDirectionProperty);
        }

        private Size RenderSize => this.Bounds.Size;

        private Size SourceSize { set; get; } =new Size(512, 512);

        protected override Size MeasureOverride(Size constraint)
        {
            if (!string.IsNullOrWhiteSpace(Data))
            {
                return Stretch.CalculateSize(constraint, SourceSize, StretchDirection);
            }
            else
            {
                return default;
            }
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (!string.IsNullOrWhiteSpace(Data))
            {
                return Stretch.CalculateSize(arrangeSize, SourceSize);
            }
            else
            {
                return default;
            }
        }

        private SKImage? QrImage;

        private readonly QRCodeGenerator qrGenerator = new ();
        private readonly object _sync = new();

        /// <summary>
        /// 更新二维码图片
        /// </summary>
        private void UpdataQrImage(string data,Color color,Color spaceColor, SKImage icon,int iconScale)
        {
            lock (_sync)
            {
                if (string.IsNullOrWhiteSpace(data))
                {
                    QrImage = null;
                    return;
                }
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(plainText:Data, eccLevel: ECCLevel.L, quietZoneSize:0);

                var info = new SKImageInfo(Convert.ToInt32( SourceSize.Width),Convert.ToInt32(SourceSize.Height));
                using var surface = SKSurface.Create(info);
                var canvas = surface.Canvas;

                qrCodeData.Render(canvas, new SKRect(0, 0, info.Width, info.Height)
                    , new SKColor(color.R, color.G, color.B, color.A)
                    , new SKColor(spaceColor.R, spaceColor.G, spaceColor.B, spaceColor.A)
                    , icon, iconScale);
                QrImage = surface.Snapshot();
            }
        }

        public override void Render(DrawingContext drawingContext)
        {
            if(!string.IsNullOrEmpty(Data))
            {
                Size sourceSize = SourceSize;

                var viewPort = new Rect(RenderSize);
                var scale = Stretch.CalculateScaling(RenderSize, sourceSize, StretchDirection);
                var scaledSize = sourceSize * scale;
                var destRect = viewPort
                    .CenterRect(new Rect(scaledSize))
                    .Intersect(viewPort);
                var sourceRect = new Rect(sourceSize)
                    .CenterRect(new Rect(destRect.Size / scale));

                //var bounds = SKRect.Create(new SKPoint(), new SKSize { Height = (float)SKRect.Create(new SKPoint(), new SKSize { Height = (float)sourceSize.Height, Width = (float)sourceSize.Width }).Height, Width = (float)sourceSize.Width });
                var bounds = SKRect.Create(0f, 0f, (float)sourceSize.Width, (float)sourceSize.Height);
                var scaleMatrix = Matrix.CreateScale(
                    destRect.Width / sourceRect.Width,
                    destRect.Height / sourceRect.Height);
                var translateMatrix = Matrix.CreateTranslation(
                    -sourceRect.X + destRect.X - bounds.Top,
                    -sourceRect.Y + destRect.Y - bounds.Left);

                using (drawingContext.PushClip(destRect))
                using (drawingContext.PushTransform(translateMatrix * scaleMatrix))
                {
                    drawingContext.Custom(new FuncCustomDrawOperation(bounds, Draw));
                }
            }
            else
            {
                base.Render(drawingContext);
            }
        }

        private void Draw(SKCanvas canvas, SKRect rect)
        {
            lock (_sync)
            {
                if (QrImage is null)
                {
                    return;
                }
                canvas.Save();
                canvas.DrawImage(QrImage, rect, default);
                canvas.Restore();
            }
        }
    }
}
