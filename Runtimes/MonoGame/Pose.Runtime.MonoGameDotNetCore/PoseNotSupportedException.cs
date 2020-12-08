using System;

namespace Pose.Runtime.MonoGameDotNetCore
{
    public class PoseNotSupportedException
    : Exception
    {
        public PoseNotSupportedException(string message)
        : base(message)
        {
        }
    }
}
