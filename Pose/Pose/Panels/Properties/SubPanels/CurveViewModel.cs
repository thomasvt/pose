using System.Windows.Media;

namespace Pose.Panels.Properties.SubPanels
{
    public class CurveViewModel
    : ViewModel
    {
        public CurveViewModel(Geometry geometry, string label)
        {
            Geometry = geometry;
            Label = label;
        }

        public Geometry Geometry { get; }
        public string Label { get; }
    }
}
