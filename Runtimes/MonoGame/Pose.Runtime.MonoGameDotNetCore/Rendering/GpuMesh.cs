using System;
using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore.Rendering
{
    /// <summary>
    /// A mesh of which vertex and index data is stored in videoram. Use this for meshes of which the geometry rarely or never changes.
    /// </summary>
    public class GpuMesh
    {
        public GpuMesh()
        {
            
        }

        public VertexBuffer GetVertexBuffer()
        {
            throw new NotImplementedException();
        }

        public IndexBuffer GetIndexBuffer()
        {
            throw new NotImplementedException();
        }

        public int VertexCount { get; }
        public int IndexCount { get; }
    }
}
