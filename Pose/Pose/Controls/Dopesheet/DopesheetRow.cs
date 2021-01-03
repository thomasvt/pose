using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!(e.Source is DopesheetRow)) // only direct click is accepted
                return;
            // copied from ListBox code, to notify owning dopesheet:
            var dopesheet = ItemsControlFromItemContainer(this) as Dopesheet;
            dopesheet?.NotifyDopesheetRowClicked(this, e.ChangedButton);
        }

        public string SortingText { get; set; }
        public ulong NodeId { get; set; }

        #region HeaderColumnWidth

        public static readonly DependencyProperty HeaderColumnWidthProperty;

        public double HeaderColumnWidth
        {
            get => (double)GetValue(HeaderColumnWidthProperty);
            set => SetValue(HeaderColumnWidthProperty, value);
        }

        #endregion

        #region IsHighlighted

        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register(
            "IsHighlighted", typeof(bool), typeof(DopesheetRow), new PropertyMetadata(default(bool)));

        public bool IsHighlighted
        {
            get => (bool) GetValue(IsHighlightedProperty);
            set => SetValue(IsHighlightedProperty, value);
        }

        #endregion
    }
}
