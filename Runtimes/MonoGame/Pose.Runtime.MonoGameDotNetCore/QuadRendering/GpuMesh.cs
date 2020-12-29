using System;
using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore.QuadRendering
{
    /// <summary>
    /// A single Textured 2D mesh linked to its counterpart resources in video ram. Optimized for performance and therefore prone to misuse. Uses fixed vertex/index max capacity to prevent array reallocation and expects you to write directly to the vertex and index arrays.
    /// </summary>
    public class GpuMesh : IDisposable
    {
        /// <summary>
        /// Write vertices directly to this buffer and call MarkVerticesChanged() each time you update vertices. Always store contiguous blocks of data starting at idx 0. Length is allowed to be less than vertexCapacity, but not more.
        /// </summary>
        public readonly VertexPositionColorTexture[] Vertices;
        /// <summary>
        /// Write indices directly to this buffer and call MarkIndicesChanged() each time you update vertices. Always store contiguous blocks of data starting at idx 0. Length is allowed to be less than indexCapacity, but not more.
        /// </summary>
        public readonly int[] Indices;

        private readonly bool _useDynamicVertexBuffer;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        internal int VertexCount, IndexCount;
        private bool _verticesDirty;
        private bool _indicesDirty;

        public GpuMesh(int vertexCapacity, int indexCapacity, Texture2D texture, bool useDynamicVertexBuffer = false)
        {
            Texture = texture;
            _useDynamicVertexBuffer = useDynamicVertexBuffer;
            Vertices = new VertexPositionColorTexture[vertexCapacity];
            Indices = new int[indexCapacity];
        }

        /// <summary>
        /// Returns the vertexbuffer. If the graphicsdevice or the vertex data has changed, a new VertexBuffer is created.
        /// </summary>
        internal VertexBuffer GetVertexBuffer(GraphicsDevice graphicsDevice)
        {
            if (_vertexBuffer == null || _vertexBuffer.GraphicsDevice != graphicsDevice)
            {
                _vertexBuffer = _useDynamicVertexBuffer
                    ? new DynamicVertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), Vertices.Length, BufferUsage.WriteOnly)
                    : new VertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), Vertices.Length, BufferUsage.WriteOnly);
                _verticesDirty = true;
            }

            if (_verticesDirty)
            {
                if (VertexCount > 0)
                {
                    _vertexBuffer.SetData(Vertices, 0, VertexCount);
                }
                _verticesDirty = false;
            }

            return _vertexBuffer;
        }

        /// <summary>
        /// Returns the indexbuffer. If the graphicsdevice or the index data has changed, a new indexbuffer is created.
        /// </summary>
        internal IndexBuffer GetIndexBuffer(GraphicsDevice graphicsDevice)
        {
            if (_indexBuffer == null || _indexBuffer.GraphicsDevice != graphicsDevice)
            {
                _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, Indices.Length, BufferUsage.WriteOnly);
                _indicesDirty = true;
            }

            if (_indicesDirty)
            {
                if (IndexCount > 0)
                {
                    _indexBuffer.SetData(Indices, 0, IndexCount);
                }

                _indicesDirty = false;
            }

            return _indexBuffer;

        }

        /// <summary>
        /// Tells this mesh that the vertices have changed and video ram needs to be updated.
        /// </summary>
        /// <param name="totalVertexCountInUse">Total vertex count used in this GpuMesh.</param>
        public void MarkVerticesChanged(int totalVertexCountInUse)
        {
            if (totalVertexCountInUse > Vertices.Length)
                throw new Exception($"Vertex capacity ({Vertices.Length}) exceeded. ({totalVertexCountInUse})");
            _verticesDirty = true;
            VertexCount = totalVertexCountInUse;
        }

        /// <summary>
        /// Tells this mesh that the indices have changed and video ram needs to be updated.
        /// </summary>
        /// /// <param name="totalIndexCountInUse">Total index count used in this GpuMesh.</param>
        public void MarkIndicesChanged(int totalIndexCountInUse)
        {
            if (totalIndexCountInUse > Indices.Length)
                throw new Exception($"Index capacity ({Indices.Length}) exceeded. ({totalIndexCountInUse})");
#if DEBUG
            if (totalIndexCountInUse % 3 != 0)
                throw new Exception($"IndexCount should always be a multitude of 3. (but {totalIndexCountInUse} received)");
#endif
            _indicesDirty = true;
            IndexCount = totalIndexCountInUse;
            TriangleCount = totalIndexCountInUse / 3;
        }

        /// <summary>
        /// Populates the indices for using this GpuMesh as a list of quads with 4 clockwise corner vertices each. After this, set the vertices with the 4 corner vertices per quad and you're done.
        /// </summary>
        public void PrepareQuadIndices(in int quadCount)
        {
            var j = 0;
            for (var i = 0; i < quadCount; i++)
            {
                var vertexIndex = i << 2;
                Indices[j++] = vertexIndex;
                Indices[j++] = vertexIndex + 1;
                Indices[j++] = vertexIndex + 3;

                Indices[j++] = vertexIndex + 1;
                Indices[j++] = vertexIndex + 2;
                Indices[j++] = vertexIndex + 3;
            }
            MarkIndicesChanged(j);
        }

        public void Dispose()
        {
            // we don't own the Texture, don't dispose it.
            _vertexBuffer?.Dispose();
            _vertexBuffer = null;
            _indexBuffer?.Dispose();
            _indexBuffer = null;
        }

        /// <summary>
        /// Amount of triangles currently in the GpuMesh. (this is your MarkIndicesChanged() divided by 3)
        /// </summary>
        public int TriangleCount { get; private set; }

        /// <summary>
        /// The texture used to render this GpuMesh.
        /// </summary>
        public Texture2D Texture { get; } // we don't need to refresh Texture upon DeviceLost, MonoGame does this.

        
    }
}
