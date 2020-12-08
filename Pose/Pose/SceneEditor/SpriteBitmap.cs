using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using Size = System.Windows.Size;

namespace Pose.SceneEditor
{
    /// <summary>
    /// The runtime bitmap representation of a sprite used for rendering and querying pixeldata.
    /// </summary>
    public class SpriteBitmap
    : IDisposable
    {
        // Note: because we cannot easily query pixels in WPF's BitmapImage (prolly because it's in videoram), we need to load the sprite in a Bitmap too. This class encapsulates both as one object.

        /// <summary>
        /// The image for querying the pixel data inside.
        /// </summary>
        private Bitmap _bitmap;

        /// <summary>
        /// The imagesource used for rendering in WPF.
        /// </summary>
        public BitmapImage BitmapImage { get; private set; }

        public Size Size => new Size(_bitmap.Width, _bitmap.Height);

        public SpriteBitmap(BitmapImage bitmapImage, Bitmap bitmap)
        {
            BitmapImage = bitmapImage;
            _bitmap = bitmap;
        }

        public Color GetPixelAtUv(Vector uv)
        {
            return _bitmap.GetPixel((int)(uv.X * _bitmap.Width), (int)(uv.Y * _bitmap.Height));
        }

        public void RefreshResources(BitmapImage bitmapImage, Bitmap bitmap)
        {
            BitmapImage = bitmapImage;
            _bitmap = bitmap;
            ResourcesReloaded?.Invoke();
        }

        public event Action ResourcesReloaded;

        public void Dispose()
        {
            _bitmap?.Dispose();
        }
    }
}
