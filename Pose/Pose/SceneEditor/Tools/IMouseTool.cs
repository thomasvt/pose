using System.Windows;
using System.Windows.Input;

namespace Pose.SceneEditor.Tools
{
    internal interface IMouseTool
    {
        void MouseLeftDown(Point mousePosition);
        void MouseDoubleClick(Point mousePosition);
        void MouseLeftUp(Point mousePosition);
        Cursor MouseCursor { get; }
    }
}