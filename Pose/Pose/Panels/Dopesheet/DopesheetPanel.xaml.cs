using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Pose.Controls;
using Pose.Controls.Dopesheet;
using Pose.Domain.Editor;
using Pose.Panels.Dopesheet;

namespace Pose.Panels.DopeSheet
{
    /// <summary>
    /// Interaction logic for DopesheetPanel.xaml
    /// </summary>
    public partial class DopesheetPanel : UserControl
    {
        private Editor _editor;

        public DopesheetPanel()
        {
            InitializeComponent();
        }

        private void DopesheetPanel_OnLoaded(object sender, RoutedEventArgs e)
        {
            _editor = ((App)System.Windows.Application.Current).ServiceProvider.GetRequiredService<Editor>();
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                _editor.RemoveSelectedAnimationKeys();
        }

        private void OnRecordButtonClick(object sender, RoutedEventArgs e)
        {
            ViewModel.ToggleAutoKeying();
        }

        private void Dopesheet_OnBeginFrameCommitted(object sender, ValueChangedEventArgs e)
        {
            ViewModel.OnAnimationBeginFrameCommitted((int)e.InitialValue, (int)e.NewValue, e.DuringMouseDrag);
        }

        private void Dopesheet_OnEndFrameCommitted(object sender, ValueChangedEventArgs e)
        {
            ViewModel.OnAnimationEndFrameCommitted((int)e.InitialValue, (int)e.NewValue, e.DuringMouseDrag);
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.TogglePlay();
        }

        private void JumpToBegin_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.JumpToAnimationBegin();
        }

        private void Dopesheet_OnRowClicked(DopesheetRow obj)
        {
            ViewModel.RowClicked(obj);
        }

        private DopesheetPanelViewModel ViewModel => DataContext as DopesheetPanelViewModel;
    }
}
