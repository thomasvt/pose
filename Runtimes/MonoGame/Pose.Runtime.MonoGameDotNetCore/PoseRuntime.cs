using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Pose.Runtime.MonoGameDotNetCore.Rendering;
using Pose.Runtime.MonoGameDotNetCore.Skeletons;

namespace Pose.Runtime.MonoGameDotNetCore
{
    /// <summary>
    /// This is the root object for rendering Pose animations and optionally all your other game entities.
    /// </summary>
    public class PoseRuntime
    : IDisposable
    {
        private readonly IRenderer _renderer;
        private readonly List<Skeleton> _skeletons;
        private readonly List<IRenderable> _renderables;

        public PoseRuntime(GraphicsDeviceManager graphicsDeviceManager)
            :this(new DefaultRenderer(graphicsDeviceManager))
        {
        }

        /// <summary>
        /// Creates a PoseRuntime with a custom <see cref="IRenderer"/>. Use the other ctor overload if you want to use the default.
        /// </summary>
        public PoseRuntime(IRenderer renderer)
        {
            _renderer = renderer;
            _skeletons = new List<Skeleton>();
            _renderables = new List<IRenderable>();

            ViewTransform = Matrix.Identity;
        }

        /// <summary>
        /// Adds a skeleton or other renderable game entity to the collection.
        /// </summary>
        public void Add(IRenderable renderable)
        {
            if (renderable is Skeleton skeleton)
            {
                _skeletons.Add(skeleton);
            }
            _renderables.Add(renderable);
        }

        /// <summary>
        /// Draws all skeletons. PoseRuntime uses Y+ = up convention. Position (0,0) with view = Identity is the center of the screen. Z can be used for depth occlusion.
        /// </summary>
        /// <param name="gameTime">Gametime in seconds. Used to update the Pose animations.</param>
        public void Draw(float gameTime)
        {
            UpdateAnimations(gameTime);
            RenderEntities();
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

        private void RenderEntities()
        {
            _renderer.ProjectionTransform = ProjectionTransform;
            _renderer.ViewTransform = ViewTransform;

            var sw = Stopwatch.StartNew();
            
            _renderer.Begin();
            //_renderables.Sort((a, b) => b.Depth.CompareTo(a.Depth)); // todo optimize for infrequent Depth changing, IsDepthDirty -> move item
            foreach (var skeleton in _renderables.OrderByDescending(r => r.Depth))
            {
                skeleton.Draw(_renderer);
            }
            _renderer.End();

            DrawTime = sw.Elapsed.TotalMilliseconds;
        }

        public void Dispose()
        {
            (_renderer as IDisposable)?.Dispose();
        }

        /// <summary>
        /// Prefab method to set ViewTransform and Projection to a default 2D orthographic camera at a certain position in the world. You must call this method each draw-frame if you use it.
        /// The camera's view has the same size as the MonoGame viewport (pixels) and has X+ pointing to the right and Y+ pointing up.
        /// Alternatively, if you want more control over ViewTransform and Projection matrices, set them directly using their properties and don't call this method.
        /// </summary>
        /// <param name="zoom">1 means: 1 world unit == 1 screen pixel, 2 means: 1 world unit == 2 pixels, ...</param>
        /// <param name="nearPlane">Z is used for sprite depth order. High Z is drawn behind low Z. Z must be between nearplane and farplane. Using a large plane range causes inaccuracies in Z ordering, so keep it close to what you need.</param>
        /// <param name="farPlane">Z is used for sprite depth order. High Z is drawn behind low Z. Z must be between nearplane and farplane. Using a large plane range causes inaccuracies in Z ordering, so keep it close to what you need.</param>
        public void SetCameraPosition(Vector2 position, float zoom = 1f, float nearPlane = 0f, float farPlane = 100f)
        {
            var viewport = _renderer.GraphicsDevice.Viewport;
            var halfWidth = viewport.Width * 0.5f / zoom;
            var halfHeight = viewport.Height * 0.5f / zoom;
            ProjectionTransform = Matrix.CreateOrthographicOffCenter(-halfWidth, +halfWidth, -halfHeight, +halfHeight, nearPlane, farPlane);
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
        /// Distributed animation calculations of all skeletons among all processors. Multi-core processing comes at an overhead cost so only enable it when you have many skeletons. Test the performance difference.
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

        /// <summary>
        /// The allowed error 
        /// </summary>
        public float BezierTolerance { get; set; }

        public IEnumerable<IRenderable> Entities => _renderables;
    }
}
