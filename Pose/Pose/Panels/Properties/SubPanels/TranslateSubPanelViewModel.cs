using Pose.Domain.Editor;
using Pose.Domain.Nodes.Properties;

namespace Pose.Panels.Properties.SubPanels
{
    public class TranslateSubPanelViewModel
    : NodeSubPanelViewModel
    {
        private string _nodeName;

        public TranslateSubPanelViewModel(Editor editor)
        : base(editor)
        {
            X = new PropertyFieldViewModel(PropertyType.TranslationX, "Translation X");
            Y = new PropertyFieldViewModel(PropertyType.TranslationY, "Y");
            RegisterField(X);
            RegisterField(Y);
        }

        public override void Refresh()
        {
            base.Refresh();
            NodeName = Editor.CurrentDocument.GetNode(NodeId).ToString();
        }

        public string NodeName
        {
            get => _nodeName;
            set
            {
                if (_nodeName == value)
                    return;

                _nodeName = value;
                OnPropertyChanged();
            } 
        }

        public PropertyFieldViewModel X { get; }

        public PropertyFieldViewModel Y { get; }
    }
}
