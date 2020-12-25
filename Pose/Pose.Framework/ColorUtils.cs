using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace Pose.Framework
{
    public static class ColorUtils
    {
        public static void RgbToHsl(byte r, byte g, byte b, out double h, out double s, out double l)
        {
            // from: http://csharphelper.com/blog/2016/08/convert-between-rgb-and-hls-color-models-in-c/

            // Convert RGB to a 0.0 to 1.0 range.
            var double_r = r / 255.0;
            var double_g = g / 255.0;
            var double_b = b / 255.0;

            // Get the maximum and minimum RGB components.
            var max = double_r;
            if (max < double_g) max = double_g;
            if (max < double_b) max = double_b;

            var min = double_r;
            if (min > double_g) min = double_g;
            if (min > double_b) min = double_b;

            var diff = max - min;
            l = (max + min) / 2;
            if (Math.Abs(diff) < 0.00001)
            {
                s = 0;
                h = 0;  // H is really undefined.
            }
            else
            {
                if (l <= 0.5) s = diff / (max + min);
                else s = diff / (2 - max - min);

                var r_dist = (max - double_r) / diff;
                var g_dist = (max - double_g) / diff;
                var b_dist = (max - double_b) / diff;

                if (double_r == max) h = b_dist - g_dist;
                else if (double_g == max) h = 2 + r_dist - b_dist;
                else h = 4 + g_dist - r_dist;

                h = h * 60;
                if (h < 0) h += 360;
            }
        }

        public static void HslToRgb(double h, double s, double l, out byte r, out byte g, out byte b)
        {
            double p2;
            if (l <= 0.5) p2 = l * (1 + s);
            else p2 = l + s - l * s;

            var p1 = 2 * l - p2;
            double double_r, double_g, double_b;
            if (s == 0)
            {
                double_r = l;
                double_g = l;
                double_b = l;
            }
            else
            {
                double_r = QqhToRgb(p1, p2, h + 120);
                double_g = QqhToRgb(p1, p2, h);
                double_b = QqhToRgb(p1, p2, h - 120);
            }

            // Convert RGB to the 0 to 255 range.
            r = (byte)(double_r * 255.0);
            g = (byte)(double_g * 255.0);
            b = (byte)(double_b * 255.0);
        }

        private static double QqhToRgb(double q1, double q2, double hue)
        {
            if (hue > 360) hue -= 360;
            else if (hue < 0) hue += 360;

            if (hue < 60) return q1 + (q2 - q1) * hue / 60;
            if (hue < 180) return q2;
            if (hue < 240) return q1 + (q2 - q1) * (240 - hue) / 60;
            return q1;
        }

        /// <summary>
        /// From HSL, all within [0, 1].
        /// </summary>
        public static Color FromHsl(float hue, float saturation, float luminance)
        {
            if (saturation == 0f)
            {
                var c = (byte)(luminance * byte.MaxValue);
                return Color.FromRgb(c, c, c);
            }

            var v2 = luminance + saturation - saturation * luminance;
            if (luminance < 0.5f)
            {
                v2 = luminance * (1 + saturation);
            }
            var v1 = 2f * luminance - v2;

            return Color.FromRgb((byte)(HueToRgb(v1, v2, hue + 1f / 3f) * byte.MaxValue), (byte)(HueToRgb(v1, v2, hue) * byte.MaxValue), (byte)(HueToRgb(v1, v2, hue - 1f / 3f) * byte.MaxValue));
        }

        private static float HueToRgb(float v1, float v2, float vH)
        {
            vH += vH < 0 ? 1 : 0;
            vH -= vH > 1 ? 1 : 0;
            var ret = v1;

            if (6 * vH < 1)
            {
                ret = v1 + (v2 - v1) * 6 * vH;
            }

            else if (2 * vH < 1)
            {
                ret = (v2);
            }

            else if (3 * vH < 2)
            {
                ret = v1 + (v2 - v1) * (2f / 3f - vH) * 6f;
            }

            return ret < 0f ? 0f : (ret > 1f ? 1f : ret);
        }

        public static string ToHexString(this Color color, bool includeAlpha = true)
        {
            var r = color.R.ToString("X").PadLeft(2, '0');
            var g = color.G.ToString("X").PadLeft(2, '0');
            var b = color.B.ToString("X").PadLeft(2, '0');
            if (includeAlpha)
            {
                var a = color.A.ToString("X").PadLeft(2, '0');
                return $"#{r}{g}{b}{a}";
            }

            return $"#{r}{g}{b}";
        }

        private static readonly Regex HexColorRegex =
            new Regex("#?(?<r>[0-9a-fA-F]{1,2})(?<g>[0-9a-fA-F]{1,2})(?<b>[0-9a-fA-F]{1,2})(?<a>[0-9a-fA-F]{1,2})?");

        public static bool IsHexCode(string hexCode)
        {
            return HexColorRegex.IsMatch(hexCode);
        }

        public static Color FromHex(string hexCode)
        {
            var match = HexColorRegex.Match(hexCode);
            if (!match.Success)
                throw new Exception($"Could not parse color from \"{hexCode}\".");
            var r = ParseChannelValue(match.Groups["r"].Value);
            var g = ParseChannelValue(match.Groups["g"].Value);
            var b = ParseChannelValue(match.Groups["b"].Value);
            if (match.Groups["a"].Success)
            {
                var a = ParseChannelValue(match.Groups["a"].Value);
                return Color.FromArgb(a, r, g, b);
            }

            return Color.FromRgb(r, g, b);
        }

        private static byte ParseChannelValue(string hexValue)
        {
            return (byte)int.Parse(hexValue, NumberStyles.HexNumber);
        }
    }
}
