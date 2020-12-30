using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pose.Runtime.MonoGameDotNetCore;
using Pose.Runtime.MonoGameDotNetCore.Rendering;
using Pose.Runtime.MonoGameDotNetCore.Skeletons;

namespace Pose.Runtime.MonoGame.TestGame
{
    public class Game1 : Game
    {
        private PoseRuntime _poseRuntime;
        private List<Skeleton> _skeletons;
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private float _cameraZoom;

        // performance tracking
        private int frameCount = 0;
        private float _averageUpdate = 0;
        private float _averageDraw = 0;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 3000,
                PreferredBackBufferHeight = 2000,
                GraphicsProfile = GraphicsProfile.Reach
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;
        }

        protected override void LoadContent()
        {
            _poseRuntime = new PoseRuntime(new Renderer(_graphicsDeviceManager))
            {
                UseMultiCore = false
            };

            
            var skeletonDefinition = SkeletonDefinition.LoadFromFiles(GraphicsDevice, "../../../../../../pose/pose/assets/poser/poser"); // this points to the original 'poser' sample files in git so we don't need to copy them over each time it changes.
            // use this variant to load via MonoGame's content pipeline (in the pipeline tool: add the .png just like any texture, add .sheet and .pose with Build Action 'Copy'_
            //var skeletonDefinition = Content.LoadPoseSkeletonDefinition("poser");

            _skeletons = new List<Skeleton>();

            // DEMO 1 -----------
            CreateDemo1(skeletonDefinition);
            // ----

            // DEMO 2 ------------------
            //CreateDemo2(skeletonDefinition);
            // ----
            
            StartAnimations("Run");
        }

        private void CreateDemo1(SkeletonDefinition skeletonDefinition)
        {
            _cameraZoom = 1f;

            // shows four running guys with different depths to show layering of sprites.
            // the second is most in front, 1 and 3 are behind that, and 4th is furthest

            _skeletons.Add(_poseRuntime.AddSkeleton(skeletonDefinition, new Vector2(-100, 0), 1, 0));
            _skeletons.Add(_poseRuntime.AddSkeleton(skeletonDefinition, new Vector2(0, 0), 0, 0));
            _skeletons.Add(_poseRuntime.AddSkeleton(skeletonDefinition, new Vector2(100, 0), 1, 0));
            _skeletons.Add(_poseRuntime.AddSkeleton(skeletonDefinition, new Vector2(200, 0), 2, 0));
        }

        private void CreateDemo2(SkeletonDefinition skeletonDefinition)
        {
            // this just shows a lot of running guys in a grid and random rotations, to test performance and try out potential improvements of the runtime logic.
            // for detailed performance measuring, I suggest using JetBrains' profiler in line-by-line mode.

            _cameraZoom = 0.3f;
            var r = new Random();
            const int count = 6000;
            var horizCount = (int) (MathF.Sqrt(count) * 1.3f);
            var vertCount = count / horizCount + 1;
            const int distance = 100;
            for (var i = 0; i < count; i++)
            {
                _skeletons.Add(_poseRuntime.AddSkeleton(skeletonDefinition,
                    new Vector2((i % horizCount - horizCount / 2) * distance, (i / horizCount - vertCount / 2) * distance),
                    0, (float) r.NextDouble() * 6.283f));
            }
        }

        private void StartAnimations(string animationName)
        {
            var t = 0;
            var offset = 0f;
            foreach (var skeleton in _skeletons)
            {
                skeleton.StartAnimation(animationName, t - offset);
                offset += 0.097f;
            }
        }

        protected override void UnloadContent()
        {
            _poseRuntime.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Azure);

            _poseRuntime.SetCameraPosition(new Vector2(0, 0), _cameraZoom);
            _poseRuntime.Draw((float)gameTime.TotalGameTime.TotalSeconds);

            MeasurePerformance();
        }

        private void MeasurePerformance()
        {
            _averageUpdate += ((float) _poseRuntime.UpdateTime - _averageUpdate) * 0.1f;
            _averageDraw += ((float) _poseRuntime.DrawTime - _averageDraw) * 0.1f;
            if (frameCount++ % 60 == 0)
            {
                Debug.WriteLine($"U = {_averageUpdate:0.0}ms    D = {_averageDraw:0.0}ms");
            }
        }
    }
}
