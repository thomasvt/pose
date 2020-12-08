using System;

namespace Pose.Domain.Curves
{
    public static class BezierMath
    {
        // (Bezier calculation was uncharted territory for me, so I added a lot of comment here for my future ignorant self. Please let me know if you know a better way to do this, once your laughter has died out.)

        // Bezier curves are not just mathematical functions f(t). They can have multiple points at a single value of t, eg. they can form a loop. Math functions cannot do that.

        // Instead, Bezier curves are a combination of two functions fx(s) and fy(s) where each 2D point is composed of (fx(t), fy(t)) 
        // The functions and their parameters (and how to calculate them from all 4 Bezier points) can be found here:
        // https://moshplant.com/direct-or/bezier/math.html 

        // A problem with bezier curves is that they get calculated per t, where t is not one of the axes, but the distance along the CURVE itself.

        // In our animation context, though, we need to find Y values for certain points in time, where Time is the X-axis. So, 't' is NOT animation time, 'X' is animation time.
        // This is confusing, but as t is used in the websites I linked, I didn't want to change the letter.

        // So, for interpolating animation values according to a bezier curve, we need to find Y at X (time). We do this in 2 steps:
        // 1. find t at a certain X (time)
        // 2. find Y at that t.

        // step 1 is the hardest: we need to find the t where fx(t) = X (=current animation time). We use root-finding for that, with Newton-Raphson. https://en.wikipedia.org/wiki/Newton's_method

        public static float GetYAtX(BezierCurve curve, float x)
        {
            var t = GetTAtX(curve, x);
            return curve.GetFy().Solve(t);
        }

        /// <summary>
        /// Fast method to find approximation of t where fx(t) ≈ x (within given tolerance). 
        /// </summary>
        private static float GetTAtX(BezierCurve curve, in float x, float tolerance = 0.001f)
        {
            if (x < 0f || x > 1f)
                throw new ArgumentOutOfRangeException(nameof(x));

            var fx = curve.GetFx();
            fx = new Polynomial3(fx.A, fx.B, fx.C, fx.D - x); // Newton-Raphson finds 0-point of function (t where f(t)=0), but we want t where f(t) = x, so subtract x from D of the polynomial to be able to look for 0-point.
            var fxDerivative = fx.GetDerivative();

            // use Newton-Raphson to find a T that yields an fx(t) closer to 0 than allowed tolerance.
            
            var guess = x; // initial guess = what t would be if the spline was perfectly linear.
            for (var i = 0; i < 10; i++)
            {
                var value = fx.Solve(guess);
                if (value >= 0 && value < tolerance || value < 0 && value > -tolerance)
                    break;
                guess -= value / fxDerivative.Calc(guess);
            }

            return guess;
        }
    }
}
