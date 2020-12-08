using System.Windows;
using Pose.SceneEditor.Tools;

namespace Pose.SceneEditor.MouseOperations
{
    /// <summary>
    /// Baseclass for all mouse dragging scene manipulations.
    /// </summary>
    internal abstract class MouseDragOperation
    {
        protected Vector InitialMousePosition;

        protected MouseDragOperation(Vector initialMousePosition)
        {
            InitialMousePosition = initialMousePosition;
        }

        /// <summary>
        /// Called each time the mouse moves
        /// </summary>
        public virtual void UpdatePosition(Vector screenPosition)
        {
        }

        /// <summary>
        /// Called when the mouse button is released.
        /// </summary>
        public virtual void Finish()
        {
        }

        /// <summary>
        /// Called after the operation has started. (eg. by a <see cref="IMouseTool"/>)
        /// </summary>
        public virtual void Begin()
        {
        }

        /// <summary>
        /// Called when user pressed Escape while dragging.
        /// </summary>
        public virtual void Cancel()
        {
            
        }
    }
}