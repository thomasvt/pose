using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pose.Persistence;
using Pose.Runtime.MonoGameDotNetCore.QuadRendering;
using Spritesheet = Pose.Persistence.Spritesheet;

namespace Pose.Runtime.MonoGameDotNetCore
{
    public static class ContentManagerExtensions
    {
        /// <summary>
        /// Loads a Pose document containing a skeleton and its animation(s) from a file created the Pose editor. Also automatically loads necessary resources (sprites).
        /// </summary>
        public static Document LoadPoseDocument(this ContentManager content, string filename)
        {
            var data = GetContentFileData(content, filename);
            return Document.Parser.ParseFrom(data);
        }

        public static QuadRendering.Spritesheet LoadSpritesheet(this ContentManager content, string filename)
        {
            var data = GetContentFileData(content, filename);
            var spritesheet = Spritesheet.Parser.ParseFrom(data);
            return new QuadRendering.Spritesheet(GetSpriteQuads(spritesheet));
        }

        private static SpriteQuad[] GetSpriteQuads(Spritesheet spritesheet)
        {
            // convert pixel range of individual sprites on the sheet to UV coords (percentages). We must point the UV's to the centers of the corner pixels, else we get color influence of adjacent sprites.
            return spritesheet.Sprites.Select(s => new SpriteQuad(s.Key, s.Width, s.Height, 
                new Vector2((s.X + 0.5f) / spritesheet.Width, (s.Y + 0.5f) / spritesheet.Height),
                new Vector2((s.X + s.Width-1f) / spritesheet.Width, (s.Y + s.Height-1f) / spritesheet.Height))).ToArray();
        }

        private static byte[] GetContentFileData(ContentManager content, string filename)
        {
            var fullPath = Path.Combine(Path.GetFullPath(content.RootDirectory), filename);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Content file not found: \"{fullPath}\".");
            var data = File.ReadAllBytes(fullPath);
            return data;
        }
    }
}
