using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pose.Runtime.MonoGameDotNetCore.QuadRendering;
using Pose.Runtime.MonoGameDotNetCore.Skeletons;

namespace Pose.Runtime.MonoGameDotNetCore
{
    public class PoseRuntime
    : IDisposable
    {
        private readonly QuadRenderer _quadRenderer;
        private readonly List<Skeleton> _skeletons;

        public PoseRuntime(QuadRenderer quadRenderer)
        {
            _quadRenderer = quadRenderer;
            _skeletons = new List<Skeleton>();

            ViewTransform = Matrix.Identity;
        }

        /// <summary>
        /// Adds the skeleton to the runtime, the runtime will dispose the skeleton.
        /// </summary>
        public Skeleton AddSkeleton(SkeletonDefinition skeletonDefinition, Vector3 position, float angle)
        {
            skeletonDefinition.RegisterTextures(_quadRenderer);
            var skeleton = skeletonDefinition.CreateInstance(position, angle);
            _skeletons.Add(skeleton);
            return skeleton;
        }

        /// <summary>
        /// Draws all skeletons. PoseRuntime uses Y+ means up convention. Position (0,0) with view = Identity is the center of the screen. Z can be used for depth occlusion, and must lie within [0,1].
        /// </summary>
        /// <param name="gameTime">Time information as received from MonoGame. Used to update the Pose animations.</param>
        public void Draw(GameTime gameTime)
        {
            var gameTimeSeconds = (float) gameTime.TotalGameTime.TotalSeconds;
            UpdateAnimations(gameTimeSeconds);
            RenderSprites();
        }

        private void UpdateAnimations(float gameTimeSeconds)
        {
            var sw = Stopwatch.StartNew();
            if (UseMultiCore)
            {
                Parallel.ForEach(_skeletons, skeleton => { skeleton.Update(gameTimeSeconds); });
            }
            else
            {
                foreach (var skeleton in _skeletons)
                {
                    skeleton.Update(gameTimeSeconds);
                }
            }

            UpdateTime = sw.Elapsed.TotalMilliseconds;
        }

        private void RenderSprites()
        {
            _quadRenderer.ProjectionTransform = ProjectionTransform;
            _quadRenderer.ViewTransform = ViewTransform;
            _quadRenderer.BlendState = BlendState.NonPremultiplied;

            var sw = Stopwatch.StartNew();
            _quadRenderer.BeginRender();
            foreach (var skeleton in _skeletons)
            {
                skeleton.Draw(_quadRenderer);
            }
            _quadRenderer.EndRender();

            DrawTime = sw.Elapsed.TotalMilliseconds;
        }

        public void Dispose()
        {
            _quadRenderer?.Dispose();
        }

        /// <summary>
        /// Prefab method to set ViewTransform and Projection to a default 2D orthographic camera at a certain position in the world. You must call this method each draw-frame if you use it.
        /// The camera's view has the same size as the MonoGame viewport (pixels) and has X+ pointing to the right and Y+ pointing up.
        /// Alternatively, if you want more control over ViewTransform and Projection matrices, set them directly using their properties and don't call this method.
        /// </summary>
        public void SetCameraPosition(Vector2 position, float zoom = 1f)
        {
            var viewport = _quadRenderer.GraphicsDevice.Viewport;
            var halfWidth = viewport.Width * 0.5f / zoom;
            var halfHeight = viewport.Height * 0.5f / zoom;
            ProjectionTransform = Matrix.CreateOrthographicOffCenter(-halfWidth, +halfWidth, -halfHeight, +halfHeight, 0f, 1f);
            ViewTransform = Matrix.CreateTranslation(-position.X, -position.Y, -1f);
        }

        /// <summary>
        /// Set this to the view matrix of your camera, or call SetCameraPosition() instead for simple cases.
        /// </summary>
        public Matrix ViewTransform { get; set; }

        /// <summary>
        /// Set this to the projection matrix of your camera, or call SetCameraPosition() instead for simple cases.
        /// </summary>
        public Matrix ProjectionTransform { get; set; }

        /// <summary>
        /// Multi-core processing or not. Multi-core processing comes at an overhead cost so only enable it when you have enough animations to justify the overhead. Test the performance difference.
        /// </summary>
        public bool UseMultiCore { get; set; }

        /// <summary>
        /// The time (ms) it took to update all skeleton animations (the previous call to Draw()).
        /// </summary>
        public double UpdateTime { get; private set; }
        /// <summary>
        /// The time (ms) it took to draw all skeletons (the previous call to Draw()).
        /// </summary>
        public double DrawTime { get; private set; }
    }
}
