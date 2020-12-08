using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore
{
    /// <summary>
    /// Default TextureStore for MonoGame. Delegates the texture loading, their reuse and their disposal to <see cref="ContentManager"/>.
    /// </summary>
    public class MonoGameTextureStore
    : ITextureStore
    {
        private readonly ContentManager _content;

        public MonoGameTextureStore(ContentManager content)
        {
            _content = content;
        }

        public Texture2D GetTexture(string assetPath)
        {
            if (Path.HasExtension(assetPath))
            {
                if (Path.GetExtension(assetPath) != ".png")
                    throw new NotSupportedException($"PoseRuntime does not support sprites with extension {Path.GetExtension(assetPath)}. Only .png");
                assetPath = Path.GetFileNameWithoutExtension(assetPath);
            }

            return _content.Load<Texture2D>(assetPath);
        }
    }
}
