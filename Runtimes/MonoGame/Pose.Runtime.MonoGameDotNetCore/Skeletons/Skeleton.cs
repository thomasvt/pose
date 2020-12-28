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
            DrawOrderZFactor = 0f;
        }

        /// <summary>
        /// Starts playing an animation on this skeleton.
        /// </summary>
        /// <param name="name">Name of animation, as assigned in the Pose editor. Case sensitive.</param>
        /// <param name="startTime">The starttime of the animation (first frame). Often this will be the gametime of the current frame, but you can use this to offset the animation.</param>
        public void StartAnimation(string name, GameTime startTime)
        {
            StartAnimation(name, (float)startTime.TotalGameTime.TotalSeconds);
        }

        /// <summary>
        /// Starts playing an animation on this skeleton.
        /// </summary>
        /// <param name="name">Name of animation, as assigned in the Pose editor. Case sensitive.</param>
        /// <param name="startTime">The starttime of the animation (first frame). Often this will be the gametime of the current frame, but you can use this to offset the animation.</param>
        public void StartAnimation(string name, float startTime)
        {
            if (!_animations.TryGetValue(name, out var animation))
                throw new PoseAnimationNotFoundException($"Animation \"{name}\" not found.");
            CurrentAnimation = animation;
            CurrentAnimation.Start(startTime);
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
            // loop over all nodes and calc their transform matrix. The nodes array is in hierarchical order, so children will always be calculated after their parent.
            ref var node = ref _nodes[0];
            node.DesignTransformation = new Transformation(Position.X, Position.Y, Angle);
            node.GlobalTransform = GetTransform(ref node, Position.Z);
            for (var i = 1; i < _nodes.Length; i++)
            {
                node = ref _nodes[i];
                ref var parentNode = ref _nodes[node.ParentNodeIdx];
                var z = node.DrawOrderIndex * DrawOrderZFactor;
                node.GlobalTransform = GetTransform(ref node, z) * parentNode.GlobalTransform;
                
                if (node.SpriteQuad == null)
                    continue;

                node.A = Vector3.Transform(new Vector3(node.SpriteQuad.Vertices[0], 0), node.GlobalTransform);
                node.B = Vector3.Transform(new Vector3(node.SpriteQuad.Vertices[1], 0), node.GlobalTransform);
                node.C = Vector3.Transform(new Vector3(node.SpriteQuad.Vertices[2], 0), node.GlobalTransform);
                node.D = Vector3.Transform(new Vector3(node.SpriteQuad.Vertices[3], 0), node.GlobalTransform);
            }
        }

        private static Matrix GetTransform(ref RTNode node, float z)
        {
            var angle = node.DesignTransformation.Angle + node.AnimateTransformation.Angle;
            var x = node.DesignTransformation.X + node.AnimateTransformation.X;
            var y = node.DesignTransformation.Y + node.AnimateTransformation.Y;
            var cos = MathF.Cos(angle);
            var sin = MathF.Sin(angle);
            return new Matrix(cos, sin, 0, 0, -sin, cos, 0, 0, 0, 0, 1, 0, x, y, z, 1);
        }

        /// <summary>
        /// The world position for this skeleton. Z is used for drawordering.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The draworder of individual sprites in a single skeleton is obtained by giving sprites different Z positions. Each sprite's Z = index-in-draworder * DrawOrderZFactor
        /// </summary>
        public float DrawOrderZFactor { get; set; }

        public float Angle { get; set; }

        public RTAnimation CurrentAnimation { get; private set; }
    }
}
