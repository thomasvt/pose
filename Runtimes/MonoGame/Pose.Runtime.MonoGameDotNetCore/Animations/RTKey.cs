namespace Pose.Runtime.MonoGameDotNetCore.Animations
{
    internal struct RTKey
    {
        public readonly float Time;
        public readonly float Value;

        public RTKey(float time, float value)
        {
            Time = time;
            Value = value;
        }
    }
}
