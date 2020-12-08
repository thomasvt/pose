namespace Pose.Runtime.MonoGameDotNetCore.Animations
{
    public class RTAnimation
    {
        private float? _startGameTime; // animation was started on this gametime.
        private readonly bool _isLoop;
        private readonly float _duration; // length of animation in sec
        private readonly RTPropertyAnimation[] _propertyAnimations;

        /// <param name="duration">Duration of animation in seconds</param>
        /// <param name="isLoop">Loop or stop at end of animation</param>
        internal RTAnimation(float duration, bool isLoop, RTPropertyAnimation[] propertyAnimations)
        {
            _duration = duration;
            _isLoop = isLoop;
            _propertyAnimations = propertyAnimations;
        }

        /// <summary>
        /// (Re)starts the animation at frame 0.
        /// </summary>
        /// <param name="gameTime">The current absolute time in seconds.</param>
        public void Start(float gameTime)
        {
            _startGameTime = gameTime;
            for (var i = 0; i < _propertyAnimations.Length; i++)
            {
                _propertyAnimations[i].Reset();
            }
        }

        /// <summary>
        /// Updates all animated properties for the given time. Optimized for forward playing, not random jumping through the animation.
        /// </summary>
        internal void PlayForwardTo(in float gameTime, RTNode[] nodes)
        {
            if (!_startGameTime.HasValue)
                return;

            var t = gameTime - _startGameTime.Value;
            if (_isLoop)
                t %= _duration;

            for (var i = 0; i < _propertyAnimations.Length; i++)
            {
                var propertyAnimation = _propertyAnimations[i];
                var newValue = propertyAnimation.PlayForwardTo(t);
                ref var node = ref nodes[propertyAnimation.NodeIdx];

                switch (propertyAnimation.NodeProperty)
                {
                    case NodeProperty.TranslationX:
                        node.AnimateTransformation.X = newValue;
                        break;
                    case NodeProperty.TranslationY:
                        node.AnimateTransformation.Y = newValue;
                        break;
                    case NodeProperty.RotationAngle:
                        node.AnimateTransformation.Angle = newValue;
                        break;
                    default:
                        throw new PoseNotSupportedException($"Animating \"{propertyAnimation.NodeProperty}\" is currently not supported.");
                }
            }
        }
    }
}
