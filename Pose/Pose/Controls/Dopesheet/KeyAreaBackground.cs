using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Pose.Controls.Dopesheet
{
    [TemplatePart(Name = "PART_ActiveAreaRectangle", Type = typeof(Rectangle))]
    public class KeyAreaBackground : Control
    {
        private Rectangle _activeAreaRectangle;

        static KeyAreaBackground()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyAreaBackground), new FrameworkPropertyMetadata(typeof(KeyAreaBackground)));
        }

        public override void OnApplyTemplate()
        {
            _activeAreaRectangle = GetTemplateChild("PART_ActiveAreaRectangle") as Rectangle;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            UpdateActiveAreaRectangle();
        }

        private void UpdateActiveAreaRectangle()
        {
            if (_activeAreaRectangle == null)
                return;

            // get left and right of active area.
            var leftMargin = (BeginFrame - FrameOffset + 1) * FrameWidth;
            var rightMargin = ActualWidth - (EndFrame - FrameOffset + 2d) * FrameWidth;

            // coerce to within bounds of control
            if (leftMargin < 0d)
                leftMargin = 0d;

            if (rightMargin < 0d)
                rightMargin = 0d;

            _activeAreaRectangle.Margin = new Thickness(leftMargin, 0, rightMargin, 0);
        }

        public static readonly DependencyProperty FrameOffsetProperty = DependencyProperty.Register(
            "FrameOffset", typeof(int), typeof(KeyAreaBackground), new PropertyMetadata(default(int), (o, args) => (o as KeyAreaBackground).UpdateActiveAreaRectangle()));

        public int FrameOffset
        {
            get => (int) GetValue(FrameOffsetProperty);
            set => SetValue(FrameOffsetProperty, value);
        }

        public static readonly DependencyProperty FrameWidthProperty = DependencyProperty.Register(
            "FrameWidth", typeof(double), typeof(KeyAreaBackground), new PropertyMetadata(default(double), (o, args) => (o as KeyAreaBackground).UpdateActiveAreaRectangle()));

        public double FrameWidth
        {
            get => (double) GetValue(FrameWidthProperty);
            set => SetValue(FrameWidthProperty, value);
        }

        public static readonly DependencyProperty BeginFrameProperty = DependencyProperty.Register(
            "BeginFrame", typeof(int), typeof(KeyAreaBackground), new PropertyMetadata(default(int), (o, args) => (o as KeyAreaBackground).UpdateActiveAreaRectangle()));

        public int BeginFrame
        {
            get => (int) GetValue(BeginFrameProperty);
            set => SetValue(BeginFrameProperty, value);
        }

        public static readonly DependencyProperty EndFrameProperty = DependencyProperty.Register(
            "EndFrame", typeof(int), typeof(KeyAreaBackground), new PropertyMetadata(default(int), (o, args) => (o as KeyAreaBackground).UpdateActiveAreaRectangle()));

        public int EndFrame
        {
            get => (int) GetValue(EndFrameProperty);
            set => SetValue(EndFrameProperty, value);
        }
    }
}
