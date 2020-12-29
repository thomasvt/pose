using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Pose.SceneEditor
{
    /// <summary>
    /// Stores and caches the WPF bitmap resources linked to files.
    /// </summary>
    public class SpriteBitmapStore
    {
        private readonly Dictionary<string, SpriteBitmap> _sprites;
        private string _assetFolder;

        public SpriteBitmapStore()
        {
            _sprites = new Dictionary<string, SpriteBitmap>();
        }

        public SpriteBitmap Get(string relativePath)
        {
            if (_sprites.TryGetValue(relativePath, out var sprite))
                return sprite;

            var bitmapImage = LoadSpriteBitmap(relativePath);
            sprite = new SpriteBitmap(bitmapImage);
            _sprites.Add(relativePath, sprite);
            return sprite;
        }

        private BitmapImage LoadSpriteBitmap(string relativePath)
        {
            var absolutePath = GetAbsoluteAssetPath(relativePath);

            if (!File.Exists(absolutePath)) 
                absolutePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Assets\\sprite-missing.png");

            return new BitmapImage(new Uri(absolutePath, UriKind.Absolute));
        }

        public void ChangeAssetFolder(string assetFolder)
        {
            _assetFolder = assetFolder;
            foreach (var kvp in _sprites)
            {
                kvp.Value.RefreshResources(LoadSpriteBitmap(kvp.Key));
            }
        }

        public string GetAbsoluteAssetPath(string path)
        {
            return Path.Combine(_assetFolder, path);
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
