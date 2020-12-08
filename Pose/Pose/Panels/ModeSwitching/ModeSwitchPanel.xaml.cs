using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pose.Panels.ModeSwitching
{
    /// <summary>
    /// Interaction logic for ModeSwitchPanel.xaml
    /// </summary>
    public partial class ModeSwitchPanel : UserControl
    {
        public ModeSwitchPanel()
        {
            InitializeComponent();
        }

        private void AnimateLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.ActivateAnimateMode();
        }

        private void DesignLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.ActivateDesignMode();
        }

        public ModeSwitchPanelViewModel ViewModel => DataContext as ModeSwitchPanelViewModel;
    }
}
