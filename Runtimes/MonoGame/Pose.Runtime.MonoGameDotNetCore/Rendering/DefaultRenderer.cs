using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

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
        private readonly UnbufferedMeshBatch _unbufferedMeshBatch;

        public DefaultRenderer(GraphicsDeviceManager graphicsDeviceManager, BlendState blendState = null, DepthStencilState depthStencilState = null)
        {
            BlendState = blendState ?? BlendState.NonPremultiplied;
            DepthStencilState = depthStencilState ?? DepthStencilState.None;
            _graphicsDeviceManager = graphicsDeviceManager;
            _unbufferedMeshBatch = new UnbufferedMeshBatch();
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
            _unbufferedMeshBatch.UpdateDeviceDependents(_graphicsDevice, _effect);
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
            _unbufferedMeshBatch.Clear();

            _effect.View = ViewTransform;
            _effect.Projection = ProjectionTransform;
            _graphicsDevice.BlendState = BlendState;
            _graphicsDevice.DepthStencilState = DepthStencilState;
        }

        /// <summary>
        /// Use this overload for procedural meshes of which the geometry changes constantly (changing meshes, particles in world coords).
        /// Meshes cannot have individual world transforms because they are batched together; so set the vertices' position to world coords yourself.
        /// </summary>
        /// <param name="mesh">The mesh to render</param>
        /// <param name="worldTransform">Sent as world transform to the gpu. Ignored when BufferMode.Unbuffered.</param>
        public void Render(Mesh mesh, Matrix worldTransform)
        {
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh));
            if (mesh.VertexCount == 0)
                return;

            if (mesh.BufferMode == BufferMode.Unbuffered)
            {
                _unbufferedMeshBatch.Render(mesh);
            }
            else
            {
                _unbufferedMeshBatch.Flush();

                _graphicsDevice.SetVertexBuffer(mesh.GetVertexBuffer(_graphicsDevice));
                _graphicsDevice.Indices = mesh.GetIndexBuffer(_graphicsDevice);
                _effect.World = worldTransform;

                foreach (var pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mesh.IndexCount / 3);
                }
            }
        }

        public void End()
        {
            _unbufferedMeshBatch.Flush();
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
