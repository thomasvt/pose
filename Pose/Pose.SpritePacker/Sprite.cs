namespace Pose.SpritePacker
{
    public class Sprite
    {
        public int Width { get; }
        public int Height { get; }
        public object Reference { get; }
        public Sprite(object reference, int width, int height)
        {
            Reference = reference;
            Width = width;
            Height = height;
        }
    }
}
