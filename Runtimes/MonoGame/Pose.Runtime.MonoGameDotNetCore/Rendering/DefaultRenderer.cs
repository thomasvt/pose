using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore.Rendering
{
    /// <summary>
    /// A renderer meant for 2D graphics with partially transparent textures (sprites). It uses no z buffer as transparency forces us to draw in depth order anyway. So call Render() from back to front rendering.
    /// Draw calls using the same Texture are batched together, which is a drastic performance boost, so try designing your game entities in favor of combining same-texture-entities at the same or adjacent depths.
    /// </summary>
    public class DefaultRenderer : IDisposable, IRenderer
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private BasicEffect _effect;
        private GraphicsDevice _graphicsDevice;
        private readonly CpuMeshBatch _cpuMeshBatch;

        public DefaultRenderer(GraphicsDeviceManager graphicsDeviceManager, BlendState blendState = null, DepthStencilState depthStencilState = null)
        {
            BlendState = blendState ?? BlendState.NonPremultiplied;
            DepthStencilState = depthStencilState ?? DepthStencilState.None;
            _graphicsDeviceManager = graphicsDeviceManager;
            _cpuMeshBatch = new CpuMeshBatch();
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
            _cpuMeshBatch.UpdateDeviceDependents(_graphicsDevice, _effect);
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

        public void Begin()
        {
            _cpuMeshBatch.Clear();

            _effect.View = ViewTransform;
            _effect.Projection = ProjectionTransform;
            _graphicsDevice.BlendState = BlendState;
            _graphicsDevice.DepthStencilState = DepthStencilState;
        }

        /// <summary>
        /// Use this overload for procedural meshes of which the geometry changes constantly (changing meshes, particles in world coords).
        /// CpuMeshes cannot have individual world transforms because they are batched together; so set the vertices' position to world coords yourself.
        /// </summary>
        public void Render(CpuMesh cpuMesh)
        {
            _cpuMeshBatch.Render(cpuMesh);
        }

        /// <summary>
        /// Use this overload for static meshes that can be preloaded into videoram, which is a lot faster than CpuMesh.
        /// </summary>
        public void Render(GpuMesh gpuMesh, Matrix worldTransform)
        {
            if (gpuMesh == null)
                throw new ArgumentNullException(nameof(gpuMesh));
            if (gpuMesh.VertexCount == 0)
                return;

            _cpuMeshBatch.Flush();

            _graphicsDevice.SetVertexBuffer(gpuMesh.GetVertexBuffer());
            _graphicsDevice.Indices = gpuMesh.GetIndexBuffer();
            _effect.World = worldTransform;

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, gpuMesh.IndexCount / 3);
            }
        }

        public void End()
        {
            _cpuMeshBatch.Flush();
        }

        public void Dispose()
        {
            _effect?.Dispose();
        }

        public Matrix ViewTransform { get; set; }
        public Matrix ProjectionTransform { get; set; }
        public BlendState BlendState { get; set; }
        public GraphicsDevice GraphicsDevice => _graphicsDeviceManager.GraphicsDevice;
        public DepthStencilState DepthStencilState { get; set; }
    }
}
