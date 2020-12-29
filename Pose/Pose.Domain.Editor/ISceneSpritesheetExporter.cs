using System.Collections.Generic;

namespace Pose.Domain.Editor
{
    /// <summary>
    /// Called when the <see cref="Editor"/> wants to save a spritesheet of a collection of sprites. The Editor domain has no access to graphical resources, so it requires this interface to be implemented elsewhere and be injected.
    /// </summary>
    public interface ISceneSpritesheetExporter
    {
        /// <summary>
        /// Creates and saves a single png spritesheet from a list of sprites.
        /// </summary>
        /// <param name="absoluteFilenameNoExt">The full path of the spritesheet file without extension (we create a .png and .json file by appending those extensions to this argument)</param>
        void ExportUsedSprites(IEnumerable<SpriteReference> spriteRefs, string filenamePng, string filenameJson);
    }
}