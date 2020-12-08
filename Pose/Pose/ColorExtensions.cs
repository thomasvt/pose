using System.Windows.Media;

namespace Pose
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, byte a)
        {
            color.A = a;
            return color;
        }
    }
}
