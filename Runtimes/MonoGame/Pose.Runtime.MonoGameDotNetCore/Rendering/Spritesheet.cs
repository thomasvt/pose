using System.Collections.Generic;
using System.Linq;

namespace Pose.Runtime.MonoGameDotNetCore.Rendering
{
    public class Spritesheet
    {
        internal Spritesheet(Sprite[] sprites)
        {
            Sprites = sprites.ToDictionary(s => s.Key);
        }

        public IReadOnlyDictionary<string, Sprite> Sprites { get; }
    }
}
