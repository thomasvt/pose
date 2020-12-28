namespace Pose.SpritePacker
{
    public class PlacedSprite
    {
        public int X { get; }
        public int Y { get; }
        public Sprite Sprite { get; }

        internal PlacedSprite(int x, int y, Sprite sprite)
        {
            X = x;
            Y = y;
            Sprite = sprite;
        }
    }
}
