using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore.Rendering
{
    /// <summary>
    /// Batches the geometry of meshes with the same texture into a single drawcall.
    /// </summary>
    internal class UnbufferedMeshBatch
    {
        private Texture2D _cpuCurrentTexture;
        private GraphicsDevice _graphicsDevice;
        private BasicEffect _effect;
        private VertexPositionColorTexture[] _cpuVertices;
        private int[] _cpuIndices;
        private int _cpuVertexCount, _cpuIndexCount;

        public UnbufferedMeshBatch()
        {
            _cpuVertices = new VertexPositionColorTexture[0];
            _cpuIndices = new int[0];
        }

        public void Clear()
        {
            _cpuCurrentTexture = null;
            _cpuVertexCount = 0;
            _cpuIndexCount = 0;
        }

        public void Render(Mesh mesh)
        {
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh));
            if (mesh.VertexCount == 0)
                return;

            if (mesh.Texture != _cpuCurrentTexture)
            {
                Flush();

                _effect.TextureEnabled = mesh.Texture != null;
                _effect.Texture = mesh.Texture;
                _cpuCurrentTexture = mesh.Texture;
            }

            var indexOffsetInBatch = _cpuVertexCount;

            // copy vertices to batch buffer

            // grow vertex batch buffer?
            if (_cpuVertexCount + mesh.VertexCount > _cpuVertices.Length)
                Array.Resize(ref _cpuVertices, (int)((_cpuVertexCount + mesh.VertexCount) * 1.5f));
            // copy vertices from cpumesh into batch buffer.
            Array.Copy(mesh.Vertices, 0, _cpuVertices, _cpuVertexCount, mesh.VertexCount);
            _cpuVertexCount += mesh.VertexCount;


            // copy indices to batch buffer

            // grow index batch buffer?
            if (_cpuIndexCount + mesh.IndexCount > _cpuIndices.Length)
                Array.Resize(ref _cpuIndices, (int)((_cpuIndexCount + mesh.IndexCount) * 1.5f));
            // Copy the indices but shift their values so they point to the corresponding vertices in the batch buffer:
            for (var i = 0; i < mesh.IndexCount; i++)
            {
                _cpuIndices[_cpuIndexCount + i] = mesh.Indices[i] + indexOffsetInBatch;
            }

            _cpuIndexCount += mesh.IndexCount;
        }

        public void Flush()
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

        public void UpdateDeviceDependents(GraphicsDevice graphicsDevice, BasicEffect effect)
        {
            _effect = effect;
            _graphicsDevice = graphicsDevice;
        }
    }
}
