using System.Windows;
using Pose.Controls;

namespace Pose.Popups.ExportSpritesheets
{
    /// <summary>
    /// Interaction logic for ExportSpritesheetWindow.xaml
    /// </summary>
    public partial class ExportSpritesheetWindow : ModernWindow
    {
        public ExportSpritesheetWindow()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public ExportSpritesheetViewModel ViewModel => DataContext as ExportSpritesheetViewModel;
    }
}
