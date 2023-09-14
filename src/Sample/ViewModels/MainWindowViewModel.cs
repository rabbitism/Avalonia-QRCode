using Avalonia.Media;
using SkiaSharp;

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

        public string Data
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

        public MainWindowViewModel()
        {
            this.Data = "https://github.com/MikeCodesDotNET/Avalonia-QRCode";
            IconScale = 15;
            HasIcon = false;
        }


        private string data;

        private bool hasIcon;
        private SKImage? iconSource;
        private int iconScale;
        private Color color = Colors.Black;
        private Color spaceColor = Colors.White;
    }
}
