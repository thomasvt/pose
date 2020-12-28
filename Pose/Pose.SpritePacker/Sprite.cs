namespace Pose.SpritePacker
{
    public class Sprite
    {
        public int Width { get; }
        public int Height { get; }
        public string Name { get; }
        public Sprite(string name, int width, int height)
        {
            Name = name;
            Width = width;
            Height = height;
        }
    }
}
