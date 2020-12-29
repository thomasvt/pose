using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using SkiaSharp;
using SkiaSharp.Views.WPF;

namespace Pose.SceneEditor
{
    /// <summary>
    /// The runtime bitmap representation of a sprite used for rendering and querying pixeldata.
    /// </summary>
    public class SpriteBitmap
    : IDisposable
    {
        // Note: because we cannot easily query individual pixels in WPF's BitmapImage, we keep a Skia Bitmap version of the same image too for all the application's pixel needs.

        /// <summary>
        /// The sprite image as a SKBitmap for use in Pose features that use Skia, and convenient querying of pixeldata.
        /// </summary>
        public SKBitmap Bitmap { get; private set; }

        /// <summary>
        /// The image as a BitmapImage for rendering in WPF. 
        /// </summary>
        public BitmapImage BitmapImage { get; private set; }

        public SpriteBitmap(BitmapImage bitmapImage)
        {
            BitmapImage = bitmapImage;
            Bitmap = bitmapImage.ToSKBitmap();
        }

        public SKColor GetPixelAtUv(Vector uv)
        {
            return Bitmap.GetPixel((int)(uv.X * Bitmap.Width), (int)(uv.Y * Bitmap.Height));
        }

        public void RefreshResources(BitmapImage bitmapImage)
        {
            BitmapImage = bitmapImage;
            Bitmap = bitmapImage.ToSKBitmap();
            ResourcesReloaded?.Invoke();
        }

        public event Action ResourcesReloaded;

        public void Dispose()
        {
            Bitmap?.Dispose();
        }
    }
}
