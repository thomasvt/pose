using System.Windows.Media.Imaging;

namespace Pose.Panels.Assets
{
    public interface IThumbnailLoader
    {
        BitmapImage LoadThumbnail(string filename, int height);
    }
}