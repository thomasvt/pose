using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore
{
    /// <summary>
    /// Provide the Pose sprite as <see cref="Texture2D"/>s to the PoseRuntime. Multiple requests for the same file are common, so caching in your implementation is advised.
    /// </summary>
    public interface ITextureStore
    {
        Texture2D GetTexture(string pathFromAssetFolder);
    }
}
