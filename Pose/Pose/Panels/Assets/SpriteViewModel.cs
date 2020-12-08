using System.Windows.Media.Imaging;
using Pose.Domain;

namespace Pose.Panels.Assets
{
    public class SpriteViewModel
    : ViewModel
    {
        private string _label;
        private BitmapImage _thumbnail;
        private string _filename;

        public string Label
        {
            get => _label;
            set
            {
                if (value == _label) return;
                _label = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage Thumbnail
        {
            get => _thumbnail;
            set
            {
                if (value == _thumbnail) return;
                _thumbnail = value;
                OnPropertyChanged();
            }
        }

        public double ThumbnailHeight => _thumbnail?.DecodePixelHeight ?? 0;

        public string Filename
        {
            get => _filename;
            set
            {
                if (value == _filename) return;
                _filename = value;
                OnPropertyChanged();
            }
        }

        public SpriteReference Sprite { get; internal  set; }
    }
}
