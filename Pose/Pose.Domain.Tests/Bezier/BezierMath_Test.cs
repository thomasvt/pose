using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pose.Common;
using Pose.Common.Curves;

namespace Pose.Domain.Tests.Bezier
{
    [TestClass]
    public class BezierMath_Test
    {
        [TestMethod]
        public void Test1()
        {
            var bezier = new BezierCurve(new Vector2(0, 0), new Vector2(1,0), new Vector2(0, 1), new Vector2(1, 1));
            PrintBezierPoints(bezier);
        }

        [TestMethod]
        public void Test2()
        {
            var bezier = new BezierCurve(new Vector2(0, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 1), new Vector2(1f, 1));
            PrintBezierPoints(bezier);
        }

        [TestMethod]
        public void Perfect_Hit_In_Newton_Approx_Should_Not_Return_NaN()
        {
            var bezier = new BezierCurve(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1));
            var result = BezierMath.GetYAtX(bezier, 0.5f);
            Assert.IsFalse(float.IsNaN(result));
        }

        private static void PrintBezierPoints(BezierCurve bezier)
        {
            var points = 20;
            for (var i = 0; i < points+1; i++)
            {
                var x = i / (float)points;
                Console.WriteLine($"({x:0.00}, {BezierMath.GetYAtX(bezier, x)})");
            }
        }
    }
}
