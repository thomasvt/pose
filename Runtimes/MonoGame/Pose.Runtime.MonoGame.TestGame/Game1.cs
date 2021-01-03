using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pose.Runtime.MonoGameDotNetCore;
using Pose.Runtime.MonoGameDotNetCore.Skeletons;

namespace Pose.Runtime.MonoGame.TestGame
{
    public class Game1 : Game
    {
        private PoseRuntime _poseRuntime;
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
            // The PoseRuntime is a light manager class for high performance 2D rendering. It supports rendering Pose animations, but also your custom (not-made-with-Pose) entities your game needs to render.
            // The runtime can therefore serve as the only renderer for your 2D game.

            // Note: the PoseRuntime is optional: you can also just load Pose skeletons and draw them through your own rendering code.
            //       When calling Skeleton.Draw() you need to supply an ICpuMeshRenderer which will receive a single Mesh from the Skeleton
            //       containing an array of vertices, indices and a texture, so all you need to do is send it to the gpu.

            // To use PoseRuntime:

            // 1. new PoseRuntime(GraphicsDeviceManager)
            // 2. load a Pose SkeletonDefinition from file or MonoGame's content pipeline
            // 3. create a Skeleton instance of the SkeletonDefinition.
            // 4. PoseRuntime.Add(mySkeleton)
            // 5. each frame: PoseRuntime.Draw().

            _poseRuntime = new PoseRuntime(_graphicsDeviceManager)
            {
                UseMultiCore = true
            };
            
            var skeletonDefinition = SkeletonDefinition.LoadFromFiles(GraphicsDevice, "../../../../../../pose/pose/assets/poser/poser"); // this points to the original 'poser' sample files in git so we don't need to copy them over each time it changes.

            // use this following variant to load via MonoGame's content pipeline 
            // note: in the MG pipeline tool: add the .png just like any texture, add the .sheet and .pose files with Build Action 'Copy'
            // var skeletonDefinition = Content.LoadPoseSkeletonDefinition("poser");

            // DEMO 1 -----------
            //CreateDemo1(skeletonDefinition);
            // ----

            // DEMO 2 ------------------
            CreateDemo2(skeletonDefinition); // don't forget setting UseMultiCore = true in the PoseRuntime.
            // ----
            
            StartAnimations("Run"); // the animationname is the one assigned to the animation in Pose Editor
        }

        private void CreateDemo1(SkeletonDefinition skeletonDefinition)
        {
            _cameraZoom = 1f;

            // shows four running guys with different depths to show layering of sprites.
            // the second is most in front, 1 and 3 are behind that, and 4th is furthest

            _poseRuntime.Add(skeletonDefinition.CreateInstance(new Vector2(-100, 0), 1, 0));
            _poseRuntime.Add(skeletonDefinition.CreateInstance(new Vector2(0, 0), 0, 0));
            _poseRuntime.Add(skeletonDefinition.CreateInstance(new Vector2(100, 0), 1, 0));
            _poseRuntime.Add(skeletonDefinition.CreateInstance(new Vector2(200, 0), 2, 0));
        }

        private void CreateDemo2(SkeletonDefinition skeletonDefinition)
        {
            // this shows a lot of running guys in a grid with random rotations. It's to test performance and try out potential improvements of the runtime logic.
            // for detailed performance measuring, I suggest using JetBrains' profiler in Line-by-Line mode.

            _cameraZoom = 0.3f;
            var r = new Random();
            const int count = 5000;
            var horizCount = (int) (MathF.Sqrt(count) * 1.3f);
            var vertCount = count / horizCount + 1;
            const int distance = 100;
            for (var i = 0; i < count; i++)
            {
                var position = new Vector2(i % horizCount - horizCount / 2, i / horizCount - vertCount / 2) * distance;
                _poseRuntime.Add(skeletonDefinition.CreateInstance(position, 0, (float) r.NextDouble() * 6.283f));
            }
        }

        private void StartAnimations(string animationName)
        {
            var t = 0;
            var offset = 0f;
            foreach (var skeleton in _poseRuntime.Entities.OfType<Skeleton>())
            {
                skeleton.StartAnimation(animationName, t - offset);
                offset += 0.097f; // add diversity in the animation's starttime
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
