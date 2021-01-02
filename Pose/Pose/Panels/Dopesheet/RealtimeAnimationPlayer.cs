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

        private void UpdateScene()
        {
            var animation = _editor.GetCurrentAnimation();
            if (animation.IsLoop != _previousIsLoop)
            {
                SetPlayCursorToCurrentFrame();
                _previousIsLoop = animation.IsLoop;
            }
            var animationFrame = CalculateCurrentFrame();
            
            _editor.ApplyFrameToScene((float)animationFrame, _isFirstFrame);
            _isFirstFrame = false;

            var animationFrameInt = (int)animationFrame;
            SetDopesheetTimeCursor(animationFrameInt);

            if (!animation.IsLoop && animationFrameInt == animation.EndFrame)
                EndReached?.Invoke();
        }

        private void SetDopesheetTimeCursor(int animationFrameInt)
        {
            if (animationFrameInt == _previousAnimationFrameInt) 
                return;

            _editor.ChangeCurrentAnimationCurrentFrameTransient(animationFrameInt, true);
            _previousAnimationFrameInt = animationFrameInt;
        }

        private double CalculateCurrentFrame()
        {
            var animation = _editor.GetCurrentAnimation();
            var animationFrameCount = animation.EndFrame - animation.BeginFrame + (animation.IsLoop ? 1 : 0); // a loop needs an extra frame to return from last to first, the last != first in our implementation. eg. 0 -> 59 had 59 frames + 1 to return to start = 60 frames
            var animationFrameDurationTicks = (long)(1d / animation.FramesPerSecond * Stopwatch.Frequency);
            var animationDurationTicks = animationFrameDurationTicks * animationFrameCount;

            var timerTicks = _initialPositionTicks + _stopwatch.ElapsedTicks;

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
