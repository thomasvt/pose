using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pose.Controls
{
    public class IconButton : Button
    {
        static IconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconButton), new FrameworkPropertyMetadata(typeof(IconButton)));
        }

        public static readonly DependencyProperty IconGeometryProperty = DependencyProperty.Register(
            "IconGeometry", typeof(Geometry), typeof(IconButton), new PropertyMetadata(default(Geometry)));

        /// <summary>
        /// Don't set Content, set your icon geometry here.
        /// </summary>
        public Geometry IconGeometry

        {
            get => (Geometry) GetValue(IconGeometryProperty);
            set => SetValue(IconGeometryProperty, value);
        }

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            "Stroke", typeof(Brush), typeof(IconButton), new PropertyMetadata(default(Brush)));

        public Brush Stroke
        {
            get => (Brush) GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness", typeof(double), typeof(IconButton), new PropertyMetadata(default(double)));

        public double StrokeThickness
        {
            get => (double) GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }
    }
}
