using System.Windows;
using System.Windows.Controls;
using Pose.Controls;

namespace Pose.Popups.ExportSpritesheets
{
    /// <summary>
    /// Interaction logic for ExportSpritesheetView.xaml
    /// </summary>
    public partial class ExportSpritesheetView : UserControl
    {
        public ExportSpritesheetView()
        {
            InitializeComponent();
        }

        private void ExportSpritesheetView_OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Initialize(RenderViewport);
        }

        public ExportSpritesheetViewModel ViewModel => DataContext as ExportSpritesheetViewModel;

        private void Scale_Changed(object? sender, ValueChangedEventArgs e)
        {
            if (!e.DuringMouseDrag)
            {
                ViewModel.SetScale(e.NewValue);
            }
        }
    }
}
