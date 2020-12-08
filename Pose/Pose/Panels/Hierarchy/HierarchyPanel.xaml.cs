using System.Windows.Controls;
using System.Windows.Input;

namespace Pose.Panels.Hierarchy
{
    public partial class HierarchyPanel : UserControl
    {
        public HierarchyPanel()
        {
            InitializeComponent();
        }

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (!(e.OriginalSource is TreeViewItem))
                return;

            switch (e.Key)
            {
                case Key.Delete:
                    ViewModel.RemoveSelectedNodes();
                    break;
                case Key.Escape:
                    ViewModel.CancelSelection();
                    break;
            }
        }

        public HierarchyPanelViewModel ViewModel => DataContext as HierarchyPanelViewModel;
    }
}
