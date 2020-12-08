using System.Windows;
using System.Windows.Controls;

namespace Pose.Controls.Dopesheet
{
    [TemplatePart(Name = "PART_Label", Type = typeof(TextBlock))]
    public class FrameCursor : DopesheetRowItem
    {
        private TextBlock _label;

        static FrameCursor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrameCursor), new FrameworkPropertyMetadata(typeof(FrameCursor)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _label = GetTemplateChild("PART_Label") as TextBlock;
            _label.Text = Frame.ToString();

        }

        public static readonly DependencyProperty FrameProperty = DependencyProperty.Register(
            "Frame", typeof(int), typeof(FrameCursor), new FrameworkPropertyMetadata(
                (o, args) =>
                {
                    if (o is FrameCursor frameCursor && frameCursor._label != null)
                    {
                        frameCursor._label.Text = args.NewValue.ToString();
                    }
                }
                )
            {
                AffectsParentArrange = true
            });

        public int Frame
        {
            get => (int)GetValue(FrameProperty);
            set => SetValue(FrameProperty, value);
        }
    }
}
