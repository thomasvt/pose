
using Pose.Runtime.MonoGameDotNetCore.Rendering;

namespace Pose.Runtime.MonoGameDotNetCore
{
    public interface ISpriteStore
    {
        SpriteQuad GetSpriteQuad(string assetPath);
    }
}
