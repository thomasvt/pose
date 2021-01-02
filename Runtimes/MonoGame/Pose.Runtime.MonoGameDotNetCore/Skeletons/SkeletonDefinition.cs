using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pose.Common.Curves;
using Pose.Persistence;
using Pose.Runtime.MonoGameDotNetCore.Animations;
using BezierCurve = Pose.Common.Curves.BezierCurve;
using Spritesheet = Pose.Runtime.MonoGameDotNetCore.Rendering.Spritesheet;

namespace Pose.Runtime.MonoGameDotNetCore.Skeletons
{
    /// <summary>
    /// Defines a Pose skeleton and contains the resources for it (eg. sprite textures, vertexbuffers) reused by all <see cref="Skeleton"/> instances. 
    /// </summary>
    public class SkeletonDefinition
    {
        private readonly Document _document;
        private readonly Spritesheet _spritesheet;

        public SkeletonDefinition(Document document, Spritesheet spritesheet, Texture2D texture)
        {
            Texture = texture;
            _document = document;
            _spritesheet = spritesheet;
            BezierTolerance = 0.005f;
        }
        
        public Skeleton CreateInstance(Vector2 position, float depth, float angle)
        {
            // TODO we could optimize by mapping a Document into a form more prepped for creating instances.
            var nodes = BuildRuntimeNodes(out var nodeIndices);
            var drawSequenceIndices = _document.DrawOrder.NodeIds.Select(id => nodeIndices[id]).Reverse().ToArray();
            var animations = BuildRuntimeAnimations(nodeIndices);

            return new Skeleton(nodes, drawSequenceIndices, animations, Texture)
            {
                Position = position,
                Depth = depth,
                Angle = angle
            };
        }

        private Dictionary<string, RTAnimation> BuildRuntimeAnimations(Dictionary<ulong, int> nodeIndices)
        {
            var animations = new Dictionary<string, RTAnimation>(_document.Animations.Count);
            foreach (var animation in _document.Animations)
            {
                var fps = animation.FramesPerSecond;
                if (fps < 1)
                    throw new Exception("FramesPerSecond must be > 0.");
                var duration = (float)(animation.EndFrame - animation.BeginFrame + 1) / fps;

                var rtPropertyAnimations = new List<RTPropertyAnimation>();
                foreach (var nodeAnimationCollection in animation.NodeAnimations)
                {
                    foreach (var propertyAnimation in nodeAnimationCollection.PropertyAnimations)
                    {
                        if (!propertyAnimation.Keys.Any())
                            continue;
                        var rtSegments = BuildPropertyAnimationSegments(propertyAnimation, animation);
                        rtPropertyAnimations.Add(new RTPropertyAnimation(nodeIndices[nodeAnimationCollection.NodeId], (NodeProperty)propertyAnimation.Property, rtSegments));
                    }
                }

                var rtAnimation = new RTAnimation(duration, animation.IsLoop, rtPropertyAnimations.ToArray());
                animations.Add(animation.Name, rtAnimation);
            }

            return animations;
        }

