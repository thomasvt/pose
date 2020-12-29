using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pose.Runtime.MonoGameDotNetCore;
using Pose.Runtime.MonoGameDotNetCore.QuadRendering;
using Pose.Runtime.MonoGameDotNetCore.Skeletons;

namespace Pose.Runtime.MonoGame.TestGame
{
    public class Game1 : Game
    {
        private PoseRuntime _poseRuntime;
        private List<Skeleton> _skeletons;
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private float _cameraZoom;

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
            _poseRuntime = new PoseRuntime(new GpuMeshRenderer(_graphicsDeviceManager, BlendState.AlphaBlend, DepthStencilState.None))
            {
                UseMultiCore = false
            };

            var poseDocument = Content.LoadPoseDocument("poser.pose");
            var spritesheet = Content.LoadSpritesheet("poser.sheet");
            var texture = Content.Load<Texture2D>("poser");
            var skeletonDefinition = new SkeletonDefinition(poseDocument, spritesheet, texture);
            _skeletons = new List<Skeleton>();

            // DEMO 1 -----------

            _cameraZoom = 1f;
            _skeletons.Add(_poseRuntime.AddSkeleton(skeletonDefinition, new Vector2(0, 0), 0, 0));
            _skeletons.Add(_poseRuntime.AddSkeleton(skeletonDefinition, new Vector2(-100, 0), 20, 0));

            // DEMO 2 ------------------

            //_cameraZoom = 0.2f;
            //var r = new Random();
            //for (var i = 0; i < 5000; i++)
            //{
            //    _skeletons.Add(_poseRuntime.AddSkeleton(skeletonDefinition, new Vector3((i % 100 - 50) * 200, (i / 100 - 25) * 200, 0), (float)r.NextDouble() * 6.283f));
            //}
        }

        protected override void UnloadContent()
        {
            _poseRuntime.Dispose();
        }

        private bool _isFirst = true;

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (!_isFirst) return;

            var t = (float) gameTime.TotalGameTime.TotalSeconds;
            var i = 0f;
            foreach (var skeleton in _skeletons)
            {
                skeleton.StartAnimation("Rest", t - i);
                i += 0.097f;
            }
            _isFirst = false;
        }

        private int frameCount = 0;
        private float _averageUpdate = 0;
        private float _averageDraw = 0;
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Azure);

            _poseRuntime.SetCameraPosition(new Vector2(0, 0), _cameraZoom);
            _poseRuntime.Draw((float)gameTime.TotalGameTime.TotalSeconds / 2f);
            _averageUpdate += ((float)_poseRuntime.UpdateTime - _averageUpdate) * 0.1f;
            _averageDraw += ((float)_poseRuntime.DrawTime - _averageDraw) * 0.1f;
            if (frameCount++ % 60 == 0)
            {
                Debug.WriteLine($"U = {_averageUpdate:0.0}ms    D = {_averageDraw:0.0}ms");
            }
        }

        protected override void EndRun()
        {
            File.WriteAllText("fps.txt", $"U = {_averageUpdate:0.0}ms    D = {_averageDraw:0.0}ms");
        }
    }
}
