using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pose.Panels.DrawOrder
{
    /// <summary>
    /// Interaction logic for DrawOrderPanel.xaml
    /// </summary>
    public partial class DrawOrderPanel : UserControl
    {
        public DrawOrderPanel()
        {
            InitializeComponent();
        }

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource is ListBoxItem && e.Key == Key.Escape)
                ViewModel.CancelSelection();
        }

        public DrawOrderPanelViewModel ViewModel => DataContext as DrawOrderPanelViewModel;
    }
}
