using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore.Rendering
{
    /// <summary>
    /// Batches cpumesh with the same texture into a single drawcalls.
    /// </summary>
    internal class CpuMeshBatch
    {
        private Texture2D _cpuCurrentTexture;
        private GraphicsDevice _graphicsDevice;
        private BasicEffect _effect;
        private VertexPositionColorTexture[] _cpuVertices;
        private int[] _cpuIndices;
        private int _cpuVertexCount, _cpuIndexCount;

        public CpuMeshBatch()
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

        public void Render(CpuMesh cpuMesh)
        {
            if (cpuMesh == null)
                throw new ArgumentNullException(nameof(cpuMesh));
            if (cpuMesh.VertexCount == 0)
                return;

            if (cpuMesh.Texture != _cpuCurrentTexture)
            {
                Flush();

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
