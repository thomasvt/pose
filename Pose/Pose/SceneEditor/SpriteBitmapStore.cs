using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Pose.Domain.Editor;

namespace Pose.SceneEditor
{
    /// <summary>
    /// Stores and caches the WPF bitmap resources linked to files.
    /// </summary>
    public class SpriteBitmapStore
    {
        private readonly Editor _editor;
        private readonly Dictionary<string, SpriteBitmap> _sprites;

        public SpriteBitmapStore(Editor editor)
        {
            _editor = editor;
            _sprites = new Dictionary<string, SpriteBitmap>();
        }

        public SpriteBitmap Get(string relativePath)
        {
            if (_sprites.TryGetValue(relativePath, out var sprite))
                return sprite;

            LoadSpriteBitmap(relativePath, out var bitmapImage, out var bitmap);
            sprite = new SpriteBitmap(bitmapImage, bitmap);
            _sprites.Add(relativePath, sprite);
            return sprite;
        }

        private void LoadSpriteBitmap(string relativePath, out BitmapImage bitmapImage, out Bitmap bitmap)
        {
            var absolutePath = _editor.CurrentDocument.GetAbsoluteAssetPath(relativePath);

            if (!File.Exists(absolutePath))
                absolutePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                    "Assets\\sprite-missing.png");

            bitmapImage = new BitmapImage(new Uri(absolutePath, UriKind.Absolute));
            bitmap = new Bitmap(absolutePath);
        }

        public void ReloadAll()
        {
            foreach (var kvp in _sprites)
            {
                LoadSpriteBitmap(kvp.Key, out var bitmapImage, out var bitmap);
                kvp.Value.RefreshResources(bitmapImage, bitmap);
            }
        }

        public void Clear()
        {
            foreach (var kvp in _sprites)
            {
                kvp.Value.Dispose();
            }
            _sprites.Clear();
        }
    }
}
