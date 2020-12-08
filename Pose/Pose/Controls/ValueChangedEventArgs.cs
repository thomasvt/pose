using System;

namespace Pose.Controls
{
    public class ValueChangedEventArgs
    : EventArgs
    {
        public ValueChangedEventArgs(float initialValue, float newValue, bool duringMouseDrag)
        {
            InitialValue = initialValue;
            NewValue = newValue;
            DuringMouseDrag = duringMouseDrag;
        }

        /// <summary>
        /// The value as it was when the user started changing it.
        /// </summary>
        public float InitialValue { get; }

        /// <summary>
        /// The new value.
        /// </summary>
        public float NewValue { get; }
        /// <summary>
        /// True if the user is changing the value while dragging with the mouse. False if otherwise changed, or upon releasing the mouse button after a mousedrag.
        /// </summary>
        public bool DuringMouseDrag { get; }
    }
}
