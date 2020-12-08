using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Pose.Framework;

namespace Pose.Panels.ModeSwitching
{
    public class BoolToLabelColorConverter
    : IValueConverter
    {
        private readonly Brush _trueBrush;
        private readonly Brush _falseBrush;

        public BoolToLabelColorConverter()
        {
            _trueBrush = new SolidColorBrush(ColorUtils.FromHex("#eeeeee"));
            _falseBrush = new SolidColorBrush(ColorUtils.FromHex("#666666"));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? _trueBrush : _falseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
