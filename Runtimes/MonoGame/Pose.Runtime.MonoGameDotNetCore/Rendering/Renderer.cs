using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore.Rendering
{
    /// <summary>
    /// A renderer meant for 2D graphics with partially transparent textures (sprites). It uses no z buffer as transparency forces us to draw in depth order anyway. So call Render() from back to front rendering.
    /// Draw calls using the same Texture are batched together, which is a drastic performance boost, so try to group per texture if depth order allows it.
    /// Use Render(<see cref="CpuMesh"/>) for procedural meshes that change constantly (animated vertices, particles). CpuMeshes cannot have individual world transforms because they are batched; so set the vertices' position to their world position.
    /// Use Render(<see cref="GpuMesh"/>) for static meshes, which is a lot faster than CpuMesh because the geometry is already in videoram.
    /// </summary>
    public class Renderer : IDisposable
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private BasicEffect _effect;
        private GraphicsDevice _graphicsDevice;

        // batching of cpu meshes:
        private Texture2D _cpuCurrentTexture;
        private VertexPositionColorTexture[] _cpuVertices;
        private int[] _cpuIndices;
        private int _cpuVertexCount, _cpuIndexCount;

        public Renderer(GraphicsDeviceManager graphicsDeviceManager, BlendState blendState = null, DepthStencilState depthStencilState = null)
        {
            BlendState = blendState ?? BlendState.NonPremultiplied;
            DepthStencilState = depthStencilState ?? DepthStencilState.None;
            _graphicsDeviceManager = graphicsDeviceManager;
            graphicsDeviceManager.DeviceCreated += (s, e) => OnGraphicsDeviceCreated();
            OnGraphicsDeviceCreated();

            _cpuVertices = new VertexPositionColorTexture[0];
            _cpuIndices = new int[0];
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

        public void Begin()
        {
            _cpuCurrentTexture = null;
            _cpuVertexCount = 0;
            _cpuIndexCount = 0;

            _effect.View = ViewTransform;
            _effect.Projection = ProjectionTransform;
            _graphicsDevice.BlendState = BlendState;
            _graphicsDevice.DepthStencilState = DepthStencilState;
        }

        public void Render(CpuMesh cpuMesh)
        {
            if (cpuMesh == null)
                throw new ArgumentNullException(nameof(cpuMesh));
            if (cpuMesh.VertexCount == 0)
                return;

            if (cpuMesh.Texture != _cpuCurrentTexture)
            {
                FlushCpuGeometry();

                _effect.TextureEnabled = cpuMesh.Texture != null;
                _effect.Texture = cpuMesh.Texture;
                _cpuCurrentTexture = cpuMesh.Texture;
            }

            var indexOffsetInBatch = _cpuVertexCount;

            // copy vertices to batch buffer

            // grow vertex batch buffer?
            if (_cpuVertexCount + cpuMesh.VertexCount > _cpuVertices.Length)
                Array.Resize(ref _cpuVertices, (int)((_cpuVertexCount + cpuMesh.VertexCount) * 1.5f));
            // copy vertices from cpumesh into batch buffer.
            Array.Copy(cpuMesh.Vertices, 0, _cpuVertices, _cpuVertexCount, cpuMesh.VertexCount);
            _cpuVertexCount += cpuMesh.VertexCount;


            // copy indices to batch buffer

            // grow index batch buffer?
            if (_cpuIndexCount + cpuMesh.IndexCount > _cpuIndices.Length)
                Array.Resize(ref _cpuIndices, (int)((_cpuIndexCount + cpuMesh.IndexCount) * 1.5f));
            // Copy the indices but shift their values so they point to the corresponding vertices in the batch buffer:
            for (var i = 0; i < cpuMesh.IndexCount; i++)
            {
                _cpuIndices[_cpuIndexCount + i] = cpuMesh.Indices[i] + indexOffsetInBatch;
            }
            
            _cpuIndexCount += cpuMesh.IndexCount;
        }

        public void Render(GpuMesh gpuMesh, Matrix worldTransform)
        {
            if (gpuMesh == null)
                throw new ArgumentNullException(nameof(gpuMesh));
            if (gpuMesh.VertexCount == 0)
                return;

            FlushCpuGeometry();

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
            FlushCpuGeometry();
        }

        private void FlushCpuGeometry()
        {
            if (_cpuVertexCount == 0)
                return;

            _effect.World = Matrix.Identity;

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _cpuVertices, 0, _cpuVertexCount, _cpuIndices, 0, _cpuVertexCount >> 1);
            }

            _cpuVertexCount = 0;
            _cpuIndexCount = 0;
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
