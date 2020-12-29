using System.Collections.Generic;
using System.Linq;
using Pose.Domain;
using Pose.Domain.Documents;
using Pose.Domain.Editor;
using Pose.SceneEditor;

namespace Pose.Spritesheets
{
    /// <summary>
    /// Creates spritesheets for the sprites used in a Pose scene (Document).
    /// </summary>
    public class SceneSpritesheetExporter : ISceneSpritesheetExporter
    {
        private readonly SpriteBitmapStore _spriteBitmapStore;

        public SceneSpritesheetExporter(SpriteBitmapStore spriteBitmapStore)
        {
            _spriteBitmapStore = spriteBitmapStore;
        }

        /// <summary>
        /// Creates and saves a single png spritesheet from the sprites used in a <see cref="Document"/>
        /// </summary>
        public void ExportUsedSprites(IEnumerable<SpriteReference> spriteRefs, string filenamePng, string filenameJson)
        {
            var sprites = spriteRefs.Select(spriteRef => new SpriteInfo(spriteRef.RelativePath, _spriteBitmapStore.Get(spriteRef.RelativePath).Bitmap));
            SpritesheetExporter.Export(sprites, filenamePng, filenameJson);
        }
    }
}
