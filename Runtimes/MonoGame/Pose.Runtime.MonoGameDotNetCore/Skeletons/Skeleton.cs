using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pose.Runtime.MonoGameDotNetCore.Animations;
using Pose.Runtime.MonoGameDotNetCore.Rendering;

namespace Pose.Runtime.MonoGameDotNetCore.Skeletons
{
    public class Skeleton : IRenderable
    {
        private readonly RTNode[] _nodes;
        private readonly int[] _drawSequenceIndices;
        private readonly Dictionary<string, RTAnimation> _animations;
        private readonly SpriteMesh _mesh;

        /// <param name="nodes">all nodes of the hierarchy, in hierarchic order, for updating transforms</param>
        /// <param name="drawSequenceIndices">the indices of the spritenodes in nodes, in draw order</param>
        internal Skeleton(RTNode[] nodes, int[] drawSequenceIndices, Dictionary<string, RTAnimation> animations, Texture2D texture)
        {
            _nodes = nodes;
            _drawSequenceIndices = drawSequenceIndices;
            _animations = animations ?? throw new ArgumentNullException(nameof(animations));
            _mesh = new SpriteMesh(drawSequenceIndices.Select(idx => nodes[idx].Sprite), texture, BufferMode.Unbuffered);
        }

        /// <summary>
        /// Starts playing an animation on this skeleton.
        /// </summary>
        /// <param name="name">Name of animation, as assigned in the Pose editor. Case sensitive.</param>
        /// <param name="startTime">The starttime of the animation (first frame). Often this will be the gametime of the current frame, but you can use this to offset the animation's time position.</param>
        public void StartAnimation(string name, GameTime startTime)
        {
            StartAnimation(name, (float)startTime.TotalGameTime.TotalSeconds);
        }

        /// <summary>
        /// Starts playing an animation on this skeleton.
        /// </summary>
        /// <param name="name">Name of animation, as assigned in the Pose editor. Case sensitive.</param>
        /// <param name="startTimeSeconds">The starttime of the animation (first frame) in seconds. Often this will be the gametime of the current frame, but you can use this to offset the animation's time position.</param>
        public void StartAnimation(string name, float startTimeSeconds)
        {
            if (!_animations.TryGetValue(name, out var animation))
                throw new PoseAnimationNotFoundException($"Animation \"{name}\" not found.");
            CurrentAnimation = animation;
            CurrentAnimation.Start(startTimeSeconds);
        }

        /// <summary>
        /// Updates the animations. You can distribute the update() call of multiple skeletons over multiple threads. But don't manipulate a single skeleton instance from more than one thread.
        /// </summary>
        public void Update(float gameTimeSeconds)
        {
            CurrentAnimation?.PlayForwardTo(gameTimeSeconds, _nodes); // update all animated properties
            ApplyTransforms(); // apply the changed transforms to all vertices
        }

        public void Draw(IMeshRenderer meshRenderer)
        {
            meshRenderer.Render(_mesh.Mesh, Matrix.Identity);
        }

        private void ApplyTransforms()
        {
            // loop over all nodes and calc their transform matrix. The nodes array is in hierarchical order, so children will always be calculated after their parent.
            ref var node = ref _nodes[0];
            node.DesignTransformation = new Transformation(Position.X, Position.Y, Angle);
            node.GlobalTransform = GetTransform(ref node);
            for (var i = 1; i < _nodes.Length; i++)
            {
                node = ref _nodes[i];
                if (!node.IsVisible)
                {
                    // set transform to zero so the quad is rendered with size 0
                    node.GlobalTransform = new Matrix();

                    // note: i didn't test the alternative, but I bet this is faster than dynamically building the vertex/index array contents each frame depending on visibility of nodes. 

                    continue;
                }
                ref var parentNode = ref _nodes[node.ParentNodeIdx];
                node.GlobalTransform = GetTransform(ref node) * parentNode.GlobalTransform;
            }

            for (var i = 0; i < _drawSequenceIndices.Length; i++)
            {
                ref var spriteNode = ref _nodes[_drawSequenceIndices[i]];
                _mesh.SetSpriteTransform(i, ref spriteNode.GlobalTransform);
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

        /// <summary>
        /// The world position for this skeleton.
        /// </summary>
        public Vector2 Position;

        public float Angle;

        /// <summary>
        /// For controlling the layering of sprites. More is further back.
        /// </summary>
        public float Depth { get; set; }

        public RTAnimation CurrentAnimation { get; private set; }
    }
}
