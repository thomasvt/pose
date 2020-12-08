using System;

namespace Pose.Domain.Editor
{
    public class UserActionException
    : Exception
    {
        public UserActionException(string message)
        : base(message)
        {
        }
    }
}
