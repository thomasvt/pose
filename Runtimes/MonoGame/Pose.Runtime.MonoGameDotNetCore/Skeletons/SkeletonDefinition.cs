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
            // we convert the keys on a propertyanimation into a series of Segments. Each segment fills up the gap between two keys and therefore has a begintime and endtime.
            // These segments don't overlap. All segments together fill up the entire timeline of the animation.
            //
            // 
            // ---------|------|--|-------|------------
            //
            // --- segment    | key
            //
            // The animation runtime will always find 1 segment that the current time is in, and can use the Interpolation info in that segment to find the correct animation value.
            // For the first and last segment, we don't have two surrounding keys to interpolate between. So, we need to fabricate a virtual key to keep using the same interpolation algorithm:
            // * for non loops: we create an interpolation of type Hold (constant value) using the value of the one key that is touching the segment.
            // * for loops: loops wrap their animation around from the last key to the first key of the next repetition. So, we create interpolation that acts as if that wrapping is just a segment like all others:
            //      we set interpolation data to interpolate between the last and first key in those edge-segments of the timeline.


            var segments = new List<RTSegment>();
            var sortedKeys = propertyAnimation.Keys.OrderBy(k => k.Frame).ToList();

            if (sortedKeys[0].Frame != animation.BeginFrame)
            {
                // first key is not on first frame -> add a segment for the part before the first key
                segments.Add(CreatePreFirstKeySegment(sortedKeys, animation));
            }

            for (var i = 0; i < sortedKeys.Count; i++)
            {
                var leftKey = sortedKeys[i];
                var leftKeyTime = FrameToTime(leftKey.Frame - animation.BeginFrame, animation);

                segments.Add(i < sortedKeys.Count - 1
                    ? CreateSegment(animation, sortedKeys, i, leftKeyTime, leftKey) // normal segment
                    : CreatePostLastKeySegment(sortedKeys, animation)); // last segment
            }

            return segments.ToArray();
        }

        private RTSegment CreateSegment(Animation animation, List<Key> sortedKeys, int i, float leftKeyTime, Key leftKey)
        {
            var rightKey = sortedKeys[i + 1];
            var rightKeyTime = FrameToTime(rightKey.Frame - animation.BeginFrame, animation);
            var interpolation = CreateRuntimeInterpolation(leftKeyTime, leftKey, rightKeyTime, rightKey);
            return new RTSegment(leftKeyTime, rightKeyTime, interpolation);
        }

        private RTSegment CreatePreFirstKeySegment(List<Key> sortedKeys, Animation animation)
        {
            var rightKey = sortedKeys[0];
            var rightKeyTime = FrameToTime(rightKey.Frame - animation.BeginFrame, animation);
            if (animation.IsLoop)
            {
                // create a segment from 0 to first key, but with interpolation between this key as rightside and the last on the timeline as left-side so the animation wraps around when looping.
                var lastKey = sortedKeys[^1];
                var leftKeyTime = FrameToTime(-animation.EndFrame + lastKey.Frame - 1, animation); // pretend that the wrapped key is more to the left (negative time), so interpolation is done just like on a normal segment.
                var interpolation = CreateRuntimeInterpolation(leftKeyTime, lastKey, rightKeyTime, rightKey);
                return new RTSegment(0, rightKeyTime, interpolation);
            }
            else
            {
                // no loop: create a startup segment with constant value.
                var interpolation = new RTInterpolation(0, rightKey.Value, rightKeyTime, rightKey.Value, CurveType.Hold);
                return new RTSegment(0, rightKeyTime, interpolation);
            }
        }

        private RTSegment CreatePostLastKeySegment(List<Key> sortedKeys, Animation animation)
        {
            var leftKey = sortedKeys[^1];
            var leftKeyTime = FrameToTime(leftKey.Frame - animation.BeginFrame, animation);
            if (animation.IsLoop)
            {
                // create a segment from last key to end of animation, but with interpolation information that wraps around the animation timeline and uses the first key of the timeline as "right" key for interpolating.
                var rightKey = sortedKeys[0];
                var rightKeyTime = FrameToTime(animation.EndFrame + rightKey.Frame - animation.BeginFrame + 1, animation);
                var interpolation = CreateRuntimeInterpolation(leftKeyTime, leftKey, rightKeyTime, rightKey);
                return new RTSegment(leftKeyTime, FrameToTime(animation.EndFrame, animation), interpolation);
            }
            else
            {
                var interpolation = new RTInterpolation(leftKeyTime, leftKey.Value, float.MaxValue, leftKey.Value, CurveType.Hold);
                return new RTSegment(leftKeyTime, float.MaxValue, interpolation);
            }
        }

        private RTInterpolation CreateRuntimeInterpolation(float leftKeyTime, Key leftKey, float rightKeyTime, Key key)
        {
            return new RTInterpolation(leftKeyTime, leftKey.Value, rightKeyTime, key.Value, MapInterpolationType(leftKey.InterpolationType), MapBezierCurve(leftKey.Curve), BezierTolerance);
        }

        private static float FrameToTime(int frame, Animation animation)
        {
            return (float)frame / animation.FramesPerSecond;
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
