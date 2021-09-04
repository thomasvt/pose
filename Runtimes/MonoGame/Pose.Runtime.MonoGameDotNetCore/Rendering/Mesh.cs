using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore.Rendering
{
    /// <summary>
    /// A single-textured 2D mesh with vertices and indices.
    /// </summary>
    public class Mesh
    {
        private bool _vertexBufferIsDirty;
        private bool _indexBufferIsDirty;
        public readonly VertexPositionColorTexture[] Vertices;
        public readonly int[] Indices;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        public Mesh(int vertexCapacity, int indexCapacity, Texture2D texture, BufferMode bufferMode)
        {
            Texture = texture;
            BufferMode = bufferMode;
            Vertices = new VertexPositionColorTexture[vertexCapacity];
            Indices = new int[indexCapacity];
            VertexCount = 0;
            IndexCount = 0;
        }

        public VertexBuffer GetVertexBuffer(GraphicsDevice graphicsDevice)
        {
            if (_vertexBuffer == null || _vertexBuffer.GraphicsDevice != graphicsDevice)
            {
                _vertexBuffer?.Dispose();
                _vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionColorTexture.VertexDeclaration, Vertices.Length, BufferUsage.None);
                _vertexBufferIsDirty = true;
            }
        
            if (_vertexBufferIsDirty)
            {
                _vertexBuffer.SetData(Vertices, 0, VertexCount);
                _vertexBufferIsDirty = false;
            }
        
            return _vertexBuffer;
        }
        
        public IndexBuffer GetIndexBuffer(GraphicsDevice graphicsDevice)
        {
            if (_indexBuffer == null || _indexBuffer.GraphicsDevice != graphicsDevice)
            {
                _indexBuffer?.Dispose();
                _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, Indices.Length, BufferUsage.None);
                _indexBufferIsDirty = true;
            }
        
            if (_indexBufferIsDirty)
            {
                _indexBuffer.SetData(Indices, 0, IndexCount);
                _indexBufferIsDirty = false;
            }
        
            return _indexBuffer;
        }

        /// <summary>
        /// Must be called after changing vertices.
        /// </summary>
        /// <param name="vertexCount">Total amount of vertices in the mesh.</param>
        public void ApplyVertexChanges(int vertexCount)
        {
            VertexCount = vertexCount;
            _vertexBufferIsDirty = true;
        }

        /// <summary>
        /// Must be called after changing indices so they can be sent to videoram.
        /// </summary>
        /// <param name="indexCount">Total amount of indices in the mesh.</param>
        public void ApplyIndexChanges(int indexCount)
        {
            IndexCount = indexCount;
            _indexBufferIsDirty = true;
        }

        public int VertexCount { get; private set; }
        public int IndexCount { get; private set; }

        public Texture2D Texture { get; } // we don't need to refresh Texture upon DeviceLost, MonoGame does this.
        public BufferMode BufferMode { get; }
    }
}