        private RTSegment[] BuildPropertyAnimationSegments(PropertyAnimation propertyAnimation, Animation animation)
        {
            // This is more complicated than you would expect as we support animation wrapping by creating some nuances in the RTSegment. 
            // BeginTime and EndTime indicate the Begin and End of the segment regarding the animation time-cursor: if the animation time is between BEginTime and EndTime, this segment is picked to calculate animation values.
            // The actual interpolation calculation does not use BeginTime and EndTime, though. It uses leftKeyTime, Duration, LeftKeyValue and RightKeyValue. This is because, when looping, we wrap keyframes by creating segments
            // at the beginning and end of the timeline where interpolation will be using a wrapped around keyframe form the other side of the timeline. Therefore, we need to supply this interpolation data separate from the BEgin/End times
            // that only decide which Segment to pick.

            var segments = new List<RTSegment>();
            var sortedKeys = propertyAnimation.Keys.OrderBy(k => k.Frame).ToList();

            if (sortedKeys[0].Frame != animation.BeginFrame)
            {
                // first key is not on first frame -> add a segment for the part before that first key
                var key = sortedKeys[0];
                var frameIndex = key.Frame - animation.BeginFrame;
                if (animation.IsLoop)
                {
                    // create a segment from 0 to first key, but with interpolation information that wraps around the animation timeline and uses the last key of the timeline as "left" key for interpolating.
                    var wrappedKey = sortedKeys[new Index(sortedKeys.Count - 1)];
                    var rightKeyTime = (float) frameIndex / animation.FramesPerSecond;
                    var leftKeyTime = -(float)(animation.EndFrame - wrappedKey.Frame + 1) / animation.FramesPerSecond;
                    var duration = rightKeyTime - leftKeyTime;
                    segments.Add(new RTSegment(0, rightKeyTime, leftKeyTime, duration, wrappedKey.Value, key.Value, MapInterpolationType(wrappedKey.InterpolationType), MapBezierCurve(wrappedKey.Curve), BezierTolerance));
                }
                else
                {
                    segments.Add(new RTSegment(0, (float)frameIndex / animation.FramesPerSecond, 0, (float)frameIndex / animation.FramesPerSecond, key.Value, key.Value, CurveType.Hold));
                }
            }

            // add segments for each key to the next key
            for (var i = 0; i < sortedKeys.Count; i++)
            {
                var key = sortedKeys[i];
                var keyTime = (float)(key.Frame - animation.BeginFrame) / animation.FramesPerSecond;

                if (i == sortedKeys.Count - 1)
                {
                    // last key, add an end-segment

                    if (animation.IsLoop)
                    {
                        // create a segment from last key to end of animation, but with interpolation information that wraps around the animation timeline and uses the first key of the timeline as "right" key for interpolating.
                        var wrappedKey = sortedKeys[new Index(0)];
                        var rightKeyTime = (float)(animation.EndFrame + wrappedKey.Frame - animation.BeginFrame + 1) / animation.FramesPerSecond;
                        var duration = rightKeyTime - keyTime;
                        segments.Add(new RTSegment(keyTime, float.MaxValue, keyTime, duration, key.Value, wrappedKey.Value, MapInterpolationType(key.InterpolationType), MapBezierCurve(key.Curve), BezierTolerance));
                    }
                    // This allows us to ensures that non-looping animations end at the exact endkey-value by allowing to render 1 frame beyond the last keyframe.
                    segments.Add(new RTSegment(keyTime, float.MaxValue, keyTime, float.MaxValue - keyTime, key.Value, key.Value, CurveType.Hold));
                }
                else
                {
                    var nextKey = sortedKeys[i + 1];
                    var nextKeyTime = (float)(nextKey.Frame - animation.BeginFrame) / animation.FramesPerSecond;
                    segments.Add(new RTSegment(keyTime, nextKeyTime, keyTime, nextKeyTime - keyTime, key.Value, nextKey.Value, MapInterpolationType(key.InterpolationType), MapBezierCurve(key.Curve), BezierTolerance));
                }
            }

            return segments.ToArray();
        }

        private BezierCurve? MapBezierCurve(Persistence.BezierCurve curve)
        {
            if (curve == null)
                return null;

            return new BezierCurve(new Common.Vector2(curve.P0.X, curve.P0.Y), new Common.Vector2(curve.P1.X, curve.P1.Y), new Common.Vector2(curve.P2.X, curve.P2.Y), new Common.Vector2(curve.P3.X, curve.P3.Y));
        }

