using System.Drawing;
using System.Windows;

namespace Pose.Controls.Dopesheet
{
    /// <summary>
    /// A Key diamond on the dopesheet timeline.
    /// </summary>
    public class TimelineKey : DopesheetRowItem
    {
        static TimelineKey()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineKey), new FrameworkPropertyMetadata(typeof(TimelineKey)));
        }

        #region Dependency Properties

        public static readonly DependencyProperty FrameProperty = DependencyProperty.Register(
            "Frame", typeof(int), typeof(TimelineKey), new PropertyMetadata(default(int)));

        /// <summary>
        /// The timeline frame where this key is located.
        /// </summary>
        public int Frame
        {
            get => (int)GetValue(FrameProperty);
            set => SetValue(FrameProperty, value);
        }

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness", typeof(double), typeof(TimelineKey), new PropertyMetadata(default(double)));

        public double StrokeThickness
        {
            get => (double) GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            "Fill", typeof(Brush), typeof(TimelineKey), new PropertyMetadata(default(Brush)));

        public Brush Fill
        {
            get => (Brush) GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            "Stroke", typeof(Brush), typeof(TimelineKey), new PropertyMetadata(default(Brush)));

        public Brush Stroke
        {
            get => (Brush) GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(TimelineKey), new PropertyMetadata(default(bool)));

        public bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

#endregion
    }
}
