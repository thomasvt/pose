using System.Windows;
using System.Windows.Controls;

namespace Pose.Controls.Dopesheet
{
    /// <summary>
    /// A colored marker bar on the dopesheet timeline.
    /// </summary>
    public class TimelineBar : DopesheetRowItem
    {
        static TimelineBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineBar), new FrameworkPropertyMetadata(typeof(TimelineBar)));
        }

        public static readonly DependencyProperty BeginFrameProperty = DependencyProperty.Register(
            "BeginFrame", typeof(int), typeof(TimelineBar), new PropertyMetadata(default(int)));

        /// <summary>
        /// The timeline frame where the bar begins.
        /// </summary>
        public int BeginFrame
        {
            get => (int) GetValue(BeginFrameProperty);
            set => SetValue(BeginFrameProperty, value);
        }

        public static readonly DependencyProperty EndFrameProperty = DependencyProperty.Register(
            "EndFrame", typeof(int), typeof(TimelineBar), new PropertyMetadata(default(int)));

        /// <summary>
        /// The timeline frame where the bar ends.
        /// </summary>
        public int EndFrame
        {
            get => (int) GetValue(EndFrameProperty);
            set => SetValue(EndFrameProperty, value);
        }
    }
}
