using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore.QuadRendering
{
    public class SpriteQuad
    {
        internal readonly Vector2[] Vertices, TextureCoords;
        internal readonly Texture2D Texture;

        public SpriteQuad(Texture2D texture)
        {
            Texture = texture;
            Vertices = PrepareVertices(texture);
            TextureCoords = PrepareTextureCoords();
            //WriteQuadVertices(GpuMesh.Vertices, GpuMesh.Indices, 0, 0, texture);
        }

        private static Vector2[] PrepareTextureCoords()
        {
            var aT = new Vector2(0f, 0f);
            var bT = new Vector2(1f, 0f);
            var cT = new Vector2(1f, 1f);
            var dT = new Vector2(0f, 1f);

            return new[] {aT, bT, cT, dT};
        }

        private static Vector2[] PrepareVertices(Texture2D texture)
        {
            var halfWidth = texture.Width * 0.5f;
            var halfHeight = texture.Height * 0.5f;

            var a = new Vector2(-halfWidth, halfHeight);
            var b = new Vector2(halfWidth, halfHeight);
            var c = new Vector2(halfWidth, -halfHeight);
            var d = new Vector2(-halfWidth, -halfHeight);

            return new[]
            {
                a, b, c, d
            };
        }
    }
}
