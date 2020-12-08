using System;
using Pose.Domain.Editor;
using Pose.Domain.Nodes.Properties;

namespace Pose.Panels.Properties.SubPanels
{
    public class RotateSubPanelViewModel
    : NodeSubPanelViewModel
    {
        private const float RadiansToDegrees = 180f / MathF.PI;

        public RotateSubPanelViewModel(Editor editor)
        : base(editor)
        {
            Angle = new PropertyFieldViewModel(PropertyType.RotationAngle, "Rotation")
            {
                DragFactor = -0.5f, // reverse how mousedrag changes the angle, so it is like steering
                DisplayValueFactor = RadiansToDegrees
            };
            RegisterField(Angle);
        }

        public PropertyFieldViewModel Angle { get; }
    }
}
