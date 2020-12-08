namespace Pose.Controls.Dopesheet
{
    public readonly struct FrameRange
    {
        public int Min { get; }
        public int Max { get; }

        public FrameRange(int min, int max)
        {
            Min = min;
            Max = max;
        }
    }
}
