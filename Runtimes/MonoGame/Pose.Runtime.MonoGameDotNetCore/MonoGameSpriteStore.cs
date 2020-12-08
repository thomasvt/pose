using System;
using System.Collections.Generic;
using Pose.Runtime.MonoGameDotNetCore.QuadRendering;

namespace Pose.Runtime.MonoGameDotNetCore
{
    /// <summary>
    /// The default SpriteStore for MonoGame. Reuses sprites if the Pose document reuses them.
    /// </summary>
    public class MonoGameSpriteStore
    : ISpriteStore
    {
        private readonly ITextureStore _textureStore;
        private readonly Dictionary<string, SpriteQuad> _sprites;

        public MonoGameSpriteStore(ITextureStore textureStore)
        {
            _textureStore = textureStore;
            _sprites = new Dictionary<string, SpriteQuad>();
        }

        public SpriteQuad GetSpriteQuad(string assetPath)
        {
            if (_sprites.TryGetValue(assetPath, out var sprite))
                return sprite;

            var texture = _textureStore.GetTexture(assetPath);
            sprite = new SpriteQuad(texture);
            _sprites.Add(assetPath, sprite);
            return sprite;
        }
    }
}
