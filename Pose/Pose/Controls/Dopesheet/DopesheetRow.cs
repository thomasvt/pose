using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Pose.Controls.Dopesheet
{
    public class DopesheetRow : HeaderedItemsControl
    {
        static DopesheetRow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DopesheetRow), new FrameworkPropertyMetadata(typeof(DopesheetRow)));
            HeaderColumnWidthProperty = Dopesheet.HeaderColumnWidthProperty.AddOwner(typeof(DopesheetRow), new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.Inherits));
        }

        public FrameRange? GetFrameExtrema()
        {
            var isEmpty = true;
            var min = int.MaxValue;
            var max = int.MinValue;
            foreach (var key in Items.OfType<TimelineKey>())
            {
                isEmpty = false;
                if (min > key.Frame)
                    min = key.Frame;
                if (max < key.Frame)
                    max = key.Frame;
            }

            return isEmpty ? (FrameRange?)null : new FrameRange(min, max);
        }
        
        #region HeaderColumnWidth

        public static readonly DependencyProperty HeaderColumnWidthProperty;

        public double HeaderColumnWidth
        {
            get => (double)GetValue(HeaderColumnWidthProperty);
            set => SetValue(HeaderColumnWidthProperty, value);
        }

        #endregion
    }
}
