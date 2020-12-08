namespace Pose.Runtime.MonoGameDotNetCore.Skeletons
{
    internal struct Transformation
    {
        public float X, Y, Angle;

        public Transformation(float x, float y, float angle)
        {
            X = x;
            Y = y;
            Angle = angle;
        }
    }
}
