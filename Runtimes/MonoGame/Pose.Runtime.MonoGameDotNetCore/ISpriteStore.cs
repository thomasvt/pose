
using Pose.Runtime.MonoGameDotNetCore.Rendering;

namespace Pose.Runtime.MonoGameDotNetCore
{
    public interface ISpriteStore
    {
        Sprite GetSpriteQuad(string assetPath);
    }
}
