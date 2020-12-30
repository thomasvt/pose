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
        private Texture2D _currentTexture;
        private GraphicsDevice _graphicsDevice;
        private BasicEffect _effect;
        private VertexPositionColorTexture[] _vertices;
        private int[] _indices;
        private int _batchVertexCount, _batchIndexCount;

        public UnbufferedMeshBatch()
        {
            _vertices = new VertexPositionColorTexture[0];
            _indices = new int[0];
        }

        public void Clear()
        {
            _currentTexture = null;
            _batchVertexCount = 0;
            _batchIndexCount = 0;
        }

        public void Render(Mesh mesh)
        {
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh));
            var vertexCount = mesh.VertexCount;
            if (vertexCount == 0)
                return;

            if (mesh.Texture != _currentTexture)
            {
                Flush();

                _effect.TextureEnabled = mesh.Texture != null;
                _effect.Texture = mesh.Texture;
                _currentTexture = mesh.Texture;
            }

            var indexOffsetInBatch = _batchVertexCount;
            // copy vertices to batch buffer

            // grow vertex batch buffer?
            if (_batchVertexCount + vertexCount > _vertices.Length)
                Array.Resize(ref _vertices, (int)((_batchVertexCount + vertexCount) * 1.5f));
            // copy vertices from cpumesh into batch buffer.
            Array.Copy(mesh.Vertices, 0, _vertices, _batchVertexCount, vertexCount);
            _batchVertexCount += vertexCount;


            // copy indices to batch buffer

            // grow index batch buffer?
            var indexCount = mesh.IndexCount;
            if (_batchIndexCount + indexCount > _indices.Length)
                Array.Resize(ref _indices, (int)((_batchIndexCount + indexCount) * 1.5f));
            // Copy the indices but shift their values so they point to the corresponding vertices in the batch buffer:
            for (var i = 0; i < indexCount; i++)
            {
                _indices[_batchIndexCount + i] = mesh.Indices[i] + indexOffsetInBatch;
            }

            _batchIndexCount += indexCount;
        }

        public void Flush()
        {
            if (_batchVertexCount == 0)
                return;

            _effect.World = Matrix.Identity;

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, _batchVertexCount, _indices, 0, _batchVertexCount >> 1);
            }

            _batchVertexCount = 0;
            _batchIndexCount = 0;
        }

        public void UpdateDeviceDependents(GraphicsDevice graphicsDevice, BasicEffect effect)
        {
            _effect = effect;
            _graphicsDevice = graphicsDevice;
        }
    }
}
