using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pose.Runtime.MonoGameDotNetCore.Animations;
using Pose.Runtime.MonoGameDotNetCore.Rendering;

namespace Pose.Runtime.MonoGameDotNetCore.Skeletons
{
    public class Skeleton
    {
        private readonly RTNode[] _nodes;
        private readonly int[] _drawSequenceIndices;
        private readonly Dictionary<string, RTAnimation> _animations;
        private readonly CpuMesh _cpuMesh;

        /// <param name="nodes">all nodes of the hierarchy, in hierarchic order, for updating transforms</param>
        /// <param name="drawSequenceIndices">the indices of the spritenodes in nodes, in draw order</param>
        internal Skeleton(RTNode[] nodes, int[] drawSequenceIndices, Dictionary<string, RTAnimation> animations, Texture2D texture)
        {
            _nodes = nodes;
            _drawSequenceIndices = drawSequenceIndices;
            _animations = animations ?? throw new ArgumentNullException(nameof(animations));
            var spriteNodeCount = nodes.Count(n => n.SpriteQuad != null);
            _cpuMesh = new CpuMesh(spriteNodeCount * 4, spriteNodeCount * 6, texture)
            {
                VertexCount = spriteNodeCount * 4, 
                IndexCount = spriteNodeCount * 6
            };
            _cpuMesh.PrepareQuadIndices(spriteNodeCount);
            PrepareVertices();
        }

        /// <summary>
        /// Initialize the vertex data that never changes in the <see cref="CpuMesh"/>.
        /// </summary>
        private void PrepareVertices()
        {
            var vertexIdx = 0;
            for (var i = 0; i < _drawSequenceIndices.Length; i++)
            {
                ref var spriteNode = ref _nodes[_drawSequenceIndices[i]];
                
                ref var vertex = ref _cpuMesh.Vertices[vertexIdx++];
                vertex.Color = Color.White;
                vertex.TextureCoordinate = spriteNode.SpriteQuad.TextureCoords[0];

                vertex = ref _cpuMesh.Vertices[vertexIdx++];
                vertex.Color = Color.White;
                vertex.TextureCoordinate = spriteNode.SpriteQuad.TextureCoords[1];

                vertex = ref _cpuMesh.Vertices[vertexIdx++];
                vertex.Color = Color.White;
                vertex.TextureCoordinate = spriteNode.SpriteQuad.TextureCoords[2];

                vertex = ref _cpuMesh.Vertices[vertexIdx++];
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

        internal void Draw(Renderer quadRenderer)
        {
            quadRenderer.Render(_cpuMesh);
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
                node.GlobalTransform = GetTransform(ref node) * parentNode.GlobalTransform;
            }

            var vertexIdx = 0;
            for (var i = 0; i < _drawSequenceIndices.Length; i++)
            {
                ref var spriteNode = ref _nodes[_drawSequenceIndices[i]];

                for (var j = 0; j < 4; j++)
                {
                    ref var vertex = ref _cpuMesh.Vertices[vertexIdx++];
                    var result = Vector2.Transform(spriteNode.SpriteQuad.Vertices[j], spriteNode.GlobalTransform);
                    vertex.Position = new Vector3(result, 0);
                }
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
        
        public float Angle { get; set; }

        public RTAnimation CurrentAnimation { get; private set; }
    }
}
