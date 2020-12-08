using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Pose.Runtime.MonoGameDotNetCore.Animations;
using Pose.Runtime.MonoGameDotNetCore.QuadRendering;

namespace Pose.Runtime.MonoGameDotNetCore.Skeletons
{
    public class Skeleton
    {
        private readonly RTNode[] _nodes;
        private readonly int[] _drawSequenceIndices;
        private readonly Dictionary<string, RTAnimation> _animations;

        /// <param name="nodes">all nodes of the hierarchy, in hierarchic order, for updating transforms</param>
        /// <param name="drawSequenceIndices">the indices of the spritenodes in nodes, in draw order</param>
        internal Skeleton(Vector3 position, RTNode[] nodes, int[] drawSequenceIndices, Dictionary<string, RTAnimation> animations)
        {
            _nodes = nodes;
            _drawSequenceIndices = drawSequenceIndices;
            _animations = animations ?? throw new ArgumentNullException(nameof(animations));
            Position = position;
        }

        public void StartAnimation(string name, GameTime gameTime)
        {
            StartAnimation(name, (float)gameTime.TotalGameTime.TotalSeconds);
        }

        /// <summary>
        /// Starts an animation at its first frame.
        /// </summary>
        /// <param name="name">Name of animation, as assigned in the Pose editor.</param>
        /// <param name="gameTime">The current gametime. This marks the start time of the animation.</param>
        public void StartAnimation(string name, float gameTimeSeconds)
        {
            if (!_animations.TryGetValue(name, out var animation))
                throw new PoseAnimationNotFoundException($"Animation \"{name}\" not found.");
            CurrentAnimation = animation;
            CurrentAnimation.Start(gameTimeSeconds);
        }

        internal void Update(float gameTime)
        {
            CurrentAnimation?.PlayForwardTo(gameTime, _nodes); // update all animated properties
            ApplyTransforms(); // apply the changed transforms to all vertices
        }

        internal void Draw(QuadRenderer quadRenderer)
        {
            for (var i = 0; i < _drawSequenceIndices.Length; i++)
            {
                ref var spriteNode = ref _nodes[_drawSequenceIndices[i]];
                quadRenderer.RenderPretransformedQuad(spriteNode.SpriteQuad, ref spriteNode.A, ref spriteNode.B, ref spriteNode.C, ref spriteNode.D);
            }
        }

        private void ApplyTransforms()
        {
            ref var node = ref _nodes[0];
            node.DesignTransformation = new Transformation(Position.X, Position.Y, Angle);
            node.GlobalTransform = GetTransform(ref node);
            for (var i = 1; i < _nodes.Length; i++)
            {
                node = ref _nodes[i];
                ref var parentNode = ref _nodes[node.ParentNodeIdx];
                node.GlobalTransform = GetTransform(ref node) * parentNode.GlobalTransform;
                
                if (node.SpriteQuad == null)
                    continue;

                node.A = Vector2.Transform(node.SpriteQuad.Vertices[0], node.GlobalTransform);
                node.B = Vector2.Transform(node.SpriteQuad.Vertices[1], node.GlobalTransform);
                node.C = Vector2.Transform(node.SpriteQuad.Vertices[2], node.GlobalTransform);
                node.D = Vector2.Transform(node.SpriteQuad.Vertices[3], node.GlobalTransform);
            }
        }

        private static Matrix GetTransform(ref RTNode node)
        {
            var angle = node.DesignTransformation.Angle + node.AnimateTransformation.Angle;
            var x = node.DesignTransformation.X + node.AnimateTransformation.X;
            var y = node.DesignTransformation.Y + node.AnimateTransformation.Y;
            var cos = MathF.Cos(angle);
            var sin = MathF.Sin(angle);
            return new Matrix(cos, sin, 0, 0, -sin, cos, 0, 0, 0, 0, 1, 0, x, y, 0, 1);
        }

        public Vector3 Position { get; set; }
        public float Angle { get; set; }
        public RTAnimation CurrentAnimation { get; private set; }
    }
}
