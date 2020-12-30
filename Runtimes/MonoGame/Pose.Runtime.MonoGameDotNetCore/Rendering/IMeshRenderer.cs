using Microsoft.Xna.Framework;

namespace Pose.Runtime.MonoGameDotNetCore.Rendering
{
    public interface IMeshRenderer
    {
        void Render(Mesh mesh, Matrix worldTransform);
    }
}