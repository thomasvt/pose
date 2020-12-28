using System.Collections.Generic;

namespace Pose.SpritePacker
{
    public class Spritesheet
    {
        public int Width { get; }
        public int Height { get; }
        public List<PlacedSprite> Sprites { get; }

        internal Spritesheet(int width, int height, List<PlacedSprite> sprites)
        {
            Width = width;
            Height = height;
            Sprites = sprites;
        }
    }
}
