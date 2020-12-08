using System.Windows;
using System.Windows.Controls;
using Pose.Controls;
using Pose.Domain;
using Pose.Domain.Curves;

namespace Pose.Panels.Properties.SubPanels
{
    /// <summary>
    /// Interaction logic for KeySubPanelView.xaml
    /// </summary>
    public partial class KeySubPanelView : UserControl
    {
        public KeySubPanelView()
        {
            InitializeComponent();
        }

        private void OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            ViewModel.OnValueChanged(e.NewValue, e.DuringMouseDrag);
        }

        private void CurveEditor_OnBezierHandleReleased()
        {
            ViewModel.OnBezierCurveChanged();
        }

        public KeySubPanelViewModel ViewModel => DataContext as KeySubPanelViewModel;

        

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            ViewModel.OnValueChanged(true);
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ViewModel.OnValueChanged(false);
        }

        private void SoftInOut_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCurve(CurveType.Bezier, BezierCurve.GetEasingCurve(0.333f));
        }

        private void MediumInOut_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCurve(CurveType.Bezier, BezierCurve.GetEasingCurve(0.5f));
        }

        private void StrongInOut_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCurve(CurveType.Bezier, BezierCurve.GetEasingCurve(0.75f));
        }

        private void OvershootInOut_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCurve(CurveType.Bezier, BezierCurve.GetEasingCurve(new Vector2(0.5f, -0.33f), new Vector2(0.5f, 1.33f)));
        }

        private void SoftOut_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCurve(CurveType.Bezier, BezierCurve.GetEasingCurve(0, 0.3f));
        }

        private void MediumOut_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCurve(CurveType.Bezier, BezierCurve.GetEasingCurve(0, 0.5f));
        }

        private void StrongOut_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCurve(CurveType.Bezier, BezierCurve.GetEasingCurve(0, 0.75f));
        }

        private void OvershootOut_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCurve(CurveType.Bezier, BezierCurve.GetEasingCurve(Vector2.Zero, new Vector2(0.5f, 1.33f)));
        }

        private void SoftIn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCurve(CurveType.Bezier, BezierCurve.GetEasingCurve( 0.3f, 0));
        }

        private void MediumIn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCurve(CurveType.Bezier, BezierCurve.GetEasingCurve(0.5f, 0));
        }

        private void StrongIn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCurve(CurveType.Bezier, BezierCurve.GetEasingCurve(0.75f, 0));
        }

        private void OvershootIn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCurve(CurveType.Bezier, BezierCurve.GetEasingCurve(new Vector2(0.5f, -0.33f), Vector2.One));
        }

        private void Linear_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCurve(CurveType.Linear);
        }

        private void Hold_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCurve(CurveType.Hold);
        }
    }
}
