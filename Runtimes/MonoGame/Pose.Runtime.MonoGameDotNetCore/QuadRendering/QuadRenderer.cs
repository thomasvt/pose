using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore.QuadRendering
{
    public class QuadRenderer : IDisposable
    {
        private readonly GpuMeshRenderer _gpuMeshRenderer;
        private readonly int _quadCapacityPerTexture;
        private readonly Dictionary<Texture2D, QuadBatch> _quadBatchPerTexture;

        public QuadRenderer(GraphicsDeviceManager graphicsDeviceManager, int quadCapacityPerTexture, BlendState blendState = null, DepthStencilState depthStencilState = null)
        {
            _gpuMeshRenderer = new GpuMeshRenderer(graphicsDeviceManager, blendState ?? BlendState.NonPremultiplied, depthStencilState ?? DepthStencilState.Default);
            _quadCapacityPerTexture = quadCapacityPerTexture;
            _quadBatchPerTexture = new Dictionary<Texture2D, QuadBatch>();
        }

        /// <summary>
        /// Registers a <see cref="Texture2D"/> in the QuadRenderer. Does nothing if it's already registered. Only quads using registered textures can be rendered.
        /// </summary>
        public void RegisterTexture(Texture2D texture)
        {
            if (_quadBatchPerTexture.ContainsKey(texture))
                return;
            _quadBatchPerTexture.Add(texture, new QuadBatch(_quadCapacityPerTexture, texture));
        }

        public void BeginRender()
        {
            foreach (var quadBatch in _quadBatchPerTexture.Values)
            {
                quadBatch.BeginRender();
            }
        }

        public void RenderQuad(SpriteQuad spriteQuad, Matrix worldTransform)
        {
            // we transform on cpu and send thousands of quads to gpu in one drawcall, because it's a lot faster than having the gpu do the transformation but having to do a drawcall for each quad.
            var a = Vector2.Transform(spriteQuad.Vertices[0], worldTransform);
            var b = Vector2.Transform(spriteQuad.Vertices[1], worldTransform);
            var c = Vector2.Transform(spriteQuad.Vertices[2], worldTransform);
            var d = Vector2.Transform(spriteQuad.Vertices[3], worldTransform);

            _quadBatchPerTexture[spriteQuad.Texture].Append(
                new VertexPositionColorTexture(new Vector3(a, 0f), Color.White, spriteQuad.TextureCoords[0]),
                new VertexPositionColorTexture(new Vector3(b, 0f), Color.White, spriteQuad.TextureCoords[1]),
                new VertexPositionColorTexture(new Vector3(c, 0f), Color.White, spriteQuad.TextureCoords[2]),
                new VertexPositionColorTexture(new Vector3(d, 0f), Color.White, spriteQuad.TextureCoords[3])
            );
        }

        public void RenderPretransformedQuad(SpriteQuad quad, ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 d)
        {
            _quadBatchPerTexture[quad.Texture].Append(
                new VertexPositionColorTexture(a, Color.White, quad.TextureCoords[0]),
                new VertexPositionColorTexture(b, Color.White, quad.TextureCoords[1]),
                new VertexPositionColorTexture(c, Color.White, quad.TextureCoords[2]),
                new VertexPositionColorTexture(d, Color.White, quad.TextureCoords[3])
            );
        }

        public void EndRender()
        {
            _gpuMeshRenderer.ProjectionTransform = ProjectionTransform;
            _gpuMeshRenderer.ViewTransform = ViewTransform;

            foreach (var quadBatch in _quadBatchPerTexture.Values)
            {
                quadBatch.EndRender();
                _gpuMeshRenderer.Render(quadBatch.GpuMesh, Matrix.Identity);
            }
        }

        public void Dispose()
        {
            foreach (var value in _quadBatchPerTexture.Values)
            {
                value.Dispose();
            }
        }

        public Matrix ProjectionTransform { get; set; }

        public Matrix ViewTransform { get; set; }

        public GraphicsDevice GraphicsDevice => _gpuMeshRenderer.GraphicsDevice;

    }
}
