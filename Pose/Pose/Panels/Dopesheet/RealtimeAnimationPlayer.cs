using System;
using System.Diagnostics;
using System.Windows.Media;
using Pose.Domain.Editor;

namespace Pose.Panels.Dopesheet
{
    /// <summary>
    /// Causes the scene to play the current animation in the editor in realtime.
    /// </summary>
    public class RealtimeAnimationPlayer : IDisposable
    {
        private readonly Editor _editor;
        private Stopwatch _stopwatch;
        private long _initialPositionTicks;
        private int _previousAnimationFrameInt = -1;
        private bool _previousIsLoop;
        private bool _isFirstFrame;

        public RealtimeAnimationPlayer(Editor editor)
        {
            _editor = editor;
        }

        public void Play()
        {
            SetPlayCursorToCurrentFrame();
            CompositionTarget.Rendering += Render;
            UpdateScene();
        }

        private void Render(object sender, EventArgs e)
        {
            UpdateScene();
        }

        private void SetPlayCursorToCurrentFrame()
        {
            _isFirstFrame = true;
            var animation = _editor.GetCurrentAnimation();
            var animationFrameDurationTicks = (long)(1d / animation.FramesPerSecond * Stopwatch.Frequency);
            _initialPositionTicks = (animation.CurrentFrame - animation.BeginFrame) * animationFrameDurationTicks;
            _stopwatch = Stopwatch.StartNew();
        }

        public void JumpToAnimationBegin()
        {
            _isFirstFrame = true;
            _initialPositionTicks = 0;
            _stopwatch = Stopwatch.StartNew();
        }

        //private double _duration;
        //private int i;

        private void UpdateScene()
        {
            //var sw = Stopwatch.StartNew();

            var animation = _editor.GetCurrentAnimation();
            if (animation.IsLoop != _previousIsLoop)
            {
                SetPlayCursorToCurrentFrame();
                _previousIsLoop = animation.IsLoop;
            }
            var animationFrame = CalculateCurrentFrame();
            var animationFrameInt = (int)animationFrame;

            _editor.ApplyFrameToScene((float)animationFrame, _isFirstFrame);
            _isFirstFrame = false;

            if (animationFrameInt != _previousAnimationFrameInt)
            {
                _editor.ChangeCurrentAnimationCurrentFrameTransient(animationFrameInt, true);
                _previousAnimationFrameInt = animationFrameInt;
            }

            //_duration += (sw.Elapsed.TotalSeconds - _duration) * 0.1d;
            //if (i++ % 60 == 0)
            //    Debug.WriteLine($"{_duration * 1000:0.0} ms");

            if (!animation.IsLoop && animationFrameInt == animation.EndFrame)
                EndReached?.Invoke();
        }

        private double CalculateCurrentFrame()
        {
            var animation = _editor.GetCurrentAnimation();
            var animationFrameCount = animation.EndFrame - animation.BeginFrame;
            var animationFrameDurationTicks = (long)(1d / animation.FramesPerSecond * Stopwatch.Frequency);
            var animationDurationTicks = animationFrameDurationTicks * animationFrameCount;

            var timerTicks = (_initialPositionTicks + _stopwatch.ElapsedTicks);

            if (!animation.IsLoop && timerTicks > animationDurationTicks)
            {
                return animation.EndFrame;
            }

            var animationPositionTicks = timerTicks % animationDurationTicks;
            var animationPositionPct = (double)animationPositionTicks / animationDurationTicks;
            return animation.BeginFrame + animationPositionPct * animationFrameCount;
        }
        
        public void Dispose()
        {
            CompositionTarget.Rendering -= Render;
        }

        /// <summary>
        /// Only when not looping.
        /// </summary>
        public Action EndReached;
    }
}
