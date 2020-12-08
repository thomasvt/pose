
using Pose.Runtime.MonoGameDotNetCore.QuadRendering;

namespace Pose.Runtime.MonoGameDotNetCore
{
    public interface ISpriteStore
    {
        SpriteQuad GetSpriteQuad(string assetPath);
    }
}
