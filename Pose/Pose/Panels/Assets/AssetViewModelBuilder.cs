using System.IO;
using Pose.Domain;
using Pose.Domain.Editor;

namespace Pose.Panels.Assets
{
    public class AssetViewModelBuilder : IAssetViewModelBuilder
    {
        private readonly IThumbnailLoader _thumbnailLoader;
        private readonly Editor _editor;

        public AssetViewModelBuilder(IThumbnailLoader thumbnailLoader, Editor editor)
        {
            _thumbnailLoader = thumbnailLoader;
            _editor = editor;
        }

        public SpriteViewModel Build(SpriteReference sprite)
        {
            var filename = _editor.CurrentDocument.GetAbsoluteAssetPath(sprite.RelativePath);
            return new SpriteViewModel
            {
                Sprite = sprite,
                Label = Path.GetFileNameWithoutExtension(sprite.RelativePath),
                Thumbnail = _thumbnailLoader.LoadThumbnail(filename, 40),
                Filename = filename
            };
        }
    }
}
