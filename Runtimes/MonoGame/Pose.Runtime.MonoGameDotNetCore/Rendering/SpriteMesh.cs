using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore.Rendering
{
    /// <summary>
    /// A renderable list of transformable sprites. Supports both vram buffered and unbuffered storage of render data, making it a lot faster for rendering eg. tiles (using )Buffered mode) than SpriteBatch.
    /// </summary>
    public class SpriteMesh
    {
        private readonly Sprite[] _sprites;
        internal readonly Mesh Mesh;

        public SpriteMesh(IEnumerable<Sprite> sprites, Texture2D texture, BufferMode bufferMode)
        {
            _sprites = sprites.ToArray();
            Mesh = new Mesh(_sprites.Length * 4, _sprites.Length * 6, texture, bufferMode);
            PrepareVertices();
            PrepareQuadIndices();
        }

        /// <summary>
        /// Populates the indices for using this Mesh as a list of quads with 4 clockwise corner vertices each. You have to construct this Mesh with 4 vertices and 6 indices per quad you intend to store.
        /// </summary>
        private void PrepareQuadIndices()
        {
            var j = 0;
            for (var i = 0; i < _sprites.Length; i++)
            {
                var vertexIndex = i << 2;
                Mesh.Indices[j++] = vertexIndex;
                Mesh.Indices[j++] = vertexIndex + 1;
                Mesh.Indices[j++] = vertexIndex + 3;

                Mesh.Indices[j++] = vertexIndex + 1;
                Mesh.Indices[j++] = vertexIndex + 2;
                Mesh.Indices[j++] = vertexIndex + 3;
            }
            Mesh.ApplyIndexChanges(j);
        }

        /// <summary>
        /// Initialize the vertex data that never changes in the <see cref="Mesh"/>.
        /// </summary>
        private void PrepareVertices()
        {
            var vertexIdx = 0;
            for (var i = 0; i < _sprites.Length; i++)
            {
                var spriteTextureCoords = _sprites[i].TextureCoords;

                ref var vertex = ref Mesh.Vertices[vertexIdx++];
                vertex.Color = Color.White;
                vertex.TextureCoordinate = spriteTextureCoords[0];

                vertex = ref Mesh.Vertices[vertexIdx++];
                vertex.Color = Color.White;
                vertex.TextureCoordinate = spriteTextureCoords[1];

                vertex = ref Mesh.Vertices[vertexIdx++];
                vertex.Color = Color.White;
                vertex.TextureCoordinate = spriteTextureCoords[2];

                vertex = ref Mesh.Vertices[vertexIdx++];
                vertex.Color = Color.White;
                vertex.TextureCoordinate = spriteTextureCoords[3];
            }
            Mesh.ApplyVertexChanges(vertexIdx);
        }

        /// <summary>
        /// Replaces the transform for the sprite at the given index.
        /// </summary>
        public void SetSpriteTransform(int spriteIdx, ref Matrix transform)
        {
            var vertexIdx = spriteIdx << 2;
            var sprite = _sprites[spriteIdx];
            for (var i = 0; i < 4; i++)
            {
                ref var vertex = ref Mesh.Vertices[vertexIdx+i];
                var result = Vector2.Transform(sprite.Vertices[i], transform);
                vertex.Position = new Vector3(result, 0);
            }
            Mesh.ApplyVertexChanges(Mesh.VertexCount); // vertexcount never changes in SpriteMesh, so just reuse the count we already know.
        }
    }
}
