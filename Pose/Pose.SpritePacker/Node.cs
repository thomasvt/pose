namespace Pose.SpritePacker
{
    internal class Node
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }
        public bool IsUsed { get; private set; }
        public Node Right { get; private set; }
        public Node Down { get; private set; }

        internal Node(int x, int y, int width, int height, Node right = null, Node down = null)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            IsUsed = right != null;
            Right = right;
            Down = down;
        }

        public Node FindEnoughSpace(in int width, in int height)
        {
            if (IsUsed)
            {
                return Right?.FindEnoughSpace(width, height) ?? Down?.FindEnoughSpace(width, height);
            }

            if (width <= Width && height <= Height)
            {
                return this;
            }

            return null;
        }


        public void Place(int width, int height)
        {
            IsUsed = true;
            Down = new Node(X, Y + height, Width, Height - height);
            Right = new Node(X + width, Y, Width - width, height);
        }
    }
}
