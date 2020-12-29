using System.Collections.Generic;
using System.Linq;

namespace Pose.Runtime.MonoGameDotNetCore.Rendering
{
    public class Spritesheet
    {
        internal Spritesheet(SpriteQuad[] sprites)
        {
            Sprites = sprites.ToDictionary(s => s.Key);
        }

        public IReadOnlyDictionary<string, SpriteQuad> Sprites { get; }
    }
}
