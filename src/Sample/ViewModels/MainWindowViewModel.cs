using Avalonia.Collections;
using Avalonia.Media;
using ReactiveUI;
using SkiaSharp;
using SkiaSharp.QrCode;
using System;
using System.IO;
using System.Reactive;

namespace Sample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public bool HasIcon
        {
            get => hasIcon;
            set
            {
                this.RaiseAndSetIfChanged(ref hasIcon, value);
                if (value)
                    IconSource = SKImage.FromEncodedData("Assets/bit.png");// Bitmap(AssetLoader.Open(new Uri("avares://Sample/Assets/bit.png")));
                else
                    IconSource = null;
            }
        }

        public string? Data
        {
            get => data;
            set => this.RaiseAndSetIfChanged(ref data, value);
        }

        public Color Color
        {
            get => color;
            set => this.RaiseAndSetIfChanged(ref color, value);
        }

        public Color SpaceColor
        {
            get => spaceColor;
            set => this.RaiseAndSetIfChanged(ref spaceColor, value);
        }

        public SKImage? IconSource
        {
            get => iconSource;
            set => this.RaiseAndSetIfChanged(ref iconSource, value);
        }

        public int IconScale
        {
            get => iconScale;
            set => this.RaiseAndSetIfChanged(ref iconScale, value);
        }

        public int QuietZoneSize
        {
            get => quietZoneSize;
            set => this.RaiseAndSetIfChanged(ref quietZoneSize, value);
        }

        public ECCLevel Level
        {
            get => level;
            set => this.RaiseAndSetIfChanged(ref level, value);
        }

        public AvaloniaList<ECCLevel> Levels
        {
            get => levels;
            set => this.RaiseAndSetIfChanged(ref levels, value);
        }

        public SKImage? QrImage
        {
            get => qrImage;
            set => this.RaiseAndSetIfChanged(ref qrImage, value);
        }

        public ReactiveCommand<Unit, Unit>? ExportCommand { get; }

        public MainWindowViewModel()
        {
            this.Data = "https://github.com/yangjieshao/Avalonia-QRCode";
            IconScale = 15;
            QuietZoneSize = 0;
            HasIcon = false;
            Level = ECCLevel.L;

            ExportCommand = ReactiveCommand.Create(Export);
        }

        private string? data;

        private bool hasIcon;
        private SKImage? iconSource;
        private SKImage? qrImage;
        private int iconScale;
        private int quietZoneSize;
        private Color color = Colors.Black;
        private ECCLevel level = ECCLevel.L;
        private Color spaceColor = Colors.White;
        private AvaloniaList<ECCLevel> levels = new() { ECCLevel.H, ECCLevel.M, ECCLevel.Q, ECCLevel.L };

        /// <summary>
        /// µ¼³ö¶þÎ¬ÂëÍ¼Æ¬
        /// </summary>
        private async void Export()
        {
            if (QrImage == null)
            {
                return;
            }

            using var newImage = SKBitmap.FromImage(QrImage);
            using var memStream = new MemoryStream();
            newImage.Encode(memStream, SKEncodedImageFormat.Png, 80);
            var buffer = memStream.ToArray();
            await File.WriteAllBytesAsync("qrCode.png", buffer);
            var path = Path.Combine(Environment.CurrentDirectory, "qrCode.png");
            System.Diagnostics.Process.Start("explorer", path);
        }
    }
}