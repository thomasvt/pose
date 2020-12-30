using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore.Rendering
{
    public interface IRenderer : IMeshRenderer
    {
        void Begin();
        void End();
        Matrix ViewTransform { get; set; }
        Matrix ProjectionTransform { get; set; }
        GraphicsDevice GraphicsDevice { get; }
    }
}