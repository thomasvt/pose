using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Pose.Persistence;
using Pose.Runtime.MonoGameDotNetCore.Animations;
using Pose.Runtime.MonoGameDotNetCore.QuadRendering;

namespace Pose.Runtime.MonoGameDotNetCore.Skeletons
{
    /// <summary>
    /// Defines a Pose skeleton and contains the resources for it (eg. sprite textures, vertexbuffers) reused by all <see cref="Skeleton"/> instances. 
    /// </summary>
    public class SkeletonDefinition
    {
        private readonly Document _document;
        private readonly Dictionary<string, SpriteQuad> _spritesPerFilePath;

        internal SkeletonDefinition(Document document, Dictionary<string, SpriteQuad> spritesPerFilePath)
        {
            _document = document;
            _spritesPerFilePath = spritesPerFilePath;
        }
        
        internal Skeleton CreateInstance(Vector3 position, float angle)
        {
            // todo Z value of the position should be used for depth sorting multiple skeletons

            var nodes = BuildRuntimeNodes(out var nodeIndices);
            var drawSequenceIndices = _document.DrawOrder.NodeIds.Select(id => nodeIndices[id]).Reverse().ToArray();
            var animations = BuildRuntimeAnimations(nodeIndices);

            return new Skeleton(position, nodes, drawSequenceIndices, animations)
            {
                Position = position,
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
            var segments = new List<RTSegment>();
            var sortedKeys = propertyAnimation.Keys.OrderBy(k => k.Frame).ToList();

            if (sortedKeys[0].Frame != animation.BeginFrame)
            {
                // first key is not on first frame -> add a segment for the part before that first key
                var key = sortedKeys[0];
                var frameIndex = key.Frame - animation.BeginFrame;
                segments.Add(new RTSegment(new RTKey(0, key.Value), new RTKey((float)frameIndex / animation.FramesPerSecond, key.Value)));
            }

            // add segments for each key to the next key
            for (var i = 0; i < sortedKeys.Count; i++)
            {
                var key = sortedKeys[i];
                var keyTime = (float)(key.Frame - animation.BeginFrame) / animation.FramesPerSecond;

                if (i == sortedKeys.Count - 1)
                {
                    // last key, add an end-segment with constant value.
                    // This allows us to ensures that non-looping animations end at the exact endkey-value by allowing to render 1 frame beyond the last keyframe.
                    segments.Add(new RTSegment(new RTKey(keyTime, key.Value), new RTKey(float.MaxValue, key.Value)));
                }
                else
                {
                    var nextKey = sortedKeys[i + 1];
                    var nextKeyTime = (float)(nextKey.Frame - animation.BeginFrame) / animation.FramesPerSecond;
                    segments.Add(new RTSegment(new RTKey(keyTime, key.Value), new RTKey(nextKeyTime, nextKey.Value)));
                }
            }

            return segments.ToArray();
        }

        private RTNode[] BuildRuntimeNodes(out Dictionary<ulong, int> nodeIndices)
        {
            // we store worldtransform of the Skeleton in the game as an extra runtime rootnode at index 0. This makes all nodes have a parent node, eliminates an IF at render-time.

            var nodeCount = _document.Nodes.Count(n => n.Type == Node.Types.NodeType.Spritenode) + 1; // +1 -> for the extra runtime root 
            var nodes = new List<RTNode>(nodeCount)
            {
                new RTNode(0, new Transformation(), new Transformation()) // root of the skeleton
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
                nodes.Add(new RTNode(parentIdx,
                    new Transformation(node.X, node.Y, node.Angle),
                    new Transformation(0,0,0),
                    node.Type == Node.Types.NodeType.Spritenode ? _spritesPerFilePath[node.SpriteFile] : null));
                nodeIndices.Add(node.Id, i + 1);
            }

            return nodes.ToArray();
        }

        public void RegisterTextures(QuadRenderer quadRenderer)
        {
            foreach (var spriteQuad in _spritesPerFilePath.Values)
            {
                quadRenderer.RegisterTexture(spriteQuad.Texture);
            }
        }
    }
}
