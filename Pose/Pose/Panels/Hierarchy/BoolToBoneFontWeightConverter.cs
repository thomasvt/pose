using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Pose.Panels.Hierarchy
{
    public class BoolToBoneFontWeightConverter
    : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool) value)
                return FontWeights.Bold;
            return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
