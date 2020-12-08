using System;

namespace Pose.Runtime.MonoGameDotNetCore
{
    public class PoseAnimationNotFoundException
    : Exception
    {
        public PoseAnimationNotFoundException(string message)
        : base(message)
        {
        }
    }
}
