using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore.QuadRendering
{
    public class GpuMeshRenderer : IDisposable
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private BasicEffect _effect;
        private GraphicsDevice _graphicsDevice;

        public GpuMeshRenderer(GraphicsDeviceManager graphicsDeviceManager, BlendState blendState)
        {
            BlendState = blendState;
            _graphicsDeviceManager = graphicsDeviceManager;
            graphicsDeviceManager.DeviceCreated += (s, e) => OnGraphicsDeviceCreated();
            OnGraphicsDeviceCreated();
        }

        /// <summary>
        /// Called when GraphicsDevice is replaced with a new instance.
        /// </summary>
        private void OnGraphicsDeviceCreated()
        {
            _graphicsDevice = _graphicsDeviceManager.GraphicsDevice;
            CreateEffect(_graphicsDevice);
        }

        private void CreateEffect(GraphicsDevice graphicsDevice)
        {
            _effect?.Dispose();
            _effect = new BasicEffect(graphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = true,
            };
        }

        public void Render(GpuMesh gpuMesh, Matrix worldTransform)
        {
            if (gpuMesh == null)
                throw new ArgumentNullException(nameof(gpuMesh));
            if (gpuMesh.TriangleCount == 0)
                return;

            _effect.View = ViewTransform;
            _effect.Projection = ProjectionTransform;
            _effect.World = worldTransform;
            _effect.TextureEnabled = gpuMesh.Texture != null;
            _effect.Texture = gpuMesh.Texture;

            _graphicsDevice.BlendState = BlendState;
            _graphicsDevice.SetVertexBuffer(gpuMesh.GetVertexBuffer(_graphicsDevice));
            _graphicsDevice.Indices = gpuMesh.GetIndexBuffer(_graphicsDevice);

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, gpuMesh.TriangleCount);
            }
        }

        public void Dispose()
        {
            _effect?.Dispose();
        }

        public Matrix ViewTransform { get; set; }
        public Matrix ProjectionTransform { get; set; }
        public BlendState BlendState { get; set; }
        public GraphicsDevice GraphicsDevice => _graphicsDeviceManager.GraphicsDevice;
    }
}
