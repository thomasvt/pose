using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Pose.Controls.Dopesheet
{
    [TemplatePart(Name = "PART_HeaderColumnSensor", Type = typeof(Control))]
    [TemplatePart(Name = "PART_TimelineScrollBar", Type = typeof(ScrollBar))]
    [TemplatePart(Name = "PART_TimelinesColumnSensor", Type = typeof(Control))]
    [TemplatePart(Name = "PART_BeginFrameInputBox", Type = typeof(NumericInputBox))]
    [TemplatePart(Name = "PART_EndFrameInputBox", Type = typeof(NumericInputBox))]
    [TemplatePart(Name = "PART_FrameCursor", Type = typeof(FrameCursor))]
    [TemplatePart(Name = "PART_Ruler", Type = typeof(TimelineRuler))]
    public class Dopesheet : ListBox
    {
        private FrameCursor _frameCursor;
        private ScrollBar _timelineScrollBar;
        private Control _timelinesColumnSensor;

        static Dopesheet()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Dopesheet), new FrameworkPropertyMetadata(typeof(Dopesheet)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ConfigureColumnWidthSensors();
            ConfigureScrollBar();
            ConfigureTimelineMinMaxInputFields();
            ConfigureFrameCursor();
            ConfigureRulerInteraction();
        }

        private void ConfigureColumnWidthSensors()
        {
            var headerSensor = GetTemplateChild("PART_HeaderColumnSensor") as Control;
            headerSensor.SizeChanged += (sender, args) =>
            {
                if (args.WidthChanged)
                    SetHeaderColumnWidth(this, args.NewSize.Width);
            };

            _timelinesColumnSensor = GetTemplateChild("PART_TimelinesColumnSensor") as Control;
            _timelinesColumnSensor.SizeChanged += (sender, args) =>
            {
                var viewportSizeInCells = (_timelinesColumnSensor.ActualWidth / FrameWidth) - 2d;
                if (viewportSizeInCells < 1)
                    viewportSizeInCells = 1; // prevent crash
                _timelineScrollBar.ViewportSize = viewportSizeInCells;
                UpdateScrollBarMax(EndFrame);
            };
        }

        private void ConfigureScrollBar()
        {
            _timelineScrollBar = GetTemplateChild("PART_TimelineScrollBar") as ScrollBar;
            _timelineScrollBar.Scroll += (sender, args) => FrameOffset = (int)args.NewValue;
        }

        private void ConfigureFrameCursor()
        {
            _frameCursor = GetTemplateChild("PART_FrameCursor") as FrameCursor;
            _frameCursor.Frame = CurrentFrame;
        }

        private void ConfigureTimelineMinMaxInputFields()
        {
            _minInputBox = GetTemplateChild("PART_BeginFrameInputBox") as NumericInputBox;
            _minInputBox.ValueChanged += (sender, args) =>
            {
                BeginFrame = (int)_minInputBox.Value;
                BeginFrameCommitted?.Invoke(this, args);
            };
            OnBeginFrameSet(BeginFrame);

            _maxInputBox = GetTemplateChild("PART_EndFrameInputBox") as NumericInputBox;
            _maxInputBox.ValueChanged += (sender, args) =>
            {
                EndFrame = (int)_maxInputBox.Value;
                EndFrameCommitted?.Invoke(this, args);
            };
            OnEndFrameSet(EndFrame);
        }

        private void ConfigureRulerInteraction()
        {
            var timelineRuler = GetTemplateChild("PART_Ruler") as TimelineRuler;

            timelineRuler.MouseDown += (sender, args) =>
            {
                if (args.ChangedButton == MouseButton.Left)
                {
                    timelineRuler.CaptureMouse(); // allow to keep dragging even though mouse leaves the narrow ruler
                    CurrentFrame = timelineRuler.GetFrameAt(args.GetPosition(timelineRuler).X);
                }
            };
            timelineRuler.MouseMove += (sender, args) =>
            {
                if (args.LeftButton == MouseButtonState.Pressed)
                    CurrentFrame = timelineRuler.GetFrameAt(args.GetPosition(timelineRuler).X);
            };
            timelineRuler.MouseUp += (sender, args) =>
            {
                if (args.ChangedButton == MouseButton.Left)
                {
                    timelineRuler.ReleaseMouseCapture();
                }
            };
        }

        private void UpdateScrollBarMax(int endFrame)
        {
            if (_timelineScrollBar == null)
                return;

            // scrollbar max should be either endFrame or a Key that is even further to the right.

            var range = GetUsedTimelineRange();
            _timelineScrollBar.Maximum = (range.HasValue ? Math.Max(endFrame, range.Value.Max) : endFrame) - _timelineScrollBar.ViewportSize + 5;
            FrameOffset = (int)_timelineScrollBar.Value; // in some edge cases, scrollbar value is coerced within bounds without raising Scroll event, so update explicitly.
        }

        private void UpdateScrollBarMin(int beginFrame)
        {
            if (_timelineScrollBar == null)
                return;

            // scrollbar min should be either beginFrame or a Key that is even further to the left.
            var range = GetUsedTimelineRange();
            _timelineScrollBar.Minimum = range.HasValue ? Math.Min(beginFrame, range.Value.Min) : beginFrame;
            FrameOffset = (int)_timelineScrollBar.Value; // in some edge cases, scrollbar value is coerced within bounds (if it was outside of bounds) without raising Scroll event, so update explicitly.
        }

        private FrameRange? GetUsedTimelineRange()
        {
            var min = int.MaxValue;
            var max = int.MinValue;
            var isEmpty = true;
            foreach (var item in Items.OfType<DopesheetRow>())
            {
                var range = item.GetFrameExtrema();
                if (!range.HasValue)
                    continue;

                isEmpty = false;

                if (min > range.Value.Min)
                    min = range.Value.Min;
                if (max < range.Value.Max)
                    max = range.Value.Max;
            }

            if (isEmpty)
                return null;

            return new FrameRange(min, max);
        }

        protected override bool ShouldApplyItemContainerStyle(DependencyObject container, object item)
        {
            return true;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DopesheetRow;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DopesheetRow();
        }

        public void NotifyDopesheetRowClicked(DopesheetRow item, MouseButton mouseButton)
        {
            if (mouseButton == MouseButton.Left)
                RowClicked?.Invoke(item);
        }

        #region HeaderColumnWidth

        public static readonly DependencyProperty HeaderColumnWidthProperty = DependencyProperty.RegisterAttached(
            "HeaderColumnWidth", typeof(double), typeof(Dopesheet), new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.Inherits));

        public double HeaderColumnWidth
        {
            get => (double)GetValue(HeaderColumnWidthProperty);
            set => SetValue(HeaderColumnWidthProperty, value);
        }

        public static void SetHeaderColumnWidth(DependencyObject target, double value)
        {
            target.SetValue(HeaderColumnWidthProperty, value);
        }

        #endregion

        #region FrameWidth

        public static readonly DependencyProperty FrameWidthProperty = DependencyProperty.RegisterAttached(
            "FrameWidth", typeof(double), typeof(Dopesheet), new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// The width in pixels of one time unit (a frame).
        /// </summary>
        public double FrameWidth
        {
            get => (double)GetValue(FrameWidthProperty);
            set => SetValue(FrameWidthProperty, value);
        }

        public static double GetFrameWidth(DependencyObject target)
        {
            return (double)target.GetValue(FrameWidthProperty);
        }

        public static void SetFrameWidth(DependencyObject target, double value)
        {
            target.SetValue(FrameWidthProperty, value);
        }

        #endregion

        #region FrameOffset

        public static readonly DependencyProperty FrameOffsetProperty = DependencyProperty.RegisterAttached(
            "FrameOffset", typeof(int), typeof(Dopesheet), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.Inherits));

        public int FrameOffset
        {
            get => (int)GetValue(FrameOffsetProperty);
            set => SetValue(FrameOffsetProperty, value);
        }

        public static int GetFrameOffset(DependencyObject target)
        {
            return (int)target.GetValue(FrameOffsetProperty);
        }

        public static void SetFrameOffset(DependencyObject target, int value)
        {
            target.SetValue(FrameOffsetProperty, value);
        }

        #endregion

        #region KeySize

        public static readonly DependencyProperty KeySizeProperty = DependencyProperty.RegisterAttached(
            "KeySize", typeof(double), typeof(Dopesheet), new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.Inherits));

        public double KeySize
        {
            get => (double)GetValue(KeySizeProperty);
            set => SetValue(KeySizeProperty, value);
        }

        public static double GetKeySize(DependencyObject target)
        {
            return (double)target.GetValue(KeySizeProperty);
        }

        public static void SetKeySize(DependencyObject target, double value)
        {
            target.SetValue(KeySizeProperty, value);
        }

        #endregion

        #region BarWidth

        public static readonly DependencyProperty BarWidthProperty = DependencyProperty.RegisterAttached(
            "BarWidth", typeof(double), typeof(Dopesheet), new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.Inherits));

        public double BarWidth
        {
            get => (double)GetValue(BarWidthProperty);
            set => SetValue(BarWidthProperty, value);
        }

        public static double GetBarWidth(DependencyObject target)
        {
            return (double)target.GetValue(BarWidthProperty);
        }

        public static void SetBarWidth(DependencyObject target, double value)
        {
            target.SetValue(BarWidthProperty, value);
        }

        #endregion

        #region BeginFrame

        public static readonly DependencyProperty BeginFrameProperty = DependencyProperty.Register(
            "BeginFrame", typeof(int), typeof(Dopesheet), new FrameworkPropertyMetadata(default(int), (o, args) =>
                {
                    var dopesheet = o as Dopesheet;
                    var beginFrame = (int)args.NewValue;

                    dopesheet.OnBeginFrameSet(beginFrame);
                })
            {
                BindsTwoWayByDefault = true
            }
            );

        private void OnBeginFrameSet(int beginFrame)
        {
            if (_minInputBox != null)
                _minInputBox.Value = beginFrame;

            UpdateScrollBarMin(beginFrame);
        }

        public int BeginFrame
        {
            get => (int)GetValue(BeginFrameProperty);
            set => SetValue(BeginFrameProperty, value);
        }

        #endregion

        #region EndFrame

        public static readonly DependencyProperty EndFrameProperty = DependencyProperty.Register(
            "EndFrame", typeof(int), typeof(Dopesheet), new FrameworkPropertyMetadata(default(int),
                (o, args) =>
                {
                    if (!(o is Dopesheet dopesheet))
                        return;

                    var endFrame = (int)args.NewValue;

                    dopesheet.OnEndFrameSet(endFrame);
                }
            )
            {
                BindsTwoWayByDefault = true
            });

        private void OnEndFrameSet(int endFrame)
        {
            if (_minInputBox != null)
                _maxInputBox.Value = endFrame;

            UpdateScrollBarMax(endFrame);
        }

        public int EndFrame
        {
            get => (int)GetValue(EndFrameProperty);
            set => SetValue(EndFrameProperty, value);
        }

        #endregion

        #region CurrentFrame

        public static readonly DependencyProperty CurrentFrameProperty = DependencyProperty.Register(
            "CurrentFrame", typeof(int), typeof(Dopesheet), new FrameworkPropertyMetadata((o, args) =>
            {
                if (o is Dopesheet dopesheet && dopesheet._frameCursor != null)
                {
                    var newFrame = (int)args.NewValue;
                    dopesheet._frameCursor.Frame = newFrame;
                }
            })
            {
                BindsTwoWayByDefault = true
            });

        private NumericInputBox _minInputBox;
        private NumericInputBox _maxInputBox;

        public int CurrentFrame
        {
            get => (int)GetValue(CurrentFrameProperty);
            set => SetValue(CurrentFrameProperty, value);
        }

        #endregion

        #region TopLeftContent

        public static readonly DependencyProperty TopLeftContentProperty = DependencyProperty.Register(
            "TopLeftContent", typeof(object), typeof(Dopesheet), new PropertyMetadata(default(object)));

        public object TopLeftContent
        {
            get => (object)GetValue(TopLeftContentProperty);
            set => SetValue(TopLeftContentProperty, value);
        }

        #endregion

        /// <summary>
        /// Triggered when BeginFrame was changed in a non temporary fashion (mid-mousedrag)
        /// </summary>
        public event EventHandler<ValueChangedEventArgs> BeginFrameCommitted;

        /// <summary>
        /// Triggered when EndFrame was changed in a non temporary fashion (mid-mousedrag)
        /// </summary>
        public event EventHandler<ValueChangedEventArgs> EndFrameCommitted;

        public event Action<DopesheetRow> RowClicked;
    }
}
