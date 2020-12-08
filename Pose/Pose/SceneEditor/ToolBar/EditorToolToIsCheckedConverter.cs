using System;
using System.Globalization;
using System.Windows.Data;
using Pose.Domain.Editor;

namespace Pose.SceneEditor.ToolBar
{
    public class EditorToolToIsCheckedConverter
    : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tool = (EditorTool) value;
            var expectedTool = (EditorTool) parameter;
            return tool == expectedTool;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
