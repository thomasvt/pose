using System;
using System.Windows.Media.Imaging;

namespace Pose.Panels.Assets
{
    public class ThumbnailLoader : IThumbnailLoader
    {
        public BitmapImage LoadThumbnail(string filename, int height)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(filename);
            image.DecodePixelHeight = height;
            image.EndInit();
            image.Freeze();
            return image;
        }
    }
}
