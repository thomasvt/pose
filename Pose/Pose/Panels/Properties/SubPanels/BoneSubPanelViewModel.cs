using Pose.Domain.Editor;
using Pose.Domain.Nodes.Properties;

namespace Pose.Panels.Properties.SubPanels
{
    internal class BoneSubPanelViewModel
    : NodeSubPanelViewModel
    {
        public BoneSubPanelViewModel(Editor editor) : base(editor)
        {
            BoneLength = new PropertyFieldViewModel(PropertyType.BoneLength, "Length")
            {
                IsKeyingEnabled = false
            };
            RegisterField(BoneLength);
        }

        public PropertyFieldViewModel BoneLength { get; }
    }
}
