using SkiaSharp;

namespace Pose.Spritesheets
{
    public class SpriteInfo
    {
        public string Key { get; }
        public SKBitmap Bitmap { get; }

        public SpriteInfo(string key, SKBitmap bitmap)
        {
            Key = key;
            Bitmap = bitmap;
        }
    }
}
