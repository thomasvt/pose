using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Pose.Domain.Animations;
using Pose.Domain.Documents;
using Pose.Domain.Editor;
using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;
using Pose.SceneEditor;
using Pose.SceneEditor.Viewport;
using Exception = System.Exception;

namespace Pose.Popups.ExportSpritesheets
{
    public class SpriteProducerD3D : ISpriteProducer
    {
        private readonly Editor _editor;
        private readonly SpriteBitmapStore _spriteBitmapStore;
        private Document _document;

        public SpriteProducerD3D(Editor editor, SpriteBitmapStore spriteBitmapStore)
        {
            _editor = editor;
            _spriteBitmapStore = spriteBitmapStore;
            Scale = 1f;
            DpiX = DpiY = 96d;
        }

        public void PrepareDocument()
        {
            var messageBus = new MessageBus();
            _document = CloneDocument(messageBus);
        }

        public RenderTargetBitmap ProduceFirstAnimationFrame(ulong animationId)
        {
            if (_document == null)
                throw new Exception("Call PrepareDocument() first");

            var animation = _document.GetAnimation(animationId);
            return ProduceAnimationFrames(animation, animation.BeginFrame, animation.BeginFrame).Single();
        }

        public List<RenderTargetBitmap> ProduceAnimationFrames(ulong animationId)
        {
            if (_document == null)
                throw new Exception("Call PrepareDocument() first");

            var animation = _document.GetAnimation(animationId);
            return ProduceAnimationFrames(animation, animation.BeginFrame, animation.EndFrame).ToList();
        }

        private IEnumerable<RenderTargetBitmap> ProduceAnimationFrames(Animation animation, int firstFrame, int lastFrame)
        {
            if (_document == null)
                throw new Exception("Call PrepareDocument() first");

            var bounds = GetAnimationWorldBounds(animation, _document);
            var cameraCenter = new Vector(bounds.X + bounds.Width * 0.5, bounds.Y + bounds.Height * 0.5);

            var scale = GetCoercedScale();

            var width = 500; //(int)Math.Ceiling(bounds.Width * scale);
            var height = 500; //(int) Math.Ceiling(bounds.Height * scale);
            ConfigureViewport(width, height);
            
            for (var i = firstFrame; i <= lastFrame; i++)
            {
                _document.ApplyAnimationToScene(animation.Id, i);

                SceneViewport.Clear();
                SceneViewport.PanTo(cameraCenter);
                SceneViewport.SetZoom(scale);

                AddSpritesToViewport(_document);
                var renderTarget = new RenderTargetBitmap(width, height, 144, 144, PixelFormats.Default);
                renderTarget.Render(SceneViewport);
                yield return renderTarget;

                SavePng(renderTarget, $"{animation.Name}_{i:000}.png");
            }
        }

        private float GetCoercedScale()
        {
            var scale = Scale;
            if (scale < 0.01f) scale = 0.01f;
            if (scale > 10f) scale = 10f;
            return scale;
        }

        private Rect GetAnimationWorldBounds(Animation animation, Document document)
        {
            // use a temporarily sized viewport to get the max extents of the animation, there is no clipping so the calculated bounds are correct, whether the viewport is large enough or not.
            ConfigureViewport(500, 500);
            var bounds = CalculateMaxBounds(animation, document);
            return new Rect(-250 + bounds.Left, 250 - bounds.Top - bounds.Height, bounds.Width, bounds.Height);
        }

        private Rect CalculateMaxBounds(Animation animation, Document document)
        {
            var globalAabb = Rect.Empty;
            for (var i = animation.BeginFrame; i <= animation.EndFrame; i++)
            {
                document.ApplyAnimationToScene(animation.Id, i);

                SceneViewport.Clear();
                var aabb = AddSpritesToViewport(document);

                globalAabb = CombineAabb(globalAabb, aabb);
            }

            return globalAabb;
        }

        private Rect AddSpritesToViewport(Document document)
        {
            var globalAabb = Rect.Empty;
            var visibleSpriteNodes = GetVisibleSpriteNodesInDrawOrder(document);
            foreach (var spriteNode in visibleSpriteNodes)
            {
                SceneViewport.AddSpriteNode(spriteNode.Id, _spriteBitmapStore.Get(spriteNode.SpriteRef.RelativePath));
                SceneViewport.SetTransform(spriteNode.Id, spriteNode.GetAnimateTransformation().GlobalTransform.ToWpfMatrix());
                var aabb = SceneViewport.GetBoundingBox(spriteNode.Id);
                globalAabb = CombineAabb(aabb, globalAabb);
            }

            return globalAabb;
        }

        private static Rect CombineAabb(Rect a, Rect b)
        {
            return new Rect(new Point(Math.Min(a.Left, b.Left), Math.Min(a.Top, b.Top)), new Point(Math.Max(a.Right, b.Right), Math.Max(a.Bottom, b.Bottom)));
        }

        private static void SavePng(BitmapSource renderTarget, string filename)
        {
            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(renderTarget));
            using var stream = File.Create(filename);
            pngEncoder.Save(stream);
        }

        private static IEnumerable<SpriteNode> GetVisibleSpriteNodesInDrawOrder(Document document)
        {
            var spriteNodes = document.GetNodeIdsInDrawOrder().Reverse().Select(nodeId => document.GetNode(nodeId))
                .Cast<SpriteNode>();
            var visibleSpriteNodes =
                spriteNodes.Where(sn => Property.ValueToBool(sn.GetProperty(PropertyType.Visibility).AnimateVisualValue));
            return visibleSpriteNodes;
        }

        private Document CloneDocument(IMessageBus messageBus)
        {
            return _editor.CloneDocument(messageBus);
        }

        private void ConfigureViewport(int width, int height)
        {
            //SceneViewport.Width = width;
            //SceneViewport.Height = height;
            //SceneViewport.InvalidateVisual();
            //SceneViewport.BeginInit();
            //SceneViewport.Measure(new Size(width, height));
            //SceneViewport.Arrange(new Rect(0, 0, width, height));
            //SceneViewport.ApplyZoomToCamera();
            //SceneViewport.EndInit();
        }

        public float Scale { get; set; }
        public double DpiX { get; set; }
        public double DpiY { get; set; }
        public SceneViewport SceneViewport { get; set; }
    }
}
