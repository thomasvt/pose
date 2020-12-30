using System.Linq;
using Microsoft.Xna.Framework;
using Pose.Runtime.MonoGameDotNetCore.Rendering;
using Spritesheet = Pose.Persistence.Spritesheet;

namespace Pose.Runtime.MonoGameDotNetCore
{
    internal static class SpritesheetMapper
    {
        internal static Rendering.Spritesheet MapSpritesheet(Spritesheet spritesheet)
        {
            // convert pixel range of individual sprites on the sheet to UV coords (percentages). We must point the UV's to the centers of the corner pixels, else we get color influence of adjacent sprites.
            var sprites = spritesheet.Sprites.Select(s => new Sprite(s.Key, s.Width, s.Height, 
                new Vector2((s.X + 0.5f) / spritesheet.Width, (s.Y + 0.5f) / spritesheet.Height),
                new Vector2((s.X + s.Width-1f) / spritesheet.Width, (s.Y + s.Height-1f) / spritesheet.Height))).ToArray();
            return new Rendering.Spritesheet(sprites);
        }
    }
}