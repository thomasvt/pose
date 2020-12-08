using System.Windows;
using System.Windows.Controls;

namespace Pose.Controls.Dopesheet
{
    public class DopesheetTimelinePanel : Panel
    {
        internal const double ThumbWidth = 24d; // framecursor's header width

        static DopesheetTimelinePanel()
        {
            FrameWidthProperty = Dopesheet.FrameWidthProperty.AddOwner(typeof(DopesheetTimelinePanel), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));
            FrameOffsetProperty = Dopesheet.FrameOffsetProperty.AddOwner(typeof(DopesheetTimelinePanel), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange));
            KeySizeProperty = Dopesheet.KeySizeProperty.AddOwner(typeof(DopesheetTimelinePanel), new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange));
            BarWidthProperty = Dopesheet.BarWidthProperty.AddOwner(typeof(DopesheetTimelinePanel), new FrameworkPropertyMetadata(8d, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // tell each child how big it's going to be according to its time coordinates.

            var keySize = KeySize;
            var barWidth = BarWidth;
            var frameWidth = FrameWidth;

            foreach (UIElement child in InternalChildren)
            {
                var size = Size.Empty;

                switch (child)
                {
                    case TimelineBar timelineBar:
                        {
                            var width = (timelineBar.EndFrame - timelineBar.BeginFrame) * frameWidth;
                            size = new Size(width, barWidth);
                            break;
                        }
                    case TimelineKey timelineKey:
                        {
                            size = new Size(keySize, keySize);
                            break;
                        }
                    case FrameCursor frameCursor:
                        {
                            size = new Size(ThumbWidth, ActualHeight);
                            break;
                        }
                }

                child.Measure(size);
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var keySize = KeySize;
            var halfKeySize = keySize * 0.5d;
            var barWidth = BarWidth;
            var halfBarWidth = barWidth * 0.5d;
            var centerY = finalSize.Height * 0.5d;
            var frameWidth = FrameWidth;
            var frameOffset = FrameOffset;
            var halfThumbWidth = ThumbWidth * 0.5d;

            foreach (UIElement child in InternalChildren)
            {
                var rect = Rect.Empty;

                switch (child)
                {
                    case TimelineBar timelineBar:
                        {
                            var beginX = (timelineBar.BeginFrame - frameOffset) * frameWidth + halfThumbWidth;
                            var endX = (timelineBar.EndFrame - frameOffset) * frameWidth + halfThumbWidth;
                            rect = new Rect(beginX, centerY - halfBarWidth, endX - beginX, barWidth);
                            break;
                        }
                    case TimelineKey timelineKey:
                        {
                            var centerX = (timelineKey.Frame - frameOffset) * frameWidth + halfThumbWidth;
                            rect = new Rect(centerX - halfKeySize, centerY - halfKeySize, keySize, keySize);
                            break;
                        }
                    case FrameCursor frameCursor:
                        {
                            var centerX = (frameCursor.Frame - frameOffset) * frameWidth;
                            rect = new Rect(centerX, 0, ThumbWidth, finalSize.Height);
                            break;
                        }
                }

                child.Arrange(rect.IsEmpty ? new Rect(0,0,0,0) : rect);
            }

            return finalSize;
        }

        #region KeySize

        public static readonly DependencyProperty KeySizeProperty;

        public double KeySize
        {
            get => (double)GetValue(KeySizeProperty);
            set => SetValue(KeySizeProperty, value);
        }

        #endregion

        #region BarWidth

        public static readonly DependencyProperty BarWidthProperty;

        public double BarWidth
        {
            get => (double)GetValue(BarWidthProperty);
            set => SetValue(BarWidthProperty, value);
        }

        #endregion

        #region FrameWidth

        public static readonly DependencyProperty FrameWidthProperty;

        public double FrameWidth
        {
            get => (double)GetValue(FrameWidthProperty);
            set => SetValue(FrameWidthProperty, value);
        }

        #endregion

        #region FrameOffset

        public static readonly DependencyProperty FrameOffsetProperty;

        public int FrameOffset
        {
            get => (int)GetValue(FrameOffsetProperty);
            set => SetValue(FrameOffsetProperty, value);
        }

        #endregion

    }
}
