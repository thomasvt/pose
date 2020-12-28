using System;
using System.Collections.Generic;
using System.Linq;

namespace Pose.SpritePacker
{
    public class SpritePacker
    {
        // translated from https://github.com/jakesgordon/bin-packing/blob/master/js/packer.growing.js

        private List<PlacedSprite> _placedSprites;
        private Node _root;

        public Spritesheet Pack(IEnumerable<Sprite> sprites)
        {
            var sortedSprites = sprites.OrderByDescending(s => Math.Max(s.Width, s.Height)).ToList();
            _placedSprites = new List<PlacedSprite>(sortedSprites.Count);
            var firstNode = sortedSprites.First();
            _root = new Node(0, 0, firstNode.Width, firstNode.Height);

            foreach (var sprite in sortedSprites)
            {
                Add(sprite);
            }

            return new Spritesheet(_root.Width, _root.Height, _placedSprites);
        }

        private void Add(Sprite sprite)
        {
            var node = _root.FindEnoughSpace(sprite.Width, sprite.Height) ?? Grow(sprite.Width, sprite.Height);

            node.Place(sprite.Width, sprite.Height);
            _placedSprites.Add(new PlacedSprite(node.X, node.Y, sprite));
        }

        /// <summary>
        /// Grow to create enough space for the requested size.
        /// </summary>
        private Node Grow(in int width, in int height)
        {
            var canGrowDown = width <= _root.Width;
            var canGrowRight = height <= _root.Height;

            var shouldGrowRight = canGrowRight && _root.Height >= _root.Width + width; // grow to stay close to square form
            var shouldGrowDown = canGrowDown && _root.Width >= _root.Height + height;

            if (shouldGrowRight)
                return GrowRight(width);
            if (shouldGrowDown)
                return GrowDown(height);
            if (canGrowRight)
                return GrowRight(width);
            if (canGrowDown)
                return GrowDown(height);

            throw new Exception("Cannot grow spritesheet. This is a bug.");
        }

        private Node GrowRight(in int width)
        {
            var down = _root;
            var right = new Node(down.Width, 0, width, down.Height);
            _root = new Node(0, 0, down.Width + width, down.Height, right, down);
            return right;
        }

        private Node GrowDown(in int height)
        {
            var right = _root;
            var down = new Node(0, right.Height, right.Width, height);
            _root = new Node(0, 0, right.Width, right.Height + height, right, down);
            return down;
        }
    }
}
