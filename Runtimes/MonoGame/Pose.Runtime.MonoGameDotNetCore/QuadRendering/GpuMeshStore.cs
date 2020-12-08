using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore.QuadRendering
{
    internal class GpuMeshStore : IDisposable
    {
        private readonly List<GpuMesh> _meshes;

        public GpuMeshStore(GpuMeshRenderer renderer)
        {
            _meshes = new List<GpuMesh>();
        }

        /// <summary>
        /// Creates a mesh ready to hold a given amount of vertices and indices. This method always returns a new mesh instance with dedicated gpu data.
        /// </summary>
        public GpuMesh CreateEmptyMesh(int vertexCapacity, int indexCapacity, Texture2D texture, bool useDynamicVertexBuffer = false)
        {
            var mesh = new GpuMesh(vertexCapacity, indexCapacity, texture, useDynamicVertexBuffer);
            _meshes.Add(mesh);
            return mesh;
        }

        public void Dispose()
        {
            foreach (var mesh in _meshes)
            {
                mesh.Dispose();
            }
        }
    }
}
