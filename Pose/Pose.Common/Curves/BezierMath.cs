using System;

namespace Pose.Common.Curves
{
    public static class BezierMath
    {
        // (Bezier calculation was uncharted territory for me, so I added a lot of comment here for my future ignorant self. Please let me know if you know a better way to do this)

        // Bezier curves are not just mathematical functions f(t). They can have multiple points at a single value of t, eg. they can form a looping. Single math functions cannot model that.

        // Instead, Bezier curves are a combination of two math functions fx(s) and fy(s) where each 2D point on the curve is (x,y) = (fx(s), fy(s)) 
        // The function definitions and their parameters (and how to calculate them from all 4 Bezier points) can be found here:
        // https://moshplant.com/direct-or/bezier/math.html 

        // A problem with bezier curves is that they get calculated per s, where s is not one of the axes, but the distance ALONG the CURVE itself. So, if you imagine a bezier curve to be a rope,
        // s is the percentage distance along that rope in a certain point on the rope. The fx(s) and fy(s) functions then return the X and Y position of where that distance along the rope is s.

        // In our animation context, though, we need to find Y values for certain points in TIME, where Time is the X-axis. So, the 's' input in the above functions is NOT our timeinput.

        // So, for interpolating animation values according to a bezier curve, we need to use 2 steps:
        // 1. use fx(s) to find s at a certain X (=animation time) - this is not straight forward as we want the opposite of what fx(s) does: we need the input s where output x is a known value.
        // 2. use fy(s) to find Y at that s. This is straight forward: y = fy(s)

        // step 1 is the hardest: we use root-finding for that, with Newton-Raphson. https://en.wikipedia.org/wiki/Newton's_method

        public static float GetYAtX(BezierCurve curve, float x)
        {
            var t = GetSAtX(curve, x);
            return curve.GetFy().Solve(t);
        }

        /// <summary>
        /// Fast method to find approximation of t where fx(t) ≈ x (within given tolerance). 
        /// </summary>
        private static float GetSAtX(BezierCurve curve, in float x, float tolerance = 0.001f)
        {
            if (x < 0f || x > 1f)
                throw new ArgumentOutOfRangeException(nameof(x));

            var fx = curve.GetFx();
            fx = new Polynomial3(fx.A, fx.B, fx.C, fx.D - x); // Newton-Raphson finds 0-point of function (s where f(s)=0), but we want s where f(s) = x, so subtract x from D of the polynomial to be able to look for 0-point.
            var fxDerivative = fx.GetDerivative();

            // use Newton-Raphson to find a s that yields an fx(s) closer to 0 than allowed tolerance.
            
            var s = x; // initial guess = what t would be if the spline was perfectly linear.
            for (var i = 0; i < 100; i++)
            {
                var resultX = fx.Solve(s);
                if (resultX >= 0 && resultX < tolerance || resultX < 0 && resultX > -tolerance)
                    break;
                s -= resultX / fxDerivative.Calc(s);
            }

            return s;
        }
    }
}
