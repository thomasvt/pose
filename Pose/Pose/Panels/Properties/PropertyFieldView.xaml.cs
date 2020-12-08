using System.Windows.Controls;
using Pose.Controls;

namespace Pose.Panels.Properties
{
    /// <summary>
    /// Interaction logic for PropertyFieldView.xaml
    /// </summary>
    public partial class PropertyFieldView : UserControl
    {
        public PropertyFieldView()
        {
            InitializeComponent();
        }

        private void OnPropertyChanged(object sender, ValueChangedEventArgs e)
        {
            ViewModel.OnChanged(e.NewValue, e.DuringMouseDrag);
        }

        private PropertyFieldViewModel ViewModel => DataContext as PropertyFieldViewModel;
    }
}
