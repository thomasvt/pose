using System.Windows.Controls;
using Pose.Controls;

namespace Pose.Panels.Properties.SubPanels
{
    /// <summary>
    /// Interaction logic for RotateSubPanel.xaml
    /// </summary>
    public partial class RotateSubPanel : UserControl
    {
        public RotateSubPanel()
        {
            InitializeComponent();
        }

        private void OnAngleChanged(object sender, ValueChangedEventArgs e)
        {
            ViewModel.Angle.OnChanged(e.NewValue, e.DuringMouseDrag);
        }

        private RotateSubPanelViewModel ViewModel => DataContext as RotateSubPanelViewModel;
    }
}
