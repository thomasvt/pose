using System;
using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore.QuadRendering
{
    /// <summary>
    /// A buffer of quads using a single texture. Allows to append quads onto a single gpu resource-set that is sent to gpu in one transaction.
    /// </summary>
    internal class QuadBatch : IDisposable
    {
        private readonly int _quadCapacity;
        internal readonly GpuMesh GpuMesh;
        private int _quadIndex;

        public QuadBatch(in int quadCapacity, Texture2D texture)
        {
            _quadCapacity = quadCapacity;
            GpuMesh = new GpuMesh(quadCapacity * 4, quadCapacity * 6, texture, true);
            PrepareIndices(); // indices never change, so we can load them just once.
        }

        private void PrepareIndices()
        {
            var j = 0;
            for (var i = 0; i < _quadCapacity; i++)
            {
                var vertexIndex = i << 2;
                GpuMesh.Indices[j++] = vertexIndex;
                GpuMesh.Indices[j++] = vertexIndex + 1;
                GpuMesh.Indices[j++] = vertexIndex + 3;

                GpuMesh.Indices[j++] = vertexIndex + 1;
                GpuMesh.Indices[j++] = vertexIndex + 2;
                GpuMesh.Indices[j++] = vertexIndex + 3;
            }
            GpuMesh.MarkIndicesChanged(j);
        }

        public void BeginRender()
        {
            _quadIndex = 0;
        }

        public void EndRender()
        {
            GpuMesh.MarkVerticesChanged(_quadIndex << 4);
        }

        public void Append(VertexPositionColorTexture a, VertexPositionColorTexture b, VertexPositionColorTexture c, VertexPositionColorTexture d)
        {
            if (_quadIndex == _quadCapacity)
                throw new QuadRenderException("QuadBatch capacity exceeded. Increase it.");

            var vertexIndex = _quadIndex << 2;
            GpuMesh.Vertices[vertexIndex++] = a;
            GpuMesh.Vertices[vertexIndex++] = b;
            GpuMesh.Vertices[vertexIndex++] = c;
            GpuMesh.Vertices[vertexIndex] = d;

            _quadIndex++;
        }

        public void Dispose()
        {
            GpuMesh.Dispose();
        }
    }
}
