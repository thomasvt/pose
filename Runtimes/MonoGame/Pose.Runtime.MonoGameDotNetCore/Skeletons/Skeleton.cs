using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pose.Runtime.MonoGameDotNetCore.Animations;
using Pose.Runtime.MonoGameDotNetCore.QuadRendering;

namespace Pose.Runtime.MonoGameDotNetCore.Skeletons
{
    public class Skeleton
    {
        private readonly RTNode[] _nodes;
        private readonly int[] _drawSequenceIndices;
        private readonly Dictionary<string, RTAnimation> _animations;
        private readonly GpuMesh _gpuMesh;

        /// <param name="nodes">all nodes of the hierarchy, in hierarchic order, for updating transforms</param>
        /// <param name="drawSequenceIndices">the indices of the spritenodes in nodes, in draw order</param>
        internal Skeleton(RTNode[] nodes, int[] drawSequenceIndices, Dictionary<string, RTAnimation> animations, Texture2D texture)
        {
            _nodes = nodes;
            _drawSequenceIndices = drawSequenceIndices;
            _animations = animations ?? throw new ArgumentNullException(nameof(animations));
            DrawOrderZFactor = 0f;
            var spriteNodeCount = nodes.Count(n => n.SpriteQuad != null);
            _gpuMesh = new GpuMesh(spriteNodeCount * 4, spriteNodeCount * 6, texture, true);
            _gpuMesh.PrepareQuadIndices(spriteNodeCount);
            PrepareVertices();
        }

        /// <summary>
        /// Initialize the vertex data that never changes in the <see cref="GpuMesh"/>.
        /// </summary>
        private void PrepareVertices()
        {
            var vertexIdx = 0;
            for (var i = 0; i < _drawSequenceIndices.Length; i++)
            {
                ref var spriteNode = ref _nodes[_drawSequenceIndices[i]];
                
                ref var vertex = ref _gpuMesh.Vertices[vertexIdx++];
                vertex.Color = Color.White;
                vertex.TextureCoordinate = spriteNode.SpriteQuad.TextureCoords[0];

                vertex = ref _gpuMesh.Vertices[vertexIdx++];
                vertex.Color = Color.White;
                vertex.TextureCoordinate = spriteNode.SpriteQuad.TextureCoords[1];

                vertex = ref _gpuMesh.Vertices[vertexIdx++];
                vertex.Color = Color.White;
                vertex.TextureCoordinate = spriteNode.SpriteQuad.TextureCoords[2];

                vertex = ref _gpuMesh.Vertices[vertexIdx++];
                vertex.Color = Color.White;
                vertex.TextureCoordinate = spriteNode.SpriteQuad.TextureCoords[3];
            }
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

        internal void Draw(GpuMeshRenderer quadRenderer)
        {
            var vertexIdx = 0;
            for (var i = 0; i < _drawSequenceIndices.Length; i++)
            {
                ref var spriteNode = ref _nodes[_drawSequenceIndices[i]];

                ref var v = ref _gpuMesh.Vertices[vertexIdx++];
                v.Position.X = spriteNode.A.X;
                v.Position.Y = spriteNode.A.Y;

                v = ref _gpuMesh.Vertices[vertexIdx++];
                v.Position.X = spriteNode.B.X;
                v.Position.Y = spriteNode.B.Y;

                v = ref _gpuMesh.Vertices[vertexIdx++];
                v.Position.X = spriteNode.C.X;
                v.Position.Y = spriteNode.C.Y;

                v = ref _gpuMesh.Vertices[vertexIdx++];
                v.Position.X = spriteNode.D.X;
                v.Position.Y = spriteNode.D.Y;
            }

            _gpuMesh.MarkVerticesChanged(_drawSequenceIndices.Length * 4);

            var worldTransform = Matrix.Identity;
            quadRenderer.Render(_gpuMesh, ref worldTransform);
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
                ref var parentNode = ref _nodes[node.ParentNodeIdx];
                var z = node.DrawOrderIndex * DrawOrderZFactor;
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

        /// <summary>
        /// The world position for this skeleton.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// For drawordering. More is further back.
        /// </summary>
        public float Depth { get; set; }

        /// <summary>
        /// The draworder of individual sprites in a single skeleton is obtained by giving sprites different Z positions. Each sprite's Z = index-in-draworder * DrawOrderZFactor
        /// </summary>
        public float DrawOrderZFactor { get; set; }

        public float Angle { get; set; }

        public RTAnimation CurrentAnimation { get; private set; }
    }
}
