using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pose.Panels.Assets
{
    /// <summary>
    /// Interaction logic for AssetPanel.xaml
    /// </summary>
    public partial class AssetPanel : UserControl
    {
        public AssetPanel()
        {
            InitializeComponent();
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ViewModel.SpriteDoubleClick();
        }

        private void SetAssetFolderButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DoSetAssetFolderWorkflow();
        }

        public AssetPanelViewModel ViewModel => (AssetPanelViewModel)DataContext;
    }
}
