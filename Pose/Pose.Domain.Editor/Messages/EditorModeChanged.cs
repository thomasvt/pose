using System;
using System.Collections.Generic;
using System.Text;

namespace Pose.Domain.Editor.Messages
{
    public class EditorModeChanged
    {
        public EditorMode Mode { get; }

        public EditorModeChanged(EditorMode mode)
        {
            Mode = mode;
        }
    }
}
