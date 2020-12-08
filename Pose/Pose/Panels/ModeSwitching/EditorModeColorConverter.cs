using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Pose.Framework;

namespace Pose.Panels.ModeSwitching
{
    public class EditorModeColorConverter
    : IValueConverter
    {
        private readonly Brush _animateBrush;
        private readonly Brush _designBrush;

        public EditorModeColorConverter()
        {
            _animateBrush = new SolidColorBrush(ColorUtils.FromHex("#C17400"));
            _designBrush = new SolidColorBrush(ColorUtils.FromHex("#2679D3"));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? _animateBrush : _designBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
