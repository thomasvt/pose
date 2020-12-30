using System;
using System.Windows;
using System.Windows.Media;
using Pose.Common;
using Pose.Common.Curves;
using Pose.Domain;

namespace Pose.Controls.CurveEditor
{
    public static class CurveGeometryBuilder
    {
        /// <summary>
        /// Generates WPF <see cref="Geometry"/> showing the curve defined by the given Pose interpolation curve.
        /// </summary>
        public static Geometry BuildBezier(BezierCurve curve, int segmentCount, double offsetY, double width, double height)
        {
            if (curve.P0 != Vector2.Zero || curve.P3 != Vector2.One)
                throw new NotSupportedException("Only supports Bezier interpolation curves from point 0,0 to 1,1.");

            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                var fx = curve.GetFx();
                var fy = curve.GetFy();
                context.BeginFigure(new Point(0, offsetY + height), false, false);
                for (var i = 1; i <= segmentCount; i++)
                {
                    var t = (float) i / segmentCount;
                    var x = fx.Solve(t);
                    var y = fy.Solve(t);
                    context.LineTo(new Point(x * width, offsetY + height - (y *  height)), true, false);
                }
            }
            geometry.Freeze();
            return geometry;
        }

        public static Geometry BuildLinear(in double offsetY, in double width, double height)
        {
            return new LineGeometry(new Point(0, offsetY + height), new Point(width, offsetY));
        }

        public static Geometry BuildHold(in double offsetY, in double width, double height)
        {
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(new Point(0, offsetY + height), false, false);
                context.LineTo(new Point(width, offsetY + height), true, false);
                context.LineTo(new Point(width, offsetY), true, false);
            }
            geometry.Freeze();
            return geometry;
        }
    }
}