        private static CurveType MapInterpolationType(Key.Types.InterpolationTypeEnum type)
        {
            return type switch
            {
                Key.Types.InterpolationTypeEnum.Linear => CurveType.Linear,
                Key.Types.InterpolationTypeEnum.Hold => CurveType.Hold,
                Key.Types.InterpolationTypeEnum.Bezier => CurveType.Bezier,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private RTNode[] BuildRuntimeNodes(out Dictionary<ulong, int> nodeIndices)
        {
            // we store worldtransform of the Skeleton in the game as an extra runtime rootnode at index 0. This ensures all nodes have a parent node, eliminating an IF at render-time and allows us to give the skeleton a world transform.

            var nodeCount = _document.Nodes.Count(n => n.Type == Node.Types.NodeType.Spritenode) + 1; // +1 -> for the extra runtime root 
            var nodes = new List<RTNode>(nodeCount)
            {
                new RTNode(0, true, new Transformation(), new Transformation(), 0) // root of the skeleton
            };
            nodeIndices = new Dictionary<ulong, int>(nodeCount)
            {
                {0, 0} // root of the skeleton
            };

            // add hierarchy:
            for (var i = 0; i < _document.Nodes.Count; i++)
            {
                var node = _document.Nodes[i];
                var parentIdx = nodeIndices[node.ParentId];

                var drawOrderIndex = 0;
                if (node.Type == Node.Types.NodeType.Spritenode)
                    drawOrderIndex = _document.DrawOrder.NodeIds.IndexOf(node.Id);

                nodes.Add(new RTNode(parentIdx,
                    node.IsVisible,
                    new Transformation(node.Position.X, node.Position.Y, node.Angle),
                    new Transformation(0,0,0),
                    drawOrderIndex,
                    node.Type == Node.Types.NodeType.Spritenode ? _spritesheet.Sprites[node.SpriteKey] : null));
                nodeIndices.Add(node.Id, i + 1);
            }

            return nodes.ToArray();
        }

        /// <summary>
        /// Loads a SkeletonDefinition from the 3 files generated by the Pose editor. The 3 filenames are created by appending their extension to the given filename: .pose, .png and .sheet 
        /// </summary>
        /// <param name="pathWithoutExtension">The filename (without extension) of the pose file. The 3 needed filenames are derived from this parameter.</param>
        public static SkeletonDefinition LoadFromFiles(GraphicsDevice graphicsDevice, string pathWithoutExtension)
        {
            return LoadFromFiles(graphicsDevice, pathWithoutExtension + ".pose", pathWithoutExtension + ".sheet", pathWithoutExtension + ".png");
        }

        /// <summary>
        /// The long overload to load a Pose definition from its files by giving all 3 filenames. If you used the default save behavior of the Pose editor, you may use the overload with just the name parameter.
        /// </summary>
        /// <param name="poseFilename">The full path of the .pose file to load</param>
        /// <param name="sheetFilename">The full path of the .sheet file (spritesheet data) to load</param>
        /// <param name="pngFilename">The full path of the .png file (spritesheet) to load</param>
        public static SkeletonDefinition LoadFromFiles(GraphicsDevice graphicsDevice, string poseFilename, string sheetFilename, string pngFilename)
        {
            var document = Document.Parser.ParseFrom(File.ReadAllBytes(poseFilename));
            var texture = Texture2D.FromFile(graphicsDevice, pngFilename);
            var sheet = SpritesheetMapper.MapSpritesheet(Persistence.Spritesheet.Parser.ParseFrom(File.ReadAllBytes(sheetFilename)));

            return new SkeletonDefinition(document, sheet, texture);
        }

        public Texture2D Texture { get; }

        /// <summary>
        /// The BezierTolerance is the required minimal correctness (percentage) of the calculated value by the approximation algorithm. You can increase this to improve performance, or decrease it to improve smoothness of animation. 0.005 (0.5%) is default.
        /// Visual stuttering of movement is a sign that this tolerance is too high. Also, the more a bezier curve resembles a straight line, the faster a value within tolerance is calculated. See <see cref="BezierMath"/> for details.
        /// </summary>
        public float BezierTolerance { get; set; }
    }
}
