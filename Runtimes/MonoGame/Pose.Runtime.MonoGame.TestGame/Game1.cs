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
        }

        protected override void LoadContent()
        {
            _poseRuntime = new PoseRuntime(new Renderer(_graphicsDeviceManager))
            {
                UseMultiCore = true
            };

            // use this variant to load from files, without using the content pipeline:
            // var skeletonDefinition = SkeletonDefinition.LoadFromFiles(GraphicsDevice, "poser");

            var skeletonDefinition = Content.LoadPoseSkeletonDefinition("poser");
            
            _skeletons = new List<Skeleton>();

            // DEMO 1 -----------
            //CreateDemo1(skeletonDefinition);

            // DEMO 2 ------------------
            CreateDemo2(skeletonDefinition);
            StartAnimations("Run");
        }

        private void CreateDemo1(SkeletonDefinition skeletonDefinition)
        {
            _cameraZoom = 1f;
            _skeletons.Add(_poseRuntime.AddSkeleton(skeletonDefinition, new Vector2(0, 0), 0, 0));
            _skeletons.Add(_poseRuntime.AddSkeleton(skeletonDefinition, new Vector2(-100, 0), 20, 0));
        }

        private void CreateDemo2(SkeletonDefinition skeletonDefinition)
        {
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
            _poseRuntime.Draw((float)gameTime.TotalGameTime.TotalSeconds / 2f);

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
