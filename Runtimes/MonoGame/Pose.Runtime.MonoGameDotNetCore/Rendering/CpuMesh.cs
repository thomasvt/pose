using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore.Rendering
{
    /// <summary>
    /// A single-textured 2D mesh with vertices and indices that get sent to gpu each time you draw it. Use this for meshes that change (eg. procedurally) (almost) each frame.
    /// </summary>
    public class CpuMesh
    {
        public readonly VertexPositionColorTexture[] Vertices;
        public readonly int[] Indices;

        public CpuMesh(int vertexCapacity, int indexCapacity, Texture2D texture)
        {
            Texture = texture;
            Vertices = new VertexPositionColorTexture[vertexCapacity];
            Indices = new int[indexCapacity];
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
        }

        public int VertexCount;
        public int IndexCount;

        /// <summary>
        /// The texture used to render this GpuMesh.
        /// </summary>
        public Texture2D Texture { get; } // we don't need to refresh Texture upon DeviceLost, MonoGame does this.
    }
}
