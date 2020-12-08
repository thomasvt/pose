using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pose.Controls.Dopesheet
{
    /// <summary>
    /// The timeline ruler with frame number labels.
    /// </summary>
    public class TimelineRuler : Control
    {
        protected override Size MeasureOverride(Size constraint)
        {
            return constraint;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var actualWidth = ActualWidth;
            var actualHeight = ActualHeight;

            drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(0,0, ActualWidth, ActualHeight));

            // find a label interval by taking amount of frames in 100px, and rounding it to the closest multitude of 5.
            var labelInterval = (int)Math.Round((100 / FrameWidth) / 5, MidpointRounding.AwayFromZero) * 5;

            // find first visible label
            var firstLabel = FrameOffset;
            if (firstLabel % labelInterval != 0)
                firstLabel = (int) Math.Ceiling((double)firstLabel / labelInterval) * labelInterval;

            // find last visible label
            var lastLabel = (int)(actualWidth / FrameWidth) + FrameOffset; // actualWidth is a bit more because frame-centers are at FrameWidth/2, but we need a bit more so we include frames out of view of which the labels that are partially visible
            if (lastLabel % labelInterval != 0)
                lastLabel = (int)Math.Floor((double)lastLabel / labelInterval) * labelInterval;

            for (var label = firstLabel; label <= lastLabel; label += labelInterval)
            {
                var text = new FormattedText(label.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, Foreground, 1d);
                drawingContext.DrawText(text, new Point((label - FrameOffset) * FrameWidth + DopesheetTimelinePanel.ThumbWidth * 0.5d - text.Width * 0.5d, actualHeight - text.Height - 3));
            }
        }

        public int GetFrameAt(double x)
        {
            return (int)(x / FrameWidth) + FrameOffset;
        }

        public static readonly DependencyProperty FrameWidthProperty = DependencyProperty.Register(
            "FrameWidth", typeof(double), typeof(TimelineRuler), new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsRender));

        public double FrameWidth
        {
            get => (double) GetValue(FrameWidthProperty);
            set => SetValue(FrameWidthProperty, value);
        }

        public static readonly DependencyProperty FrameOffsetProperty = DependencyProperty.Register(
            "FrameOffset", typeof(int), typeof(TimelineRuler), new FrameworkPropertyMetadata(default(int), FrameworkPropertyMetadataOptions.AffectsRender));

        public int FrameOffset
        {
            get => (int) GetValue(FrameOffsetProperty);
            set => SetValue(FrameOffsetProperty, value);
        }
    }
}
