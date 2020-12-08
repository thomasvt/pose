using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pose.Panels.Animations
{
    /// <summary>
    /// Interaction logic for AnimationsPanelView.xaml
    /// </summary>
    public partial class AnimationsPanelView : UserControl
    {
        public AnimationsPanelView()
        {
            InitializeComponent();
        }

        private void AddNewAnimationButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewAnimation();
        }

        private void RemoveAnimationButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveSelectedAnimation();
        }

        private void AnimationList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                ViewModel.RemoveSelectedAnimation();
        }

        public AnimationsPanelViewModel ViewModel => DataContext as AnimationsPanelViewModel;
    }
}
