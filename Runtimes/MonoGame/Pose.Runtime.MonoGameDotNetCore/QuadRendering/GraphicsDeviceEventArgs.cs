using System;
using Microsoft.Xna.Framework.Graphics;

namespace Pose.Runtime.MonoGameDotNetCore.QuadRendering
{
    public class GraphicsDeviceEventArgs
        : EventArgs
    {
        public GraphicsDevice GraphicsDevice { get; }

        public GraphicsDeviceEventArgs(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }
    }
}
